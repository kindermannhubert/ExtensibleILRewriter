using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace ExtensibleILRewriter
{
    public static class StateInstancesManager
    {
        private static readonly object Sync = new object();
        private static readonly Dictionary<StateInstanceId, object> Instaces = new Dictionary<StateInstanceId, object>();
        private static readonly Dictionary<StateInstanceId, IntPtr> InstanceHolderFieldAddresses = new Dictionary<StateInstanceId, IntPtr>();
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

        public static void RegisterInstance(StateInstanceId id, object instance)
        {
            lock (Sync)
            {
                if (Instaces.ContainsKey(id))
                {
                    throw new InvalidOperationException($"Instance with {nameof(id)} = '{id}' was already registered.");
                }

                Instaces.Add(id, instance);

                IntPtr staticFieldAddress;
                if (InstanceHolderFieldAddresses.TryGetValue(id, out staticFieldAddress))
                {
                    SetInstanceToStaticFieldAddress(staticFieldAddress, instance);
                }
            }
        }

        public static void RegisterInstanceHolderFieldAddress(StateInstanceId id, IntPtr instanceHolderFieldAddress)
        {
            lock (Sync)
            {
                if (InstanceHolderFieldAddresses.ContainsKey(id))
                {
                    throw new InvalidOperationException($"Instance holder field address was already added for {nameof(id)} = '{id}'.");
                }
                else
                {
                    InstanceHolderFieldAddresses.Add(id, instanceHolderFieldAddress);

                    object alreadyExistingInstance;
                    if (Instaces.TryGetValue(id, out alreadyExistingInstance))
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
