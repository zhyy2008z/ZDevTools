using System;
using System.Data.SqlClient;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;

namespace ZDevTools.Data
{
    /// <summary>
    /// 针对SqlServer数据库使用的优化版本的DBHelper
    /// </summary>
    public class SqlHelper : DbHelper<SqlConnection, SqlTransaction, SqlCommand, SqlDataReader, SqlParameter, SqlDataAdapter, SqlCommandBuilder>
    {
        /// <summary>
        /// 初始化一个SqlHelper对象
        /// </summary>
        /// <param name="connectionString">Sql Server链接字符串</param>
        public SqlHelper(string connectionString) : base(connectionString) { }

        /// <summary>
        /// 创建一个字段参数
        /// </summary>
        /// <param name="name">字段名</param>
        /// <param name="sqlDbType">字段类型</param>
        /// <param name="value">参数值</param>
        /// <returns></returns>
        public SqlParameter CreateParameter(string name, SqlDbType sqlDbType, object value)//v2.1 新增创建参数
        {
            var parameter = new SqlParameter();
            parameter.ParameterName = name;
            parameter.SqlDbType = sqlDbType;
            parameter.Value = value ?? DBNull.Value; //v2.4 当为CreateParameter函数的value参数赋null值时导致提示"未提供该参数"错误
            return parameter;
        }

        /// <summary>
        /// 创建一个字段参数
        /// </summary>
        /// <param name="name">字段名</param>
        /// <param name="sqlDbType">字段类型</param>
        /// <param name="size">字段大小</param>
        /// <param name="value">参数值</param>
        /// <returns></returns>
        public SqlParameter CreateParameter(string name, SqlDbType sqlDbType, int size, object value)//v2.1 新增创建参数
        {
            var parameter = new SqlParameter();
            parameter.ParameterName = name;
            parameter.SqlDbType = sqlDbType;
            parameter.Size = size;
            parameter.Value = value ?? DBNull.Value;//v2.4 当为CreateParameter函数的value参数赋null值时导致提示"未提供该参数"错误
            return parameter;
        }

        /// <summary>
        /// 还原到保存的事务点
        /// </summary>
        public void RollBackTransactionPoint(string pointName)
        {
            if (Transaction == null)
                throw new InvalidOperationException("没有开启事务，不能回滚还原点！");

            if (IsCommitted) //v3.3 修正可能的重复提交问题
                throw new InvalidOperationException("事务已提交，不能回滚还原点！");

            Transaction.Rollback(pointName);
        }

        /// <summary>
        /// 保存一个事务还原点
        /// </summary>
        public void SaveTransactionPoint(string pointName)
        {
            if (Transaction == null)
                throw new InvalidOperationException("没有开启事务，不能保存还原点！");

            if (IsCommitted) //v3.3 修正可能的重复提交问题
                throw new InvalidOperationException("事务已提交，不能保存还原点！");

            Transaction.Save(pointName);
        }

    }
}
