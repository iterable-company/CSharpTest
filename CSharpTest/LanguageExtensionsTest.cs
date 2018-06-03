using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using LanguageExt;
using LanguageExt.ClassInstances;
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
        public void OptionFromMatchTest()
        {
            Option<int> two = Some(2);
            Option<int> four = Some(4);
            Option<int> six = Some(6);
            Option<int> none = None;

            int toBeTested = match(
                from x in two
                from y in four
                from z in six
                select x + y + z,
                Some: v => v * 2,
                None: () => 0
                );
            Assert.AreEqual(24, toBeTested);

            toBeTested = match(
                from a in two
                from b in four
                from c in none
                from d in six
                select a + b + c + d,
                Some: v => v * 2,
                None: () => 0
                );
            Assert.AreEqual(0, toBeTested);
        }
        [TestMethod]
        public void OptionSumTTest()
        {
            var list = List(Some(1), None, Some(2), None, Some(3));
            var preDouble = list.SumT<TInt, int>();
            Assert.AreEqual(6, preDouble);

            var doubled = list.MapT(x => x * 2);
            Assert.AreEqual(5, doubled.Count);
            Assert.AreEqual(Some(2), doubled[0]);
            Assert.AreEqual(None, doubled[1]);
            Assert.AreEqual(Some(4), doubled[2]);
            Assert.AreEqual(None, doubled[3]);
            Assert.AreEqual(Some(6), doubled[4]);

            var postDouble = list.MapT(x => x * 2).SumT<TInt, int>();
            Assert.AreEqual(12, postDouble);
        }
        [TestMethod]
        public void OptionNotUsingSumTTest()
        {
            var list = List(Some(1), None, Some(2), None, Some(3));
            var preDouble = list.Map(x => x.Sum());
            Assert.AreEqual(5, preDouble.Count);
            Assert.AreEqual(1, preDouble[0]);
            Assert.AreEqual(0, preDouble[1]);
            Assert.AreEqual(2, preDouble[2]);
            Assert.AreEqual(0, preDouble[3]);
            Assert.AreEqual(3, preDouble[4]);

            var postDouble = list.Map(x => x.Map(y => y * 2));
            Assert.AreEqual(5, postDouble.Count);
            Assert.AreEqual(Some(2), postDouble[0]);
            Assert.AreEqual(None, postDouble[1]);
            Assert.AreEqual(Some(4), postDouble[2]);
            Assert.AreEqual(None, postDouble[3]);
            Assert.AreEqual(Some(6), postDouble[4]);

            var postToInt = postDouble.Map(x => x.Sum());
            Assert.AreEqual(5, postToInt.Count);
            Assert.AreEqual(2, postToInt[0]);
            Assert.AreEqual(0, postToInt[1]);
            Assert.AreEqual(4, postToInt[2]);
            Assert.AreEqual(0, postToInt[3]);
            Assert.AreEqual(6, postToInt[4]);
        }
    }
}
