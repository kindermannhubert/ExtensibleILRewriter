using System;
using System.Collections.Generic;
using Mono.Cecil;
using Mono.Cecil.Pdb;
using ExtensibleILRewriter.Extensions;
using System.IO;
using ExtensibleILRewriter.Logging;

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

        public List<IComponentProcessor<AssemblyProcessableComponent, ComponentProcessorConfiguration>> AssemblyProcessors { get; } = new List<IComponentProcessor<AssemblyProcessableComponent, ComponentProcessorConfiguration>>();

        public List<IComponentProcessor<ModuleProcessableComponent, ComponentProcessorConfiguration>> ModuleProcessors { get; } = new List<IComponentProcessor<ModuleProcessableComponent, ComponentProcessorConfiguration>>();

        public List<IComponentProcessor<TypeProcessableComponent, ComponentProcessorConfiguration>> TypeProcessors { get; } = new List<IComponentProcessor<TypeProcessableComponent, ComponentProcessorConfiguration>>();

        public List<IComponentProcessor<MethodProcessableComponent, ComponentProcessorConfiguration>> MethodProcessors { get; } = new List<IComponentProcessor<MethodProcessableComponent, ComponentProcessorConfiguration>>();

        public List<IComponentProcessor<MethodParameterProcessableComponent, ComponentProcessorConfiguration>> ParameterProcessors { get; } = new List<IComponentProcessor<MethodParameterProcessableComponent, ComponentProcessorConfiguration>>();

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

            // needs to copy out, because processors can modified the collection
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

            // needs to copy out, because processors can modified the collection
            var types = module.Types.ToArray();
            foreach (var type in types)
            {
                ProcessType(type);
            }

            logger.Progress("Processing module done.");
        }

        private void ProcessType(TypeDefinition type)
        {
            logger.Progress($"Processing type: '{type.Name}'.");
            ProcessComponent(type.ToProcessableComponent(), TypeProcessors, logger);

            // needs to copy out, because processors can modified the collection
            var methods = type.Methods.ToArray();
            foreach (var method in methods)
            {
                ProcessMethod(method);
            }

            logger.Progress("Processing type done.");
        }

        private void ProcessMethod(MethodDefinition method)
        {
            logger.Notice($"Processing method: '{method.FullName}'.");
            ProcessComponent(method.ToProcessableComponent(), MethodProcessors, logger);

            // needs to copy out, because processors can modified the collection
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

        private static void ProcessComponent<ProcessableComponentType>(
            ProcessableComponentType component,
            IEnumerable<IComponentProcessor<ProcessableComponentType, ComponentProcessorConfiguration>> componentProcessors,
            ILogger logger)
            where ProcessableComponentType : IProcessableComponent
        {
            foreach (var processor in componentProcessors)
            {
                try
                {
                    processor.Process(component);
                }
                catch (Exception e)
                {
                    logger.Error($"There was an error while processing '{component.FullName}' with processor '{processor}'. Exception: {e}");
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