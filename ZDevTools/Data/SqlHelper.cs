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

            Transaction.Rollback(pointName);
        }

        /// <summary>
        /// 保存一个事务还原点
        /// </summary>
        public void SaveTransactionPoint(string pointName)
        {
            if (Transaction == null)
                throw new InvalidOperationException("没有开启事务，不能保存还原点！");

            Transaction.Save(pointName);
        }


        /// <summary>
        /// SqlBulkCopy方式复制dataTable到数据库中
        /// </summary>
        /// <param name="dataTable">要复制的数据表，请保证数据表名称与数据库中表名一致</param>
        /// <param name="destinationTableName">目标表名，如果为null或<see cref="string.Empty"/>则使用<see cref="DataTable.TableName"/></param>
        /// <param name="mappingColumnName">是否使用<see cref="DataTable"/>中每列的列名【<see cref="DataTable.TableName"/>与数据库表字段匹配是大小写敏感的】进行映射，默认为true；如果<see cref="DataTable"/>中每列的位置均与目的数据表一致，那么此处可以为false，稍微提高一些性能</param>
        public void BulkCopy(DataTable dataTable, string destinationTableName = null, bool mappingColumnName = true)
        {
            Execute((SqlConnection conn) =>
            {
                SqlBulkCopy sqlBulkCopy;
                if (Transaction != null)
                    sqlBulkCopy = new SqlBulkCopy(conn, SqlBulkCopyOptions.Default, Transaction);
                else
                    sqlBulkCopy = new SqlBulkCopy(conn);

                using (sqlBulkCopy)
                {
                    sqlBulkCopy.DestinationTableName = string.IsNullOrEmpty(destinationTableName) ? dataTable.TableName : destinationTableName;

                    if (mappingColumnName)
                        foreach (DataColumn column in dataTable.Columns)
                            sqlBulkCopy.ColumnMappings.Add(column.ColumnName, column.ColumnName);//此处Add方法第二个参数destinationColumn要求与数据库表字段名称大小写也要一致

                    sqlBulkCopy.WriteToServer(dataTable);
                }
            });
        }

        /// <summary>
        /// 使用SqlBulkCopy在默认参数下复制数据到数据库
        /// </summary>
        /// <param name="job">以SqlBulkCopy对象为参数的委托</param>
        public void BulkCopy(Action<SqlBulkCopy> job)
        {
            Execute((SqlConnection conn) =>
            {
                SqlBulkCopy sqlBulkCopy;
                if (Transaction != null)
                    sqlBulkCopy = new SqlBulkCopy(conn, SqlBulkCopyOptions.Default, Transaction);
                else
                    sqlBulkCopy = new SqlBulkCopy(conn);

                using (sqlBulkCopy)
                    job(sqlBulkCopy);
            });
        }

        /// <summary>
        /// 使用SqlBulkCopy在用户给定参数下复制数据到数据库
        /// </summary>
        /// <param name="job">以SqlBulkCopy对象为参数的委托</param>
        /// <param name="copyOptions">创建SqlBulkCopy所用的复制选项【注意：在<see cref="SqlHelper"/>开启事务时，不能使用<see cref="SqlBulkCopyOptions.UseInternalTransaction"/>选项】</param>
        public void BulkCopy(SqlBulkCopyOptions copyOptions, Action<SqlBulkCopy> job)
        {
            Execute((SqlConnection conn) =>
            {
                using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(conn, copyOptions, Transaction))
                    job(sqlBulkCopy);
            });
        }

    }
}
