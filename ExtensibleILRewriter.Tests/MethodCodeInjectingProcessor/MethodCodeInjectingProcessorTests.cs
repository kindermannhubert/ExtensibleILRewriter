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
            StateInstancesManager.ClearInstanceRegistrations();
            StateInstancesManager.RegisterInstance("TestsInjectionState", state);
        }

        [TestMethod]
        public void Test1()
        {
            Assert.AreEqual(0, state.Items.Count);

            var test = new Injection_TestClass1();
            test.Inject_Method1();
            Assert.AreEqual(1, state.Items.Count);
            Assert.AreEqual(MethodInjectionCodeProvider.NoInputItem, state.Items[0]);

            test.Inject_Method1();
            Assert.AreEqual(2, state.Items.Count);
            Assert.AreEqual(MethodInjectionCodeProvider.NoInputItem, state.Items[1]);

            state.Items.Clear();
        }

        [TestMethod]
        public void Test2()
        {
            Assert.AreEqual(0, state.Items.Count);

            int value = 13;

            var test = new Injection_TestClass1();
            test.Inject_Method2(value);
            Assert.AreEqual(1, state.Items.Count);
            Assert.AreEqual(value.ToString(), state.Items[0]);

            value *= 2;
            test.Inject_Method2(value);
            Assert.AreEqual(2, state.Items.Count);
            Assert.AreEqual(value.ToString(), state.Items[1]);

            state.Items.Clear();
        }
    }
}
