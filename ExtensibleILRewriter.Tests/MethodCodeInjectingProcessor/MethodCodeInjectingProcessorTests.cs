using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExtensibleILRewriter.Tests.MethodCodeInjectingProcessor
{
    [TestClass]
    public class MethodCodeInjectingProcessorTests
    {
        private readonly MethodInjectionCodeProvider.State state = new MethodInjectionCodeProvider.State();

        [TestInitialize]
        public void Initialize()
        {
            StateInstancesManager.RegisterInstance("TestsInjectionState", state);
        }

        [TestMethod]
        public void Test()
        {
            Assert.AreEqual(0, state.Items.Count);

            var test = new Injection_TestClass1();
            test.Inject_Method1();

            Assert.AreEqual(1, state.Items.Count);
        }
    }
}
