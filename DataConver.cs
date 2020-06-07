using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FastSql.Core
{
    /// <summary>
    /// 数据对象转换器
    /// </summary>
    public static class DataConver
    {
        /// <summary>
        /// 实体类列表转化
        /// </summary>
        /// <param name="DataReader">SqlDataReader</param>
        /// <returns></returns>
        public static List<T> ToList<T>(this DbDataReader DataReader) where T : class, new()
        {
            List<T> result = null;
            Type type = typeof(T);
            var method = type.GetProperties();
            if (DataReader.HasRows)
            {
                result = new List<T>();
                while (DataReader.Read())
                {
                    var func = Activator.CreateInstance(type);
                    foreach (PropertyInfo p in method)
                    {
                        if (DataReader[p.Name] == System.DBNull.Value)
                        {
                            continue;
                        }
                        else
                            p.SetValue(func, DataReader[p.Name], null);
                    }
                    result.Add(func as T);
                }
            }
            return result;

        }
        /// <summary>
        /// 转换单个对象
        /// </summary>
        /// <param name="DataReader">DataReader</param>
        /// <returns></returns>
        public static T ToModel<T>(this DbDataReader DataReader) where T : class, new()
        {
            T result = default(T);
            Type type = typeof(T);
            var method = type.GetProperties();
            if (DataReader.HasRows)
            {
                while (DataReader.Read())
                {
                    var func = Activator.CreateInstance(type);
                    foreach (PropertyInfo p in method)
                    {
                        if (DataReader[p.Name] == System.DBNull.Value)
                        {
                            continue;
                        }
                        else
                            p.SetValue(func, DataReader[p.Name], null);
                    }
                    result = func as T;
                }
            }
            return result;

        }
        /// <summary>
        /// DataTable转成对象
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        public static List<T> ToList<T>(this DataTable table) where T : class, new()
        {
            List<T> result = null;
            Type type = typeof(T);
            if (table.Rows.Count > 0)
            {
                result = new List<T>();
                foreach (DataRow item in table.Rows)
                {
                    var c = Activator.CreateInstance(type);
                    foreach (PropertyInfo p in c.GetType().GetProperties())
                    {
                        if (item[p.Name] == System.DBNull.Value)
                        {
                            continue;
                        }
                        else
                            p.SetValue(c, item[p.Name], null);
                    }
                    result.Add(c as T);
                }
            }
            return result;

        }
        /// <summary>
        /// 将objects模型对象转为实体对象
        /// </summary>
        /// <typeparam name="T">实本类</typeparam>
        /// <param name="values">object对象</param>
        /// <returns></returns>
        public static T ToMode<T>(this object values) where T : class, new()
        {
            var result = default(T);
            try
            {
                Type type = typeof(T);
                var members = type.GetProperties();
                if (values != null)
                {
                    var func = Activator.CreateInstance(type);
                    foreach (PropertyInfo item in members)
                    {
                        if (values.GetType().GetProperty(item.Name) != null)
                        {
                            item.SetValue(func, values.GetType().GetProperty(item.Name).GetValue(values));
                        }

                    }
                    result = func as T;
                }
            }
            catch (Exception)
            {
                throw;
            }
            return result;

        }

        /// <summary>    

        /// 转化一个DataTable    

        /// </summary>    

        /// <typeparam name="T"></typeparam>    
        /// <param name="list"></param>    
        /// <returns></returns>    
        public static DataTable ToDataTable<T>(this IEnumerable<T> list)
        {

            //创建属性的集合    
            List<PropertyInfo> pList = new List<PropertyInfo>();
            //获得反射的入口    

            Type type = typeof(T);
            DataTable dt = new DataTable();
            //把所有的public属性加入到集合 并添加DataTable的列    
            Array.ForEach<PropertyInfo>(type.GetProperties(), p => { pList.Add(p); dt.Columns.Add(p.Name, p.PropertyType); });
            foreach (var item in list)
            {
                //创建一个DataRow实例    
                DataRow row = dt.NewRow();
                //给row 赋值    
                pList.ForEach(p => row[p.Name] = p.GetValue(item, null));
                //加入到DataTable    
                dt.Rows.Add(row);
            }
            return dt;
        }


        /// <summary>    
        /// DataTable 转换为List 集合    
        /// </summary>    
        /// <typeparam name="TResult">类型</typeparam>    
        /// <param name="dt">DataTable</param>    
        /// <returns></returns>    
        public static List<T> TableToList<T>(this DataTable dt) where T : class, new()
        {
            //创建一个属性的列表    
            List<PropertyInfo> prlist = new List<PropertyInfo>();
            //获取TResult的类型实例  反射的入口    

            Type t = typeof(T);

            //获得TResult 的所有的Public 属性 并找出TResult属性和DataTable的列名称相同的属性(PropertyInfo) 并加入到属性列表     
            Array.ForEach<PropertyInfo>(t.GetProperties(), p => { if (dt.Columns.IndexOf(p.Name) != -1) prlist.Add(p); });

            //创建返回的集合    

            List<T> oblist = new List<T>();

            foreach (DataRow row in dt.Rows)
            {
                //创建TResult的实例    
                T ob = new T();
                //找到对应的数据  并赋值    
                prlist.ForEach(p => { if (row[p.Name] != DBNull.Value) p.SetValue(ob, row[p.Name], null); });
                //放入到返回的集合中.    
                oblist.Add(ob);
            }
            return oblist;
        }




        /// <summary>    
        /// 将集合类转换成DataTable    
        /// </summary>    
        /// <param name="list">集合</param>    
        /// <returns></returns>    
        public static DataTable ToDataTable(this IList list)
        {
            DataTable result = new DataTable();
            if (list.Count > 0)
            {
                PropertyInfo[] propertys = list[0].GetType().GetProperties();

                foreach (PropertyInfo pi in propertys)
                {
                    result.Columns.Add(pi.Name, pi.PropertyType);
                }
                for (int i = 0; i < list.Count; i++)
                {
                    ArrayList tempList = new ArrayList();
                    foreach (PropertyInfo pi in propertys)
                    {
                        object obj = pi.GetValue(list[i], null);
                        tempList.Add(obj);
                    }
                    object[] array = tempList.ToArray();
                    result.LoadDataRow(array, true);
                }
            }
            return result;
        }



        /// <summary>    
        /// 将泛型集合类转换成DataTable    
        /// </summary>    
        /// <typeparam name="T">集合项类型</typeparam>   
        /// <param name="list">集合</param>    
        /// <returns>数据集(表)</returns>    
        public static DataTable ToDataTable<T>(this IList<T> list) where T : new()
        {
            return ToDataTable<T>(list, null);

        }



        /// <summary>    
        /// 将泛型集合类转换成DataTable    
        /// </summary>    
        /// <typeparam name="T">集合项类型</typeparam>    
        /// <param name="list">集合</param>    
        /// <param name="propertyName">需要返回的列的列名</param>    
        /// <returns>数据集(表)</returns>    
        public static DataTable ToDataTable<T>(IList<T> list, params string[] propertyName)
        {
            List<string> propertyNameList = new List<string>();
            if (propertyName != null)
                propertyNameList.AddRange(propertyName);
            DataTable result = new DataTable();
            if (list.Count > 0)
            {
                PropertyInfo[] propertys = list[0].GetType().GetProperties();
                foreach (PropertyInfo pi in propertys)
                {
                    if (propertyNameList.Count == 0)
                    {
                        result.Columns.Add(pi.Name, pi.PropertyType);
                    }
                    else
                    {
                        if (propertyNameList.Contains(pi.Name))
                            result.Columns.Add(pi.Name, pi.PropertyType);
                    }
                }

                for (int i = 0; i < list.Count; i++)
                {
                    ArrayList tempList = new ArrayList();
                    foreach (PropertyInfo pi in propertys)
                    {
                        if (propertyNameList.Count == 0)
                        {
                            object obj = pi.GetValue(list[i], null);
                            tempList.Add(obj);
                        }
                        else
                        {
                            if (propertyNameList.Contains(pi.Name))
                            {
                                object obj = pi.GetValue(list[i], null);
                                tempList.Add(obj);
                            }
                        }
                    }
                    object[] array = tempList.ToArray();
                    result.LoadDataRow(array, true);
                }
            }
            return result;
        }
        /// <summary> 
        /// 获取DataTable前几条数据 
        /// </summary> 
        /// <param name="TopItem">前N条数据</param> 
        /// <param name="DT">源DataTable</param> 
        /// <returns></returns> 
        public static DataTable DtSelectTop(int TopItem, DataTable DT)
        {
            if (DT.Rows.Count < TopItem) return DT;

            DataTable NewTable = DT.Clone();
            DataRow[] rows = DT.Select("1=1");
            for (int i = 0; i < TopItem; i++)
            {
                NewTable.ImportRow((DataRow)rows[i]);
            }
            return NewTable;
        }

    }
}
