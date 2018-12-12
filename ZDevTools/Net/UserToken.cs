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
        /// Accept socket.
        /// </summary>
        public Socket Socket { get; internal set; }

        /// <summary>
        /// 数据流
        /// </summary>
        internal MemoryStream Stream { get; } = new MemoryStream();

        /// <summary>
        /// Release instance.
        /// </summary>
        public void Dispose()
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

}
