using System;

namespace ExtensibleILRewriter
{
    public struct StateInstanceId : IEquatable<StateInstanceId>
    {
        public StateInstanceId(string instanceName)
        {
            Name = instanceName;
        }

        public string Name { get; }

        public override string ToString()
        {
            return Name;
        }

        public string Serialize()
        {
            return Name;
        }

        public static StateInstanceId Deserialize(string serializedStateInstanceId)
        {
            return new StateInstanceId(serializedStateInstanceId);
        }

        public bool Equals(StateInstanceId other)
        {
            return Name == other.Name;
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }
    }
}
