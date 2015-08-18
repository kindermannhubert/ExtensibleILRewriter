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
        public const string InjectAttribute1Prefix = "Decorate1_";
        public const string InjectAttribute2Prefix = "Decorate2_";

        [TestMethod]
        public void AddAttributeToTypeTest()
        {
            foreach (var type in GetTestingTypes())
            {
                var name = type.Name;
                CheckType(type, name, InjectAttribute1Prefix, typeof(Injected1Attribute));
                CheckType(type, name, InjectAttribute2Prefix, typeof(Injected2Attribute));
            }
        }

        [TestMethod]
        public void AddAttributeToFieldTest()
        {
            foreach (var type in GetTestingTypes())
            {
                foreach (var field in type.GetFields())
                {
                    var name = field.Name;
                    CheckField(field, name, InjectAttribute1Prefix, typeof(Injected1Attribute));
                    CheckField(field, name, InjectAttribute2Prefix, typeof(Injected2Attribute));
                }
            }
        }

        [TestMethod]
        public void AddAttributeToPropertyTest()
        {
            foreach (var type in GetTestingTypes())
            {
                foreach (var property in type.GetProperties())
                {
                    var name = property.Name;
                    CheckProperty(property, name, InjectAttribute1Prefix, typeof(Injected1Attribute));
                    CheckProperty(property, name, InjectAttribute2Prefix, typeof(Injected2Attribute));
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
                    CheckMethod(method, name, InjectAttribute1Prefix, typeof(Injected1Attribute));
                    CheckMethod(method, name, InjectAttribute2Prefix, typeof(Injected2Attribute));
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
                        CheckMethodParameter(methodParameter, name, InjectAttribute1Prefix, typeof(Injected1Attribute));
                        CheckMethodParameter(methodParameter, name, InjectAttribute2Prefix, typeof(Injected2Attribute));
                    }
                }
            }
        }

        private static void CheckType(Type type, string name, string decorationPrefix, Type attributeType)
        {
            if (name.StartsWith(decorationPrefix))
            {
                var attribute = type.CustomAttributes.Where(a => a.AttributeType == attributeType).Single();
                CheckAttribute(attribute, attributeType, name, ProcessableComponentType.Type, type);
            }
            else
            {
                Assert.IsTrue(type.CustomAttributes.All(a => a.AttributeType != attributeType));
            }
        }

        private static void CheckProperty(PropertyInfo property, string name, string decorationPrefix, Type attributeType)
        {
            if (name.StartsWith(decorationPrefix))
            {
                var attribute = property.CustomAttributes.Single();
                CheckAttribute(attribute, attributeType, name, ProcessableComponentType.Property, null);
            }
            else
            {
                Assert.IsTrue(property.CustomAttributes.All(a => a.AttributeType != attributeType));
            }
        }

        private static void CheckMethod(MethodInfo method, string name, string decorationPrefix, Type attributeType)
        {
            if (name.StartsWith(decorationPrefix))
            {
                var attribute = method.CustomAttributes.Single();
                CheckAttribute(attribute, attributeType, name, ProcessableComponentType.Method, null);
            }
            else
            {
                Assert.IsTrue(method.CustomAttributes.All(a => a.AttributeType != attributeType));
            }
        }

        private static void CheckField(FieldInfo field, string name, string decorationPrefix, Type attributeType)
        {
            if (name.StartsWith(decorationPrefix))
            {
                var attribute = field.CustomAttributes.Single();
                CheckAttribute(attribute, attributeType, name, ProcessableComponentType.Field, null);
            }
            else
            {
                Assert.IsTrue(field.CustomAttributes.All(a => a.AttributeType != attributeType));
            }
        }

        private static void CheckMethodParameter(ParameterInfo parameter, string name, string decorationPrefix, Type attributeType)
        {
            if (name.StartsWith(decorationPrefix))
            {
                var attribute = parameter.CustomAttributes.Single();
                CheckAttribute(attribute, attributeType, name, ProcessableComponentType.MethodParameter, null);
            }
            else
            {
                Assert.IsTrue(parameter.CustomAttributes.All(a => a.AttributeType != attributeType));
            }
        }

        private static IEnumerable<Type> GetTestingTypes()
        {
            return Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(t => t.FullName.StartsWith("ExtensibleILRewriter.Tests.AddAttributeProcessor"));
        }

        private static void CheckAttribute(CustomAttributeData att, Type attributeType, string componentName, ProcessableComponentType componentType, Type typeType)
        {
            if (attributeType == typeof(Injected1Attribute))
            {
                Assert.AreEqual(attributeType, att.AttributeType);
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
            else if (attributeType == typeof(Injected2Attribute))
            {
                Assert.AreEqual(attributeType, att.AttributeType);
                Assert.AreEqual(4, att.ConstructorArguments.Count);

                Assert.AreEqual(typeof(ProcessableComponentType[]), att.ConstructorArguments[0].ArgumentType);
                foreach (var item in (IEnumerable<CustomAttributeTypedArgument>)att.ConstructorArguments[0].Value)
                {
                    Assert.AreEqual(typeof(ProcessableComponentType), item.ArgumentType);
                    Assert.AreEqual((int)componentType, item.Value);
                }

                Assert.AreEqual(typeof(Type[]), att.ConstructorArguments[1].ArgumentType);
                foreach (var item in (IEnumerable<CustomAttributeTypedArgument>)att.ConstructorArguments[1].Value)
                {
                    Assert.AreEqual(typeof(Type), item.ArgumentType);
                    Assert.AreEqual(typeType, item.Value);
                }

                Assert.AreEqual(typeof(int[]), att.ConstructorArguments[2].ArgumentType);
                foreach (var item in (IEnumerable<CustomAttributeTypedArgument>)att.ConstructorArguments[2].Value)
                {
                    Assert.AreEqual(typeof(int), item.ArgumentType);
                    Assert.AreEqual(componentName.GetHashCode(), item.Value);
                }

                Assert.AreEqual(typeof(string[]), att.ConstructorArguments[3].ArgumentType);
                foreach (var item in (IEnumerable<CustomAttributeTypedArgument>)att.ConstructorArguments[3].Value)
                {
                    Assert.AreEqual(typeof(string), item.ArgumentType);
                    Assert.AreEqual(componentName, item.Value);
                }
            }
            else
            {
                throw new InvalidOperationException();
            }
        }
    }
}
