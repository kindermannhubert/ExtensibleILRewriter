using ExtensibleILRewriter.Extensions;
using ExtensibleILRewriter.MethodProcessors;
using ExtensibleILRewriter.MethodProcessors.Contracts;
using ExtensibleILRewriter.MethodProcessors.Helpers;
using ExtensibleILRewriter.MsBuild.Configuration;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Serialization;

namespace ExtensibleILRewriter.MsBuild
{
    public class AssemblyRewrite : Task
    {
        [Required]
        public string AssemblyPath { get; set; }

        [Required]
        public string ConfigurationPath { get; set; }

        public override bool Execute()
        {
            var logger = new MsBuildLogger(Log);

            return Execute(AssemblyPath, logger);
        }

        public bool Execute(string outputPath, ILogger logger = null)
        {
            if (logger == null) logger = new DummyLogger();

            try
            {
                logger.Progress("Loading configuration '\{ConfigurationPath ?? string.Empty}'.");
                var configuration = LoadConfiguration();
                configuration.Check();
                logger.Progress("Loading configuration done.");

                var rewriter = new AssemblyRewriter(AssemblyPath, logger);

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

        private AssemblyRewriteConfiguration LoadConfiguration()
        {
            if (string.IsNullOrEmpty(ConfigurationPath))
            {
                throw new InvalidOperationException("You have to specify \{nameof(ConfigurationPath)} for \{nameof(AssemblyRewrite)} task.");
            }

            if (!File.Exists(ConfigurationPath))
            {
                throw new InvalidOperationException("Configuration file for rewriting task '\{ConfigurationPath}' does not exist.");
            }

            var serializer = new XmlSerializer(typeof(AssemblyRewriteConfiguration));
            using (var stream = File.OpenRead(ConfigurationPath))
            {
                return (AssemblyRewriteConfiguration)serializer.Deserialize(stream);
            }
        }

        private void LoadProcessors(AssemblyRewriter assemblyRewriter, AssemblyRewriteConfiguration configuration, [NotNull]ILogger logger)
        {
            var executingAssemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            ResolveEventHandler currentDomain_AssemblyResolve =
                (sender, args) =>
                {
                    var loadedAssembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.FullName == args.Name);
                    if (loadedAssembly != null) return loadedAssembly;

                    var path = Path.Combine(executingAssemblyPath, GetAssemblyNameFromFullName(args.Name));
                    if (File.Exists(path + ".dll")) path = path + ".dll";
                    else if (File.Exists(path + ".exe")) path = path + ".exe";
                    else return null;

                    return Assembly.Load(File.ReadAllBytes(path));
                };

            try
            {
                AppDomain.CurrentDomain.AssemblyResolve += currentDomain_AssemblyResolve;

                var assembliesDict = configuration.Assemblies.ToDictionary(a => a.Alias, a => new Lazy<Assembly>(() => LoadProcessorsAssembly(a.Path, executingAssemblyPath, logger)));

                assemblyRewriter.AssemblyProcessors.AddRange(LoadProcessors<AssemblyDefinition>(configuration.AssemblyProcessors, logger, assembliesDict));
                assemblyRewriter.ModuleProcessors.AddRange(LoadProcessors<ModuleDefinition>(configuration.ModuleProcessors, logger, assembliesDict));
                assemblyRewriter.TypeProcessors.AddRange(LoadProcessors<TypeDefinition>(configuration.TypeProcessors, logger, assembliesDict));
                assemblyRewriter.MethodProcessors.AddRange(LoadProcessors<MethodDefinition>(configuration.MethodProcessors, logger, assembliesDict));
            }
            finally
            {
                AppDomain.CurrentDomain.AssemblyResolve -= currentDomain_AssemblyResolve;
            }
        }

        private static IEnumerable<IComponentProcessor<ComponentType, ComponentProcessorConfiguration>> LoadProcessors<ComponentType>(ProcessorDefinition[] processorDefinitions, ILogger logger, Dictionary<string, Lazy<Assembly>> assembliesDict)
        {
            foreach (var processorDefinition in processorDefinitions)
            {
                logger.Notice("Loading processor \{processorDefinition.ProcessorName}.");

                var assembly = assembliesDict[processorDefinition.AssemblyAlias].Value;
                var processorType = assembly.GetType(processorDefinition.ProcessorName);
                if (processorType == null) throw new InvalidOperationException("Unable to load '\{processorDefinition.ProcessorName}' processor from assembly '\{assembly.FullName}'.");

                var processorProperties = new ComponentProcessorProperties(processorDefinition.Properties.Select(p => Tuple.Create(p.Name, p.Value)));
                var processorBaseGenericInterface = processorType.GetInterfaces().FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IComponentProcessor<,>));
                var processorConfigurationType = processorBaseGenericInterface.GenericTypeArguments[1];
                var processorConfiguration = (ComponentProcessorConfiguration)Activator.CreateInstance(processorConfigurationType);
                processorConfiguration.LoadFromProperties(processorProperties);

                var processor = (IComponentProcessor<ComponentType, ComponentProcessorConfiguration>)Activator.CreateInstance(processorType, processorConfiguration, logger);
                yield return processor;
            }
        }

        private Assembly LoadProcessorsAssembly(string path, string currentPath, [NotNull]ILogger logger)
        {
            if (!Path.IsPathRooted(path)) path = Path.Combine(currentPath, path);
            if (!File.Exists(path))
            {
                throw new FileNotFoundException("Assembly file '\{path}' with processors does not exist.");
            }

            var assemblyDefinition = Mono.Cecil.AssemblyDefinition.ReadAssembly(path);
            var loadedAssembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.FullName == assemblyDefinition.FullName);
            if (loadedAssembly != null) return loadedAssembly;

            return Assembly.Load(File.ReadAllBytes(path));
        }

        private string GetAssemblyNameFromFullName([NotNull]string fullName)
        {
            int firstComma = fullName.IndexOf(',');
            return fullName.Substring(0, firstComma);
        }
    }
}