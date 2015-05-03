//using ExtensibleILRewriter.CodeInjection;
//using ExtensibleILRewriter.Processors.Modules;
//using ExtensibleILRewriter.Tests;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using System;

//namespace ExtensibleILRewriter.Tests
//{
//    [TestClass]
//    public class AddAttributeToComponentTests
//    {
//        [TestMethod]
//        public void AddAttributeToMethod1()
//        {
//        }

//        [TestMethod]
//        public void TestModuleInitializer2()
//        {
//        }

//        class __deco_DecorableClass1
//        {
//            public void __deco_DecorableClass1Method(int __deco_i)
//            {
//            }
//        }

//        [AttributeUsage(AttributeTargets.All)]
//        class InjectedAttribute : Attribute
//        {
//            public InjectedAttribute(int a1, string a2)
//            {
//            }
//        }

//        public class CustomAssemblyInfoAttributeProvider : AttributeProvider
//        {
//            protected override AttributeProviderAttributeArgument[] GetAttributeArguments(IProcessableComponent component)
//            {
//                return new AttributeProviderAttributeArgument[]
//                {
//                    AttributeProviderAttributeArgument.CreateParameterArgument("a1", 123456),
//                    AttributeProviderAttributeArgument.CreateParameterArgument("a2", "hello")
//                };
//            }

//            protected override Type GetAttributeType(IProcessableComponent component)
//            {
//                return typeof(InjectedAttribute);
//            }

//            protected override bool ShouldBeInjected(IProcessableComponent component)
//            {
//                return component.Name.StartsWith("__deco_");
//            }
//        }
//    }
//}
