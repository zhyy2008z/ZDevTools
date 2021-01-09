using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xunit;

using ZDevTools.Collections;

namespace ZDevTools.Test.Collections
{
    public class BufferQueueTest
    {
#if NETCOREAPP
        [Fact]
        public void TestPart1()
        {
            BufferQueue<int> queue = new BufferQueue<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };

            queue.AddRange(new[] { 10, 11, 12, 13, 14, 15, 16, 17 });
            Assert.Equal(new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17 }, queue);

            queue.RemoveAt(10);
            Assert.Equal(new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 11, 12, 13, 14, 15, 16, 17 }, queue);

            queue.RemoveRange(1, 5);
            Assert.Equal(new[] { 0, 6, 7, 8, 9, 11, 12, 13, 14, 15, 16, 17 }, queue);

            queue.Insert(2, new[] { 3, 4, 5 });
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

            queue.Clear(3);
            Assert.Equal(new[] { 3, 3, 5, 2, 5, 3, 32, 2, 3 }, queue);

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
            BufferQueue<int> queue = new BufferQueue<int>(0) { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };

            queue.AddRange(new[] { 10, 11, 12, 13, 14, 15, 16, 17 });
            Assert.Equal(new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17 }, queue);

            queue.RemoveAt(10);
            Assert.Equal(new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 11, 12, 13, 14, 15, 16, 17 }, queue);

            queue.RemoveRange(1, 5);
            Assert.Equal(new[] { 0, 6, 7, 8, 9, 11, 12, 13, 14, 15, 16, 17 }, queue);

            queue.Insert(2, new[] { 3, 4, 5 });
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

            queue.Clear(3);
            Assert.Equal(new[] { 3, 3, 5, 2, 5, 3, 32, 2, 3 }, queue);

            Assert.ThrowsAny<Exception>(() => queue.Capacity = 0);

            queue.Clear();
            Assert.Equal(new int[] { }, queue);

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
        public void TestPart3()
        {
            var list = new int[] { 1, 2, 3, 4, 5, 6, 7 };

            var queue = list.ToBufferQueue();

            Assert.Equal(7, queue.Count);


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

            queue.Insert(4, new[] { 3, 3, 3, 5 });
            Assert.Equal(new List<int>() { 0, 1, 2, 7, 3, 3, 3, 5, 8, 9 }, queue);

            queue.RemoveRange(4, 4);

            Assert.Equal(new List<int>() { 0, 1, 2, 7, 8, 9 }, queue);
        }


#else
        [Fact]
        public void TestPart1()
        {
            BufferQueue<int> queue = new BufferQueue<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };

            queue.AddRange(new ArraySegment<int>(new[] { 10, 11, 12, 13, 14, 15, 16, 17 }));
            Assert.Equal(new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17 }, queue);

            queue.RemoveAt(10);
            Assert.Equal(new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 11, 12, 13, 14, 15, 16, 17 }, queue);

            queue.RemoveRange(1, 5);
            Assert.Equal(new[] { 0, 6, 7, 8, 9, 11, 12, 13, 14, 15, 16, 17 }, queue);

            queue.Insert(2, new[] { 3, 4, 5 }.AsArraySegment());
            Assert.Equal(new[] { 0, 6, 3, 4, 5, 7, 8, 9, 11, 12, 13, 14, 15, 16, 17 }, queue);

            queue.Enqueue(5);
            Assert.Equal(new[] { 0, 6, 3, 4, 5, 7, 8, 9, 11, 12, 13, 14, 15, 16, 17, 5 }, queue);

            queue.Enqueue(new[] { 3, 3, 3, 3, 3, 3, 3 }.AsArraySegment());
            Assert.Equal(new[] { 0, 6, 3, 4, 5, 7, 8, 9, 11, 12, 13, 14, 15, 16, 17, 5, 3, 3, 3, 3, 3, 3, 3 }, queue);

            queue.Dequeue(10, out int[] outBytes);
            Assert.Equal(new[] { 13, 14, 15, 16, 17, 5, 3, 3, 3, 3, 3, 3, 3 }, queue);
            Assert.Equal(new[] { 0, 6, 3, 4, 5, 7, 8, 9, 11, 12, }, outBytes);

            queue.Pop(out var t);
            Assert.Equal(new[] { 14, 15, 16, 17, 5, 3, 3, 3, 3, 3, 3, 3 }, queue);
            Assert.Equal(13, t);

            queue.Push(3);
            queue.Push(4);
            Assert.Equal(new[] { 4, 3, 14, 15, 16, 17, 5, 3, 3, 3, 3, 3, 3, 3 }.AsArraySegment(), queue);

            queue.SetRange(0, new[] { 3, 3, 3, 3, 3 }.AsArraySegment());
            Assert.Equal(new[] { 3, 3, 3, 3, 3, 17, 5, 3, 3, 3, 3, 3, 3, 3 }, queue);

            queue.SetRange(0, new[] { 3, 3, 3, 3, 3 }.AsArraySegment());
            Assert.Equal(new[] { 3, 3, 3, 3, 3, 17, 5, 3, 3, 3, 3, 3, 3, 3 }, queue);

            queue.SetRange(5, new[] { 3, 3, 3, 3, 3 }.AsArraySegment());
            Assert.Equal(new[] { 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3 }, queue);

            Assert.ThrowsAny<Exception>(() => queue.SetRange(100, new int[0].AsArraySegment()));

            queue.Remove(3);
            Assert.Equal(new[] { 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3 }, queue);

            queue.RemoveAt(3);
            Assert.Equal(new[] { 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3 }, queue);

            queue.SetRange(5, new[] { 5, 2, 5, 3, 32, 2 }.AsArraySegment());
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

            queue.Clear(3);
            Assert.Equal(new[] { 3, 3, 5, 2, 5, 3, 32, 2, 3 }, queue);

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
            BufferQueue<int> queue = new BufferQueue<int>(0) { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };

            queue.AddRange(new[] { 10, 11, 12, 13, 14, 15, 16, 17 }.AsArraySegment());
            Assert.Equal(new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17 }, queue);

            queue.RemoveAt(10);
            Assert.Equal(new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 11, 12, 13, 14, 15, 16, 17 }, queue);

            queue.RemoveRange(1, 5);
            Assert.Equal(new[] { 0, 6, 7, 8, 9, 11, 12, 13, 14, 15, 16, 17 }, queue);

            queue.Insert(2, new[] { 3, 4, 5 }.AsArraySegment());
            Assert.Equal(new[] { 0, 6, 3, 4, 5, 7, 8, 9, 11, 12, 13, 14, 15, 16, 17 }, queue);

            queue.Enqueue(5);
            Assert.Equal(new[] { 0, 6, 3, 4, 5, 7, 8, 9, 11, 12, 13, 14, 15, 16, 17, 5 }, queue);

            queue.Enqueue(new[] { 3, 3, 3, 3, 3, 3, 3 }.AsArraySegment());
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

            queue.SetRange(0, new[] { 3, 3, 3, 3, 3 }.AsArraySegment());
            Assert.Equal(new[] { 3, 3, 3, 3, 3, 17, 5, 3, 3, 3, 3, 3, 3, 3 }, queue);

            queue.SetRange(0, new[] { 3, 3, 3, 3, 3 }.AsArraySegment());
            Assert.Equal(new[] { 3, 3, 3, 3, 3, 17, 5, 3, 3, 3, 3, 3, 3, 3 }, queue);

            queue.SetRange(5, new[] { 3, 3, 3, 3, 3 }.AsArraySegment());
            Assert.Equal(new[] { 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3 }, queue);

            Assert.ThrowsAny<Exception>(() => queue.SetRange(100, new int[0].AsArraySegment()));

            queue.Remove(3);
            Assert.Equal(new[] { 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3 }, queue);

            queue.RemoveAt(3);
            Assert.Equal(new[] { 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3 }, queue);

            queue.SetRange(5, new[] { 5, 2, 5, 3, 32, 2 }.AsArraySegment());
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

            queue.Clear(3);
            Assert.Equal(new[] { 3, 3, 5, 2, 5, 3, 32, 2, 3 }, queue);

            Assert.ThrowsAny<Exception>(() => queue.Capacity = 0);

            queue.Clear();
            Assert.Equal(new int[] { }, queue);

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
        public void TestPart3()
        {
            var list = new int[] { 1, 2, 3, 4, 5, 6, 7 };

            var queue = list.ToBufferQueue();

            Assert.Equal(7, queue.Count);


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
#endif
    }
}
