using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using Xunit;
using Xunit.Abstractions;

using ZDevTools.Collections;

namespace ZDevTools.Test.Collections
{
    public class MovingCacheTest
    {
        readonly ITestOutputHelper Output;
        public MovingCacheTest(ITestOutputHelper output)
        {
            Output = output;
        }

        [Fact]
        public void Test()
        {
            MovingCache<int> cache = new MovingCache<int>(10);

            cache.Enqueue(1);
            cache.Enqueue(2);
            cache.Enqueue(3);
            cache.Enqueue(4);
            cache.Enqueue(5);
            cache.Enqueue(6);
            cache.Enqueue(7);
            cache.Enqueue(8);
            cache.Enqueue(9);
            Assert.False(cache.IsFull);

            cache.Enqueue(10);
            Assert.True(cache.IsFull);
            Assert.Equal(new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }, cache);
            Assert.Equal(new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }, cache.ToArray());
            Assert.Equal(2, cache[1]);
            Assert.Equal(10, cache[9]);

            cache.Enqueue(11);
            Assert.Equal(new[] { 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, }, cache);
            Assert.Equal(new[] { 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, }, cache.ToArray());
            Assert.Equal(3, cache[1]);
            Assert.Equal(11, cache[9]);

            cache[1] = 99;
            Assert.Equal(99, cache[1]);
            Assert.Equal(new[] { 2, 99, 4, 5, 6, 7, 8, 9, 10, 11, }, cache);

            cache[9] = 100;
            Assert.Equal(100, cache[9]);
            Assert.Equal(new[] { 2, 99, 4, 5, 6, 7, 8, 9, 10, 100, }, cache);

            cache.Clear();
            Assert.Equal(0, cache.Count);
            Assert.Equal(10, cache.Capacity);

            cache.EraseExcess();

            cache.Enqueue(35);
            Assert.Equal(1, cache.Count);
        }

        [Fact]
        public void TestPerformance()
        {
            MovingCache<double> cache = new MovingCache<double>(500000);
            Random random = new Random();
            foreach (var item in Enumerable.Range(0, 1000000))
            {
                cache.Enqueue(random.Next() + random.NextDouble());
            }

            List<double> times = new List<double>();
            for (int i = 0; i < 10; i++)
            {
                Stopwatch stopwatch = Stopwatch.StartNew();
                cache.Average();
                times.Add(stopwatch.ElapsedMilliseconds);
            }

            var t = times.Average();

            times.Clear();


            for (int i = 0; i < 10; i++)
            {
                Stopwatch stopwatch = Stopwatch.StartNew();
                cache.Buffer.Average();
                times.Add(stopwatch.ElapsedMilliseconds);
            }

            var t2 = times.Average();


            times.Clear();


            for (int i = 0; i < 10; i++)
            {
                Stopwatch stopwatch = Stopwatch.StartNew();
                double sum = 0;
                for (int j = 0; j < cache.Buffer.Length; j++)
                    sum += cache.Buffer[j];
                var a = sum / cache.Buffer.Length;
                times.Add(stopwatch.ElapsedMilliseconds);
            }

            var t3 = times.Average();
            Output.WriteLine($"{t:f1} vs {t2:f1} vs {t3:f1}");
        }
    }
}
