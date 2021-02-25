
using System;
using System.Data;
using System.Data.SqlClient;

namespace FastSql.Core
{
    /// <summary>
    /// 事务封装
    /// </summary>
    public class DapperTransaction : IDisposable
    {
        /// <summary>
        /// 连接对象
        /// </summary>
        public IDbConnection dbConnection;
        /// <summary>
        /// 事务对象
        /// </summary>
        public IDbTransaction dbTransaction;

        public DapperTransaction() {

            dbConnection = new SqlConnection(DbConfig.SqlConnectString);
            if (dbConnection.State != ConnectionState.Open)
            {
                dbConnection.Open();
                dbTransaction = dbConnection.BeginTransaction();
            }
            else
            {
                dbTransaction = dbConnection.BeginTransaction();
            }
        }
        public DapperTransaction(string con)
        {
            if (!string.IsNullOrEmpty(con))
            {
                dbConnection = new SqlConnection(con);
                if (dbConnection.State != ConnectionState.Open)
                {
                    dbConnection.Open();
                    dbTransaction = dbConnection.BeginTransaction();
                }
                else
                {
                    dbTransaction = dbConnection.BeginTransaction();
                }
            }

        }
        /// <summary>
        /// 提交
        /// </summary>
        public void Commit()
        {
            dbTransaction.Commit();
        }
        /// <summary>
        /// 回滚事务
        /// </summary>
        public void Rollback() {

            dbTransaction.Rollback();
        }
        /// <summary>
        /// 事务资源回收
        /// </summary>
        public void Dispose()
        {
            dbTransaction.Dispose();
            dbConnection.Close();           
            dbConnection.Dispose();
        }
    }
}
