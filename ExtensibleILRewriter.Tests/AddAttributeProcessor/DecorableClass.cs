using System;

#pragma warning disable

namespace ExtensibleILRewriter.Tests.AddAttributeProcessor
{
    public class Decorate_DecorableClass1
    {
        private string field1;
        private double field2;
        private Type Decorate_Field1;
        private double Decorate_Field2;

        public Decorate_DecorableClass1()
        {
            field1 = null;
            field2 = 0;
            Decorate_Field1 = null;
            Decorate_Field2 = 0;
        }

        protected int Decorate_Count { get; set; }

        private static int Decorate_StaticCount { get; set; }

        public void Decorate_DecorableClass1Method(int param1, string Decorate_param2)
        {
        }

        public static void Decorate_DecorableClass2Method(int Decorate_param1, string param2)
        {
        }
    }
}
