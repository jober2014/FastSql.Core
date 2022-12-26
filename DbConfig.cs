using System;
using System.Collections.Generic;
using System.Text;

namespace FastSql.Core
{
    /// <summary>
    /// 数据库连接配制
    /// </summary>
    public class DbConfig
    {
        /// <summary>
        /// 数据库连接配制
        /// </summary>
        public static string SqlConnectString { private set; get; }
        /// <summary>
        /// 数据库类型
        /// </summary>
        public static string DatabaseType { private set; get; } = "sqlserver";
        /// <summary>
        /// 数据库连接设置
        /// </summary>
        /// <param name="constr"></param>
        public static void SetSqlConnect(string constr)
        {
            SqlConnectString = constr;
        }
        /// <summary>
        /// 数据库连接设置
        /// </summary>
        /// <param name="constr">连接串</param>
        /// <param name="dbtype">数据库类型：sqlserver,access,mysql,oracle,postgresql,sqlite</param>
        public static void SetSqlConnect(string constr, string dbtype)
        {
            SqlConnectString = constr;
            DatabaseType = dbtype;
        }


    }
}
