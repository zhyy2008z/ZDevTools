using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using ZDevTools.Collections;
using System.Runtime.CompilerServices;

namespace ZDevTools.Net
{
    /// <summary>
    /// 与连接关联的上下文（可继承扩展这个上下文）
    /// </summary>
    public class UserToken : IDisposable
    {
        /// <summary>
        /// 代表客户端与服务器通讯的套接字
        /// </summary>
        public Socket Socket { get; internal set; }

        internal BufferQueue<byte> ByteQueue { get; private set; }

        /// <summary>
        /// 接收缓存
        /// </summary>
#if NETCOREAPP
        internal Memory<byte> ReceivingBuffer { get; set; }
#else
        internal byte[] ReceivingBuffer { get; set; }
#endif

        /// <summary>
        /// 是否需要关闭与客户通讯的套接字
        /// </summary>
        public bool IsClosingSocket { get; set; }

        /// <summary>
        /// 内部字节列队是否为空
        /// </summary>
        public bool IsByteQueueEmpty
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ByteQueue == null || ByteQueue.Length == 0;
        }

#if NETCOREAPP
        /// <summary>
        /// 从网络流获取缓冲队列
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public BufferQueue<byte> GetByteQueue(Memory<byte> data)
        {
            if (ByteQueue == null) ByteQueue = new BufferQueue<byte>(ReceivingBuffer.Length);
            ByteQueue.Enqueue(data.Span);
            return ByteQueue;
        }
#else
        /// <summary>
        /// 从网络流获取缓冲队列
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public BufferQueue<byte> GetByteQueue(ArraySegment<byte> data)
        {
            if (ByteQueue == null) ByteQueue = new BufferQueue<byte>(ReceivingBuffer.Length);
            ByteQueue.Enqueue(data);
            return ByteQueue;
        }
#endif

        /// <summary>
        /// 重写该方法以释放你自己的资源
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                try
                {
                    this.Socket.Shutdown(SocketShutdown.Send);
                }
                catch { }
                finally
                {
                    this.Socket.Close();
                }
            }
        }

        /// <summary>
        /// Release instance.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 析构函数
        /// </summary>
        ~UserToken()
        {
            Dispose(false);
        }
    }

}
