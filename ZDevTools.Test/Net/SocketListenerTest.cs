using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using ZDevTools.Net;
using System.Net.Sockets;
using System.Linq;
using Xunit.Abstractions;

namespace ZDevTools.Test.Net
{
    public class SocketListenerTest
    {
        readonly ITestOutputHelper Logger;

        public SocketListenerTest(ITestOutputHelper logger)
        {
            Logger = logger;
        }


        [Fact]
        public void AsyncTest()
        {
            SocketListener<UserToken> listener = new SocketListener<UserToken>(10, 10000000, true);

            listener.AcceptedHandlerAsync = async token => true;

#if NETCOREAPP

            listener.MessageHandlerAsync = async (token, data) =>
            {
                var queue = token.GetByteQueue(data.Span);

                while (queue.Dequeue(1000000, out Memory<byte> memory))
                {
                    Logger.WriteLine(memory.Length.ToString());
                }

                return data;
            };
#else
            listener.MessageHandlerAsync = async (token, data) =>
            {
                var queue = token.GetByteQueue(data);

                while (queue.Dequeue(1000000, out ArraySegment<byte> segment))
                {
                    Logger.WriteLine(segment.Count.ToString());
                }

                return data;
            };
#endif

            listener.Start(10001);

            TcpClient tcpClient = new TcpClient();
            tcpClient.Connect("localhost", 10001);
            var stream = tcpClient.GetStream();
            Random random = new Random();

            for (int i = 0; i < 10; i++)
            {
                int length = random.Next(1, 10000000);

                byte[] source1 = new byte[length];
                random.NextBytes(source1);

                Logger.WriteLine("发送数据 {0} 个", source1.Length);


                stream.Write(source1, 0, source1.Length / 2);


                stream.Write(source1, source1.Length / 2, source1.Length - source1.Length / 2);

                var result1 = readBytes(stream, source1.Length);

                Assert.True(source1 != result1);

                Assert.Equal(source1, result1);

            }

            tcpClient.Close();

            listener.Stop(true);

        }



        /// <summary>
        /// 保证从流中读取到指定的目标长度的数据并返回数据数组
        /// </summary>
        /// <param name="stream">目标流</param>
        /// <param name="cache">可重用的cache数组</param>
        /// <param name="buffer">可重用的buffer数组</param>
        /// <param name="targetSize">目标大小</param>
        /// <returns></returns>
        private static byte[] readBytes(NetworkStream stream, int targetSize)
        {
            byte[] buffer = new byte[targetSize];
            int readedCount = default;
            do
            {
                int count = stream.Read(buffer, readedCount, targetSize - readedCount);
                if (count == 0)
                    throw new InvalidOperationException();
                readedCount += count;
            } while (readedCount < targetSize);
            return buffer;
        }



        [Fact]
        public void SyncTest()
        {
            SocketListener<UserToken> listener = new SocketListener<UserToken>(10, 10000000);


            listener.AcceptedHandler = token => true;


#if NETCOREAPP

            listener.MessageHandler = (token, data) =>
            {
                var queue = token.GetByteQueue(data.Span);

                while (queue.Dequeue(1000000, out Memory<byte> memory))
                {
                    Logger.WriteLine(memory.Length.ToString());
                }

                return data;
            };
#else
            listener.MessageHandler = (token, data) =>
            {
                var queue = token.GetByteQueue(data);

                while (queue.Dequeue(1000000, out ArraySegment<byte> segment))
                {
                    Logger.WriteLine(segment.Count.ToString());
                }

                return data;
            };
#endif

            listener.Start(10001);

            TcpClient tcpClient = new TcpClient();
            tcpClient.Connect("localhost", 10001);
            var stream = tcpClient.GetStream();
            Random random = new Random();

            for (int i = 0; i < 10; i++)
            {
                int length = random.Next(1, 10000000);

                byte[] source1 = new byte[length];
                random.NextBytes(source1);

                Logger.WriteLine("发送数据 {0} 个", source1.Length);


                stream.Write(source1, 0, source1.Length / 2);


                stream.Write(source1, source1.Length / 2, source1.Length - source1.Length / 2);

                var result1 = readBytes(stream, source1.Length);

                Assert.True(source1 != result1);

                Assert.Equal(source1, result1);

            }

            tcpClient.Close();

            listener.Stop(true);

        }

    }
}
