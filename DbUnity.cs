using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using Dapper;
namespace FastSql.Core
{
    /// <summary>
    /// 基础数据层统一操作
    /// </summary>
    public static class DbUnity
    {
        /// <summary>
        /// 数据查询
        /// </summary>
        /// <param name="dbConnection"></param>
        /// <param name="sql"></param>
        /// <param name="pram"></param>
        /// <returns></returns>
        public static dynamic SqlQuery(this IDbConnection dbConnection, string sql, object pram)
        {
            dynamic result = new ExpandoObject();
            if (dbConnection != null)
            {
                if (dbConnection.State != ConnectionState.Open)
                {
                    dbConnection.Open();
                }

                result = dbConnection.Query(sql, pram);
            }
            return result;

        }
        /// <summary>
        /// 数据查询
        /// </summary>
        /// <param name="dbConnection"></param>
        /// <param name="sql"></param>
        /// <param name="pram"></param>
        /// <returns></returns>
        public static T SqlQuery<T>(this IDbConnection dbConnection, string sql, object pram)
        {
            var result = default(T);
            if (dbConnection != null)
            {
                if (dbConnection.State != ConnectionState.Open)
                {
                    dbConnection.Open();
                }

                result = dbConnection.Query<T>(sql, pram).FirstOrDefault();
            }
            return result;

        }

        /// <summary>
        /// 数据列表查询
        /// </summary>
        /// <typeparam name="T">对应类</typeparam>
        /// <param name="dbConnection">数据库连接对象</param>
        /// <param name="sql">SQL</param>
        /// <param name="pram">参数对象</param>
        /// <returns></returns>
        public static List<T> SqlQueryList<T>(this IDbConnection dbConnection, string sql, object pram)
        {
            var result = new List<T>();
            if (dbConnection != null)
            {
                if (dbConnection.State != ConnectionState.Open)
                {
                    dbConnection.Open();
                }
                result = dbConnection.Query<T>(sql, pram).ToList();
            }
            return result;

        }

        /// <summary>
        /// 查询返回dataTable
        /// </summary>
        /// <param name="dbConnection">连接对象</param>
        /// <param name="sql">SQL</param>
        /// <param name="pram">参数</param>
        /// <param name="tableName">表名</param>
        /// <param name="ctype">操作方式</param>
        /// <param name="tran">事务</param>
        /// <param name="timeout">超时时间</param>
        /// <returns></returns>
        public static DataTable SqlQueryTable(this IDbConnection dbConnection, string sql, object pram = null, string tableName = "table", CommandType ctype = CommandType.Text, IDbTransaction tran = null, int? timeout = null)
        {

            var result = new DataTable(tableName);
            if (dbConnection != null)
            {
                if (dbConnection.State != ConnectionState.Open)
                {
                    dbConnection.Open();
                }
                var reader = dbConnection.ExecuteReader(sql, pram, tran, timeout, ctype);
                result.Load(reader);
            }

            return result;
        }

        /// <summary>
        /// 执行SQL够，返回影响行数
        /// </summary>
        /// <param name="dbConnection">数据库连接对象</param>
        /// <param name="sql">SQL</param>
        /// <param name="pram">参数</param>
        /// <returns></returns>
        public static int SqlExecute(this IDbConnection dbConnection, string sql, object pram)
        {

            var result = 0;
            if (dbConnection != null)
            {
                if (dbConnection.State != ConnectionState.Open)
                {
                    dbConnection.Open();
                }
                result = dbConnection.Execute(sql, pram);
            }
            return result;
        }
        /// <summary>
        /// 执行SQL够，返回影响行数
        /// </summary>
        /// <param name="dbConnection">数据库连接对象</param>
        /// <param name="sql">SQL</param>
        /// <param name="tran">事务对象</param>
        /// <param name="pram">参数</param>
        /// <returns></returns>
        public static int SqlExecute(this IDbConnection dbConnection, string sql, IDbTransaction tran, object pram)
        {
            var result = 0;
            if (dbConnection != null)
            {
                if (dbConnection.State != ConnectionState.Open)
                {
                    dbConnection.Open();
                }
                result = dbConnection.Execute(sql,pram, tran);
            }
            return result;

        }
        /// <summary>
        /// 执行SQL够，返回影响行数
        /// </summary>
        /// <param name="dbConnection">数据库连接对象</param>
        /// <param name="sql">SQL</param>
        /// <param name="tran">事务对象</param>
        /// <param name="ctype">SQL类型</param>
        /// <param name="pram">参数</param>
        /// <returns></returns>
        public static int SqlExecute(this IDbConnection dbConnection, string sql, IDbTransaction tran, CommandType ctype, object pram)
        {
            var result = 0;
            if (dbConnection != null)
            {
                if (dbConnection.State != ConnectionState.Open)
                {
                    dbConnection.Open();
                }
                result = dbConnection.Execute(sql, pram,tran,null,ctype);
            }
            return result;

        }
        /// <summary>
        /// 获取执行SQL返回数单一数据
        /// </summary>      
        /// <param name="dbConnection">连接对象</param>
        /// <param name="sql">SQL</param>
        /// <param name="pram">参数</param>
        /// <returns></returns>
        public static object SqlExecuteScalar(this IDbConnection dbConnection, string sql, object pram)
        {
            var result = new object();
            if (dbConnection != null)
            {
                if (dbConnection.State != ConnectionState.Open)
                {
                    dbConnection.Open();
                }
                result = dbConnection.ExecuteScalar(sql, pram);
            }
            return result;

        }
        /// <summary>
        /// 获取执行SQL返回数单一数据
        /// </summary>      
        /// <param name="dbConnection">连接对象</param>
        /// <param name="sql">SQL</param>
        /// <param name="pram">参数</param>
        /// <param name="tran">事务</param>
        /// <returns></returns>
        public static object SqlExecuteScalar(this IDbConnection dbConnection, IDbTransaction tran, string sql, object pram)
        {
            var result = new object();
            if (dbConnection != null)
            {
                if (dbConnection.State != ConnectionState.Open)
                {
                    dbConnection.Open();
                }
                result = dbConnection.ExecuteScalar(sql, pram, tran);
            }
            return result;

        }
        /// <summary>
        /// 获取执行SQL返回数单一数据
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="dbConnection">连接对象</param>
        /// <param name="sql">SQL</param>
        /// <param name="pram">参数</param>
        /// <returns></returns>
        public static T SqlExecuteScalar<T>(this IDbConnection dbConnection, string sql, object pram)
        {
            var result = default(T);
            if (dbConnection != null)
            {
                if (dbConnection.State != ConnectionState.Open)
                {
                    dbConnection.Open();
                }
                result = dbConnection.ExecuteScalar<T>(sql, pram);
            }
            return result;

        }

        /// <summary>
        /// 获取执行SQL返回数单一数据
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="dbConnection">连接对象</param>
        /// <param name="sql">SQL</param>
        /// <param name="tran">事务</param>
        /// <param name="pram">参数</param>

        /// <returns></returns>
        public static T SqlExecuteScalar<T>(this IDbConnection dbConnection, string sql, IDbTransaction tran, object pram)
        {
            var result = default(T);
            if (dbConnection != null)
            {
                if (dbConnection.State != ConnectionState.Open)
                {
                    dbConnection.Open();
                }
                result = dbConnection.ExecuteScalar<T>(sql, pram, tran);
            }
            return result;

        }




    }
}
