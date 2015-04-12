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

        public List<IComponentProcessor<AssemblyDefinition, NoDeclaringComponent, ComponentProcessorConfiguration>> AssemblyProcessors { get; } = new List<IComponentProcessor<AssemblyDefinition, NoDeclaringComponent, ComponentProcessorConfiguration>>();
        public List<IComponentProcessor<ModuleDefinition, AssemblyDefinition, ComponentProcessorConfiguration>> ModuleProcessors { get; } = new List<IComponentProcessor<ModuleDefinition, AssemblyDefinition, ComponentProcessorConfiguration>>();
        public List<IComponentProcessor<TypeDefinition, ModuleDefinition, ComponentProcessorConfiguration>> TypeProcessors { get; } = new List<IComponentProcessor<TypeDefinition, ModuleDefinition, ComponentProcessorConfiguration>>();
        public List<IComponentProcessor<MethodDefinition, TypeDefinition, ComponentProcessorConfiguration>> MethodProcessors { get; } = new List<IComponentProcessor<MethodDefinition, TypeDefinition, ComponentProcessorConfiguration>>();
        public List<IComponentProcessor<ParameterDefinition, MethodDefinition, ComponentProcessorConfiguration>> ParameterProcessors { get; } = new List<IComponentProcessor<ParameterDefinition, MethodDefinition, ComponentProcessorConfiguration>>();

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
            ProcessComponent(assembly, null, AssemblyProcessors, logger);

            //needs to copy out, because processors can modified the collection
            var modules = assembly.Modules.ToArray();
            foreach (var module in modules) ProcessModule(module, assembly);

            logger.Progress("Processing assembly done.");
            return assembly;
        }

        private void ProcessModule(ModuleDefinition module, AssemblyDefinition declaringAssembly)
        {
            logger.Progress("Processing module: '\{module.Name}'.");
            ProcessComponent(module, declaringAssembly, ModuleProcessors, logger);

            //needs to copy out, because processors can modified the collection
            var types = module.Types.ToArray();
            foreach (var type in types) ProcessType(type, module);

            logger.Progress("Processing module done.");
        }

        private void ProcessType(TypeDefinition type, ModuleDefinition declaringModule)
        {
            logger.Progress("Processing type: '\{type.Name}'.");
            ProcessComponent(type, declaringModule, TypeProcessors, logger);

            //needs to copy out, because processors can modified the collection
            var methods = type.Methods.ToArray();
            foreach (var method in methods) ProcessMethod(method, type);

            logger.Progress("Processing type done.");
        }

        private void ProcessMethod(MethodDefinition method, TypeDefinition declaringType)
        {
            logger.Notice("Processing method: '\{method.FullName}'.");
            ProcessComponent(method, declaringType, MethodProcessors, logger);

            //needs to copy out, because processors can modified the collection
            var parameters = method.Parameters.ToArray();
            foreach (var parameter in parameters) ProcessParameter(parameter, method);

            logger.Notice("Processing method done.");
        }

        private void ProcessParameter(ParameterDefinition parameter, MethodDefinition declaringMethod)
        {
            ProcessComponent(parameter, declaringMethod, ParameterProcessors, logger);
        }

        private static void ProcessComponent<ComponentType, DeclaringComponentType>(
            ComponentType component,
            DeclaringComponentType declaringComponent,
            IEnumerable<IComponentProcessor<ComponentType, DeclaringComponentType, ComponentProcessorConfiguration>> componentProcessors,
            ILogger logger)
        {
            foreach (var processor in componentProcessors)
            {
                try
                {
                    processor.Process(component, declaringComponent);
                }
                catch (Exception e)
                {
                    logger.Error("There was an error while processing \{typeof(ComponentType).Name}: '\{component}' with processor '\{processor}'. Exception: \{e}");
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