using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mono.Cecil;
using Mono.Cecil.Pdb;
using ILTools.Extensions;
using System.IO;

namespace ILTools
{
    public class AssemblyRewriter
    {
        private readonly string assemblyPath;
        private readonly ILogger logger;

        public List<ComponentProcessor<AssemblyDefinition>> AssemblyProcessors { get; } = new List<ComponentProcessor<AssemblyDefinition>>();
        public List<ComponentProcessor<ModuleDefinition>> ModuleProcessors { get; } = new List<ComponentProcessor<ModuleDefinition>>();
        public List<ComponentProcessor<TypeDefinition>> TypeProcessors { get; } = new List<ComponentProcessor<TypeDefinition>>();
        public List<ComponentProcessor<MethodDefinition>> MethodProcessors { get; } = new List<ComponentProcessor<MethodDefinition>>();

        public AssemblyRewriter(string assemblyPath, ILogger logger = null)
        {
            this.assemblyPath = assemblyPath;
            this.logger = logger ?? new DummyLogger();
        }

        public void ProcessAssemblyAndSave(string rewrittenAssemblyPath)
        {
            var rewrittenAssembly = ProcessAssembly();

            logger.Progress("Saving assembly '\{rewrittenAssembly.FullName}' to '\{rewrittenAssemblyPath}'");
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
                logger.Error("Error while saving assembly '\{rewrittenAssemblyPath}'\{Environment.NewLine}\{e}.");
                throw;
            }
            logger.Progress("Saving assembly done");
        }

        public AssemblyDefinition ProcessAssembly()
        {
            var assembly = LoadAssembly();

            logger.Progress("Processing assembly: '\{assembly.FullName}'");
            ProcessComponent(assembly, AssemblyProcessors, logger);

            //needs to copy out, because processors can modified the collection
            var modules = assembly.Modules.ToArray();
            foreach (var module in modules) ProcessModule(module);

            return assembly;
        }

        private void ProcessModule(ModuleDefinition module)
        {
            logger.Notice("\tProcessing module: '\{module.Name}'");
            ProcessComponent(module, ModuleProcessors, logger);

            //needs to copy out, because processors can modified the collection
            var types = module.Types.ToArray();
            foreach (var type in types) ProcessType(type);
        }

        private void ProcessType(TypeDefinition type)
        {
            logger.Notice("\t\tProcessing type: '\{type.Name}'");
            ProcessComponent(type, TypeProcessors, logger);

            //needs to copy out, because processors can modified the collection
            var methods = type.Methods.ToArray();
            foreach (var method in methods) ProcessMethod(method);
        }

        private void ProcessMethod(MethodDefinition method)
        {
            logger.Notice("\t\t\tProcessing method: '\{method.FullName}'");
            ProcessComponent(method, MethodProcessors, logger);
        }

        private static void ProcessComponent<ComponentType>(ComponentType component, IEnumerable<ComponentProcessor<ComponentType>> componentProcessors, ILogger logger)
        {
            foreach (var processor in componentProcessors)
            {
                try
                {
                    processor.Process(component, logger);
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
                logger.Error("Error while loading assembly '\{assemblyPath}'\{Environment.NewLine}\{e}.");
                throw;
            }
        }
    }
}