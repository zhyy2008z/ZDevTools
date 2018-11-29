using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ZDevTools.Net
{
    public sealed class Token : IDisposable
    {
        /// <summary>
        /// Class constructor.
        /// </summary>
        /// <param name="socket">Socket to accept incoming data.</param>
        /// <param name="bufferSize">Buffer size for accepted data.</param>
        public Token()
        {
            MessageBytes = new List<byte>();
        }

        /// <summary>
        /// Accept socket.
        /// </summary>
        public Socket Socket { get; set; }

        /// <summary>
        /// 从上次发送到现在接收到的所有数据
        /// </summary>
        public List<byte> MessageBytes { get; }

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
            }
        }
    }

}
