using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ExtensibleILRewriter.Processors.Parameters;

namespace ExtensibleILRewriter.Tests
{
    [TestClass]
    public class NotNullAttributeTests
    {
        [TestMethod]
        public void TestReferenceType1()
        {
            Assert.IsTrue(CheckParams1(new object()));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestReferenceType2()
        {
            CheckParams1(null);
        }

        [TestMethod]
        public void TestNullableStruct1()
        {
            Assert.IsTrue(CheckParams2(7));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestNullableStruct2()
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
