using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.IO;

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

        /// <summary>
        /// 数据流
        /// </summary>
        internal MemoryStream Stream { get; } = new MemoryStream();

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
                    this.Stream.Close();
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
