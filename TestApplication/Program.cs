using ILTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TestApplication
{
    class Program
    {
        private static void Main(string[] args)
        {
            //A.Test();
            A.Test(new object(), null);

            Console.WriteLine("Done");
            Console.ReadLine();
        }
    }

    class A
    {
        public static void Test([NotNull]object o)
        {
            //Console.WriteLine("Is parameter null? \{o == null}");
        }

        public static void Test([NotNull]object a, [NotNull]string b)
        {
            //Console.WriteLine("Is parameter null? \{a == null}");
        }

        public static void IfObjectNull(object o)
        {
            if (o == null) throw new ArgumentNullException("xxx");
        }
    }
}
