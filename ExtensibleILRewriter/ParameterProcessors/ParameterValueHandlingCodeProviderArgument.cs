using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtensibleILRewriter.ParameterProcessors
{
    public struct ParameterValueHandlingCodeProviderArgument
    {
        public ParameterDefinition Parameter { get; }
        public MethodDefinition Method { get; }
        public FieldDefinition StateField { get; }

        public ParameterValueHandlingCodeProviderArgument(ParameterDefinition parameter, MethodDefinition method, FieldDefinition stateField)
        {
            Parameter = parameter;
            Method = method;
            StateField = stateField;
        }
    }
}
