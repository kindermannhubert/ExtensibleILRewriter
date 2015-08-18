using ExtensibleILRewriter.Processors.Parameters;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace ExtensibleILRewriter.Tests
{
    [TestClass]
    public class NotNullAttributeTests
    {
        [TestMethod]
        public void TestNotNullReferenceType1()
        {
            Assert.IsTrue(CheckParams1(new object()));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestNotNullReferenceType2()
        {
            CheckParams1(null);
        }

        [TestMethod]
        public void TestNotNullNullableStruct1()
        {
            Assert.IsTrue(CheckParams2(7));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestNotNullNullableStruct2()
        {
            CheckParams2(null);
        }

        private bool CheckParams1([NotNull]object o)
        {
            return o != null;
        }

        private bool CheckParams2([NotNull]double? o)
        {
            return o.HasValue;
        }
    }
}
