using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace ExtensibleILRewriter
{
    public static class StateInstancesManager
    {
        private static readonly object Sync = new object();
        private static readonly Dictionary<string, object> Instaces = new Dictionary<string, object>();
        private static readonly Dictionary<string, IntPtr> InstanceHolderFieldAddresses = new Dictionary<string, IntPtr>();
        private static Action<IntPtr, object> setInstanceToStaticFieldAddress;

        public static void ClearInstanceRegistrations()
        {
            lock (Sync)
            {
                Instaces.Clear();

                foreach (var staticFieldAddress in InstanceHolderFieldAddresses.Values)
                {
                    SetInstanceToStaticFieldAddress(staticFieldAddress, null);
                }
            }
        }

        public static void RegisterInstance(string instanceName, object instance)
        {
            lock (Sync)
            {
                if (Instaces.ContainsKey(instanceName))
                {
                    throw new InvalidOperationException($"Instance with name '{instanceName}' was already registered.");
                }

                Instaces.Add(instanceName, instance);

                IntPtr staticFieldAddress;
                if (InstanceHolderFieldAddresses.TryGetValue(instanceName, out staticFieldAddress))
                {
                    SetInstanceToStaticFieldAddress(staticFieldAddress, instance);
                }
            }
        }

        public static void RegisterInstanceHolderFieldAddress(string instanceName, IntPtr instanceHolderFieldAddress)
        {
            lock (Sync)
            {
                if (InstanceHolderFieldAddresses.ContainsKey(instanceName))
                {
                    throw new InvalidOperationException($"Instance holder field address was already added for instanceName = '{instanceName}'.");
                }
                else
                {
                    InstanceHolderFieldAddresses.Add(instanceName, instanceHolderFieldAddress);

                    object alreadyExistingInstance;
                    if (Instaces.TryGetValue(instanceName, out alreadyExistingInstance))
                    {
                        SetInstanceToStaticFieldAddress(instanceHolderFieldAddress, alreadyExistingInstance);
                    }
                }
            }
        }

        private static void SetInstanceToStaticFieldAddress(IntPtr address, object instance)
        {
            if (setInstanceToStaticFieldAddress == null)
            {
                var dynamicMethod = new DynamicMethod(nameof(setInstanceToStaticFieldAddress), null, new Type[] { typeof(IntPtr), typeof(object) }, MethodInfo.GetCurrentMethod().Module);
                var ilGen = dynamicMethod.GetILGenerator();

                ilGen.Emit(OpCodes.Ldarg_0);
                ilGen.Emit(OpCodes.Ldarg_1);
                ilGen.Emit(OpCodes.Stind_Ref);
                ilGen.Emit(OpCodes.Ret);

                setInstanceToStaticFieldAddress = (Action<IntPtr, object>)dynamicMethod.CreateDelegate(typeof(Action<IntPtr, object>));
            }

            setInstanceToStaticFieldAddress(address, instance);
        }
    }
}
