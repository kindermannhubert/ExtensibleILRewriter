using ExtensibleILRewriter;
using ExtensibleILRewriter.MethodProcessors.Helpers;
using System;
using ExtensibleILRewriter.Contracts;

namespace TestApplication
{
    class Program
    {
        private static void Main(string[] args)
        {
            A.Test(new object());
            A.Test((int?)null);
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

        public static void Test([NotNull]int? i)
        {
            //Console.WriteLine("Is parameter null? \{a == null}");
        }

        public static void Test<T>([NotNull]T? i)
            where T : struct
        {
            //Console.WriteLine("Is parameter null? \{a == null}");
        }

        [MakeStaticVersion("__static_" + nameof(HandleParameter))]
        public void HandleParameter<ParameterType>(object state, ParameterType parameter, string parameterName)
        {
            if (parameter == null) throw new ArgumentNullException(parameterName);
        }

        public static void IfObjectNull(object o)
        {
            if (o == null) throw new ArgumentNullException("xxx");
        }

        //[MakeStaticVersion("__static_" + nameof(Hello))]
        //public void Hello(string text)
        //{
        //    Console.WriteLine(text);
        //}
    }


    //public class NotNullArgumentHandligCodeProvider<ArgumentType>
    //{
    //    [MakeStaticVersion("__static_" + nameof(HandleArgument))]
    //    public void HandleArgument(ArgumentType argument, string argumentName)
    //    {
    //        if (argument == null) throw new ArgumentNullException(argumentName);
    //    }
    //}

    //public interface II
    //{
    //    void Foo([NotNull] object o);
    //}
}
