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
        private readonly AssemblyDefinition assembly;

        public List<IMethodRewriter> MethodRewriters { get; } = new List<IMethodRewriter>();

        public AssemblyRewriter(string assemblyPath)
        {
            var assemblyResolver = new DefaultAssemblyResolver();
            assemblyResolver.AddSearchDirectory(assemblyPath);

            var symbolReaderProvider = new PdbReaderProvider();

            var readerParameters = new ReaderParameters(ReadingMode.Immediate)
            {
                ReadSymbols = true,
                AssemblyResolver = assemblyResolver,
                SymbolReaderProvider = symbolReaderProvider
            };
            assembly = AssemblyDefinition.ReadAssembly(assemblyPath, readerParameters);
        }

        public AssemblyDefinition Rewrite()
        {
            foreach (var module in assembly.Modules)
            {
                foreach (var type in module.Types)
                {
                    foreach (var method in type.Methods)
                    {
                        foreach (var rewriter in MethodRewriters) rewriter.Rewrite(method);
                    }
                }
            }
            return assembly;
        }

        public void RewriteAndSave(string rewrittenAssemblyPath)
        {
            var rewrittenAssembly = Rewrite();

            var symbolWriterProvider = new PdbWriterProvider();
            var writerParameters = new WriterParameters()
            {
                WriteSymbols = true,
                SymbolWriterProvider = symbolWriterProvider
            };
            rewrittenAssembly.Write(rewrittenAssemblyPath, new WriterParameters() { WriteSymbols = true });
        }
    }
}