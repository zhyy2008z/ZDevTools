﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xunit;

using ZDevTools.Collections;

namespace ZDevTools.Test.Collections
{
    public class MovingCacheTest
    {
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
        }


    }
}
