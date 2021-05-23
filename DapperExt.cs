
using System;
using System.Collections.Generic;
using System.Linq;
using Dapper;
using System.Data;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;
using System.Data.OracleClient;
using Npgsql;
using System.Data.OleDb;
using System.Data.SQLite;
using Microsoft.Data.Sqlite;

namespace FastSql.Core
{
    /// <summary>
    /// dapper扩展操作
    /// </summary>
    public static class DapperExt
    {
        public static string DBType = DataBaseType.SqlServer;


        private static IDbConnection GetConnection()
        {

            switch (DataBaseType.SelectSqlType)
            {
                case DataBaseType.SqlServer:
                    return new SqlConnection(DbConfig.SqlConnectString);
                case DataBaseType.MySql:
                    return new MySqlConnection(DbConfig.SqlConnectString);
                case DataBaseType.Oracle:
                    return new OracleConnection(DbConfig.SqlConnectString);
                case DataBaseType.PostGreSql:
                    return new NpgsqlConnection(DbConfig.SqlConnectString);
                case DataBaseType.Sqlite:
                    return new SqliteConnection(DbConfig.SqlConnectString);
                case DataBaseType.Access:
                    return new OleDbConnection(DbConfig.SqlConnectString);
                default:
                    return new SqlConnection(DbConfig.SqlConnectString);
            }

        }

        private static IDataAdapter getDataAdapter(string sql)
        {
            switch (DataBaseType.SelectSqlType)
            {
                case DataBaseType.SqlServer:
                    return new SqlDataAdapter(sql, DbConfig.SqlConnectString);
                case DataBaseType.MySql:
                    return new MySqlDataAdapter(sql, DbConfig.SqlConnectString);
                case DataBaseType.Oracle:
                    return new OracleDataAdapter(sql, DbConfig.SqlConnectString);
                case DataBaseType.PostGreSql:
                    return new NpgsqlDataAdapter(sql, DbConfig.SqlConnectString);
                case DataBaseType.Sqlite:
                    return new SQLiteDataAdapter(sql, DbConfig.SqlConnectString);
                case DataBaseType.Access:
                    return new OleDbDataAdapter(sql, DbConfig.SqlConnectString);
                default:
                    return new SqlDataAdapter(sql, DbConfig.SqlConnectString);
            }

        }
        /// <summary>
        /// 查询某个对象对象
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <returns></returns>
        public static T QueryFirst<T>(this CreateSql<T> createSql) where T : class, new()
        {
            var result = default(T);
            using (var con = GetConnection())
            {
                result = con.Query<T>(createSql.ToSqlString()).FirstOrDefault();
            }
            return result;

        }
        /// <summary>
        /// 查询某个对象对象
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <returns></returns>
        public static T QueryFirst<T>(this CreateSql<T> createSql, object param) where T : class, new()
        {
            var result = default(T);
            using (var con = GetConnection())
            {
                result = con.Query<T>(createSql.ToSqlString(), param).FirstOrDefault();
            }
            return result;

        }

        /// <summary>
        /// 查询对象列表
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <returns></returns>
        public static List<T> QueryList<T>(this CreateSql<T> createSql) where T : class, new()
        {
            var result = new List<T>();

            using (var con = GetConnection())
            {
                result = con.Query<T>(createSql.ToSqlString()).AsList();
            }
            return result;

        }
        /// <summary>
        ///  查询对象列表
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="createSql">SQL构造器</param>
        /// <param name="param">条件参数</param>
        /// <returns></returns>
        public static List<T> QueryList<T>(this CreateSql<T> createSql, object param) where T : class, new()
        {
            var result = new List<T>();

            using (var con = GetConnection())
            {
                result = con.Query<T>(createSql.ToSqlString(), param).AsList();
            }
            return result;

        }
        /// <summary>
        /// 添加插入操作
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="model">对象</param>
        /// <returns></returns>
        public static bool Add<T>(this CreateSql<T> createSql, T model) where T : class, new()
        {
            var result = false;
            using (var con = GetConnection())
            {
                result = con.Execute(createSql.ToSqlString(), model) > 0;
            }
            return result;

        }
        /// <summary>
        /// 添加插入操作
        /// </summary>
        /// <typeparam name="T">实体对象</typeparam>
        /// <param name="dapperTransaction">事任封装</param>
        /// <param name="model"></param>
        /// <returns></returns>
        public static bool Add<T>(this CreateSql<T> createSql, DapperTransaction dapperTransaction, T model) where T : class, new()
        {
            var result = false;
            if (dapperTransaction != null)
            {
                result = dapperTransaction.dbConnection.Execute(createSql.ToSqlString(), model, dapperTransaction.dbTransaction) > 0;
            }
            return result;

        }
        /// <summary>
        /// 批量添加插入操作
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="model">对象实体</param>
        /// <returns></returns>
        public static bool Add<T>(this CreateSql<T> createSql, List<T> model) where T : class, new()
        {
            var result = false;
            using (var dt = new DapperTransaction())
            {
                try
                {
                    if (model != null && model.Count > 0)
                    {
                        foreach (var item in model)
                        {
                            dt.dbConnection.Execute(createSql.ToSqlString(), item, dt.dbTransaction);
                        }

                    }
                    else
                    {
                        return false;
                    }
                    dt.Commit();
                    result = true;
                }
                catch (Exception)
                {
                    dt.Rollback();
                    throw;
                }
            }
            return result;

        }

        /// <summary>
        /// 修改更新操作
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="createSql"></param>
        /// <param name="models"></param>
        /// <returns></returns>
        public static bool Modify<T>(this CreateSql<T> createSql, T models) where T : class, new()
        {
            var result = false;

            using (var con = GetConnection())
            {
                result = con.Execute(createSql.ToSqlString(), models) > 0;
            }
            return result;

        }
        /// <summary>
        /// 修改更新操作
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="createSql"></param>
        /// <param name="models"></param>
        /// <returns></returns>
        public static bool Modify<T>(this CreateSql<T> createSql, object models) where T : class, new()
        {
            var result = false;

            using (var con = GetConnection())
            {
                result = con.Execute(createSql.ToSqlString(), models) > 0;
            }
            return result;

        }

        /// <summary>
        /// 修改更新事务操作
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="dtran">事务</param>
        /// <param name="createSql">SQL构造器</param>
        /// <param name="models">对象</param>
        /// <returns></returns>
        public static bool Modify<T>(this CreateSql<T> createSql, DapperTransaction dtran, T models) where T : class, new()
        {
            var result = false;

            if (dtran != null)
            {
                result = dtran.dbConnection.Execute(createSql.ToSqlString(), models, dtran.dbTransaction) > 0;
            }

            return result;

        }

        /// <summary>
        /// 修改更新事务操作
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="dtran">事务</param>
        /// <param name="createSql">SQL构造器</param>
        /// <param name="models">对象</param>
        /// <returns></returns>
        public static bool Modify<T>(this CreateSql<T> createSql, DapperTransaction dtran, object models) where T : class, new()
        {
            var result = false;

            if (dtran != null)
            {
                result = dtran.dbConnection.Execute(createSql.ToSqlString(), models, dtran.dbTransaction) > 0;
            }

            return result;

        }
        /// <summary>
        /// 修改更新操作
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="createSql">SQL</param>
        /// <param name="models">对象实体</param>
        /// <returns></returns>
        public static bool Modify<T>(this string createSql, T models) where T : class, new()
        {
            var result = false;

            using (var con = GetConnection())
            {
                result = con.Execute(createSql, models) > 0;
            }
            return result;

        }

        /// <summary>
        /// 修改更新操作
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="createSql">SQL</param>
        /// <param name="models">对象实体</param>
        /// <returns></returns>
        public static bool Modify<T>(this string createSql, object models) where T : class, new()
        {
            var result = false;

            using (var con = GetConnection())
            {
                result = con.Execute(createSql, models) > 0;
            }
            return result;

        }

        /// <summary>
        /// 修改更新操作
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="createSql">SQL</param>
        /// <param name="models">对象实体</param>
        /// <returns></returns>
        public static bool Modify<T>(this string createSql, DapperTransaction dtran, T models) where T : class, new()
        {
            var result = false;

            if (dtran != null)
            {
                result = dtran.dbConnection.Execute(createSql, models, dtran.dbTransaction) > 0;
            }
            return result;

        }

        /// <summary>
        /// 修改更新操作
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="createSql">SQL</param>
        /// <param name="models">对象实体</param>
        /// <returns></returns>
        public static bool Modify<T>(this string createSql, DapperTransaction dtran, object models) where T : class, new()
        {
            var result = false;

            if (dtran != null)
            {
                result = dtran.dbConnection.Execute(createSql, models, dtran.dbTransaction) > 0;
            }
            return result;

        }
        /// <summary>
        /// 删除操作
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="createSql"></param>
        /// <returns></returns>
        public static bool Remove<T>(this CreateSql<T> createSql) where T : class, new()
        {

            var result = false;

            using (var con = GetConnection())
            {
                result = con.Execute(createSql.ToSqlString()) > 0;
            }
            return result;
        }

        /// <summary>
        ///  删除操作
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="createSql"></param>
        /// <param name="models"></param>
        /// <returns></returns>
        public static bool Remove<T>(this CreateSql<T> createSql, object models) where T : class, new()
        {

            var result = false;

            using (var con = GetConnection())
            {
                result = con.Execute(createSql.ToSqlString(), models) > 0;
            }
            return result;
        }
        /// <summary>
        /// 删除操作
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="createSql"></param>
        /// <returns></returns>
        public static bool Remove<T>(this CreateSql<T> createSql, DapperTransaction dtran) where T : class, new()
        {

            var result = false;

            if (dtran != null)
            {
                result = dtran.dbConnection.Execute(createSql.ToSqlString(), null, dtran.dbTransaction) > 0;
            }
            return result;
        }

        /// <summary>
        /// 代事务删除操作
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="dtran">事务</param>
        /// <param name="createSql">SQL构造器</param>
        /// <param name="models">参数</param>
        /// <returns></returns>
        public static bool Remove<T>(this CreateSql<T> createSql, DapperTransaction dtran, object models) where T : class, new()
        {

            var result = false;

            if (dtran != null)
            {
                result = dtran.dbConnection.Execute(createSql.ToSqlString(), models, dtran.dbTransaction) > 0;
            }
            return result;
        }
        /// <summary>
        /// 执行SQL并返回指定数据
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="createSql">SQL构造器</param>
        /// <returns></returns>
        public static object ExeScalar<T>(this CreateSql<T> createSql, T models) where T : class, new()
        {
            object result = null;
            using (var con = GetConnection())
            {
                result = con.ExecuteScalar(createSql.ToSqlString(), models);
            }
            return result;

        }
        /// <summary>
        /// 执行SQL并返回指定数据事务处理
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="createSql"></param>
        /// <param name="dtran"></param>
        /// <returns></returns>
        public static object ExeScalar<T>(this CreateSql<T> createSql, T models, DapperTransaction dtran) where T : class, new()
        {
            object result = null;
            if (dtran != null)
            {
                result = dtran.dbConnection.ExecuteScalar(createSql.ToSqlString(), models, dtran.dbTransaction);
            }
            return result;

        }
        /// <summary>
        ///  执行SQL并返回指定数据
        /// </summary>
        /// <typeparam name="T">类类型</typeparam>
        /// <param name="createSql">SQL构造器</param>
        /// <param name="models">参数</param>
        /// <returns></returns>
        public static object ExeScalar<T>(this CreateSql<T> createSql, object models = null) where T : class, new()
        {
            object result = null;
            using (var con = GetConnection())
            {
                if (models != null)
                {
                    result = con.ExecuteScalar(createSql.ToSqlString(), models);
                }
                else
                {
                    result = con.ExecuteScalar(createSql.ToSqlString());
                }

            }
            return result;

        }
        /// <summary>
        /// 执行SQL并返回指定数据事务处理
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="createSql">SQL构造器</param>
        /// <param name="dtran">事务</param>
        /// <param name="models">参数</param>
        /// <returns></returns>
        public static object ExeScalar<T>(this CreateSql<T> createSql, DapperTransaction dtran, object models) where T : class, new()
        {
            object result = null;
            if (dtran != null)
            {
                result = dtran.dbConnection.ExecuteScalar(createSql.ToSqlString(), models, dtran.dbTransaction);
            }
            return result;

        }
        /// <summary>
        /// 返回datatable
        /// </summary>
        /// <param name="sql">SQL</param>
        /// <returns></returns>
        public static DataTable GetDataTable(this string sql)
        {

            var dt = new DataSet();
            try
            {
                var adp = getDataAdapter(sql);
                adp.Fill(dt);
            }
            catch (Exception)
            {

                throw;
            }
            return dt.Tables[0];

        }
        /// <summary>
        /// 返回datatable
        /// </summary>
        /// <param name="sql">SQL</param>
        /// <returns></returns>
        public static DataTable GetDataTable<T>(this CreateSql<T> createSql) where T : class, new()
        {

            var dt = new DataSet();
            try
            {
                var adp = getDataAdapter(createSql.ToSqlString());
                adp.Fill(dt);
            }
            catch (Exception)
            {
                throw;
            }
            return dt.Tables[0];

        }
        /// <summary>
        /// 返回Datatable
        /// </summary>
        /// <param name="sql">sql</param>
        /// <param name="sqlparam">参数</param>
        /// <returns></returns>
        public static DataTable GetDataTable(string sql, Dictionary<string, object> sqlparam)
        {

            var dt = new DataSet();
            var adp = getDataAdapter(sql);
            try
            {
                if (sqlparam != null)
                {
                    foreach (var item in sqlparam)
                    {
                        switch (DataBaseType.SelectSqlType)
                        {
                            case DataBaseType.SqlServer:
                                IDataParameter parm = new SqlParameter(item.Key, item.Value);
                                (adp as SqlDataAdapter).SelectCommand.Parameters.Add(parm); break;
                            case DataBaseType.MySql:
                                IDataParameter myparm = new MySqlParameter(item.Key, item.Value);
                                (adp as MySqlDataAdapter).SelectCommand.Parameters.Add(myparm); break;

                            case DataBaseType.Oracle:
                                IDataParameter orcalparm = new OracleParameter(item.Key, item.Value);
                                (adp as OracleDataAdapter).SelectCommand.Parameters.Add(orcalparm); break;

                            case DataBaseType.PostGreSql:
                                IDataParameter npsqlparm = new NpgsqlParameter(item.Key, item.Value);
                                (adp as OracleDataAdapter).SelectCommand.Parameters.Add(npsqlparm); break;

                            case DataBaseType.Sqlite:

                                IDataParameter sqllitparm = new SqliteParameter(item.Key, item.Value);
                                (adp as SQLiteDataAdapter).SelectCommand.Parameters.Add(sqllitparm); break;

                            case DataBaseType.Access:
                                IDataParameter oledparm = new OleDbParameter(item.Key, item.Value);
                                (adp as OleDbDataAdapter).SelectCommand.Parameters.Add(oledparm); break;

                            default:
                                IDataParameter sqlparm = new SqlParameter(item.Key, item.Value);
                                (adp as SqlDataAdapter).SelectCommand.Parameters.Add(sqlparm); break;

                        }
                    }
                }
                adp.Fill(dt);
            }
            catch (Exception)
            {
                throw;
            }
          
            return dt.Tables[0];

        }
        /// <summary>
        /// 返回Datatable
        /// </summary>
        /// <param name="sql">sql</param>
        /// <param name="sqlparam">参数</param>
        /// <returns></returns>
        public static DataTable GetDataTable<T>(this CreateSql<T> createSql, Dictionary<string, object> sqlparam) where T : class, new()
        {
            return GetDataTable(createSql.ToSqlString(),sqlparam);
        }

        /// <summary>
        /// 分页扩展
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="data">数据</param>
        /// <param name="pageIndex">当前页</param>
        /// <param name="pageSize">单页大小</param>
        /// <returns></returns>
        public static IList<T> PageList<T>(this IList<T> data, int pageIndex = 1, int pageSize = 10)
        {
            IList<T> result = null;
            if (data != null && data.Count > 0)
            {
                result = data.Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList();
            }
            return result;
        }
        /// <summary>
        /// 分页扩展
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="data">数据</param>
        /// <param name="pageIndex">当前页</param>
        /// <param name="pageSize">单页大小</param>
        /// <returns></returns>
        public static IEnumerable<T> PageList<T>(this IEnumerable<T> data, int pageIndex = 1, int pageSize = 10)
        {
            IEnumerable<T> result = null;
            if (data != null && data.Any())
            {
                result = data.Skip(pageSize * (pageIndex - 1)).Take(pageSize);
            }
            return result;
        }
    }
}
