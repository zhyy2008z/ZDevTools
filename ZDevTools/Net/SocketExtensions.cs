using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace ZDevTools.Net
{
    /// <summary>
    /// 套接字扩展方法
    /// </summary>
    public static class SocketExtensions
    {
        /// <summary>
        /// 设置TCP心跳保持
        /// </summary>
        public static void SetTcpKeepAlive(this Socket socket, TcpKeepAlive tcpKeepAlive)
        {
            if (socket.IOControl(IOControlCode.KeepAliveValues, tcpKeepAlive.ToBytes(), null) > 0)
                throw new InvalidOperationException("设置TcpKeepAlive失败");
        }
    }
}
