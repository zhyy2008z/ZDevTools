using System;
using System.Data;
using System.Data.Common;
using System.Text;
using System.Text.RegularExpressions;

namespace ZDevTools.Data
{
    /// <summary>
    /// <para>数据库通用访问辅助类</para>
    /// <para>开发者：穿越中的逍遥</para>
    /// <para>版本：3.5</para>
    /// <para>日期：2016年1月20日</para>
    /// <para>简介：</para>
    /// <para>虽然是个辅助类，但是支持事务管理（仅单事务管理）。您可以通过继承或填充泛型参数直接操作其他类型的数据库，如Oracle、MySql等。</para>
    /// <para>本辅助类支持占位符，使用方法如下： //v3.1 增加占位符功能的描述</para>
    /// <para>{update}占位符，替换该占位符为 参数名1=@参数名1[...[,参数名n=@参数名n]] 形式</para>
    /// <para>{where}占位符，替换该占位符为 参数名1=@参数名1[...[ and 参数名n=@参数名n]] 形式</para>
    /// <para>{insf}占位符，替换该占位符为 参数名1[...[,参数名n]] 形式</para>        
    /// <para>{insv}占位符，替换该占位符为 @参数名1[...[,@参数名n]] 形式</para>
    /// <para> {q\d+}占位符，当\d+代表的数字(num)大于0时用于开启查询(query)模式。在query模式下，{where}将仅使用前num个参数替换{where}占位符，用除前num个参数外的参数来替换剩余其他占位符。{q\d+}占位符可以放置于Sql语句的任何位置，该占位符最终被删除。</para>        
    /// <para>使用提醒：本辅助类本身没有多线程同步机制，不保证线程安全！如若在多线程环境下使用，请君自己做好线程同步维护工作！</para>
    /// <para>历史记录</para>
    /// <para>
    /// 2014年8月22日 v1.0
    /// 1.在原有SqlHelper类的基础上，创建了改原始版本。
    /// 
    /// 2014年8月24日 v1.1
    /// 更新内容已遗失
    /// 
    /// 2014年8月26日 v1.2
    /// 1.不允许连续两次打开连接，连续打开将导致连接失去控制，从而加速资源流失
    /// 2.修改ConfigAndOpen方法以使sqlhelper总是使用同一连接对象，减少资源消耗
    /// 
    /// 2014年8月28日 v2.0
    /// 1.添加paramNames支持输出参数名称数组，以供进行其他处理
    /// 2.重大升级，支持sql语句中包含{update}、{where}等类似的占位符，自动填充参数
    /// 
    /// 2014年10月23日 v2.1
    /// 1.使用region关键字整理代码大纲
    /// 2.helper增加一个CreateParameter辅助方法用来快速创建字段参数
    /// 3.为buildFromPattern方法添加一个TrimStart处理，以兼容用户输入"@parametername"这样的参数名
    /// 
    /// 2015年8月19日 v2.2
    /// 1.修正bug：现在可以嵌套调用查询方法
    /// 
    /// 2015年9月7日 v2.3
    /// 1.修正Adapter没有被及时释放的问题
    /// 2.修正parameter绑定到cmd后不能用到其他cmd的问题
    /// 3.添加一个重载方法
    /// 
    /// 2015年9月8日 v2.4
    /// 1.解决一个兼容问题：当为CreateParameter函数的value参数赋null值时导致提示"未提供该参数"错误
    /// 
    /// 2015年9月12日 v3.0
    /// 1.重大版本升级，通过引入泛型、抛弃接口、牺牲些许通用性，增强了本通用类的执行性能和类型特化能力，同时提高了本通用类的可维护性，SqlHelper彻底并入本类型。
    /// 
    /// 2015年9月18日 v3.1
    /// 1.修改正则表达式，使匹配速度更优
    /// 2.增加占位符功能的描述
    /// 
    /// 2015年9月26日 v3.2
    /// 1.內部代码结构重构，代码更加简约、紧凑。对功能，性能无任何影响。
    /// 
    /// 2015年9月29日 v3.3
    /// 1.修正可能的重复回滚问题
    /// 2.原Open方法重命名为BeginTransaction方法
    /// 2.新的Open方法，不再开启事务
    /// 
    /// 2015年10月10日 v3.4
    /// 1.为Open方法添加返回值
    /// 2.修改事务提交与回滚逻辑
    /// 
    /// 2016年1月20日 v3.5
    /// 1.连接字符串参数不变时不再重复赋值，优化性能
    /// 
    /// 2016年3月4日 v3.6
    /// 1.新增保护方法Execute(Action<TConnection> job)，用于继承类实现特殊功能
    /// </para>
    /// </summary>
    public class DbHelper<TConnection, TTransaction, TCommand, TDataReader, TParameter, TDataAdapter, TCommandBuilder> : IDisposable
        where TConnection : DbConnection, new()
        where TTransaction : DbTransaction
        where TCommand : DbCommand
        where TDataReader : DbDataReader
        where TParameter : DbParameter, new()
        where TDataAdapter : DbDataAdapter, new()
        where TCommandBuilder : DbCommandBuilder, new()
    {
        #region 字段及属性
        bool manualOpen;

        TConnection conn;

        /// <summary>
        /// 代表一个事务
        /// </summary>
        protected TTransaction Transaction { get; private set; }

        /// <summary>
        /// 事务是否已被提交
        /// </summary>
        protected bool IsCommitted { get; private set; } //v3.4 修改事务提交与回滚逻辑

        string connectionString;

        #endregion

        #region 构造函数
        /// <summary>
        /// 实例化一个DbHelper对象
        /// </summary>
        /// <param name="connectionString">连接字符串</param>
        public DbHelper(string connectionString)
        {
            this.ConnectionString = connectionString;
        }

        #endregion

        #region 基础设施
        /// <summary>
        /// 转换公共参数为sqlserver支持的参数类型
        /// </summary>
        /// <param name="parameters">参数列表</param>
        /// <param name="paramNames">输出的参数名数组</param>
        /// <returns></returns>
        static TParameter[] ConvertParameter(TParameter[] parameters, out string[] paramNames)
        //v2.0 添加paramNames支持输出参数名称数组，以供进行其他处理
        {
            var length = parameters.Length;
            paramNames = new string[length];
            for (int i = 0; i < length; i++)
                paramNames[i] = parameters[i].ParameterName;

            return parameters;
        }

        static TParameter[] ConvertParameter(object[] parameters, out string[] paramNames)
        //v2.0 添加paramNames支持输出参数名称数组，以供进行其他处理
        {
            if (parameters.Length % 2 != 0)
                throw new ArgumentException("参数填写有误，name value数量不匹配！");
            var length = parameters.Length / 2;
            paramNames = new string[length];
            TParameter[] parama = new TParameter[length];
            for (int i = 0; i < length; i++)
            {
                var j = 2 * i;
                string paramName = parameters[j] as string;
                if (paramName == null) throw new ArgumentException("参数填写有误，name应为字符串类型且name不能为null！");
                paramNames[i] = paramName;
                parama[i] = new TParameter() { ParameterName = paramName, Value = parameters[j + 1] };
            }

            return parama;
        }

        /// <summary>
        /// 创建sql代码片段
        /// </summary>
        /// <param name="sql">sql语句，语句中支持{update}|{where}|{insf}|{insv}|{q\d+}这样的占位符</param>
        /// <param name="paramNames">作为填充为占位符的基础数据</param>
        /// <returns></returns>
        static string buildSql(string sql, string[] paramNames)
        //v2.0 重大升级，支持sql语句中包含{update}、{where}等类似的占位符，自动填充参数
        {
            var matches = Regex.Matches(sql, @"\{update\}|\{where\}|\{insf\}|\{insv\}|\{q\d+\}"); //v3.1 修改正则表达式，使匹配速度更优
            int length = matches.Count;
            int queryCount = 0;
            for (int i = length - 1; i > -1; i--)
            {
                var value = matches[i].Value;
                if (value.IndexOf("{q") == 0 && value.Length > 3) //如果是{q\d+} q模式
                {
                    var count = int.Parse(value.Substring(2, value.Length - 3));
                    if (count > 0) queryCount = count;
                    break;
                }
            }
            for (int i = 0; i < length; i++)
            {
                var value = matches[i].Value;
                var count = paramNames.Length;
                switch (value)
                {
                    case "{update}":
                        {
                            const string patternHead = "{0}=@{0}";
                            const string patternBody = ",{0}=@{0}";
                            var from = queryCount;
                            var to = count;
                            sql = buildFromPattern(sql, paramNames, patternHead, patternBody, value, from, to);
                        }
                        break;
                    case "{where}":
                        {
                            const string patternHead = "{0}=@{0}";
                            const string patternBody = " and {0}=@{0}";
                            int from = 0, to;
                            if (queryCount > 0) //是q模式，需要为where做特殊处理
                                to = queryCount > count ? count : queryCount;
                            else
                                to = count;
                            sql = buildFromPattern(sql, paramNames, patternHead, patternBody, value, from, to);
                        }
                        break;
                    case "{insf}":
                        {
                            const string patternHead = "{0}";
                            const string patternBody = ",{0}";
                            var from = queryCount;
                            var to = count;
                            sql = buildFromPattern(sql, paramNames, patternHead, patternBody, value, from, to);
                        }
                        break;
                    case "{insv}":
                        {
                            const string patternHead = "@{0}";
                            const string patternBody = ",@{0}";
                            var from = queryCount;
                            var to = count;
                            sql = buildFromPattern(sql, paramNames, patternHead, patternBody, value, from, to);
                        }
                        break;
                    default://仅剩{q\d+}这一种形式，删除之
                        sql = sql.Replace(value, null);
                        break;
                }
            }
            return sql;
        }

        /// <summary>
        /// 通过指定的模式生成sql片段来替还sql语句中的占位符，这些片段是由一个patternHead与多个patternBody组成的
        /// </summary>
        /// <returns></returns>
        static string buildFromPattern(string sql, string[] paramNames, string patternHead, string patternBody, string value, int from, int to)
        //v2.0 重大升级，支持sql语句中包含{update}、{where}等类似的占位符，自动填充参数
        {
            if (to - from > 0)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(string.Format(patternHead, paramNames[from].TrimStart('@')));//v2.1 添加一个TrimStart处理，以兼容用户输入"@parametername"这样的参数名
                for (int i = from + 1; i < to; i++)
                    sb.Append(string.Format(patternBody, paramNames[i].TrimStart('@')));//v2.1 添加一个TrimStart处理，以兼容用户输入"@parametername"这样的参数名
                sql = sql.Replace(value, sb.ToString());
            }
            else
                sql = sql.Replace(value, null);//没有可用的参数，直接删除占位符
            return sql;
        }

        /// <summary>
        /// 配置并打开一个连接
        /// </summary>
        void configAndOpen() //v1.2 修改该方法以使sqlhelper总是使用同一连接对象，减少资源消耗
        {
            if (conn == null)
            {
                conn = new TConnection();
                conn.ConnectionString = ConnectionString; //v3.5 连接字符串参数不变时不再重复赋值，优化性能
            }
            conn.Open();
        }

        #endregion

        #region 实现接口方法

        #region 操作helper
        /// <summary>
        /// 获取/设置Data辅助器的连接字符串
        /// </summary>
        public string ConnectionString
        {
            get { return connectionString; }
            set
            {
                if (string.IsNullOrEmpty(value))
                    throw new InvalidOperationException("连接字符串不能为空！");

                if (conn != null)
                {
                    if (conn.State != ConnectionState.Closed)
                        throw new InvalidOperationException("连接开启时不允许设置连接字符串！关闭连接后方可设置连接字符串！");
                    else
                        conn.ConnectionString = value; //v3.5 连接字符串参数不变时不再重复赋值，优化性能
                }

                connectionString = value;
            }
        }

        /// <summary>
        /// 手动打开连接并一直保持开启状态
        /// </summary>
        /// <returns>返回对象本身，使用<see cref="IDisposable"/>接口</returns>
        public IDisposable Open() //v3.3 新的Open方法，不再自动开启事务
        {
            if (manualOpen) //v1.2 不允许连续两次打开连接，连续打开将导致连接失去控制，从而加速资源流失
                throw new InvalidOperationException("不能连续手动打开连接两次，请先Close当前连接！");
            configAndOpen();
            manualOpen = true;
            return this; //v3.4 为Open方法添加返回值
        }

        /// <summary>
        /// 开启一个事务，提交事务请显式调用<see cref="Commit()"/>，否则事务无法提交。
        /// </summary>
        /// <returns>返回对象本身，使用<see cref="IDisposable"/>接口</returns>
        public IDisposable BeginTransaction() //v3.3 原Open方法重命名为BeginTransaction方法
        {
            if (manualOpen) //v1.2 不允许连续两次打开连接，连续打开将导致连接失去控制，从而加速资源流失
                throw new InvalidOperationException("不能连续手动打开连接两次，请先Close当前连接！");
            configAndOpen();
            manualOpen = true;
            Transaction = (TTransaction)conn.BeginTransaction();
            return this;
        }

        /// <summary>
        /// 提交事务，之后您还应当调用Close方法，以彻底释放事务并关闭连接
        /// </summary>
        public void Commit()
        {
            if (Transaction == null)
                throw new InvalidOperationException("没有开启事务，无法提交！");

            if (IsCommitted) //v3.3 修正可能的重复提交问题
                throw new InvalidOperationException("事务已提交，请勿重复提交！");

            Transaction.Commit();
            IsCommitted = true;
        }

        /// <summary>
        /// 如果您调用了<see cref="Open()"/>或<see cref="BeginTransaction()"/>方法，则必须调用此方法以释放连接
        /// </summary>
        public void Close()
        {
            //处理事务
            if (Transaction != null)
            {
                if (!IsCommitted)//如果未提交就回滚事务
                    Transaction.Rollback();
                Transaction.Dispose();
                Transaction = null;
                IsCommitted = false;
            }

            //处理基础连接
            if (conn != null)
                conn.Close();
            manualOpen = false;
        }

        /// <summary>
        /// 本方法的內部直接调用Close方法。
        /// </summary>
        public void Dispose()
        {
            Close();
        }

        #endregion

        #region 查询与执行

        #region Scalar
        /// <summary>
        /// 获取标量值
        /// </summary>
        /// <typeparam name="T">类型参数</typeparam>
        /// <param name="sql">sql语句</param>
        /// <param name="commandType">命令类型</param>
        /// <param name="parameters">参数</param>
        public T GetScalar<T>(string sql, CommandType commandType, params TParameter[] parameters)
        {
            T r = default(T);
            Execute((TCommand cmd) =>
            {
                string[] paramNames;
                cmd.Parameters.AddRange(ConvertParameter(parameters, out paramNames));
                try
                {
                    cmd.CommandText = buildSql(sql, paramNames);
                    cmd.CommandType = commandType;
                    r = (T)cmd.ExecuteScalar();
                }
                finally
                {
                    cmd.Parameters.Clear(); //v2.3 修正parameter绑定到cmd后不能用到其他cmd的问题
                }
            });
            return r;
        }

        /// <summary>
        /// 获取标量值
        /// </summary>
        /// <typeparam name="T">类型参数</typeparam>
        /// <param name="sql">sql语句</param>
        /// <param name="parameters">参数</param>
        public T GetScalar<T>(string sql, params TParameter[] parameters) //v2.3 添加一个重载方法
        {
            T r = default(T);
            Execute((TCommand cmd) =>
            {
                string[] paramNames;
                cmd.Parameters.AddRange(ConvertParameter(parameters, out paramNames));
                try
                {
                    cmd.CommandText = buildSql(sql, paramNames);
                    r = (T)cmd.ExecuteScalar();
                }
                finally
                {
                    cmd.Parameters.Clear(); //v2.3 修正parameter绑定到cmd后不能用到其他cmd的问题
                }
            });
            return r;
        }

        /// <summary>
        /// 获取标量值
        /// </summary>
        /// <typeparam name="T">类型参数</typeparam>
        /// <param name="sql">sql语句</param>
        /// <param name="parameters">参数，样例："p1",234,"p2",22,"p3",2342.324</param>
        public T GetScalar<T>(string sql, params object[] parameters)
        {
            T r = default(T);
            Execute((TCommand cmd) =>
            {
                string[] paramNames;
                cmd.Parameters.AddRange(ConvertParameter(parameters, out paramNames));
                cmd.CommandText = buildSql(sql, paramNames);
                r = (T)cmd.ExecuteScalar();
            });
            return r;
        }

        /// <summary>
        /// 获取标量值
        /// </summary>
        /// <typeparam name="T">泛型参数</typeparam>
        /// <param name="sql">查询字符串</param>
        /// <returns></returns>
        public T GetScalar<T>(string sql)
        {
            T r = default(T);
            Execute((TCommand cmd) =>
            {
                cmd.CommandText = sql;
                r = (T)cmd.ExecuteScalar();
            });
            return r;
        }
        #endregion

        #region Execute

        /// <summary>
        /// 执行查询
        /// </summary>
        /// <param name="sql">查询字符串</param>
        /// <param name="commandType">命令类型</param>
        /// <param name="parameters">参数</param>
        /// <returns></returns>
        public int Execute(string sql, CommandType commandType, params TParameter[] parameters)
        {
            int affectedRowCount = 0;
            Execute((TCommand cmd) =>
            {
                string[] paramNames;
                cmd.Parameters.AddRange(ConvertParameter(parameters, out paramNames));
                try
                {
                    cmd.CommandText = buildSql(sql, paramNames);
                    cmd.CommandType = commandType;
                    affectedRowCount = cmd.ExecuteNonQuery();
                }
                finally
                {
                    cmd.Parameters.Clear(); //v2.3 修正parameter绑定到cmd后不能用到其他cmd的问题
                }
            });
            return affectedRowCount;
        }

        /// <summary>
        /// 执行查询
        /// </summary>
        /// <param name="sql">查询字符串</param>
        /// <param name="parameters">参数</param>
        /// <returns></returns>
        public int Execute(string sql, params TParameter[] parameters) //v2.3 添加一个重载方法
        {
            int affectedRowCount = 0;
            Execute((TCommand cmd) =>
            {
                string[] paramNames;
                cmd.Parameters.AddRange(ConvertParameter(parameters, out paramNames));
                try
                {
                    cmd.CommandText = buildSql(sql, paramNames);
                    affectedRowCount = cmd.ExecuteNonQuery();
                }
                finally
                {
                    cmd.Parameters.Clear(); //v2.3 修正parameter绑定到cmd后不能用到其他cmd的问题
                }
            });
            return affectedRowCount;
        }

        /// <summary>
        /// 执行查询
        /// </summary>
        /// <param name="sql">查询字符串</param>
        /// <param name="parameters">参数，样例："p1",234,"p2",22,"p3",2342.324</param>
        /// <returns></returns>
        public int Execute(string sql, params object[] parameters)
        {
            int affectedRowCount = 0;
            Execute((TCommand cmd) =>
            {
                string[] paramNames;
                cmd.Parameters.AddRange(ConvertParameter(parameters, out paramNames));
                cmd.CommandText = buildSql(sql, paramNames);
                affectedRowCount = cmd.ExecuteNonQuery();
            });
            return affectedRowCount;
        }

        /// <summary>
        /// 执行查询
        /// </summary>
        /// <param name="sql">查询字符串</param>
        /// <returns></returns>
        public int Execute(string sql)
        {
            int affectedRowCount = 0;
            Execute((TCommand cmd) =>
            {
                cmd.CommandText = sql;
                affectedRowCount = cmd.ExecuteNonQuery();
            });
            return affectedRowCount;
        }

        /// <summary>
        /// 执行一个数据库操作，允许访问DataReader对象，reader对象自动销毁，不需要手动释放资源
        /// </summary>
        /// <param name="sql">查询字符串</param>
        /// <param name="job">对Reader操作的委托</param>
        /// <param name="commandType">命令类型</param>
        /// <param name="parameters">参数</param>
        public void Execute(string sql, CommandType commandType, Action<TDataReader> job, params TParameter[] parameters)
        {
            Execute((TCommand cmd) =>
            {
                string[] paramNames;
                cmd.Parameters.AddRange(ConvertParameter(parameters, out paramNames));
                try
                {
                    cmd.CommandText = buildSql(sql, paramNames);
                    cmd.CommandType = commandType;
                    using (var reader = (TDataReader)cmd.ExecuteReader())
                        job(reader);
                }
                finally
                {
                    cmd.Parameters.Clear(); //v2.3 修正parameter绑定到cmd后不能用到其他cmd的问题
                }
            });
        }

        /// <summary>
        /// 执行一个数据库操作，允许访问DataReader对象，reader对象自动销毁，不需要手动释放资源
        /// </summary>
        /// <param name="sql">查询字符串</param>
        /// <param name="job">对Reader操作的委托</param>
        /// <param name="parameters">参数</param>
        public void Execute(string sql, Action<TDataReader> job, params TParameter[] parameters) //v2.3 添加一个重载方法
        {
            Execute((TCommand cmd) =>
            {
                string[] paramNames;
                cmd.Parameters.AddRange(ConvertParameter(parameters, out paramNames));
                try
                {
                    cmd.CommandText = buildSql(sql, paramNames);
                    using (var reader = (TDataReader)cmd.ExecuteReader())
                        job(reader);
                }
                finally
                {
                    cmd.Parameters.Clear(); //v2.3 修正parameter绑定到cmd后不能用到其他cmd的问题
                }
            });
        }

        /// <summary>
        /// 执行一个数据库操作，允许访问DataReader对象，reader对象自动销毁，不需要手动释放资源
        /// </summary>
        /// <param name="sql">查询字符串</param>
        /// <param name="job">对Reader操作的委托</param>
        /// <param name="parameters">参数，样例："p1",234,"p2",22,"p3",2342.324</param>
        public void Execute(string sql, Action<TDataReader> job, params object[] parameters)
        {
            Execute((TCommand cmd) =>
            {
                string[] paramNames;
                cmd.Parameters.AddRange(ConvertParameter(parameters, out paramNames));
                cmd.CommandText = buildSql(sql, paramNames);
                using (var reader = (TDataReader)cmd.ExecuteReader())
                    job(reader);
            });
        }

        /// <summary>
        /// 执行一个数据库操作，允许访问DataReader对象，reader对象自动销毁，不需要手动释放资源
        /// </summary>
        /// <param name="sql">查询字符串</param>
        /// <param name="job">对Reader操作的委托</param>
        public void Execute(string sql, Action<TDataReader> job)
        {
            Execute((TCommand cmd) =>
            {
                cmd.CommandText = sql;
                using (var reader = (TDataReader)cmd.ExecuteReader())
                    job(reader);
            });
        }

        /// <summary>
        /// 执行一个数据库操作，允许访问Command对象，cmd对象自动销毁，不需要手动释放资源
        /// </summary>
        /// <param name="job">对Command操作的委托</param>
        public void Execute(Action<TCommand> job)//将需要做之事全部委托于此方法，此方法为job提供调用前及调用后做好维护服务
        {
            if (IsCommitted)
                throw new InvalidOperationException("对不起，事务已提交，您不能再使用本连接!请先关闭连接，然后再次开启事务处理！");
            bool isClosed = conn == null || conn.State == ConnectionState.Closed; //v2.2 修正：不能嵌套调用查询方法的bug
            try
            {
                if (isClosed)//根据当前连接状态自动配置和打开连接
                    configAndOpen();
                using (var cmd = (TCommand)conn.CreateCommand())
                {
                    if (Transaction != null)
                        cmd.Transaction = Transaction;
                    job(cmd);
                }
            }
            finally
            {
                if (isClosed) //保持连接的原始状态
                    conn.Close();
            }
        }

        /// <summary>
        /// 执行一个数据库操作，允许访问Connection对象，Connection对象维持上一次操作状态，不需要手动关闭连接
        /// </summary>
        /// <param name="job">对Connection操作的委托</param>
        /// <remarks>此方法主要用于继承类实现自己的特殊功能所用</remarks>
        protected void Execute(Action<TConnection> job)
        //将需要做之事全部委托于此方法，此方法为job提供调用前及调用后做好维护服务
        //v3.6 新增保护方法Execute(Action<TConnection> job)，用于继承类实现特殊功能
        {
            if (IsCommitted)
                throw new InvalidOperationException("对不起，事务已提交，您不能再使用本连接!请先关闭连接，然后再次开启事务处理！");
            bool isClosed = conn == null || conn.State == ConnectionState.Closed; //v2.2 修正：不能嵌套调用查询方法的bug
            try
            {
                if (isClosed)//根据当前连接状态自动配置和打开连接
                    configAndOpen();

                job(conn);
            }
            finally
            {
                if (isClosed) //保持连接的原始状态
                    conn.Close();
            }
        }

        #endregion

        #region DataSet
        /// <summary>
        /// 获取数据集
        /// </summary>
        /// <param name="sql">查询字符串</param>
        /// <param name="commandType">命令类型</param>
        /// <param name="parameters">参数</param>
        /// <returns></returns>
        public DataSet GetDataSet(string sql, CommandType commandType, params TParameter[] parameters)
        {
            DataSet dataSet = null;
            Execute((TCommand cmd) =>
            {
                string[] paramNames;
                cmd.Parameters.AddRange(ConvertParameter(parameters, out paramNames));
                try
                {
                    cmd.CommandText = buildSql(sql, paramNames);
                    cmd.CommandType = commandType;
                    using (var adapter = new TDataAdapter() { SelectCommand = cmd })//v2.3 修正Adapter没有被及时释放的问题
                    {
                        dataSet = new DataSet();
                        adapter.Fill(dataSet);
                    }
                }
                finally
                {
                    cmd.Parameters.Clear(); //v2.3 修正parameter绑定到cmd后不能用到其他cmd的问题
                }
            });
            return dataSet;
        }

        /// <summary>
        /// 获取数据集
        /// </summary>
        /// <param name="sql">查询字符串</param>
        /// <param name="parameters">参数</param>
        /// <returns></returns>
        public DataSet GetDataSet(string sql, params TParameter[] parameters) //v2.3 添加一个重载方法
        {
            DataSet dataSet = null;
            Execute((TCommand cmd) =>
            {
                string[] paramNames;
                cmd.Parameters.AddRange(ConvertParameter(parameters, out paramNames));
                try
                {
                    cmd.CommandText = buildSql(sql, paramNames);
                    using (var adapter = new TDataAdapter() { SelectCommand = cmd })//v2.3 修正Adapter没有被及时释放的问题
                    {
                        dataSet = new DataSet();
                        adapter.Fill(dataSet);
                    }
                }
                finally
                {
                    cmd.Parameters.Clear(); //v2.3 修正parameter绑定到cmd后不能用到其他cmd的问题
                }
            });
            return dataSet;
        }

        /// <summary>
        /// 获取数据集
        /// </summary>
        /// <param name="sql">查询字符串</param>
        /// <param name="parameters">参数，样例："p1",234,"p2",22,"p3",2342.324</param>
        /// <returns></returns>
        public DataSet GetDataSet(string sql, params object[] parameters)
        {
            DataSet dataSet = null;
            Execute((TCommand cmd) =>
            {
                string[] paramNames;
                cmd.Parameters.AddRange(ConvertParameter(parameters, out paramNames));
                cmd.CommandText = buildSql(sql, paramNames);
                using (var adapter = new TDataAdapter() { SelectCommand = cmd })//v2.3 修正Adapter没有被及时释放的问题
                {
                    dataSet = new DataSet();
                    adapter.Fill(dataSet);
                }
            });
            return dataSet;
        }

        /// <summary>
        /// 获取数据集
        /// </summary>
        /// <param name="sql">查询字符串</param>
        /// <returns></returns>
        public DataSet GetDataSet(string sql)
        {
            DataSet dataSet = null;
            Execute((TCommand cmd) =>
            {
                cmd.CommandText = sql;
                using (var adapter = new TDataAdapter() { SelectCommand = cmd })//v2.3 修正Adapter没有被及时释放的问题
                {
                    dataSet = new DataSet();
                    adapter.Fill(dataSet);
                }
            });
            return dataSet;
        }

        /// <summary>
        /// 更新数据集
        /// </summary>
        /// <param name="sql">查询字符串，用来获取表结构【主要是指出要更新的字段】，不会读取数据的，无需担心性能问题</param>
        /// <param name="dataSet">要更新的数据集</param>
        public void UpdateDataSet(string sql, DataSet dataSet)
        {
            Execute((TCommand cmd) =>
            {
                cmd.CommandText = sql;
                using (TDataAdapter adapter = new TDataAdapter() { SelectCommand = cmd })//v2.3 修正Adapter没有被及时释放的问题
                using (new TCommandBuilder() { DataAdapter = adapter })
                    adapter.Update(dataSet);
            });
        }
        #endregion

        #region DataTable

        /// <summary>
        /// 获取数据表
        /// </summary>
        /// <param name="sql">查询字符串</param>
        /// <param name="commandType">命令类型</param>
        /// <param name="parameters">参数</param>
        /// <returns></returns>
        public DataTable GetDataTable(string sql, CommandType commandType, params TParameter[] parameters)
        {
            DataTable dataTable = null;
            Execute((TCommand cmd) =>
            {
                string[] paramNames;
                cmd.Parameters.AddRange(ConvertParameter(parameters, out paramNames));
                try
                {
                    cmd.CommandText = buildSql(sql, paramNames);
                    cmd.CommandType = commandType;
                    using (var adapter = new TDataAdapter() { SelectCommand = cmd })//v2.3 修正Adapter没有被及时释放的问题
                    {
                        dataTable = new DataTable();
                        adapter.Fill(dataTable);
                    }
                }
                finally
                {
                    cmd.Parameters.Clear(); //v2.3 修正parameter绑定到cmd后不能用到其他cmd的问题
                }
            });
            return dataTable;
        }

        /// <summary>
        /// 获取数据表
        /// </summary>
        /// <param name="sql">查询字符串</param>
        /// <param name="parameters">参数</param>
        /// <returns></returns>
        public DataTable GetDataTable(string sql, params TParameter[] parameters) //v2.3 添加一个重载方法
        {
            DataTable dataTable = null;
            Execute((TCommand cmd) =>
            {
                string[] paramNames;
                cmd.Parameters.AddRange(ConvertParameter(parameters, out paramNames));
                try
                {
                    cmd.CommandText = buildSql(sql, paramNames);
                    using (var adapter = new TDataAdapter() { SelectCommand = cmd })//v2.3 修正Adapter没有被及时释放的问题
                    {
                        dataTable = new DataTable();
                        adapter.Fill(dataTable);
                    }
                }
                finally
                {
                    cmd.Parameters.Clear(); //v2.3 修正parameter绑定到cmd后不能用到其他cmd的问题
                }
            });
            return dataTable;
        }

        /// <summary>
        /// 获取数据表
        /// </summary>
        /// <param name="sql">查询字符串</param>
        /// <param name="parameters">参数，样例："p1",234,"p2",22,"p3",2342.324</param>
        /// <returns></returns>
        public DataTable GetDataTable(string sql, params object[] parameters)
        {
            DataTable dataTable = null;
            Execute((TCommand cmd) =>
            {
                string[] paramNames;
                cmd.Parameters.AddRange(ConvertParameter(parameters, out paramNames));
                cmd.CommandText = buildSql(sql, paramNames);
                using (var adapter = new TDataAdapter() { SelectCommand = cmd })//v2.3 修正Adapter没有被及时释放的问题
                {
                    dataTable = new DataTable();
                    adapter.Fill(dataTable);
                }
            });
            return dataTable;
        }

        /// <summary>
        /// 获取数据表
        /// </summary>
        /// <param name="sql">查询字符串</param>
        /// <returns></returns>
        public DataTable GetDataTable(string sql)
        {
            DataTable dataTable = null;
            Execute((TCommand cmd) =>
            {
                cmd.CommandText = sql;
                using (var adapter = new TDataAdapter() { SelectCommand = cmd })//v2.3 修正Adapter没有被及时释放的问题
                {
                    dataTable = new DataTable();
                    adapter.Fill(dataTable);
                }
            });
            return dataTable;
        }

        /// <summary>
        /// 更新数据表
        /// </summary>
        /// <param name="sql">查询字符串，用来获取表结构【主要是指出要更新的字段】，不会读取数据的，无需担心性能问题</param>
        /// <param name="dataTable">要更新的数据表</param>
        public void UpdateDataTable(string sql, DataTable dataTable)
        {
            Execute((TCommand cmd) =>
            {
                cmd.CommandText = sql;
                using (TDataAdapter adapter = new TDataAdapter() { SelectCommand = cmd })//v2.3 修正Adapter没有被及时释放的问题
                using (new TCommandBuilder() { DataAdapter = adapter })
                    adapter.Update(dataTable);
            });
        }
        #endregion

        #endregion

        #region 辅助方法

        /// <summary>
        /// 创建一个字段参数
        /// </summary>
        /// <param name="name">字段名</param>
        /// <returns></returns>
        public TParameter CreateParameter(string name)//v2.1 新增创建参数
        {
            var parameter = new TParameter();
            parameter.ParameterName = name;
            return parameter;
        }

        /// <summary>
        /// 创建一个字段参数
        /// </summary>
        /// <param name="name">字段名</param>
        /// <param name="value">参数值</param>
        /// <returns></returns>
        public TParameter CreateParameter(string name, object value)//v2.1 新增创建参数
        {
            var parameter = new TParameter();
            parameter.ParameterName = name;
            parameter.Value = value ?? DBNull.Value; //v2.4 当为CreateParameter函数的value参数赋null值时导致提示"未提供该参数"错误
            return parameter;
        }

        /// <summary>
        /// 创建一个字段参数
        /// </summary>
        /// <param name="name">字段名</param>
        /// <param name="dbType">字段类型</param>
        /// <param name="value">参数值</param>
        /// <returns></returns>
        public TParameter CreateParameter(string name, DbType dbType, object value)//v2.1 新增创建参数
        {
            var parameter = new TParameter();
            parameter.ParameterName = name;
            parameter.DbType = dbType;
            parameter.Value = value ?? DBNull.Value; //v2.4 当为CreateParameter函数的value参数赋null值时导致提示"未提供该参数"错误
            return parameter;
        }

        /// <summary>
        /// 创建一个字段参数
        /// </summary>
        /// <param name="name">字段名</param>
        /// <param name="dbType">字段类型</param>
        /// <param name="size">字段大小</param>
        /// <param name="value">参数值</param>
        /// <returns></returns>
        public TParameter CreateParameter(string name, DbType dbType, int size, object value)//v2.1 新增创建参数
        {
            var parameter = new TParameter();
            parameter.ParameterName = name;
            parameter.DbType = dbType;
            parameter.Size = size;
            parameter.Value = value ?? DBNull.Value;//v2.4 当为CreateParameter函数的value参数赋null值时导致提示"未提供该参数"错误
            return parameter;
        }
        #endregion

        #endregion
    }
}
