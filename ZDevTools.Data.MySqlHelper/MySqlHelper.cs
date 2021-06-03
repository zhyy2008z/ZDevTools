using System;

using MySql.Data.MySqlClient;

namespace ZDevTools.Data
{
    /// <summary>
    /// 针对MySql.Data实现的Sql Helper
    /// </summary>
    public class MySqlHelper : DbHelper<MySqlConnection, MySqlTransaction, MySqlCommand, MySqlDataReader, MySqlParameter, MySqlDataAdapter, MySqlCommandBuilder>
    {
        /// <summary>
        /// 初始化一个新的MySqlHelper
        /// </summary>
        /// <param name="connectionString">Sql Server链接字符串</param>
        public MySqlHelper(string connectionString) : base(connectionString) { }

        /// <summary>
        /// 创建一个字段参数
        /// </summary>
        /// <param name="name">字段名</param>
        /// <param name="mySqlDbType">字段类型</param>
        /// <param name="value">参数值</param>
        /// <returns></returns>
        public MySqlParameter CreateParameter(string name, MySqlDbType mySqlDbType, object value)//v2.1 新增创建参数
        {
            var parameter = new MySqlParameter();
            parameter.ParameterName = name;
            parameter.MySqlDbType = mySqlDbType;
            parameter.Value = value; //由于为null时，mysql是接受这个null值的，因此不再需要将该值替换为DBNull。
            return parameter;
        }

        /// <summary>
        /// 创建一个字段参数
        /// </summary>
        /// <param name="name">字段名</param>
        /// <param name="mySqlDbType">字段类型</param>
        /// <param name="size">字段大小</param>
        /// <param name="value">参数值</param>
        /// <returns></returns>
        public MySqlParameter CreateParameter(string name, MySqlDbType mySqlDbType, int size, object value)//v2.1 新增创建参数
        {
            var parameter = new MySqlParameter();
            parameter.ParameterName = name;
            parameter.MySqlDbType = mySqlDbType;
            parameter.Size = size;
            parameter.Value = value;
            return parameter;
        }

        /// <summary>
        /// 创建为In语句赋值的可枚举参数
        /// </summary>
        /// <param name="name">参数名</param>
        /// <param name="mySqlDbType">参数类型</param>
        /// <param name="values">参数值</param>
        /// <returns></returns>
        public InParameter CreateInParameter(string name, MySqlDbType mySqlDbType, params object[] values)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("参数名不能为空");

            name = name.TrimStart('@');

            MySqlParameter[] parameters = new MySqlParameter[values.Length];
            for (int i = 0; i < values.Length; i++)
            {
                var parameter = new MySqlParameter();
                parameter.ParameterName = $"{InStatementAutoVariablePrefix}{name}_{i}";
                parameter.MySqlDbType = mySqlDbType;
                parameter.Value = values[i];
                parameters[i] = parameter;
            }
            return new InParameter(name, parameters);
        }

        /// <summary>
        /// 创建为In语句赋值的可枚举参数
        /// </summary>
        /// <param name="name">参数名</param>
        /// <param name="mySqlDbType">参数类型</param>
        /// <param name="size">参数大小</param>
        /// <param name="values">参数值</param>
        /// <returns></returns>
        public InParameter CreateInParameter(string name, MySqlDbType mySqlDbType, int size, params object[] values)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("参数名不能为空");

            name = name.TrimStart('@');

            MySqlParameter[] parameters = new MySqlParameter[values.Length];
            for (int i = 0; i < values.Length; i++)
            {
                var parameter = new MySqlParameter();
                parameter.ParameterName = $"{InStatementAutoVariablePrefix}{name}_{i}";
                parameter.MySqlDbType = mySqlDbType;
                parameter.Size = size;
                parameter.Value = values[i];
                parameters[i] = parameter;
            }
            return new InParameter(name, parameters);
        }

    }
}
