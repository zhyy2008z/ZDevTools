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
        /// 消息处理函数（返回回应数据）
        /// </summary>
        public Func<TUserToken, byte[], byte[]> MessageHandler { get; set; }

        /// <summary>
        /// 发送超时设置，默认30000毫秒，该值仅在<see cref="Start(int)"/>时起作用
        /// </summary>
        public int SendTimeout { get; set; } = 30000;

        /// <summary>
        /// 接收超时设置，默认30000毫秒，该值仅在<see cref="Start(int)"/>时起作用
        /// </summary>
        public int ReceiveTimeout { get; set; } = 30000;

        /// <summary>
        /// 停止客户端连接关闭等待时间，默认30000毫秒，该值仅在<see cref="Stop()"/>时起作用
        /// </summary>
        public int StopTimeout { get; set; } = 30000;

        /// <summary>
        /// 监听器基础套接字
        /// </summary>
        public Socket Socket { get => _socket; }

        /// <summary>
        /// Create an uninitialized server instance.  
        /// To start the server listening for connection requests
        /// call the Init method followed by Start method.
        /// </summary>
        /// <param name="maxConnections">Maximum number of connections to be handled simultaneously.</param>
        /// <param name="bufferSize">Buffer size to use for each socket I/O operation.一次最多只能发送/接收这么多数据</param>
        public SocketListener(int maxConnections, int bufferSize)
        {
            // Create the socket which listens for incoming connections.
            this.MaxConnectionCount = maxConnections;
            this.BufferSize = bufferSize;
            this.EPool = new ConcurrentStack<SocketAsyncEventArgs>();
            this.Clients = new ConcurrentDictionary<Socket, Socket>();
            this.Semaphore = new SemaphoreSlim(MaxConnectionCount, MaxConnectionCount);
            this.ManualResetEvent = new ManualResetEvent(true);

            // Preallocate pool of SocketAsyncEventArgs objects.
            for (int i = 0; i < this.MaxConnectionCount; i++)
            {
                SocketAsyncEventArgs e = new SocketAsyncEventArgs();
                e.Completed += onCompleted;
                e.SetBuffer(new byte[this.BufferSize], 0, this.BufferSize);
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

            lock (_locker)
            {
                if (_isStopping || _socket != null)
                    return;

                var bindedIPAddress = IPAddress.Parse("0.0.0.0");
                this._socket = new Socket(bindedIPAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                this._socket.ReceiveBufferSize = this.BufferSize;
                this._socket.SendBufferSize = this.BufferSize;
                this._socket.ReceiveTimeout = ReceiveTimeout;
                this._socket.SendTimeout = SendTimeout;

                // Get endpoint for the listener.
                IPEndPoint localEndPoint = new IPEndPoint(bindedIPAddress, port);

                this._socket.Bind(localEndPoint);

                // Start the server.
                this._socket.Listen(this.MaxConnectionCount);

                //创建一个接受请求专用的EventArgs
                SocketAsyncEventArgs e = new SocketAsyncEventArgs();
                e.Completed += onCompleted;
                //不要设置Buffer，因为接受连接用不上缓存，除非你真的需要。
                this.startAccept(e);
            }
        }

        /// <summary>
        /// 关键性错误处理
        /// </summary>
        public Action<string, Exception> CriticalErrorHandler { get; set; }

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
                        throw new Exception($"无法根据上次{e.LastOperation}操作做出对应动作");
                }
            }
            catch (Exception ex)
            {
                var message = $"未能继续处理{e.LastOperation}操作，{nameof(SocketListener<TUserToken>)}内部错误";
                if (CriticalErrorHandler != null)
                    CriticalErrorHandler(message, ex);
                else
                {
                    reportError(message, ex);
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
        /// 已成功建立连接处理器，返回值可以确定是否接受该客户端的连接，不接受返回false，监听器会自动关闭该连接
        /// </summary>
        public Func<TUserToken, bool> AcceptedHandler { get; set; }

        /// <summary>
        /// Process the accept for the socket listener.
        /// </summary>
        /// <param name="e">SocketAsyncEventArg associated with the completed accept operation.</param>
        private void processAccept(SocketAsyncEventArgs e)
        {
            Socket socket = e.AcceptSocket;

            if (socket.Connected)
            {
                Clients.TryAdd(socket, socket);//客户端已连接

                EPool.TryPop(out var args);

                ManualResetEvent.Reset();

                args.UserToken = new TUserToken() { Socket = socket /* 关联Socket */ }; //每个新连接都重新分配一个UserToken上下文

                var acceptedHandler = AcceptedHandler;
                if (acceptedHandler != null && !acceptedHandler((TUserToken)args.UserToken))
                {
                    closeClientSocket(args);
                }
                else
                {
                    if (!socket.ReceiveAsync(args))
                    {
                        this.processReceive(args);
                    }
                }

                // Accept the next connection request.
                this.startAccept(e);
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
                    UserToken token = (UserToken)e.UserToken;

                    token.Stream.Write(e.Buffer, e.Offset, e.BytesTransferred);

                    Socket socket = token.Socket;

                    if (socket.Available == 0)
                    {
                        // Set return buffer.
                        byte[] bytes;

                        try
                        {
                            bytes = MessageHandler((TUserToken)token, token.Stream.ToArray());
                        }
                        catch (Exception ex)
                        {
                            reportError($"用户消息处理函数发生错误，本次通讯连接将被关闭", ex);
                            closeClientSocket(e);
                            return;
                        }

                        if (bytes != null) //返回的结果不为null，需要进行发送操作
                        {
                            if (bytes.Length >= e.Count) //要发送的信息大于等于缓冲大小
                            {
                                e.SetBuffer(bytes, 0, bytes.Length);
                            }
                            else //小于缓冲大小（将数据进行复制，而不是替换，这样可以保证缓存大小一定是大于等于 BufferSize 的）
                            {
                                Array.Copy(bytes, e.Buffer, bytes.Length);
                                e.SetBuffer(0, bytes.Length);
                            }
                            token.Stream.SetLength(0);
                            if (!socket.SendAsync(e))
                            {
                                this.processSend(e);
                            }
                        }
                        else //不需要进行发送操作，但要将缓存数据清零，以备再次接收数据
                        {
                            token.Stream.SetLength(0);
                            if (!socket.ReceiveAsync(e)) //接收下一次数据
                            {
                                // Read the next block of data sent by client.
                                this.processReceive(e);
                            }
                        }
                    }
                    else if (!socket.ReceiveAsync(e))
                    {
                        // Read the next block of data sent by client.
                        this.processReceive(e);
                    }
                }
                else
                {
                    this.processError(e, "接收");
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
                UserToken token = (UserToken)e.UserToken;
                e.SetBuffer(0, BufferSize);
                if (!token.Socket.ReceiveAsync(e))
                {
                    // Read the next block of data send from the client.
                    this.processReceive(e);
                }
            }
            else
            {
                this.processError(e, "发送");
            }
        }

        static readonly object _locker = new object();

        /// <summary>
        /// Stop the server.
        /// </summary>
        public void Stop(bool waitClientClosed = false)
        {
            if (_isStopping || _socket == null)
                return;

            lock (_locker)
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
            this.closeClientSocket((UserToken)e.UserToken, e);
        }

        /// <summary>
        /// 客户端已关闭处理器
        /// </summary>
        public Action<TUserToken> ClientClosedHanlder { get; set; }

        private void closeClientSocket(UserToken token, SocketAsyncEventArgs e)
        {
            Clients.TryRemove(token.Socket, out _); //移除客户端

            token.Dispose();
            // Free the SocketAsyncEventArg so they can be reused by another client.
            this.EPool.Push(e);
            // Decrement the counter keeping track of the total number of clients connected to the server.
            this.Semaphore.Release();
            if (EPool.Count == MaxConnectionCount)
                ManualResetEvent.Set();

            ClientClosedHanlder?.Invoke((TUserToken)token);
        }

        /// <summary>
        /// 一般错误处理
        /// </summary>
        public Action<string, Exception> GeneralErrorHandler { get; set; }

        private void processError(SocketAsyncEventArgs e, string operationName)
        {
            UserToken token = (UserToken)e.UserToken;
            reportError($"与{token.Socket.RemoteEndPoint}进行{operationName}通讯操作出错，连接即将关闭");
            this.closeClientSocket(token, e);
        }

        void reportError(string message, Exception exception)
        {
            GeneralErrorHandler?.Invoke(message, exception);
        }

        void reportError(string message)
        {
            reportError(message, null);
        }
    }
}
