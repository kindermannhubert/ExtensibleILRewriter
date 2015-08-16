using System;

#pragma warning disable

namespace ExtensibleILRewriter.Tests.AddAttributeProcessor
{
    public class Decorate1_TestClass1
    {
        private string field1;
        private double field2;
        private Type Decorate1_Field1;
        private double Decorate1_Field2;

        public Decorate1_TestClass1()
        {
            field1 = null;
            field2 = 0;
            Decorate1_Field1 = null;
            Decorate1_Field2 = 0;
        }

        protected int Decorate1_Count { get; set; }

        private static int Decorate1_StaticCount { get; set; }

        public void Decorate1_DecorableClass1Method(int param1, string Decorate1_param2)
        {
        }

        public static void Decorate1_DecorableClass2Method(int Decorate1_param1, string param2)
        {
        }
    }

    public class Decorate2_TestClass2
    {
        private string field1;
        private double field2;
        private Type Decorate2_Field1;
        private double Decorate2_Field2;

        public Decorate2_TestClass2()
        {
            field1 = null;
            field2 = 0;
            Decorate2_Field1 = null;
            Decorate2_Field2 = 0;
        }

        protected int Decorate2_Count { get; set; }

        private static int Decorate2_StaticCount { get; set; }

        public void Decorate2_DecorableClass1Method(int param1, string Decorate2_param2)
        {
        }

        public static void Decorate2_DecorableClass2Method(int Decorate2_param1, string param2)
        {
        }
    }
}
