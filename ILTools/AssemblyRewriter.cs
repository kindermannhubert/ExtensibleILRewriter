using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mono.Cecil;
using Mono.Cecil.Pdb;

namespace ILTools
{
    public class AssemblyRewriter
    {
        private readonly string assemblyPath;
        private readonly ILogger logger;

        public List<IMethodProcessor> MethodProcessors { get; } = new List<IMethodProcessor>();

        public AssemblyRewriter(string assemblyPath, ILogger logger = null)
        {
            this.assemblyPath = assemblyPath;
            this.logger = logger ?? new DummyLogger();
        }

        public AssemblyDefinition ProcessAssembly()
        {
            var assembly = LoadAssembly();

            logger.Progress("Processing assembly: '\{assembly.FullName}'");
            foreach (var module in assembly.Modules)
            {
                logger.Notice("\tProcessing module: '\{module.Name}'");
                foreach (var type in module.Types)
                {
                    logger.Notice("\t\tProcessing type: '\{type.Name}'");
                    foreach (var method in type.Methods)
                    {
                        logger.Notice("\t\t\tProcessing method: '\{method.FullName}'");
                        foreach (var rewriter in MethodProcessors)
                        {
                            rewriter.Process(method, logger);
                        }
                    }
                }
            }
            return assembly;
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

        private AssemblyDefinition LoadAssembly()
        {
            try
            {
                var assemblyResolver = new DefaultAssemblyResolver();
                assemblyResolver.AddSearchDirectory(assemblyPath);

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