using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace Miq.Tests.Nursery
{
    [TestClass]
    public class SICPTests
    {
        [TestMethod]
        [Ignore]
        public void TestMethod1()
        {
            var x = MyEnumerableExtensions.Integers().Take(10).ToList();
            var z = MyEnumerableExtensions.PowersOf2().Take(10).ToList();
        }
    }

    public static class MyEnumerableExtensions
    {
        public static IEnumerable<int> PowersOf2()
        {

            return Concat(1, PowersOf2().ScaleStream(2));
        }

        public static IEnumerable<int> Concat(int number, IEnumerable<int> stream)
        {
            yield return 1;
            foreach (var i in stream) yield return i;
        }

        static IEnumerable<int> MakeEnumerable(int number)
        {
            yield return number;
        }

        static IEnumerable<int> Prepend(this IEnumerable<int> stream, int number)
        {
            yield return number;
            foreach (var item in stream)
                yield return item;
        }

        static IEnumerable<int> ScaleStream(this IEnumerable<int> stream, int factor)
        {
            return stream.Map(a => a * factor);
        }

        static IEnumerable<int> Fibonaccies()
        {
            yield return 0;
            yield return 1;
            foreach (var item in Add(Fibonaccies().Skip(1), Fibonaccies()))
            {
                yield return item;
            }
        }

        static IEnumerable<int> Map(this IEnumerable<int> stream, Func<int, int> proc)
        {
            foreach (var item in stream)
            {
                yield return proc(item);
            }
        }

        static IEnumerable<int> Map(Func<int, int, int> proc, IEnumerable<int> s1, IEnumerable<int> s2)
        {
            var e1 = s1.GetEnumerator();
            var e2 = s2.GetEnumerator();

            while (e1.MoveNext() && e2.MoveNext())
            {
                yield return proc(e1.Current, e2.Current);
            }
        }

        static IEnumerable<int> Add(IEnumerable<int> s1, IEnumerable<int> s2)
        {
            return Map((x, y) => x + y, s1, s2);
        }

        public static IEnumerable<int> Integers()
        {
            yield return 1;
            foreach (var item in Add(Ones(), Integers()))
                yield return item;
        }

        static IEnumerable<int> Ones()
        {
            while (true)
            {
                yield return 1;
            }
        }
    }
}
