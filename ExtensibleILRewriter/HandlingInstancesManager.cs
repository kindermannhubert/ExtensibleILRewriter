using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace ExtensibleILRewriter
{
    public static class HandlingInstancesManager
    {
        private static readonly object sync = new object();
        private static readonly Dictionary<string, object> instaces = new Dictionary<string, object>();
        private static readonly Dictionary<string, IntPtr> instanceHolderFieldAddresses = new Dictionary<string, IntPtr>();

        public static void RegisterInstance(string instanceName, object instance)
        {
            lock (sync)
            {
                if (instaces.ContainsKey(instanceName)) throw new InvalidOperationException("Instance with name '\{instanceName}' was already registered.");
                instaces.Add(instanceName, instance);

                IntPtr staticFieldAddress;
                if (instanceHolderFieldAddresses.TryGetValue(instanceName, out staticFieldAddress))
                {
                    SetInstanceToStaticFieldAddress(staticFieldAddress, instance);
                }
            }
        }

        public static void RegisterInstanceHolderFieldAddress(string instanceName, IntPtr instanceHolderFieldAddress)
        {
            lock (sync)
            {
                if (instanceHolderFieldAddresses.ContainsKey(instanceName))
                {
                    throw new InvalidOperationException("Instance holder field address was already added for instanceName = '\{instanceName}'.");
                }
                else
                {
                    instanceHolderFieldAddresses.Add(instanceName, instanceHolderFieldAddress);

                    object alreadyExistingInstance;
                    if (instaces.TryGetValue(instanceName, out alreadyExistingInstance))
                    {
                        SetInstanceToStaticFieldAddress(instanceHolderFieldAddress, alreadyExistingInstance);
                    }
                }
            }
        }

        private static Action<IntPtr, object> _setInstanceToStaticFieldAddress;
        private static void SetInstanceToStaticFieldAddress(IntPtr address, object instance)
        {
            if (_setInstanceToStaticFieldAddress == null)
            {
                var dynamicMethod = new DynamicMethod(nameof(_setInstanceToStaticFieldAddress), null, new Type[] { typeof(IntPtr), typeof(object) });
                var ilGen = dynamicMethod.GetILGenerator();

                ilGen.Emit(OpCodes.Ldarg_0);
                ilGen.Emit(OpCodes.Ldarg_1);
                ilGen.Emit(OpCodes.Stind_Ref);

                _setInstanceToStaticFieldAddress = (Action<IntPtr, object>)dynamicMethod.CreateDelegate(typeof(Action<IntPtr, object>));
            }
            _setInstanceToStaticFieldAddress(address, instance);
        }
    }
}
