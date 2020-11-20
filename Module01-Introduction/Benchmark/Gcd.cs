using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Benchmark
{
    public class Gcd
    {
        private static readonly Random Rnd = new Random((int)DateTime.Now.Ticks & 0x0000FFFF);

        public static IEnumerable<IEnumerable<ulong>> Data()
        {
            yield return Enumerable.Range(0, 10).Select(_ => (ulong)Rnd.Next());
            yield return Enumerable.Range(0, 100).Select(_ => (ulong)Rnd.Next());
            yield return Enumerable.Range(0, 1000).Select(_ => (ulong)Rnd.Next());
        }

        [Benchmark]
        [ArgumentsSource(nameof(Data))]
        public ulong LeastCommonMultiple(IEnumerable<ulong> enumerable)
        {
            return enumerable.Count() == 2
                ? LeastCommonMultiple(enumerable.ElementAt(0), enumerable.ElementAt(1))
                : enumerable.ElementAt(0) * LeastCommonMultiple(enumerable.Skip(1));
        }

        [Benchmark]
        [ArgumentsSource(nameof(Data))]
        public ulong LeastCommonMultipleLinq(IEnumerable<ulong> enumerable)
        {
            return enumerable.Aggregate(LeastCommonMultiple);
        }

        /// <summary>
        /// Dividing first to avoid overflows
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        private static ulong LeastCommonMultiple(ulong a, ulong b)
        {
            return a / GreatesCommonDivisor(a, b) * b;
        }

        /// <summary>
        /// Euclidean Algorithm
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        private static ulong GreatesCommonDivisor(ulong a, ulong b)
        {
            while (b != 0)
            {
                // From C# 7
                // (a, b) = b, a % b; #

                var r = a % b;
                a = b;
                b = r;
            }

            return a;
        }
    }
}
