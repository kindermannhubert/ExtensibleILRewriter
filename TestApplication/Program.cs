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
            A.Test(null);

            Console.WriteLine("Done");
            Console.ReadLine();
        }
    }

    class A
    {
        public static void Test([NotNull]object o)
        {
            Console.WriteLine("Is parameter null? \{o == null}");
        }
    }
}
