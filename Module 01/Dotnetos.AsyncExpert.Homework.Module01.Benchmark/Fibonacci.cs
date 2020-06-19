using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Diagnostics.Windows.Configs;

namespace Dotnetos.AsyncExpert.Homework.Module01.Benchmark
{
    [ShortRunJob]
    [MemoryDiagnoser]
    [NativeMemoryProfiler]
    [DisassemblyDiagnoser(exportCombinedDisassemblyReport: true)]
    public class FibonacciCalc
    {
        // HOMEWORK:
        // 1. Write implementations for RecursiveWithMemoization and Iterative solutions
        // 2. Add memory profilers (MemoryDiagnoser and NativeMemoryProfiler) to the benchmark
        // 3. Run with release configuration and compare results
        // 4. Open disassembler report and compare machine code
        //
        // You can use the discussion panel to compare your results with other students

        [Benchmark]
        [ArgumentsSource(nameof(Data))]
        public ulong Recursive(ulong n)
        {
            if (n == 1 || n == 2) return 1;
            return Recursive(n - 2) + Recursive(n - 1);
        }

        [Benchmark(Baseline = true)]
        [ArgumentsSource(nameof(Data))]
        public ulong Iterative(ulong n)
        {
            if (n == 1 || n == 2) return 1;

            ulong previousPreviousNumber, previousNumber = 0, currentNumber = 1;

            for (ulong i = 1; i < n; ++i)
            {
                previousPreviousNumber = previousNumber;
                previousNumber = currentNumber;
                currentNumber = previousPreviousNumber + previousNumber;
            }

            return currentNumber;
        }

        [Benchmark]
        [ArgumentsSource(nameof(Data))]
        public ulong RecursiveWithMemoization(ulong n)
        {
            if (n == 1 || n == 2) return 1;

            return RecursiveWithMemoizationInternal(n, new Dictionary<ulong, ulong>((int)n) { [1] = 1, [2] = 1 });
        }

        private static ulong RecursiveWithMemoizationInternal(ulong n, Dictionary<ulong, ulong> dictionary)
        {
            return CalculateItem(dictionary, n - 2) + CalculateItem(dictionary, n - 1);
        }

        private static ulong CalculateItem(Dictionary<ulong, ulong> dictionary, ulong index)
        {
            if (!dictionary.TryGetValue(index, out var value))
            {
                value = dictionary[index] = RecursiveWithMemoizationInternal(index, dictionary);
            }

            return value;
        }

        public static IEnumerable<ulong> Data()
        {
            yield return 1;
            yield return 2;
            yield return 15;
            yield return 35;
        }
    }
}
