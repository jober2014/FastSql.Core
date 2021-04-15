
using System;
using System.Collections.Generic;
using System.Linq;
using Dapper;
using System.Data;
using System.Data.SqlClient;

namespace FastSql.Core
{
    /// <summary>
    /// dapper扩展操作
    /// </summary>
    public static class DapperExt
    {
        /// <summary>
        /// 查询某个对象对象
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <returns></returns>
        public static T QueryFirst<T>(this CreateSql<T> createSql) where T : class, new()
        {
            var result = default(T);
            using (var con = new SqlConnection(DbConfig.SqlConnectString))
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
            using (var con = new SqlConnection(DbConfig.SqlConnectString))
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

            using (var con = new SqlConnection(DbConfig.SqlConnectString))
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

            using (var con = new SqlConnection(DbConfig.SqlConnectString))
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
            using (var con = new SqlConnection(DbConfig.SqlConnectString))
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

            using (var con = new SqlConnection(DbConfig.SqlConnectString))
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

            using (var con = new SqlConnection(DbConfig.SqlConnectString))
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

            using (var con = new SqlConnection(DbConfig.SqlConnectString))
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

            using (var con = new SqlConnection(DbConfig.SqlConnectString))
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

            using (var con = new SqlConnection(DbConfig.SqlConnectString))
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

            using (var con = new SqlConnection(DbConfig.SqlConnectString))
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
            using (var con = new SqlConnection(DbConfig.SqlConnectString))
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
        public static object ExeScalar<T>(this CreateSql<T> createSql, object models) where T : class, new()
        {
            object result = null;
            using (var con = new SqlConnection(DbConfig.SqlConnectString))
            {
                result = con.ExecuteScalar(createSql.ToSqlString(), models);
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

            var dt = new DataTable();
            using (var con = new SqlConnection(DbConfig.SqlConnectString))
            {
                using (var sqlDataAdapter = new SqlDataAdapter(sql, con))
                {
                    sqlDataAdapter.Fill(dt);
                }

            }
            return dt;

        }
        /// <summary>
        /// 返回datatable
        /// </summary>
        /// <param name="sql">SQL</param>
        /// <returns></returns>
        public static DataTable GetDataTable<T>(this CreateSql<T> createSql) where T : class, new()
        {

            var dt = new DataTable();
            using (var con = new SqlConnection(DbConfig.SqlConnectString))
            {
                using (var sqlDataAdapter = new SqlDataAdapter(createSql.ToSqlString(), con))
                {
                    sqlDataAdapter.Fill(dt);
                }

            }
            return dt;

        }
        /// <summary>
        /// 返回Datatable
        /// </summary>
        /// <param name="sql">sql</param>
        /// <param name="sqlparam">参数</param>
        /// <returns></returns>
        public static DataTable GetDataTable(string sql,Dictionary<string,object> sqlparam )
        {

            var dt = new DataTable();
            using (var con = new SqlConnection(DbConfig.SqlConnectString))
            {             

                using (var sqlDataAdapter = new SqlDataAdapter(sql, con))
                {
                    if (sqlparam != null)
                    {
                        foreach (var item in sqlparam)
                        {
                            var parm = new SqlParameter(item.Key,item.Value);
                            sqlDataAdapter.SelectCommand.Parameters.Add(parm);
                        }
                    }                  
                    sqlDataAdapter.Fill(dt);
                }

            }
            return dt;

        }
        /// <summary>
        /// 返回Datatable
        /// </summary>
        /// <param name="sql">sql</param>
        /// <param name="sqlparam">参数</param>
        /// <returns></returns>
        public static DataTable GetDataTable<T>(this CreateSql<T> createSql, Dictionary<string, object> sqlparam) where T : class, new()
        {

            var dt = new DataTable();
            using (var con = new SqlConnection(DbConfig.SqlConnectString))
            {

                using (var sqlDataAdapter = new SqlDataAdapter(createSql.ToSqlString(), con))
                {
                    if (sqlparam != null)
                    {
                        foreach (var item in sqlparam)
                        {
                            var parm = new SqlParameter(item.Key, item.Value);
                            sqlDataAdapter.SelectCommand.Parameters.Add(parm);
                        }
                    }
                    sqlDataAdapter.Fill(dt);
                }

            }
            return dt;

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
