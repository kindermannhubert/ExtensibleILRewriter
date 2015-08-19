using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace ExtensibleILRewriter.Tests.MethodCodeInjectingProcessor
{
    [TestClass]
    public class MethodCodeInjectingProcessorTests
    {
        private readonly MethodInjectionTestCodeProvider.State state = new MethodInjectionTestCodeProvider.State();

        [TestInitialize]
        public void Initialize()
        {
            StateInstancesManager.ClearInstanceRegistrations();
            StateInstancesManager.RegisterInstance(new StateInstanceId("TestsInjectionState"), state);
        }

        [TestMethod]
        public void InjectCodeAtBeginingTest()
        {
            Assert.AreEqual(0, state.Items.Count);

            var test = new Injection_TestClass1();
            test.InjectAtBegining_WithoutParam();
            Assert.AreEqual(1, state.Items.Count);
            Assert.AreEqual(MethodInjectionTestCodeProvider.NoInputItem, state.Items[0]);

            test.InjectAtBegining_WithoutParam();
            Assert.AreEqual(2, state.Items.Count);
            Assert.AreEqual(MethodInjectionTestCodeProvider.NoInputItem, state.Items[1]);

            state.Items.Clear();
        }

        [TestMethod]
        public void InjectCodeAtBeginingWithParameterTest()
        {
            Assert.AreEqual(0, state.Items.Count);

            int value = 13;

            var test = new Injection_TestClass1();
            test.InjectAtBegining_WithParam(value);
            Assert.AreEqual(1, state.Items.Count);
            Assert.AreEqual(value.ToString(), state.Items[0]);

            value *= 2;
            test.InjectAtBegining_WithParam(value);
            Assert.AreEqual(2, state.Items.Count);
            Assert.AreEqual(value.ToString(), state.Items[1]);

            state.Items.Clear();
        }

        [TestMethod]
        public void InjectCodeOnExitTest()
        {
            Assert.AreEqual(0, state.Items.Count);

            var test = new Injection_TestClass1();
            test.InjectOnExit_WithoutParam();
            Assert.AreEqual(1, state.Items.Count);
            Assert.AreEqual(MethodInjectionTestCodeProvider.NoInputItem, state.Items[0]);

            test.InjectOnExit_WithoutParam();
            Assert.AreEqual(2, state.Items.Count);
            Assert.AreEqual(MethodInjectionTestCodeProvider.NoInputItem, state.Items[1]);

            state.Items.Clear();
        }

        [TestMethod]
        public void InjectCodeOnExitWithParameterTest()
        {
            Assert.AreEqual(0, state.Items.Count);

            int value = 13;

            var test = new Injection_TestClass1();
            test.InjectOnExit_WithParam(value);
            Assert.AreEqual(1, state.Items.Count);
            Assert.AreEqual(value.ToString(), state.Items[0]);

            value *= 2;
            test.InjectOnExit_WithParam(value);
            Assert.AreEqual(2, state.Items.Count);
            Assert.AreEqual(value.ToString(), state.Items[1]);

            state.Items.Clear();
        }

        [TestMethod]
        public void InjectCodeAtBeginingWithChangingParameterTest()
        {
            Assert.AreEqual(0, state.Items.Count);

            int value = 13;

            var test = new Injection_TestClass1();
            test.InjectAtBegining_WithChangingParam(value);
            Assert.AreEqual(1, state.Items.Count);
            Assert.AreEqual(value.ToString(), state.Items[0]);

            state.Items.Clear();
        }

        [TestMethod]
        public void InjectCodeOnExitWithChangingParameterTest()
        {
            Assert.AreEqual(0, state.Items.Count);

            int value = 13;

            var test = new Injection_TestClass1();
            test.InjectOnExit_WithChangingParam(value);
            Assert.AreEqual(1, state.Items.Count);
            Assert.AreEqual((2 * value).ToString(), state.Items[0]);

            state.Items.Clear();
        }

        [TestMethod]
        public void InjectCodeOnExitWithMoreExitsTest()
        {
            Assert.AreEqual(0, state.Items.Count);

            int value = 13;
            var test = new Injection_TestClass1();
            var list = new List<string>();

            test.InjectOnExit_MoreExits(value, item => list.Add(item));
            Assert.AreEqual(1, state.Items.Count);
            Assert.AreEqual(value.ToString(), state.Items[0]);
            Assert.AreEqual(2, list.Count);
            Assert.AreEqual("if", list[0]);
            Assert.AreEqual("exit 1", list[1]);

            value = -17;
            state.Items.Clear();
            list.Clear();
            test.InjectOnExit_MoreExits(value, item => list.Add(item));
            Assert.AreEqual(1, state.Items.Count);
            Assert.AreEqual(value.ToString(), state.Items[0]);
            Assert.AreEqual(2, list.Count);
            Assert.AreEqual("else", list[0]);
            Assert.AreEqual("exit 2", list[1]);

            state.Items.Clear();
        }

        [TestMethod]
        public void InjectCodeOnExitWithJumpToExit()
        {
            Assert.AreEqual(0, state.Items.Count);

            var test = new Injection_TestClass1();
            test.InjectOnExit_WithJumpToEnd();
            Assert.AreEqual(1, state.Items.Count);
            Assert.AreEqual(MethodInjectionTestCodeProvider.NoInputItem, state.Items[0]);

            state.Items.Clear();
        }
    }
}
