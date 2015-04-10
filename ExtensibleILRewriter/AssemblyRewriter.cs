using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mono.Cecil;
using Mono.Cecil.Pdb;
using ExtensibleILRewriter.Extensions;
using System.IO;

namespace ExtensibleILRewriter
{
    public class AssemblyRewriter
    {
        private readonly string assemblyPath;
        private readonly ILogger logger;

        public List<IComponentProcessor<AssemblyDefinition, ComponentProcessorConfiguration>> AssemblyProcessors { get; } = new List<IComponentProcessor<AssemblyDefinition, ComponentProcessorConfiguration>>();
        public List<IComponentProcessor<ModuleDefinition, ComponentProcessorConfiguration>> ModuleProcessors { get; } = new List<IComponentProcessor<ModuleDefinition, ComponentProcessorConfiguration>>();
        public List<IComponentProcessor<TypeDefinition, ComponentProcessorConfiguration>> TypeProcessors { get; } = new List<IComponentProcessor<TypeDefinition, ComponentProcessorConfiguration>>();
        public List<IComponentProcessor<MethodDefinition, ComponentProcessorConfiguration>> MethodProcessors { get; } = new List<IComponentProcessor<MethodDefinition, ComponentProcessorConfiguration>>();

        public AssemblyRewriter(string assemblyPath, ILogger logger = null)
        {
            this.assemblyPath = assemblyPath;
            this.logger = logger ?? new DummyLogger();
        }

        public void ProcessAssemblyAndSave(string rewrittenAssemblyPath)
        {
            var rewrittenAssembly = ProcessAssembly();

            logger.Progress("Saving assembly '\{rewrittenAssembly.FullName}' to '\{rewrittenAssemblyPath}'.");
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
                throw new InvalidOperationException("Error while saving assembly '\{rewrittenAssemblyPath}'.", e);
            }
            logger.Progress("Saving assembly done.");
        }

        public AssemblyDefinition ProcessAssembly()
        {
            var assembly = LoadAssembly();

            logger.Progress("Processing assembly: '\{assembly.FullName}'.");
            ProcessComponent(assembly, AssemblyProcessors, logger);

            //needs to copy out, because processors can modified the collection
            var modules = assembly.Modules.ToArray();
            foreach (var module in modules) ProcessModule(module);

            logger.Progress("Processing assembly done.");
            return assembly;
        }

        private void ProcessModule(ModuleDefinition module)
        {
            logger.Progress("Processing module: '\{module.Name}'.");
            ProcessComponent(module, ModuleProcessors, logger);

            //needs to copy out, because processors can modified the collection
            var types = module.Types.ToArray();
            foreach (var type in types) ProcessType(type);

            logger.Progress("Processing module done.");
        }

        private void ProcessType(TypeDefinition type)
        {
            logger.Progress("Processing type: '\{type.Name}'.");
            ProcessComponent(type, TypeProcessors, logger);

            //needs to copy out, because processors can modified the collection
            var methods = type.Methods.ToArray();
            foreach (var method in methods) ProcessMethod(method);

            logger.Progress("Processing type done.");
        }

        private void ProcessMethod(MethodDefinition method)
        {
            logger.Notice("Processing method: '\{method.FullName}'.");
            ProcessComponent(method, MethodProcessors, logger);
            logger.Notice("Processing method done.");
        }

        private static void ProcessComponent<ComponentType>(ComponentType component, IEnumerable<IComponentProcessor<ComponentType, ComponentProcessorConfiguration>> componentProcessors, ILogger logger)
        {
            foreach (var processor in componentProcessors)
            {
                try
                {
                    processor.Process(component);
                }
                catch (Exception e)
                {
                    logger.Error("There was an error while processing '\{component}' with processor '\{processor}'. Exception: \{e}");
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

                logger.Progress("Loading assembly: '\{assemblyPath}'");
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
                throw new InvalidOperationException("Error while loading assembly '\{assemblyPath}'.", e);
            }
        }
    }
}