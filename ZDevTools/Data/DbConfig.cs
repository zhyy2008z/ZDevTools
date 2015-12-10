using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZDevTools.Data
{
	/// <summary>
	/// 数据库配置静态类
	/// </summary>
	public static class DbConfig
	{
		/// <summary>
		/// 获取数据库读取辅助器
		/// </summary>
		/// <returns></returns>
		public static SqlHelper GetSqlHelper()
		{
			return new SqlHelper(Properties.Settings.Default.Conn);
		}

		/// <summary>
		/// 获取数据库读取辅助器
		/// </summary>
		/// <param name="conn">数据库字符串</param>
		/// <returns></returns>
		public static SqlHelper GetSqlHelper(string conn)
		{
			return new SqlHelper(conn);
		}

		/// <summary>
		/// 设置默认连接字符串
		/// </summary>
		/// <param name="conn">连接字符串</param>
		public static void SetDefaultConnectionString(string conn)
		{
			Properties.Settings.Default.Conn = conn;
		}
	}
}
