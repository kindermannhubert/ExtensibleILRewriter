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
        private readonly ILogger logger;
        private readonly AssemblyDefinition assembly;

        public List<IMethodProcessor> MethodProcessors { get; } = new List<IMethodProcessor>();

        public AssemblyRewriter(string assemblyPath, ILogger logger = null)
        {
            if (logger == null) logger = new DummyLogger();

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

            assembly = AssemblyDefinition.ReadAssembly(assemblyPath, readerParameters);
            logger.Progress("Loading assembly done");
        }

        public AssemblyDefinition ProcessAssembly()
        {
            logger.Progress("Processing assembly: '\{assembly.FullName}'");
            foreach (var module in assembly.Modules)
            {
                logger.Notice("Processing module: '\{module.Name}'");
                foreach (var type in module.Types)
                {
                    logger.Notice("Processing type: '\{type.Name}'");
                    foreach (var method in type.Methods)
                    {
                        logger.Notice("Processing name: '\{method.Name}'");
                        foreach (var rewriter in MethodProcessors) rewriter.Rewrite(method);
                    }
                }
            }
            return assembly;
        }

        public void ProcessAssemblyAndSave(string rewrittenAssemblyPath)
        {
            var rewrittenAssembly = ProcessAssembly();

            logger.Progress("Saving assembly '\{assembly.FullName}' to '\{rewrittenAssemblyPath}'");
            var symbolWriterProvider = new PdbWriterProvider();
            var writerParameters = new WriterParameters()
            {
                WriteSymbols = true,
                SymbolWriterProvider = symbolWriterProvider
            };
            rewrittenAssembly.Write(rewrittenAssemblyPath, new WriterParameters() { WriteSymbols = true });
            logger.Progress("Saving assembly done");
        }
    }
}