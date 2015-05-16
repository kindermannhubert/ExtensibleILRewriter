using System;
using ExtensibleILRewriter.Processors.Parameters;

namespace TestApplication
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            A.Test(new object());
            A.Test(new object(), string.Empty);
            A.Test((int?)null);
            A.Test(new object(), null);

            Console.WriteLine("Done");
            Console.ReadLine();
        }
    }

    internal class A
    {
        public static void Test([NotNull]object o)
        {
            Console.WriteLine($"Is parameter null? {o == null}");
        }

        public static void Test([NotNull]object a, [NotNull]string b)
        {
            Console.WriteLine($"Is parameter null? a: {a == null}; b: {b == null}");
        }

        public static void Test([NotNull]int? i)
        {
            // Console.WriteLine("Is parameter null? \{a == null}");
        }

        public static void Test<T>([NotNull]T? i)
            where T : struct
        {
            // Console.WriteLine("Is parameter null? \{a == null}");
        }

        public static void IfObjectNull(object o)
        {
            if (o == null)
            {
                throw new ArgumentNullException("xxx");
            }
        }
    }
}
