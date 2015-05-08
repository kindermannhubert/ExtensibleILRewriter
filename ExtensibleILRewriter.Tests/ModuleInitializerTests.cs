using ExtensibleILRewriter.Processors.Modules;
using ExtensibleILRewriter.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[module: ModuleInitializer("ExtensibleILRewriter.Tests.ModuleInitializerTests", nameof(ModuleInitializerTests.InitializeModule1))]
[module: ModuleInitializer("ExtensibleILRewriter.Tests.ModuleInitializerTests", nameof(ModuleInitializerTests.InitializeModule2))]

namespace ExtensibleILRewriter.Tests
{
    [TestClass]
    public class ModuleInitializerTests
    {
        private static int constant1, constant2;

        [TestMethod]
        public void TestModuleInitializer1()
        {
            Assert.AreEqual(123, constant1);
        }

        [TestMethod]
        public void TestModuleInitializer2()
        {
            Assert.AreEqual(456, constant2);
        }

        public static void InitializeModule1()
        {
            constant1 = 123;
        }

        public static void InitializeModule2()
        {
            constant2 = 456;
        }
    }
}
