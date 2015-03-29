using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mono.Cecil;

namespace ILTools
{
    public class AssemblyRewriter
    {
        private readonly AssemblyDefinition assembly;

        public List<IMethodRewriter> MethodRewriters { get; } = new List<IMethodRewriter>();

        public AssemblyRewriter(string assemblyPath)
        {
            assembly = AssemblyDefinition.ReadAssembly(assemblyPath, new ReaderParameters(ReadingMode.Immediate) { ReadSymbols = true });
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
            rewrittenAssembly.Write(rewrittenAssemblyPath, new WriterParameters() { WriteSymbols = true });
        }
    }
}