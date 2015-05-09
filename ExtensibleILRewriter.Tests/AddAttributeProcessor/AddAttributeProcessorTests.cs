using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ExtensibleILRewriter.Tests.AddAttributeProcessor
{
    [TestClass]
    public class AddAttributeProcessorTests
    {
        public const string InjectAttributePrefix = "Decorate_";

        [TestMethod]
        public void AddAttributeToTypeTest()
        {
            foreach (var type in GetTestingTypes())
            {
                var name = type.Name;
                if (name.StartsWith(InjectAttributePrefix))
                {
                    var attribute = type.CustomAttributes.Where(a => a.AttributeType == typeof(InjectedAttribute)).Single();
                    CheckAttribute(attribute, name, ProcessableComponentType.Type, type);
                }
                else
                {
                    Assert.IsTrue(type.CustomAttributes.All(a => a.AttributeType != typeof(InjectedAttribute)));
                }
            }
        }

        [TestMethod]
        public void AddAttributeToMethodTest()
        {
            foreach (var type in GetTestingTypes())
            {
                foreach (var method in type.GetMethods())
                {
                    var name = method.Name;
                    if (name.StartsWith(InjectAttributePrefix))
                    {
                        var attribute = method.CustomAttributes.Single();
                        CheckAttribute(attribute, name, ProcessableComponentType.Method, null);
                    }
                    else
                    {
                        Assert.IsTrue(method.CustomAttributes.All(a => a.AttributeType != typeof(InjectedAttribute)));
                    }
                }
            }
        }

        [TestMethod]
        public void AddAttributeToMethodParameterTest()
        {
            foreach (var type in GetTestingTypes())
            {
                foreach (var method in type.GetMethods())
                {
                    foreach (var methodParameter in method.GetParameters())
                    {
                        var name = methodParameter.Name;
                        if (name.StartsWith(InjectAttributePrefix))
                        {
                            var attribute = methodParameter.CustomAttributes.Single();
                            CheckAttribute(attribute, name, ProcessableComponentType.MethodParameter, null);
                        }
                        else
                        {
                            Assert.IsTrue(methodParameter.CustomAttributes.All(a => a.AttributeType != typeof(InjectedAttribute)));
                        } 
                    }
                }
            }
        }

        private static IEnumerable<Type> GetTestingTypes()
        {
            return Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(t => t.FullName.StartsWith("ExtensibleILRewriter.Tests.AddAttributeProcessor"));
        }

        private static void CheckAttribute(CustomAttributeData att, string componentName, ProcessableComponentType componentType, Type typeType)
        {
            Assert.AreEqual(typeof(InjectedAttribute), att.AttributeType);
            Assert.AreEqual(4, att.ConstructorArguments.Count);

            Assert.AreEqual(typeof(ProcessableComponentType), att.ConstructorArguments[0].ArgumentType);
            Assert.AreEqual((int)componentType, att.ConstructorArguments[0].Value);

            Assert.AreEqual(typeof(Type), att.ConstructorArguments[1].ArgumentType);
            Assert.AreEqual(typeType, att.ConstructorArguments[1].Value);

            Assert.AreEqual(typeof(int), att.ConstructorArguments[2].ArgumentType);
            Assert.AreEqual(componentName.GetHashCode(), att.ConstructorArguments[2].Value);

            Assert.AreEqual(typeof(string), att.ConstructorArguments[3].ArgumentType);
            Assert.AreEqual(componentName, att.ConstructorArguments[3].Value);
        }
    }
}
