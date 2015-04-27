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
        public static int Constant1, Constant2;

        [TestMethod]
        public void TestModuleInitializer1()
        {
            Assert.AreEqual(123, Constant1);
        }

        [TestMethod]
        public void TestModuleInitializer2()
        {
            Assert.AreEqual(456, Constant2);
        }

        public static void InitializeModule1()
        {
            Constant1 = 123;
        }

        public static void InitializeModule2()
        {
            Constant2 = 456;
        }
    }
}
