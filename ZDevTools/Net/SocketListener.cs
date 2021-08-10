using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ZDevTools.Net
{
    /// <summary>
    /// 一问一答式高性能Socket监听器
    /// </summary>
    public sealed class SocketListener<TUserToken>
        where TUserToken : UserToken, new()
    {
        /// <summary>
        /// The socket used to listen for incoming connection requests.
        /// </summary>
        volatile Socket _socket;

        /// <summary>
        /// 正在停止中
        /// </summary>
        volatile bool _isStopping;

        /// <summary>
        /// Buffer size to use for each socket I/O operation.
        /// </summary>
        readonly int BufferSize;

        /// <summary>
        /// the maximum number of connections the sample is designed to handle simultaneously.
        /// </summary>
        readonly int MaxConnectionCount;

        /// <summary>
        /// Pool of reusable SocketAsyncEventArgs objects for write, read and accept socket operations.
        /// </summary>
        readonly ConcurrentStack<SocketAsyncEventArgs> EPool;

        /// <summary>
        /// 控制连接到本服务器的客户端数量
        /// </summary>
        readonly SemaphoreSlim Semaphore;

        /// <summary>
        /// 同步构造
        /// </summary>
        readonly ManualResetEvent ManualResetEvent;

        /// <summary>
        /// 所有正在与本监听器通讯的Sockets
        /// </summary>
        public ConcurrentDictionary<Socket, Socket> Clients { get; }

        /// <summary>
        /// 是否正在停止中
        /// </summary>
        public bool IsStopping { get { return _isStopping; } }

        /// <summary>
        /// 已成功建立连接处理器，返回值可以确定是否接受该客户端的连接，不接受返回false，监听器会自动关闭该连接
        /// </summary>
        public Func<TUserToken, bool> AcceptedHandler { get; set; }


#if NETCOREAPP
        /// <summary>
        /// 同步消息处理函数（返回null如果不需要返回数据，否则返回回应数据，如果需要关闭连接请设置<see cref="UserToken.IsClosingSocket"/>为 true）
        /// </summary>
        public Func<TUserToken, Memory<byte>, Memory<byte>> MessageHandler { get; set; }
#else
        /// <summary>
        /// 同步消息处理函数（返回null如果不需要返回数据，否则返回回应数据，如果需要关闭连接请设置<see cref="UserToken.IsClosingSocket"/>为 true）
        /// </summary>
        public Func<TUserToken, ArraySegment<byte>, ArraySegment<byte>> MessageHandler { get; set; }
#endif

        ///// <summary>
        ///// 发送超时设置，默认30000毫秒，该值仅在<see cref="Start(int)"/>时起作用
        ///// </summary>
        //public int SendTimeout { get; set; } = 30000;

        ///// <summary>
        ///// 接收超时设置，默认30000毫秒，该值仅在<see cref="Start(int)"/>时起作用
        ///// </summary>
        //public int ReceiveTimeout { get; set; } = 30000;

        /// <summary>
        /// 停止客户端连接关闭等待时间，默认30000毫秒，该值仅在<see cref="Stop(bool)"/>时起作用
        /// </summary>
        public int StopTimeout { get; set; } = 30000;

        /// <summary>
        /// 监听器基础套接字
        /// </summary>
        public Socket Socket { get => _socket; }

        /// <summary>
        /// 设置心跳探测功能参数
        /// </summary>
        public TcpKeepAlive TcpKeepAlive { get; set; }

        /// <summary>
        /// 是否为异步处理模式（启动Socket监听器前设置生效）
        /// </summary>
        public bool IsAsyncMode { get; }

        /// <summary>
        /// Create an uninitialized server instance.  
        /// To start the server listening for connection requests
        /// call the Init method followed by Start method.
        /// </summary>
        /// <param name="maxConnections">Maximum number of connections to be handled simultaneously.</param>
        /// <param name="bufferSize">Buffer size to use for each socket I/O operation.内部单次接收所用，发送数据不受限制。</param>
        /// <param name="isAsyncMode">是否异步模式</param>
        public SocketListener(int maxConnections, int bufferSize, bool isAsyncMode = false)
        {
            // Create the socket which listens for incoming connections.
            this.MaxConnectionCount = maxConnections;
            this.BufferSize = bufferSize;
            this.EPool = new ConcurrentStack<SocketAsyncEventArgs>();
            this.Clients = new ConcurrentDictionary<Socket, Socket>();
            this.Semaphore = new SemaphoreSlim(MaxConnectionCount, MaxConnectionCount);
            this.ManualResetEvent = new ManualResetEvent(true);
            this.IsAsyncMode = isAsyncMode;

            // Preallocate pool of SocketAsyncEventArgs objects.
            for (int i = 0; i < this.MaxConnectionCount; i++)
            {
                SocketAsyncEventArgs e = new SocketAsyncEventArgs();
                if (IsAsyncMode)
                    e.Completed += onCompletedAsync;
                else
                    e.Completed += onCompleted;
                // Add SocketAsyncEventArg to the pool.
                this.EPool.Push(e);
            }
        }

        /// <summary>
        /// Starts the server listening for incoming connection requests.
        /// </summary>
        /// <param name="port">Port where the server will listen for connection requests.</param>
        public void Start(int port)
        {
            if (_isStopping || _socket != null)
                return;

            lock (Locker)
            {
                if (_isStopping || _socket != null)
                    return;

                var bindedIPAddress = IPAddress.Parse("0.0.0.0");
                this._socket = new Socket(bindedIPAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                //默认值就挺好，改的意义不大，不如不改
                //this._socket.ReceiveBufferSize = this.BufferSize;
                //this._socket.SendBufferSize = this.BufferSize;
                //这两个超时时间仅对同步调用起作用，而我们这里是异步调用
                //this._socket.ReceiveTimeout = ReceiveTimeout;
                //this._socket.SendTimeout = SendTimeout;

                //设置心跳包
                if (TcpKeepAlive.IsOn != default)
                    _socket.SetTcpKeepAlive(TcpKeepAlive);

                // Get endpoint for the listener.
                IPEndPoint localEndPoint = new IPEndPoint(bindedIPAddress, port);

                this._socket.Bind(localEndPoint);

                // Start the server.
                this._socket.Listen(this.MaxConnectionCount);

                //创建一个接受请求专用的EventArgs
                SocketAsyncEventArgs e = new SocketAsyncEventArgs();
                if (IsAsyncMode)
                    e.Completed += onCompletedAsync;
                else
                    e.Completed += onCompleted;

                if (IsAsyncMode)
                    _ = this.startAcceptAsync(e);
                else
                    this.startAccept(e);
            }
        }

        /// <summary>
        /// 关键性错误处理
        /// </summary>
        public Action<string, Exception> CriticalErrorHandler { get; set; }

        /// <summary>
        /// 客户端已关闭处理器
        /// </summary>
        public Action<TUserToken> ClientClosedHandler { get; set; }

        /// <summary>
        /// 一般错误处理
        /// </summary>
        public Action<string, Exception> GeneralErrorHandler { get; set; }

        /// <summary>
        /// 内部锁
        /// </summary>
        static readonly object Locker = new object();


        #region Sync Mode
        /// <summary>
        /// Callback called whenever a receive or send operation is completed on a socket.
        /// </summary>
        /// <param name="sender">Object who raised the event.</param>
        /// <param name="e">SocketAsyncEventArg associated with the completed send/receive operation.</param>
        private void onCompleted(object sender, SocketAsyncEventArgs e)
        { //在该函数内调用ShutDown/Close可能导致Socket未处理，下次再启动同一端口时会继续处理未Accept的请求
            try
            {
                switch (e.LastOperation)
                {
                    case SocketAsyncOperation.Receive:
                        this.processReceive(e);
                        break;
                    case SocketAsyncOperation.Send:
                        this.processSend(e);
                        break;
                    case SocketAsyncOperation.Accept:
                        this.processAccept(e);
                        break;
                    default:
                        throw new Exception($"无法根据上次 {e.LastOperation} 操作做出对应动作");
                }
            }
            catch (Exception ex)
            {
                closeClientSocket(e); //回收客户端socket

                var message = $"未能继续处理 {e.LastOperation} 操作，{nameof(SocketListener<TUserToken>)} 内部错误";
                var criticalErrorHandler = CriticalErrorHandler;
                if (criticalErrorHandler != null)
                    criticalErrorHandler(message, ex);
                else
                {
                    reportGeneralError(message, ex);
                    throw;
                }
            }
        }

        /// <summary>
        /// Begins an operation to accept a connection request from the client.
        /// </summary>
        /// <param name="e">The context object to use when issuing 
        /// the accept operation on the server's listening socket.</param>
        private void startAccept(SocketAsyncEventArgs e)
        {
            if (_isStopping) //不再接收新的请求
                return;

            this.Semaphore.Wait(); //检查信号量是否充足，如果不足，就一直等待
            e.AcceptSocket = null;
            if (!this._socket.AcceptAsync(e))
            {
                this.processAccept(e);
            }
        }

        /// <summary>
        /// Process the accept for the socket listener.
        /// </summary>
        /// <param name="eAccept">SocketAsyncEventArg associated with the completed accept operation.</param>
        private void processAccept(SocketAsyncEventArgs eAccept)
        {
            Socket socket = eAccept.AcceptSocket;

            // 在这里设置心跳包参数也是可以的，但没有直接在server的Socket中设置效率高，因此，不在这里进行设置。
            //if (TcpKeepAlive.IsOn != default)
            //    if (socket.IOControl(IOControlCode.KeepAliveValues, TcpKeepAlive.GetBytes(), null) > 0) throw new InvalidOperationException("无法设置KeepAliveValues");

            //hack:  调查socket为null的原因
            if (socket == null)
            {
                reportGeneralError($"AcceptSocket为 null，SocketError为 " + eAccept.SocketError);
            }

            if (socket != null && socket.Connected)
            {
                Clients.TryAdd(socket, socket);//客户端已连接

                EPool.TryPop(out var e);

                ManualResetEvent.Reset();

                TUserToken token;
                e.UserToken = token = new TUserToken() { Socket = socket /* 关联Socket */, ReceivingBuffer = new byte[BufferSize] }; //每个新连接都重新分配一个UserToken上下文
                var acceptedHandler = AcceptedHandler;
                if (acceptedHandler != null && !acceptedHandler(token))
                {
                    closeClientSocket(e);
                }
                else
                {
                    setBuffer(e, token);
                    if (!socket.ReceiveAsync(e))
                    {
                        this.processReceive(e);
                    }
                }

                // Accept the next connection request.
                this.startAccept(eAccept);
            }
            else
            {
                //说明我们把服务器关了，要释放一个接受信号
                Semaphore.Release();
            }
        }

        /// <summary>
        /// This method is invoked when an asynchronous receive operation completes. 
        /// If the remote host closed the connection, then the socket is closed.  
        /// If data was received then the data is echoed back to the client.
        /// </summary>
        /// <param name="e">SocketAsyncEventArg associated with the completed receive operation.</param>
        private void processReceive(SocketAsyncEventArgs e)
        {
            // Check if the remote host closed the connection.
            if (e.BytesTransferred > 0)
            {
                if (e.SocketError == SocketError.Success)
                {
                    var token = (TUserToken)e.UserToken;
                    Socket socket = token.Socket;

                    var messageHandler = MessageHandler;
#if NETCOREAPP
                    Memory<byte> memory;
                    try
                    {
                        memory = messageHandler == null ? default : messageHandler(token, e.MemoryBuffer.Slice(e.Offset, e.BytesTransferred));
                    }
#else
                    ArraySegment<byte> segment;
                    try
                    {
                        segment = messageHandler == null ? default : messageHandler(token, new ArraySegment<byte>(e.Buffer, e.Offset, e.BytesTransferred));
                    }

#endif
                    catch (Exception ex)
                    {
                        reportGeneralError($"用户消息处理函数发生错误，本次通讯连接将被关闭", ex);
                        closeClientSocket(e);
                        return;
                    }
#if NETCOREAPP
                    if (!memory.IsEmpty) //返回的结果不为Empty，需要进行发送操作
                    {
                        e.SetBuffer(memory);
#else
                    if (segment.Count > 0) //返回的结果不为空，需要进行发送操作，大于0，则Array肯定不为null
                    {
                        e.SetBuffer(segment.Array, segment.Offset, segment.Count);
#endif
                        if (!socket.SendAsync(e)) //向客户端发送数据
                        {
                            this.processSend(e);
                        }
                    }
                    else //不需要进行发送操作
                    {
                        if (token.IsClosingSocket) //如果消息处理器要求关闭连接，那么关闭连接
                        {
                            closeClientSocket(e);
                            return;
                        }
                        if (!socket.ReceiveAsync(e)) //接收下一次数据
                        {
                            // Read the next block of data sent by client.
                            this.processReceive(e);
                        }
                    }
                }
                else
                {
                    this.reportOperationErrorWithClose(e, "接收");
                }
            }
            else
            {   //通讯结束正常执行操作
                this.closeClientSocket(e);
            }
        }

        /// <summary>
        /// This method is invoked when an asynchronous send operation completes.  
        /// The method issues another receive on the socket to read any additional 
        /// data sent from the client.
        /// </summary>
        /// <param name="e">SocketAsyncEventArg associated with the completed send operation.</param>
        private void processSend(SocketAsyncEventArgs e)
        {
            if (e.SocketError == SocketError.Success)
            {
                var token = (TUserToken)e.UserToken;
                if (token.IsClosingSocket) //如果用户处理器要求发送完毕后关闭连接，那么关闭连接
                {
                    closeClientSocket(e);
                    return;
                }
                setBuffer(e, token);
                if (!token.Socket.ReceiveAsync(e))
                {
                    // Read the next block of data send from the client.
                    this.processReceive(e);
                }
            }
            else
            {
                this.reportOperationErrorWithClose(e, "发送");
            }
        }
        #endregion

        #region Async Mode
        /// <summary>
        /// 已成功建立连接处理器，返回值可以确定是否接受该客户端的连接，不接受返回false，监听器会自动关闭该连接(异步版本)
        /// </summary>
        public Func<TUserToken, ValueTask<bool>> AcceptedHandlerAsync { get; set; }


#if NETCOREAPP
        /// <summary>
        /// 异步消息处理函数
        /// </summary>
        public Func<TUserToken, Memory<byte>, ValueTask<Memory<byte>>> MessageHandlerAsync { get; set; }
#else
        /// <summary>
        /// 异步消息处理函数
        /// </summary>
        public Func<TUserToken, ArraySegment<byte>, ValueTask<ArraySegment<byte>>> MessageHandlerAsync { get; set; }
#endif

        /// <summary>
        /// Callback called whenever a receive or send operation is completed on a socket.
        /// </summary>
        /// <param name="sender">Object who raised the event.</param>
        /// <param name="e">SocketAsyncEventArg associated with the completed send/receive operation.</param>
        private async void onCompletedAsync(object sender, SocketAsyncEventArgs e)
        { //在该函数内调用ShutDown/Close可能导致Socket未处理，下次再启动同一端口时会继续处理未Accept的请求
            try
            {
                switch (e.LastOperation)
                {
                    case SocketAsyncOperation.Receive:
                        await this.processReceiveAsync(e);
                        break;
                    case SocketAsyncOperation.Send:
                        await this.processSendAsync(e);
                        break;
                    case SocketAsyncOperation.Accept:
                        await this.processAcceptAsync(e);
                        break;
                    default:
                        throw new Exception($"无法根据上次 {e.LastOperation} 操作做出对应动作");
                }
            }
            catch (Exception ex)
            {
                closeClientSocket(e); //回收客户端socket

                var message = $"未能继续处理 {e.LastOperation} 操作，{nameof(SocketListener<TUserToken>)} 内部错误";
                var criticalErrorHandler = CriticalErrorHandler;
                if (criticalErrorHandler != null)
                    criticalErrorHandler(message, ex);
                else
                {
                    reportGeneralError(message, ex);
                    throw;
                }
            }
        }

        /// <summary>
        /// This method is invoked when an asynchronous receive operation completes. 
        /// If the remote host closed the connection, then the socket is closed.  
        /// If data was received then the data is echoed back to the client.
        /// </summary>
        /// <param name="e">SocketAsyncEventArg associated with the completed receive operation.</param>
        private async ValueTask processReceiveAsync(SocketAsyncEventArgs e)
        {
            // Check if the remote host closed the connection.
            if (e.BytesTransferred > 0)
            {
                if (e.SocketError == SocketError.Success)
                {
                    var token = (TUserToken)e.UserToken;
                    Socket socket = token.Socket;

                    var messageHandlerAsync = MessageHandlerAsync;
#if NETCOREAPP
                    Memory<byte> memory;
                    try
                    {
                        memory = messageHandlerAsync == null ? default : await messageHandlerAsync(token, e.MemoryBuffer.Slice(e.Offset, e.BytesTransferred));
                    }
#else
                    ArraySegment<byte> segment;
                    try
                    {
                        segment = messageHandlerAsync == null ? default : await messageHandlerAsync(token, new ArraySegment<byte>(e.Buffer, e.Offset, e.BytesTransferred));
                    }
#endif
                    catch (Exception ex)
                    {
                        reportGeneralError($"用户消息处理函数发生错误，本次通讯连接将被关闭", ex);
                        closeClientSocket(e);
                        return;
                    }
#if NETCOREAPP
                    if (!memory.IsEmpty) //返回的结果不为Empty，需要进行发送操作
                    {
                        e.SetBuffer(memory);
#else
                    if (segment.Count > 0) //返回的结果不为空，需要进行发送操作，大于0，则Array肯定不为null
                    {
                        e.SetBuffer(segment.Array, segment.Offset, segment.Count);
#endif
                        if (!socket.SendAsync(e)) //向客户端发送数据
                        {
                            await this.processSendAsync(e);
                        }
                    }
                    else //不需要进行发送操作
                    {
                        if (token.IsClosingSocket) //如果消息处理器要求关闭连接，那么关闭连接
                        {
                            closeClientSocket(e);
                            return;
                        }
                        if (!socket.ReceiveAsync(e)) //接收下一次数据
                        {
                            // Read the next block of data sent by client.
                            await processReceiveAsync(e);
                        }
                    }
                }
                else
                {
                    this.reportOperationErrorWithClose(e, "接收");
                }
            }
            else
            {   //通讯结束正常执行操作
                this.closeClientSocket(e);
            }
        }

        /// <summary>
        /// This method is invoked when an asynchronous send operation completes.  
        /// The method issues another receive on the socket to read any additional 
        /// data sent from the client.
        /// </summary>
        /// <param name="e">SocketAsyncEventArg associated with the completed send operation.</param>
        private async ValueTask processSendAsync(SocketAsyncEventArgs e)
        {
            if (e.SocketError == SocketError.Success)
            {
                var token = (TUserToken)e.UserToken;
                if (token.IsClosingSocket) //如果用户处理器要求发送完毕后关闭连接，那么关闭连接
                {
                    closeClientSocket(e);
                    return;
                }
                setBuffer(e, token);
                if (!token.Socket.ReceiveAsync(e))
                {
                    // Read the next block of data send from the client.
                    await processReceiveAsync(e);
                }
            }
            else
            {
                this.reportOperationErrorWithClose(e, "发送");
            }
        }

        /// <summary>
        /// Process the accept for the socket listener.
        /// </summary>
        /// <param name="eAccept">SocketAsyncEventArg associated with the completed accept operation.</param>
        private async ValueTask processAcceptAsync(SocketAsyncEventArgs eAccept)
        {
            Socket socket = eAccept.AcceptSocket;

            //hack:  调查socket为null的原因
            if (socket == null)
            {
                reportGeneralError($"AcceptSocket为 null，SocketError为 " + eAccept.SocketError);
            }

            if (socket != null && socket.Connected)
            {
                Clients.TryAdd(socket, socket);//客户端已连接

                EPool.TryPop(out var e);

                ManualResetEvent.Reset();

                TUserToken token;
                e.UserToken = token = new TUserToken() { Socket = socket /* 关联Socket */, ReceivingBuffer = new byte[BufferSize] }; //每个新连接都重新分配一个UserToken上下文
                var acceptedHandlerAsync = AcceptedHandlerAsync;
                if (acceptedHandlerAsync != null && !await acceptedHandlerAsync(token))
                {
                    closeClientSocket(e);
                }
                else
                {
                    setBuffer(e, token);
                    if (!socket.ReceiveAsync(e))
                    {
                        await processReceiveAsync(e);
                    }
                }
                // Accept the next connection request.
                await this.startAcceptAsync(eAccept);
            }
            else
            {
                //说明我们把服务器关了，要释放一个接受信号
                Semaphore.Release();
            }
        }

        /// <summary>
        /// Begins an operation to accept a connection request from the client.
        /// </summary>
        /// <param name="e">The context object to use when issuing 
        /// the accept operation on the server's listening socket.</param>
        private async ValueTask startAcceptAsync(SocketAsyncEventArgs e)
        {
            if (_isStopping) //不再接收新的请求
                return;

            this.Semaphore.Wait(); //检查信号量是否充足，如果不足，就一直等待
            e.AcceptSocket = null;
            if (!this._socket.AcceptAsync(e))
            {
                await this.processAcceptAsync(e);
            }
        }

        #endregion

        #region Shared

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        private static void setBuffer(SocketAsyncEventArgs e, TUserToken token)
        {
#if NETCOREAPP
            e.SetBuffer(token.ReceivingBuffer);
#else
            e.SetBuffer(token.ReceivingBuffer, 0, token.ReceivingBuffer.Length);
#endif
        }

        /// <summary>
        /// Stop the server.
        /// </summary>
        public void Stop(bool waitClientClosed = false)
        {
            if (_isStopping || _socket == null)
                return;

            lock (Locker)
            {
                if (_isStopping || _socket == null)
                    return;

                _isStopping = true;

                if (waitClientClosed)
                {
                    bool timeOut = !ManualResetEvent.WaitOne(StopTimeout);
                    closeSocket(); //得体地强制关闭客户端连接
                    if (timeOut) //扔出超时错误
                        throw new TimeoutException($"等待 {nameof(SocketListener<TUserToken>)} 关闭超时");
                }
                else
                    closeSocket();
            }
        }

        private void closeSocket()
        {
            //关闭所有正在连接的客户端
            while (Clients.Count > 0)
            {
                foreach (var client in Clients)
                {
                    client.Value.Close();
                }
                Thread.Sleep(500);
            }
            this._socket.Close();
            this._socket = null;
            _isStopping = false;
        }

        /// <summary>
        /// Close the socket associated with the client.
        /// </summary>
        /// <param name="e">SocketAsyncEventArg associated with the completed send/receive operation.</param>
        private void closeClientSocket(SocketAsyncEventArgs e)
        {
            this.closeClientSocket((TUserToken)e.UserToken, e);
        }

        private void closeClientSocket(TUserToken token, SocketAsyncEventArgs e)
        {
            Clients.TryRemove(token.Socket, out _); //移除客户端

            token.Dispose();
            // Free the SocketAsyncEventArg so they can be reused by another client.
            this.EPool.Push(e);
            // Decrement the counter keeping track of the total number of clients connected to the server.
            this.Semaphore.Release();
            if (EPool.Count == MaxConnectionCount)
                ManualResetEvent.Set();

            ClientClosedHandler?.Invoke(token);
        }

        private void reportOperationErrorWithClose(SocketAsyncEventArgs e, string operationName)
        {
            var token = (TUserToken)e.UserToken;
            reportGeneralError($"与 {token.Socket.RemoteEndPoint} 进行 {operationName} 通讯操作出错，连接即将关闭");
            this.closeClientSocket(token, e);
        }

        void reportGeneralError(string message, Exception exception)
        {
            GeneralErrorHandler?.Invoke(message, exception);
        }

        void reportGeneralError(string message)
        {
            reportGeneralError(message, null);
        }
        #endregion
    }
}
