using System;
using System.Collections.Generic;
using System.Text;

namespace FastSql.Core
{
  public  class DataBaseType
    {
        /// <summary>
        /// sql server
        /// </summary>
        public const string SqlServer = "sqlserver";

        /// <summary>
        ///  OleDb
        /// </summary>
        public const string Access = "access";
        /// <summary>
        ///MySql
        /// </summary>
        public const string MySql = "mysql";
        /// <summary>
        /// Oracle
        /// </summary>
        public const string Oracle = "oracle";

        /// <summary>
        /// Postgresql
        /// </summary>
        public const string PostGreSql = "postgresql";

        /// <summary>
        /// Sqlite
        /// </summary>
        public const string Sqlite = "sqlite";
        /// <summary>
        /// 选择的数据库
        /// </summary>
        internal static string SelectSqlType { get; set; } = "sqlserver";
    }
}
