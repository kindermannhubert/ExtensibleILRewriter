using ILTools;
using ILTools.MethodProcessors.Contracts;
using ILTools.MethodProcessors.Helpers;
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
            A.Test(new object());
            A.Test(new object(), "");
            A.Test(new object(), null);

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

        public static void Test([NotNull]object a, [NotNull]string b)
        {
            Console.WriteLine("Is parameter null? a: \{a == null}; b: \{b == null}");
        }

        //public static void Test([NotNull]int i, [NotNull]string b)
        //{
        //    //Console.WriteLine("Is parameter null? \{a == null}");
        //}

        public static void IfObjectNull(object o)
        {
            if (o == null) throw new ArgumentNullException("xxx");
        }

        [MakeStaticVersion("__static_" + nameof(Hello))]
        public void Hello(string text)
        {
            Console.WriteLine(text);
        }
    }


    public class NotNullArgumentHandligCodeProvider<ArgumentType>
    {
        [MakeStaticVersion("__static_" + nameof(HandleArgument))]
        public void HandleArgument(ArgumentType argument, string argumentName)
        {
            if (argument == null) throw new ArgumentNullException(argumentName);
        }
    }

    //public interface II
    //{
    //    void Foo([NotNull] object o);
    //}
}
