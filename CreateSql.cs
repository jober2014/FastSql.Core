using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace FastSql.Core
{
    /// <summary>
    /// SQL创建器
    /// </summary>
    public class CreateSql<T> where T : class, new()
    {
        private StringBuilder Sqlbuilder;

        private Type _type;

        private String TableName;

        private PropertyInfo[] pro;

        private readonly T mode;

        private string mark = "@";

        private string databaseType = "sqlserver";

        public CreateSql(string table = null)
        {
            mode = default(T);
            Sqlbuilder = new StringBuilder();
            _type = typeof(T);
            if (string.IsNullOrEmpty(table))
            {
                TableName = typeof(T).Name;
            }
            else
            {
                TableName = table;
            }

            pro = _type.GetProperties();

        }
        public CreateSql(string DatabaseType, string table = null)
        {
            mode = default(T);
            Sqlbuilder = new StringBuilder();
            _type = typeof(T);
            if (string.IsNullOrEmpty(table))
            {
                TableName = typeof(T).Name;
            }
            else
            {
                TableName = table;
            }
            if (!string.IsNullOrEmpty(DatabaseType))
            {
                switch (DatabaseType)
                {
                    case DataBaseType.SqlServer: this.mark = "@"; break;
                    case DataBaseType.MySql: this.mark = "?"; break;
                    case DataBaseType.Oracle: this.mark = ":"; break;
                    case DataBaseType.PostGreSql: this.mark = ":"; break;
                    case DataBaseType.Sqlite: this.mark = "@"; break;
                    case DataBaseType.Access: this.mark = "@"; break;
                }
                this.databaseType = DatabaseType;
            }
            pro = _type.GetProperties();

        }
        /// <summary>
        /// 查询SQL
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="mode">对象</param>
        /// <param name="colnm">选择字段：默认所有</param>
        /// <returns></returns>
        public CreateSql<T> Select(params string[] colnm)
        {
            var sb = new StringBuilder();
            string sqlstr = "SELECT {0} FROM [{1}]";
            foreach (var item in pro)
            {
                if (colnm.Length > 0)
                {
                    if (colnm.Contains(item.Name))
                    {
                        sb.Append($"[{item.Name}],");
                    }
                }
                else
                    sb.Append($"[{item.Name}],");
            }
            Sqlbuilder.Append(string.Format(sqlstr, sb.ToString().TrimEnd(','), TableName));
            sb.Clear();
            return this;
        }

        public CreateSql<T> Select(Expression<Func<T, object>> expression)
        {
            var sb = new StringBuilder();
            string sqlstr = "SELECT {0} FROM [{1}]";
            if (expression.Body.GetType().Name == "PropertyExpression")
            {
                dynamic body = expression.Body;
                if (body != null)
                {
                    sb.Append($"[{body.Member.Name}]");
                }
            }
            else if (expression.Body.GetType().Name == "NewExpression")
            {
                var body = expression.Body as NewExpression;
                if (body != null)
                {
                    foreach (var item in body.Members)
                    {
                        sb.Append($"[{item.Name}],");
                    }
                }
            }
            else if (expression.Body.GetType().Name == "UnaryExpression")
            {
                dynamic body = expression.Body;
                sb.Append($"[{body.Operand.Member.Name}]");
            }
            Sqlbuilder.Append(string.Format(sqlstr, sb.ToString().TrimEnd(','), TableName));
            sb.Clear();
            return this;
        }
        /// <summary>
        /// 查询SQL
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="mode">对象</param>
        /// <param name="colnm">选择字段：默认所有</param>
        /// <returns></returns>
        public CreateSql<T> SelectNoLock(params string[] colnm)
        {
            var sb = new StringBuilder();
            string sqlstr = "SELECT {0} FROM [{1}] WITH(NOLOCK)";
            foreach (var item in pro)
            {
                if (colnm.Length > 0)
                {
                    if (colnm.Contains(item.Name))
                    {
                        sb.Append($"[{item.Name}],");
                    }
                }
                else
                    sb.Append($"[{item.Name}],");
            }
            Sqlbuilder.Append(string.Format(sqlstr, sb.ToString().TrimEnd(','), TableName));
            sb.Clear();
            return this;
        }
        /// <summary>
        /// 查询SQL
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="mode">对象</param>
        /// <param name="colnm">选择字段：默认所有</param>
        /// <returns></returns>
        public CreateSql<T> SelectNoLock(Expression<Func<T, object>> expression)
        {
            var sb = new StringBuilder();
            string sqlstr = "SELECT {0} FROM [{1}] WITH(NOLOCK)";
            if (expression.Body.GetType().Name == "PropertyExpression")
            {
                dynamic body = expression.Body;
                if (body != null)
                {
                    sb.Append($"[{body.Member.Name}]");
                }
            }
            else if (expression.Body.GetType().Name == "NewExpression")
            {
                var body = expression.Body as NewExpression;
                if (body != null)
                {
                    foreach (var item in body.Members)
                    {
                        sb.Append($"[{item.Name}],");
                    }
                }
            }
            else if (expression.Body.GetType().Name == "UnaryExpression")
            {
                dynamic body = expression.Body;
                sb.Append($"[{body.Operand.Member.Name}]");
            }
            Sqlbuilder.Append(string.Format(sqlstr, sb.ToString().TrimEnd(','), TableName));
            sb.Clear();
            return this;
        }

        /// <summary>
        /// 插入数据SQL
        /// </summary>
        /// <param name="mark">参数符：默认@</param>
        /// <param name="hasKey">是否包含主键：默认True</param>
        /// <returns></returns>
        public CreateSql<T> Insert(bool hasKey = true)
        {
            string sqlstr = "INSERT INTO [{2}]({0}) VALUES({1})";
            var sb = new StringBuilder();
            var pb = new StringBuilder();
            var key = pro.Where(w => w.GetCustomAttributes(typeof(KeyAttribute), false).Length > 0);
            foreach (var item in pro)
            {
                if (hasKey)
                {
                    sb.Append($"[{item.Name}],");
                    pb.Append($"{mark}{item.Name},");
                }
                else
                {
                    if (key != null && key.Select(s => s.Name).Contains(item.Name))
                    {
                        continue;
                    }
                    else
                    {
                        sb.Append($"[{item.Name}],");
                        pb.Append($"{mark}{item.Name},");
                    }

                }


            }
            Sqlbuilder.Append(string.Format(sqlstr, sb.ToString().TrimEnd(','), pb.ToString().TrimEnd(','), TableName));
            sb.Clear();
            pb.Clear();
            return this;
        }
        /// <summary>
        /// 插入数据SQL用于自增主键（该方法只对SQLSERVER有效）
        /// </summary>
        /// <returns></returns>
        public CreateSql<T> InsertAutoKey()
        {
            string sqlstr = "INSERT INTO [{2}]({0}) VALUES({1});SELECT SCOPE_IDENTITY();";
            var sb = new StringBuilder();
            var pb = new StringBuilder();
            var key = pro.Where(w => w.GetCustomAttributes(typeof(KeyAttribute), false).Length > 0);
            foreach (var item in pro)
            {
                //排除主键
                if (key != null && key.Select(s => s.Name).Contains(item.Name))
                {
                    continue;
                }
                else
                {
                    sb.Append($"[{item.Name}],");
                    pb.Append($"@{item.Name},");
                }

            }
            Sqlbuilder.Append(string.Format(sqlstr, sb.ToString().TrimEnd(','), pb.ToString().TrimEnd(','), TableName));
            sb.Clear();
            pb.Clear();
            return this;
        }

        /// <summary>
        /// SQL修改
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="mode">对象实体</param>
        /// <param name="mark">参数符号：默认@</param>
        /// <param name="remove">过滤字段</param>
        /// <returns></returns>
        public CreateSql<T> Updata(params string[] colnm)
        {

            string sqlstr = "UPDATE [{0}] SET {1}";
            var sb = new StringBuilder();
            //获取实体主键
            var key = pro.Where(w => w.GetCustomAttributes(typeof(KeyAttribute), false).Length > 0);
            foreach (var item in pro)
            {
                if (colnm.Length > 0)
                {
                    if (!colnm.Contains(item.Name))
                    {
                        continue;
                    }
                }
                //过滤主键或ID
                if (key != null && key.Any())
                {
                    if (key.Select(s => s.Name).Contains(item.Name))
                    {
                        continue;
                    }
                    else
                    {
                        sb.Append($"[{item.Name}]={mark}{item.Name},");

                    }
                }
                else
                {
                    if (item.Name.ToLower().Equals("id"))
                    {
                        continue;
                    }
                    else
                    {
                        sb.Append($"[{item.Name}]={mark}{item.Name},");
                    }
                }


            }
            Sqlbuilder.Append(string.Format(sqlstr, TableName, sb.ToString().TrimEnd(',')));
            sb.Clear();
            return this;
        }

        /// <summary>
        /// SQL修改
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="mode">对象实体</param>
        /// <param name="mark">参数符号：默认@</param>
        /// <param name="remove">过滤字段</param>
        /// <returns></returns>
        public CreateSql<T> Updata(Expression<Func<T, object>> expression)
        {

            string sqlstr = "UPDATE [{0}] SET {1}";
            var sb = new StringBuilder();
            var colnm = ResloveName(expression);
            //获取实体主键
            var key = pro.Where(w => w.GetCustomAttributes(typeof(KeyAttribute), false).Length > 0);
            foreach (var item in pro)
            {
                if (colnm.Length > 0)
                {
                    if (!colnm.Contains(item.Name))
                    {
                        continue;
                    }
                }
                //过滤主键或ID
                if (key != null && key.Any())
                {
                    if (key.Select(s => s.Name).Contains(item.Name))
                    {
                        continue;
                    }
                    else
                    {

                        sb.Append($"[{item.Name}]={mark}{item.Name},");

                    }
                }
                else
                {
                    if (item.Name.ToLower().Equals("id"))
                    {
                        continue;
                    }
                    else
                    {
                        sb.Append($"[{item.Name}]={mark}{item.Name},");
                    }
                }


            }
            Sqlbuilder.Append(string.Format(sqlstr, TableName, sb.ToString().TrimEnd(',')));
            sb.Clear();
            return this;
        }
        /// <summary>
        /// 构造删除SQL
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="mode"></param>
        /// <param name="mark"></param>
        /// <returns></returns>
        public CreateSql<T> Delete()
        {
            var sql = "DELETE FROM [{0}]";
            Sqlbuilder.Append(string.Format(sql, TableName));
            return this;

        }
        /// <summary>
        /// 拼接条件
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public CreateSql<T> Where(string _where = "")
        {
            if (Sqlbuilder != null)
            {
                Sqlbuilder.Append(" WHERE " + _where);
            }
            return this;

        }

        /// <summary>
        /// 拼接条件
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public CreateSql<T> Where(Expression<Func<T, bool>> predicate)
        {
            if (predicate != null)
            {
                var sqlWhere = SqlBuild.WhereByLambda<T>(predicate);
                Sqlbuilder.Append(" WHERE " + sqlWhere);
            }
            return this;

        }


        /// <summary>
        /// and条件
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public CreateSql<T> And(string _and = "")
        {
            if (Sqlbuilder != null)
            {
                Sqlbuilder.Append(" AND " + _and);
            }
            return this;

        }
        /// <summary>
        /// 拼接or条件
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public CreateSql<T> Or(string _or = "")
        {
            if (Sqlbuilder != null)
            {
                Sqlbuilder.Append(" OR " + _or);
            }
            return this;

        }

        /// <summary>
        /// Exists条件
        /// </summary>
        /// <param name="str"></param>
        /// <param name="exi"></param>
        /// <returns></returns>
        public CreateSql<T> Exists(string exi)
        {
            if (Sqlbuilder != null)
            {
                Sqlbuilder.Append($" EXISTS ({exi})");
            }
            return this;
        }
        /// <summary>
        /// NOT EXISTS 
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="sqlstr"></param>
        /// <returns></returns>
        public CreateSql<T> NotExists(string sqlstr)
        {
            if (Sqlbuilder != null)
            {
                Sqlbuilder.Append($" NOT EXISTS({sqlstr})");
            }
            return this;
        }
        /// <summary>
        /// IN 操作
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="sqlstr"></param>
        /// <returns></returns>
        public CreateSql<T> In(string sqlstr)
        {
            if (Sqlbuilder != null)
            {
                Sqlbuilder.Append($" IN ({sqlstr})");
            }
            return this;
        }
        /// <summary>
        /// IN 操作
        /// </summary>
        /// <typeparam name="T">数组对象</typeparam>
        /// <param name="sb"></param>
        /// <param name="arr">数组</param>
        /// <returns></returns>
        public CreateSql<T> In<K>(K[] arr) where K : struct
        {
            if (arr != null)
            {
                var str = ArrayToSplit(arr);
                Sqlbuilder.Append($" IN ({str})");
            }
            return this;
        }

        public CreateSql<T> In(Guid[] arr)
        {
            if (arr != null)
            {
                var str = ArrayToSplit(arr);
                Sqlbuilder.Append($" IN ({str})");
            }
            return this;
        }
        /// <summary>
        ///  IN 操作
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="sqlstr">字符数组</param>
        /// <returns></returns>
        public CreateSql<T> In(string[] sqlstr)
        {
            if (sqlstr != null)
            {
                var arrstr = ArrayToSplit(sqlstr);
                Sqlbuilder.Append($" IN ({arrstr})");
            }
            return this;
        }
        /// <summary>
        /// not in 操作
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="sqlstr">字符数据</param>
        /// <returns></returns>
        public CreateSql<T> NotIn(string[] sqlstr)
        {
            if (sqlstr != null)
            {
                var arrstr = ArrayToSplit(sqlstr);
                Sqlbuilder.Append($" NOT IN ({arrstr})");
            }
            return this;
        }
        /// <summary>
        ///  not in 操作
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sb"></param>
        /// <param name="sqlstr">字符数据</param>
        /// <returns></returns>
        public CreateSql<T> NotIn<K>(K[] sqlstr) where K : struct
        {
            if (sqlstr != null)
            {
                var arrstr = ArrayToSplit(sqlstr);
                Sqlbuilder.Append($" NOT IN ({arrstr})");
            }
            return this;
        }
        /// <summary>
        /// LIKE 操作
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sb"></param>
        /// <param name="sqlstr">条件</param>
        /// <returns></returns>
        public CreateSql<T> Like(string sqlstr)
        {
            if (sqlstr != null)
            {
                Sqlbuilder.Append($" LIKE  {sqlstr}");
            }
            return this;
        }
        /// <summary>
        /// 排序操作
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sb"></param>
        /// <param name="sqlstr">条件</param>
        /// <returns></returns>
        public CreateSql<T> Order_By(string sqlstr)
        {
            if (sqlstr != null)
            {
                Sqlbuilder.Append($" ORDER BY {sqlstr}");
            }
            return this;
        }
        /// <summary>
        /// 排序操作
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sb"></param>
        /// <param name="sqlstr">条件</param>
        /// <returns></returns>
        public CreateSql<T> Order_By(Expression<Func<T, object>> expression)
        {
            var sb = new StringBuilder();
            if (expression != null)
            {
                if (expression.Body.GetType().Name == "PropertyExpression")
                {
                    dynamic body = expression.Body;
                    if (body != null)
                    {
                        sb.Append($"[{body.Member.Name}]");
                    }
                }
                else if (expression.Body.GetType().Name == "NewExpression")
                {
                    var body = expression.Body as NewExpression;
                    if (body != null)
                    {
                        foreach (var item in body.Members)
                        {
                            sb.Append($"[{item.Name}],");
                        }
                    }
                }
                else if (expression.Body.GetType().Name == "UnaryExpression")
                {
                    dynamic body = expression.Body;
                    sb.Append($"[{body.Operand.Member.Name}]");
                }
                Sqlbuilder.Append($" ORDER BY {sb.ToString().TrimEnd(',')}");
            }
            return this;
        }
        /// <summary>
        ///  顺序
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sb"></param>
        /// <param name="sqlstr">条件</param>
        /// <returns></returns>
        public CreateSql<T> Desc()
        {
            Sqlbuilder.Append(" DESC ");
            return this;
        }
        public CreateSql<T> Append(string str)
        {
            Sqlbuilder.Append(" " + str);
            return this;

        }
        /// <summary>
        /// 获取SQL字符串
        /// </summary>
        /// <returns></returns>
        public string ToSqlString()
        {
            return Sqlbuilder.ToString();
        }
        private string ArrayToSplit(string[] Arr)
        {
            var str = string.Empty;
            if (Arr != null)
            {
                var sb = new StringBuilder();
                for (int i = 0; i < Arr.Length; i++)
                {
                    sb.Append($"'{Arr[i]}',");
                }
                str = sb.ToString().TrimEnd(',');
            }
            return str.TrimEnd(',');

        }

        private string ArrayToSplit(Guid[] Arr)
        {
            var str = string.Empty;
            if (Arr != null)
            {
                var sb = new StringBuilder();
                for (int i = 0; i < Arr.Length; i++)
                {
                    sb.Append($"'{Arr[i]}',");
                }
                str = sb.ToString().TrimEnd(',');
            }
            return str.TrimEnd(',');

        }

        private string ArrayToSplit<K>(K[] arr) where K : struct
        {
            var str = string.Empty;
            if (arr != null)
            {
                str = string.Join(",", arr);
            }
            return str;
        }
        private string[] ResloveName(Expression<Func<T, object>> expression)
        {
            var result = new List<string>();
            if (expression.Body.GetType().Name == "PropertyExpression")
            {
                dynamic body = expression.Body;
                if (body != null)
                {
                    result.Add(body.Member.Name);
                }
            }
            else if (expression.Body.GetType().Name == "NewExpression")
            {
                var body = expression.Body as NewExpression;
                if (body != null)
                {
                    foreach (var item in body.Members)
                    {
                        result.Add(item.Name);
                    }
                }
            }
            else if (expression.Body.GetType().Name == "UnaryExpression")
            {
                dynamic body = expression.Body;              
                result.Add(body.Operand.Member.Name);
            }
            return result.ToArray();
        }
    }
}
