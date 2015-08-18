using ExtensibleILRewriter.Extensions;
using ExtensibleILRewriter.Logging;
using Mono.Cecil;
using Mono.Cecil.Pdb;
using System;
using System.Collections.Generic;
using System.IO;

namespace ExtensibleILRewriter
{
    public class AssemblyRewriter
    {
        private readonly string assemblyPath;
        private readonly ILogger logger;

        public AssemblyRewriter(string assemblyPath, ILogger logger = null)
        {
            this.assemblyPath = assemblyPath;
            this.logger = logger ?? new DummyLogger();
        }

        public List<IComponentProcessor<ComponentProcessorConfiguration>> AssemblyProcessors { get; } = new List<IComponentProcessor<ComponentProcessorConfiguration>>();

        public List<IComponentProcessor<ComponentProcessorConfiguration>> ModuleProcessors { get; } = new List<IComponentProcessor<ComponentProcessorConfiguration>>();

        public List<IComponentProcessor<ComponentProcessorConfiguration>> TypeProcessors { get; } = new List<IComponentProcessor<ComponentProcessorConfiguration>>();

        public List<IComponentProcessor<ComponentProcessorConfiguration>> FieldProcessors { get; } = new List<IComponentProcessor<ComponentProcessorConfiguration>>();

        public List<IComponentProcessor<ComponentProcessorConfiguration>> PropertyProcessors { get; } = new List<IComponentProcessor<ComponentProcessorConfiguration>>();

        public List<IComponentProcessor<ComponentProcessorConfiguration>> MethodProcessors { get; } = new List<IComponentProcessor<ComponentProcessorConfiguration>>();

        public List<IComponentProcessor<ComponentProcessorConfiguration>> ParameterProcessors { get; } = new List<IComponentProcessor<ComponentProcessorConfiguration>>();

        public void ProcessAssemblyAndSave(string rewrittenAssemblyPath)
        {
            var rewrittenAssembly = ProcessAssembly();

            logger.Progress($"Saving assembly '{rewrittenAssembly.FullName}' to '{rewrittenAssemblyPath}'.");
            try
            {
                var symbolWriterProvider = new PdbWriterProvider();
                var writerParameters = new WriterParameters()
                {
                    WriteSymbols = true,
                    SymbolWriterProvider = symbolWriterProvider
                };
                rewrittenAssembly.Write(rewrittenAssemblyPath, new WriterParameters() { WriteSymbols = true });
            }
            catch (Exception e)
            {
                throw new InvalidOperationException($"Error while saving assembly '{rewrittenAssemblyPath}'.", e);
            }

            logger.Progress("Saving assembly done.");
        }

        public AssemblyDefinition ProcessAssembly()
        {
            var assembly = LoadAssembly();

            logger.Progress($"Processing assembly: '{assembly.FullName}'.");
            ProcessComponent(assembly.ToProcessableComponent(), AssemblyProcessors, logger);

            // needs to copy out, because processors could modify the collection
            var modules = assembly.Modules.ToArray();
            foreach (var module in modules)
            {
                ProcessModule(module);
            }

            logger.Progress("Processing assembly done.");
            return assembly;
        }

        private void ProcessModule(ModuleDefinition module)
        {
            logger.Progress($"Processing module: '{module.Name}'.");
            ProcessComponent(module.ToProcessableComponent(), ModuleProcessors, logger);

            // needs to copy out, because processors could modify the collection
            var types = module.Types.ToArray();
            foreach (var type in types)
            {
                ProcessType(type);

                if (type.HasNestedTypes)
                {
                    foreach (var nestedType in type.NestedTypes)
                    {
                        ProcessType(nestedType);
                    }
                }
            }

            logger.Progress("Processing module done.");
        }

        private void ProcessType(TypeDefinition type)
        {
            logger.Progress($"Processing type: '{type.Name}'.");
            ProcessComponent(type.ToProcessableComponent(), TypeProcessors, logger);

            if (type.HasFields)
            {
                // needs to copy out, because processors could modify the collection
                var fields = type.Fields.ToArray();
                foreach (var field in fields)
                {
                    ProcessField(field);
                }
            }

            if (type.HasProperties)
            {
                // needs to copy out, because processors could modify the collection
                var properties = type.Properties.ToArray();
                foreach (var property in properties)
                {
                    ProcessProperty(property);
                }
            }

            if (type.HasMethods)
            {
                // needs to copy out, because processors could modify the collection
                var methods = type.Methods.ToArray();
                foreach (var method in methods)
                {
                    ProcessMethod(method);
                }
            }

            logger.Progress("Processing type done.");
        }

        private void ProcessField(FieldDefinition field)
        {
            ProcessComponent(field.ToProcessableComponent(), FieldProcessors, logger);
        }

        private void ProcessProperty(PropertyDefinition property)
        {
            ProcessComponent(property.ToProcessableComponent(), PropertyProcessors, logger);
        }

        private void ProcessMethod(MethodDefinition method)
        {
            logger.Notice($"Processing method: '{method.FullName}'.");
            ProcessComponent(method.ToProcessableComponent(), MethodProcessors, logger);

            // needs to copy out, because processors could modify the collection
            var parameters = method.Parameters.ToArray();
            foreach (var parameter in parameters)
            {
                ProcessParameter(parameter, method);
            }

            logger.Notice("Processing method done.");
        }

        private void ProcessParameter(ParameterDefinition parameter, MethodDefinition declaringMethod)
        {
            ProcessComponent(parameter.ToProcessableComponent(declaringMethod), ParameterProcessors, logger);
        }

        private static void ProcessComponent(
            IProcessableComponent component,
            IEnumerable<IComponentProcessor<ComponentProcessorConfiguration>> componentProcessors,
            ILogger logger)
        {
            foreach (var processor in componentProcessors)
            {
                try
                {
                    processor.Process(component);
                }
                catch (Exception e)
                {
                    logger.Error($"There was an error while processing '{component.FullName}' with processor '{processor}'. CurrentDir: '{Environment.CurrentDirectory}'. Exception: {e}");
                }
            }
        }

        private AssemblyDefinition LoadAssembly()
        {
            try
            {
                var assemblyResolver = new DefaultAssemblyResolver();
                assemblyResolver.AddSearchDirectory(Path.GetDirectoryName(assemblyPath));

                var symbolReaderProvider = new PdbReaderProvider();

                logger.Progress($"Loading assembly: '{assemblyPath}'");
                var readerParameters = new ReaderParameters(ReadingMode.Immediate)
                {
                    ReadSymbols = true,
                    AssemblyResolver = assemblyResolver,
                    SymbolReaderProvider = symbolReaderProvider
                };

                var assembly = AssemblyDefinition.ReadAssembly(assemblyPath, readerParameters);
                logger.Progress("Loading assembly done");
                return assembly;
            }
            catch (Exception e)
            {
                throw new InvalidOperationException($"Error while loading assembly '{assemblyPath}'.", e);
            }
        }
    }
}