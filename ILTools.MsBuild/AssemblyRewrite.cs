using ILTools.Extensions;
using ILTools.MethodProcessors;
using ILTools.MethodProcessors.Contracts;
using ILTools.MethodProcessors.Helpers;
using ILTools.MsBuild.Configuration;
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

namespace ILTools.MsBuild
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

            var configuration = LoadConfiguration(logger);
            configuration.Check(logger);

            var rewriter = new AssemblyRewriter(AssemblyPath, logger);

            PrepareRewriters(rewriter, configuration, logger);

            rewriter.ProcessAssemblyAndSave(outputPath);

            return true;
        }

        private AssemblyRewriteConfiguration LoadConfiguration([NotNull]ILogger logger)
        {
            if (string.IsNullOrEmpty(ConfigurationPath))
            {
                var message = "You have to specify \{nameof(ConfigurationPath)} for \{nameof(AssemblyRewrite)} task.";
                logger.Error(message);
                throw new InvalidOperationException(message);
            }

            if (!File.Exists(ConfigurationPath))
            {
                var message = "Configuration file for rewriting task '\{ConfigurationPath}' does not exist.";
                logger.Error(message);
                throw new InvalidOperationException(message);
            }

            var serializer = new XmlSerializer(typeof(AssemblyRewriteConfiguration));
            using (var stream = File.OpenRead(ConfigurationPath))
            {
                return (AssemblyRewriteConfiguration)serializer.Deserialize(stream);
            }
        }

        private void PrepareRewriters(AssemblyRewriter assemblyRewriter, AssemblyRewriteConfiguration configuration, [NotNull]ILogger logger)
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

                    logger.Warning(path);
                    return Assembly.Load(File.ReadAllBytes(path));
                };

            try
            {
                AppDomain.CurrentDomain.AssemblyResolve += currentDomain_AssemblyResolve;

                var assembliesDict = configuration.AssembliesWithProcessors.ToDictionary(a => a.Name, a => new Lazy<Assembly>(() => LoadProcessorsAssembly(a.Path, executingAssemblyPath, logger)));

                foreach (var processorDefinition in configuration.MethodProcessors)
                {
                    var assembly = assembliesDict[processorDefinition.AssemblyName].Value;

                    var processorType = assembly.GetType(processorDefinition.ProcessorName);
                    var processor = (IComponentProcessor<MethodDefinition>)Activator.CreateInstance(processorType);
                    assemblyRewriter.MethodProcessors.Add(processor);
                }
            }
            finally
            {
                AppDomain.CurrentDomain.AssemblyResolve -= currentDomain_AssemblyResolve;
            }
        }

        private Assembly LoadProcessorsAssembly(string path, string currentPath, [NotNull]ILogger logger)
        {
            if (!Path.IsPathRooted(path)) path = Path.Combine(currentPath, path);
            if (!File.Exists(path))
            {
                var message = "Assembly file '\{path}' with processors does not exist.";
                logger.Error(message);
                throw new FileNotFoundException(message);
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