using ExtensibleILRewriter.Extensions;
using ExtensibleILRewriter.Logging;
using ExtensibleILRewriter.MsBuild.Configuration;
using ExtensibleILRewriter.Processors.Parameters;
using Microsoft.Build.Utilities;
using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;
using Ms = Microsoft.Build.Framework;

namespace ExtensibleILRewriter.MsBuild
{
    public class AssemblyRewrite : Task
    {
        private readonly FilePathResolver filePathResolver = new FilePathResolver();

        [Ms.Required]
        public string AssemblyPath { get; set; }

        [Ms.Required]
        public string ConfigurationPath { get; set; }

        public override bool Execute()
        {
            var logger = new MsBuildLogger(Log);

            return Execute(AssemblyPath, logger);
        }

        public bool Execute(string outputPath, ILogger logger = null)
        {
            if (logger == null)
            {
                logger = new DummyLogger();
            }

            try
            {
                if (string.IsNullOrEmpty(AssemblyPath))
                {
                    throw new InvalidOperationException($"You have to specify {nameof(AssemblyPath)} for {nameof(AssemblyRewrite)} task.");
                }

                if (!File.Exists(AssemblyPath))
                {
                    throw new FileNotFoundException($"Unable to find assembly '{AssemblyPath}'.");
                }

                filePathResolver.AddSearchPath(Path.GetDirectoryName(Path.GetFullPath(AssemblyPath)));

                logger.Progress($"Loading configuration '{ConfigurationPath ?? string.Empty}'.");
                var configuration = LoadConfiguration();
                configuration.Check();
                logger.Progress("Loading configuration done.");

                var rewriter = new AssemblyRewriter(AssemblyPath, logger);

                AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;

                logger.Progress("Loading processors.");
                LoadProcessors(rewriter, configuration, logger);
                logger.Progress("Loading processors done.");

                rewriter.ProcessAssemblyAndSave(outputPath);
            }
            catch (Exception e)
            {
                logger.Error(e.ToString());
                return false;
            }

            return true;
        }

        private Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            var loadedAssembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.FullName == args.Name);
            if (loadedAssembly != null)
            {
                return loadedAssembly;
            }

            string assemblyPath;
            string assemblyName = GetAssemblyNameFromFullName(args.Name);

            if (!filePathResolver.TryResolveFilePath(assemblyName + ".dll", out assemblyPath))
            {
                if (!filePathResolver.TryResolveFilePath(assemblyName + ".exe", out assemblyPath))
                {
                    throw new FileNotFoundException($"Unable to find assembly '{assemblyName}' at search paths.");
                }
            }

            return Assembly.Load(File.ReadAllBytes(assemblyPath));
        }

        private AssemblyRewriteConfiguration LoadConfiguration()
        {
            if (string.IsNullOrEmpty(ConfigurationPath))
            {
                throw new InvalidOperationException($"You have to specify {nameof(ConfigurationPath)} for {nameof(AssemblyRewrite)} task.");
            }

            if (!File.Exists(ConfigurationPath))
            {
                throw new InvalidOperationException($"Configuration file for rewriting task '{ConfigurationPath}' does not exist.");
            }

            var serializer = new XmlSerializer(typeof(AssemblyRewriteConfiguration));
            using (var stream = File.OpenRead(ConfigurationPath))
            {
                return (AssemblyRewriteConfiguration)serializer.Deserialize(stream);
            }
        }

        private void LoadProcessors(AssemblyRewriter assemblyRewriter, AssemblyRewriteConfiguration configuration, [NotNull]ILogger logger)
        {
            var assembliesDict = new Dictionary<string, LazyAssembly>();
            foreach (var assemblyCfg in configuration.Assemblies)
            {
                var lazyAssemblyDefinition = new Lazy<AssemblyDefinition>(() => LoadProcessorsAssemblyDefinition(assemblyCfg.Path));
                var lazyAssembly = new Lazy<Assembly>(() => LoadProcessorsAssembly(assemblyCfg.Path, lazyAssemblyDefinition.Value));
                assembliesDict.Add(assemblyCfg.Alias, new LazyAssembly(lazyAssembly, lazyAssemblyDefinition));
            }

            var typeAliasResolver = new TypeAliasResolver(
                assembliesDict.ToDictionary(kv => kv.Key, kv => kv.Value.AssemblyDefinition),
                assembliesDict.ToDictionary(kv => kv.Key, kv => kv.Value.Assembly),
                configuration.Types.ToDictionary(t => t.Alias, t => new TypeAliasResolver.TypeAliasDefinition(t.AssemblyAlias, t.Name)));

            var processors = LoadProcessors(configuration.Processors, logger, assembliesDict, typeAliasResolver);

            foreach (var processor in processors)
            {
                if (processor.SupportedComponents.Count == 0)
                {
                    throw new InvalidOperationException($"Processor '{processor.GetType().FullName}' contains no supported components.");
                }

                foreach (var supportedComponent in processor.SupportedComponents)
                {
                    switch (supportedComponent)
                    {
                        case ProcessableComponentType.Assembly:
                            assemblyRewriter.AssemblyProcessors.Add(processor);
                            break;
                        case ProcessableComponentType.Module:
                            assemblyRewriter.ModuleProcessors.Add(processor);
                            break;
                        case ProcessableComponentType.Type:
                            assemblyRewriter.TypeProcessors.Add(processor);
                            break;
                        case ProcessableComponentType.Field:
                            assemblyRewriter.FieldProcessors.Add(processor);
                            break;
                        case ProcessableComponentType.Property:
                            assemblyRewriter.PropertyProcessors.Add(processor);
                            break;
                        case ProcessableComponentType.Method:
                            assemblyRewriter.MethodProcessors.Add(processor);
                            break;
                        case ProcessableComponentType.MethodParameter:
                            assemblyRewriter.ParameterProcessors.Add(processor);
                            break;
                        default:
                            throw new InvalidOperationException($"Unknown {nameof(ProcessableComponentType)}: '{supportedComponent}'.");
                    }
                }
            }
        }

        private static IEnumerable<IComponentProcessor<ComponentProcessorConfiguration>> LoadProcessors(
            ProcessorDefinition[] processorDefinitions, ILogger logger,
            Dictionary<string, LazyAssembly> assembliesDict, TypeAliasResolver typeAliasResolver)
        {
            foreach (var processorDefinition in processorDefinitions)
            {
                logger.Notice($"Loading processor {processorDefinition.ProcessorName}.");

                var assembly = assembliesDict[processorDefinition.AssemblyAlias].Assembly.Value;
                var processorType = assembly.GetType(processorDefinition.ProcessorName);
                if (processorType == null)
                {
                    throw new InvalidOperationException($"Unable to load '{processorDefinition.ProcessorName}' processor from assembly '{assembly.FullName}'. Cannot find specified type in assembly.");
                }

                var processorTypeGenericArgs = processorType.GetGenericArguments();
                int numberOfProcessorGenericParameters = processorTypeGenericArgs.Where(arg => arg.IsGenericParameter).Count();
                if (numberOfProcessorGenericParameters != processorDefinition.GenericArguments.Length)
                {
                    throw new InvalidOperationException($"Unable to load '{processorDefinition.ProcessorName}' processor from assembly '{assembly.FullName}'. Number of generic parameters (={numberOfProcessorGenericParameters}) is different from number of configured processor generic arguments (={processorDefinition.GenericArguments.Length}).");
                }

                if (numberOfProcessorGenericParameters > 0)
                {
                    var genericArgumentTypes = processorDefinition.GenericArguments.Select(arg => typeAliasResolver.ResolveType(arg)).ToArray();
                    processorType = processorType.MakeGenericType(genericArgumentTypes);
                }

                var processorProperties = new ComponentProcessorProperties(processorDefinition.Properties.Select(p => Tuple.Create(p.Name, p.Value)));

                var processorBaseGenericInterface = processorType.GetInterfaces().FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IComponentProcessor<>));
                if (processorBaseGenericInterface == null)
                {
                    throw new InvalidOperationException($"Unable to load '{processorDefinition.ProcessorName}' processor from assembly '{assembly.FullName}' because it does not implement {typeof(IComponentProcessor<>).FullName} interface.");
                }

                var processorConfigurationType = processorBaseGenericInterface.GenericTypeArguments[0];

                var processorConfiguration = (ComponentProcessorConfiguration)Activator.CreateInstance(processorConfigurationType);
                processorConfiguration.LoadFromProperties(processorProperties, typeAliasResolver, processorDefinition.ProcessorName);

                var processor = (IComponentProcessor<ComponentProcessorConfiguration>)Activator.CreateInstance(processorType, processorConfiguration, logger);
                yield return processor;
            }
        }

        private Mono.Cecil.AssemblyDefinition LoadProcessorsAssemblyDefinition(string path)
        {
            path = filePathResolver.ResolveFilePath(path);

            return Mono.Cecil.AssemblyDefinition.ReadAssembly(path);
        }

        private Assembly LoadProcessorsAssembly(string path, Mono.Cecil.AssemblyDefinition assemblyDefinition)
        {
            path = filePathResolver.ResolveFilePath(path);

            var loadedAssembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.FullName == assemblyDefinition.FullName);
            if (loadedAssembly != null)
            {
                return loadedAssembly;
            }

            return Assembly.Load(File.ReadAllBytes(path));
        }

        private string GetAssemblyNameFromFullName([NotNull]string fullName)
        {
            int firstComma = fullName.IndexOf(',');
            return fullName.Substring(0, firstComma);
        }

        private class LazyAssembly
        {
            public LazyAssembly(Lazy<Assembly> assembly, Lazy<AssemblyDefinition> assemblyDefinition)
            {
                Assembly = assembly;
                AssemblyDefinition = assemblyDefinition;
            }

            public Lazy<Assembly> Assembly { get; }

            public Lazy<AssemblyDefinition> AssemblyDefinition { get; }
        }
    }
}