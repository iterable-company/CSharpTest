using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using LanguageExt;
using static LanguageExt.Prelude;

using Football;

namespace CSharpTest
{
    [TestClass]
    public class LanguageExtensionsTest
    {
        [TestMethod]
        public void TupleTest()
        {
            var name = Tuple("hoge","fuga");
            var toBeTested = name.Map( (first, second) => $"{first},{second}");
            Assert.AreEqual("hoge,fuga", toBeTested);
        }
        [TestMethod]
        public void OptionTest()
        {

        }
    }
}
