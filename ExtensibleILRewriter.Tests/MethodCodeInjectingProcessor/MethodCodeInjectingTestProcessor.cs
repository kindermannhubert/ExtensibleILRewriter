using System;
using ExtensibleILRewriter.Logging;
using ExtensibleILRewriter.Processors.Methods;
using ExtensibleILRewriter.Processors.Parameters;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExtensibleILRewriter.Tests.MethodCodeInjectingProcessor
{
    public class MethodCodeInjectingTestProcessor : MethodCodeInjectingProcessor<MethodCodeInjectingProcessorConfiguration>
    {
        public MethodCodeInjectingTestProcessor([NotNull]MethodCodeInjectingProcessorConfiguration configuration, [NotNull]ILogger logger)
            : base(configuration, logger)
        {
        }

        protected override MethodInjectionPlace GetInjectionPlace(MethodProcessableComponent method)
        {
            var name = method.Name;
            if (name.StartsWith(MethodInjectionTestCodeProvider.InjectAtBeginingPrefix))
            {
                return MethodInjectionPlace.Begining;
            }
            else if (name.StartsWith(MethodInjectionTestCodeProvider.InjectOnExitPrefix))
            {
                return MethodInjectionPlace.Exit;
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        protected override string GetStateInstanceName(MethodProcessableComponent method)
        {
            return "TestsInjectionState";
        }
    }
}
