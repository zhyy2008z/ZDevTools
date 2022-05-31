using System;
using System.Collections.Generic;
using System.Linq;

using Xunit;

using ZDevTools.Collections;

namespace ZDevTools.Test.Collections
{
    public class BufferQueueTest
    {
        [Fact]
        public void TestPart1()
        {
            BufferQueue<int> queue = new BufferQueue<int>(2048) { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };

            Assert.True(queue.Capacity > 0);

            Assert.Equal(new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 }, queue);

            queue.AddRange(new[] { 10, 11, 12, 13, 14, 15, 16, 17 });
            Assert.Equal(new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17 }, queue);

            queue.RemoveAt(10);
            Assert.Equal(new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 11, 12, 13, 14, 15, 16, 17 }, queue);

            queue.RemoveRange(1, 5);
            Assert.Equal(new[] { 0, 6, 7, 8, 9, 11, 12, 13, 14, 15, 16, 17 }, queue);

            queue.InsertRange(2, new[] { 3, 4, 5 });
            Assert.Equal(new[] { 0, 6, 3, 4, 5, 7, 8, 9, 11, 12, 13, 14, 15, 16, 17 }, queue);

            queue.Enqueue(5);
            Assert.Equal(new[] { 0, 6, 3, 4, 5, 7, 8, 9, 11, 12, 13, 14, 15, 16, 17, 5 }, queue);

            queue.Enqueue(new[] { 3, 3, 3, 3, 3, 3, 3 });
            Assert.Equal(new[] { 0, 6, 3, 4, 5, 7, 8, 9, 11, 12, 13, 14, 15, 16, 17, 5, 3, 3, 3, 3, 3, 3, 3 }, queue);

            queue.Dequeue(10, out int[] outBytes);
            Assert.Equal(new[] { 13, 14, 15, 16, 17, 5, 3, 3, 3, 3, 3, 3, 3 }, queue);
            Assert.Equal(new[] { 0, 6, 3, 4, 5, 7, 8, 9, 11, 12, }, outBytes);

            queue.Pop(out var t);
            Assert.Equal(new[] { 14, 15, 16, 17, 5, 3, 3, 3, 3, 3, 3, 3 }, queue);
            Assert.Equal(13, t);

            queue.Push(3);
            queue.Push(4);
            Assert.Equal(new[] { 4, 3, 14, 15, 16, 17, 5, 3, 3, 3, 3, 3, 3, 3 }, queue);

            queue.SetRange(0, new[] { 3, 3, 3, 3, 3 });
            Assert.Equal(new[] { 3, 3, 3, 3, 3, 17, 5, 3, 3, 3, 3, 3, 3, 3 }, queue);

            queue.SetRange(0, new[] { 3, 3, 3, 3, 3 });
            Assert.Equal(new[] { 3, 3, 3, 3, 3, 17, 5, 3, 3, 3, 3, 3, 3, 3 }, queue);

            queue.SetRange(5, new[] { 3, 3, 3, 3, 3 });
            Assert.Equal(new[] { 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3 }, queue);

            Assert.ThrowsAny<Exception>(() => queue.SetRange(100, new int[0]));

            queue.Remove(3);
            Assert.Equal(new[] { 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3 }, queue);

            queue.RemoveAt(3);
            Assert.Equal(new[] { 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3 }, queue);

            queue.SetRange(5, new[] { 5, 2, 5, 3, 32, 2 });
            Assert.Equal(new[] { 3, 3, 3, 3, 3, 5, 2, 5, 3, 32, 2, 3 }, queue);

            Assert.Equal(5, queue.IndexOf(5));
            Assert.Equal(0, queue.IndexOf(3));
            Assert.Equal(11, queue.LastIndexOf(3));

            Assert.Equal(new[] { 2, 3 }, queue.Skip(10));

            queue.Peek(10, 1, out int[] b);
            Assert.Equal(new[] { 2 }, b);

            Assert.Equal(new[] { 3, 3, 3, 3, 3, 5, 2, 5, 3, 32, 2, 3 }, queue.ToArray());

            List<int> list = new List<int>();
            foreach (var item in queue)
            {
                list.Add(item);
            }
            Assert.Equal(new[] { 3, 3, 3, 3, 3, 5, 2, 5, 3, 32, 2, 3 }, list);

            Assert.True(queue.Clear(3));
            Assert.Equal(new[] { 3, 3, 5, 2, 5, 3, 32, 2, 3 }, queue);

            Assert.False(queue.Clear(0));
            Assert.False(queue.Clear(99));
            Assert.False(queue.Clear(-2));
            Assert.Equal(new[] { 3, 3, 5, 2, 5, 3, 32, 2, 3 }, queue);

            Assert.True(queue.ClearOne());
            Assert.Equal(new[] { 3, 5, 2, 5, 3, 32, 2, 3 }, queue);

            Assert.ThrowsAny<Exception>(() => queue.Capacity = 0);

            queue.Clear();
            Assert.Equal(new int[] { }, queue);

            Assert.Equal(2048, queue.Capacity);

            queue.Capacity = 3;

            Assert.Equal(3, queue.Capacity);

            queue.Capacity = 0;

            Assert.Equal(0, queue.Capacity);

            queue.Enqueue(3);

            Assert.True(queue.Contains(3));

            queue.Insert(0, 34);

            Assert.Equal(1, queue.LastIndexOf(3));

            queue.Insert(0, 343);
            queue.Enqueue(34);

            Assert.Equal(0, queue.IndexOf(343));

            Assert.Equal(3, queue.LastIndexOf(34));

            Assert.ThrowsAny<Exception>(() =>
            {
                foreach (var item in queue)
                {
                    queue.Enqueue(3);
                }
            });

            queue.EraseExcess();

            queue.Capacity = 100;

            Assert.Equal(100, queue.Capacity);

            queue.TrimExcess();

            Assert.Equal(queue.Length, queue.Capacity);
        }

        [Fact]
        public void TestPart2()
        {
            BufferQueue<int> queue = new BufferQueue<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };

            queue.Dequeue(10, out Span<int> cache);

            Assert.Equal(10, cache.Length);
            Assert.Empty(queue);

            Assert.False(queue.ClearOne());

            queue.Add(4);

            Assert.Single(queue);

            Assert.Equal(4, queue[0]);

            Assert.True(queue.ClearOne());

            Assert.Empty(queue);
        }

        [Fact]
        public void TestPart3()
        {
            var list = new int[] { 1, 2, 3, 4, 5, 6, 7 };

            var queue = list.ToBufferQueue();

            Assert.Equal(7, queue.Count);

            Assert.Equal(list, queue);

            queue.Push(9);
            Assert.Equal(8, queue.Count);
            Assert.Equal(7, list.Length);

            queue[1] = 3;

            Assert.Equal(2, list[1]);

            Assert.Equal(3, queue[1]);


            var buffer = new int[] { 2, 3, 4, 5, 6, 7, 8 };

            var queue2 = new BufferQueue<int>(buffer);

            Assert.Equal(7, queue2.Count);

            queue2[1] = 6;

            Assert.Equal(6, buffer[1]);

            Assert.Equal(6, queue2[1]);

            queue2.Push(9);
            Assert.Equal(8, queue2.Count);
            Assert.Equal(7, buffer.Length);

            queue2[1] = 4;

            Assert.Equal(6, buffer[1]);

            Assert.Equal(4, queue2[1]);
        }


        [Fact]
        public void TestPart4()
        {
            BufferQueue<int> queue = new BufferQueue<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };

            queue.RemoveAll(n => n > 2 && n < 7);
            Assert.Equal(new List<int>() { 0, 1, 2, 7, 8, 9 }, queue);

            queue.InsertRange(4, new[] { 3, 3, 3, 5 });
            Assert.Equal(new List<int>() { 0, 1, 2, 7, 3, 3, 3, 5, 8, 9 }, queue);

            queue.RemoveRange(4, 4);

            Assert.Equal(new List<int>() { 0, 1, 2, 7, 8, 9 }, queue);

            queue = new BufferQueue<int>();
            Assert.Equal(0, queue.Capacity);

            queue = new BufferQueue<int>(new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 });
            Assert.Equal(new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 }, queue);

            queue = new BufferQueue<int>(new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 }.AsSpan());
            Assert.Equal(new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 }, queue);

            queue = new BufferQueue<int>(100);
            Assert.Equal(100, queue.Capacity);
        }

        [Fact]
        public void TestInsertRange1()
        {
            //测试离头部更近，头部无足够空间，插入部分可以全部放在头部，无剩余有效数据搬运需要搬运

            BufferQueue<int> queue = new BufferQueue<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };

            queue.Capacity = 12;

            queue.Clear(3);
            Assert.Equal(new[] { 3, 4, 5, 6, 7, 8, 9 }, queue);


            queue.InsertRange(1, new[] { 2, 2, 1, 5 });

            Assert.Equal(new[] { 3, 2, 2, 1, 5, 4, 5, 6, 7, 8, 9 }, queue);
        }

        [Fact]
        public void TestInsertRange2()
        {
            //测试离头部更近，头部无足够空间，插入部分可以全部放在头部，有剩余有效数据搬运到缓存的开始位置

            BufferQueue<int> queue = new BufferQueue<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };

            queue.Capacity = 13;

            queue.Clear(3);
            Assert.Equal(new[] { 3, 4, 5, 6, 7, 8, 9 }, queue);


            queue.InsertRange(2, new[] { 2, 2, 1, 5, });

            Assert.Equal(new[] { 3, 4, 2, 2, 1, 5, 5, 6, 7, 8, 9 }, queue);
        }

        [Fact]
        public void TestInsertRange3()
        {
            //测试离头部更近，头部无足够空间，插入部分只有一部分可以放在头部

            BufferQueue<int> queue = new BufferQueue<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };

            queue.Capacity = 13;

            queue.Clear(2);
            Assert.Equal(new[] { 2, 3, 4, 5, 6, 7, 8, 9 }, queue);


            queue.InsertRange(2, new[] { 2, 2, 1, 5, 5 });

            Assert.Equal(new[] { 2, 3, 2, 2, 1, 5, 5, 4, 5, 6, 7, 8, 9 }, queue);
        }

        [Fact]
        public void TestInsertRange4()
        {
            //测试离尾部更近，尾部无足够空间，插入数据可以全部放在尾部，没有剩余有效数据需要搬运到尾部

            BufferQueue<int> queue = new BufferQueue<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };

            queue.Capacity = 13;

            queue.Clear(4);
            Assert.Equal(new[] { 4, 5, 6, 7, 8, 9 }, queue);

            queue.InsertRange(4, new[] { 2, 2, 1, 5, 5 });

            Assert.Equal(new[] { 4, 5, 6, 7, 2, 2, 1, 5, 5, 8, 9 }, queue);
        }

        [Fact]
        public void TestInsertRange5()
        {
            //测试离尾部更近，尾部无足够空间，插入数据可以全部放在尾部，有剩余有效数据需要搬运到尾部

            BufferQueue<int> queue = new BufferQueue<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };

            queue.Capacity = 13;

            queue.Clear(4);
            Assert.Equal(new[] { 4, 5, 6, 7, 8, 9 }, queue);

            queue.InsertRange(3, new[] { 2, 2, 1, 5, 5 });

            Assert.Equal(new[] { 4, 5, 6, 2, 2, 1, 5, 5, 7, 8, 9 }, queue);
        }

        [Fact]
        public void TestInsertRange6()
        {
            //测试离尾部更近，尾部无足够空间，插入数据只有部分可以放在尾部

            BufferQueue<int> queue = new BufferQueue<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };

            queue.Capacity = 13;

            queue.Clear(4);
            Assert.Equal(new[] { 4, 5, 6, 7, 8, 9 }, queue);

            queue.InsertRange(4, new[] { 2, 2, 1, 5, 5, 9 });

            Assert.Equal(new[] { 4, 5, 6, 7, 2, 2, 1, 5, 5, 9, 8, 9 }, queue);
        }

        [Fact]
        public void TestInsert1()
        {
            //测试离头部更近，头部有空间

            BufferQueue<int> queue = new BufferQueue<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };

            queue.Capacity = 13;

            queue.Clear(4);

            Assert.Equal(new[] { 4, 5, 6, 7, 8, 9 }, queue);

            queue.Insert(2, 9);

            Assert.Equal(new[] { 4, 5, 9, 6, 7, 8, 9 }, queue);
        }

        [Fact]
        public void TestInsert2()
        {
            //测试离头部更近，头部无足够空间

            BufferQueue<int> queue = new BufferQueue<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };

            queue.Capacity = 13;

            Assert.Equal(new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 }, queue);

            queue.Insert(1, 9);

            Assert.Equal(new[] { 0, 9, 1, 2, 3, 4, 5, 6, 7, 8, 9 }, queue);
        }

        [Fact]
        public void TestInsert3()
        {
            //测试离尾部更近，尾部有足够空间

            BufferQueue<int> queue = new BufferQueue<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };

            queue.Capacity = 13;

            Assert.Equal(new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 }, queue);

            queue.Insert(8, 19);

            Assert.Equal(new[] { 0, 1, 2, 3, 4, 5, 6, 7, 19, 8, 9, }, queue);
        }

        [Fact]
        public void TestInsert4()
        {
            //测试离尾部更近，尾部无足够空间

            BufferQueue<int> queue = new BufferQueue<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };

            queue.Clear(4);

            Assert.Equal(new[] { 4, 5, 6, 7, 8, 9 }, queue);

            queue.Insert(4, item: 9);

            Assert.Equal(new[] { 4, 5, 6, 7, 9, 8, 9 }, queue);
        }

        [Fact]
        public void TestInserRandom()
        {
            BufferQueue<int> queue = new BufferQueue<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            Random random = new Random();
            for (int i = 0; i < 10000; i++)
            {
                var index = random.Next(queue.Count + 1);
                var item = random.Next();
                var expect = queue.Take(index).Append(item).Concat(queue.Skip(index)).ToArray();
                queue.Insert(index, item);
                Assert.Equal(expect, queue);
            }
        }

        [Fact]
        public void TestInsertRangeRandom()
        {
            BufferQueue<int> queue = new BufferQueue<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            Random random = new Random();
            for (int i = 0; i < 1000; i++)
            {
                var length = random.Next(10);
                var items = new int[length];
                for (int j = 0; j < length; j++)
                    items[j] = random.Next();
                var index = random.Next(queue.Count + 1);
                var expect = queue.Take(index).Concat(items).Concat(queue.Skip(index)).ToArray();
                queue.InsertRange(index, items);
                Assert.Equal(expect, queue);
            }
        }
    }
}
