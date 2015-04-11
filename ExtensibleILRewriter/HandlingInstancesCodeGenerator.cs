using ExtensibleILRewriter.Extensions;
using ExtensibleILRewriter.ModuleProcessors;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Collections.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtensibleILRewriter
{
    public static class HandlingInstancesCodeGenerator
    {
        private const string NamespaceName = "__ExtensibleILRewriter";
        private const string TypeName = "__HandlingInstacesHolder";

        public static FieldDefinition PrepareInstanceHoldingField(ModuleDefinition moduleToExtend, Type fieldType, string fieldName, string instanceRegistrationName)
        {
            var holderType = moduleToExtend.Types.FirstOrDefault(t => t.Name == TypeName && t.Namespace == NamespaceName);
            if (holderType == null)
            {
                holderType = new TypeDefinition(NamespaceName, TypeName, TypeAttributes.AutoClass | TypeAttributes.Abstract | TypeAttributes.Sealed | TypeAttributes.BeforeFieldInit, moduleToExtend.TypeSystem.Object);
                moduleToExtend.Types.Add(holderType);
            }

            var holderField = holderType.Fields.FirstOrDefault(f => f.Name == fieldName);
            if (holderField == null)
            {
                holderField = new FieldDefinition(fieldName, FieldAttributes.Public | FieldAttributes.Static, moduleToExtend.Import(fieldType));
                holderType.Fields.Add(holderField);

                RegisterInstanceRegisteredEvent(moduleToExtend, instanceRegistrationName, holderField);
            }

            return holderField;
        }

        private static void RegisterInstanceRegisteredEvent(ModuleDefinition module, string instanceName, FieldDefinition instanceHolderField)
        {
            var moduleInitializer = ModuleInitializerProcessor.FindOrCreateInitializer(module);

            Action<string, IntPtr> registerMethod = HandlingInstancesManager.RegisterInstanceHolderFieldAddress;
            var registerMethodReference = module.Import(registerMethod.Method);

            var newInstructions = new Collection<Instruction>();
            newInstructions.Add(Instruction.Create(OpCodes.Ldstr, instanceName));
            newInstructions.Add(Instruction.Create(OpCodes.Ldsflda, instanceHolderField));
            newInstructions.Add(Instruction.Create(OpCodes.Call, registerMethodReference));

            moduleInitializer.Body.AddInstructionsToBegining(newInstructions);
        }
    }
}
