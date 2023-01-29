# FastSql.Core
Light and convenient ORM Framework of .netcore

Demo:

 class Program
    {
        static string sqlcon = "Data Source = 127.0.0.1;Initial Catalog = Test;User Id = sa;Password = sa@2019;";
        static void Main(string[] args)
        {
           
           //FIND DATA
            var Sql = new CreateSql<Users>().Select().Where("ID=@ID");
            //UPDATA DATA
            // var Sql = new CreateSql<Users>().Updata().Where("ID=@ID");          
            //Delete DATA
            //  var Sql = new CreateSql<Users>().Delete().Where("ID=@ID");
            //exec sql
            using (var conn = new SqlConnection(sqlcon))
            {
                var rdata = conn.SqlQuery<Users>(Sql.ToSqlString(),new { ID=Guid.Parse("088941FE-075A-42C6-8937-5D1A1B795ACD") });
                Console.WriteLine(rdata.ToJsonString());
            }
    
       //Insert DATA
       {
  
                var Sql = new CreateSql<Users>().Insert();
                VAR parm= new Users();
                 using (var conn = new SqlConnection(sqlcon))
            {
                var rdata = conn.SqlExecute(Sql.ToSqlString(),parm);
                Console.WriteLine(rdata);
            }
 
        }



            Console.ReadKey();
        }


        public class Users
        {
            [Key]
            public Guid ID { get; set; }
            public string Name { get; set; }
            public int Age { get; set; }
            public string Sex { get; set; }
            public string Addr { get; set; }
            public string Tell { get; set; }

        }
    }
    --------------------------------------------------------------------------------------------------------------------------------------------
    2.0.0 Version 更新说明

    注意：使用此方法时请先配制且加载默认数据库连接配制：

    DbConfig.SqlConnectString="Data Source = 127.0.0.1;Initial Catalog = Test;User Id = sa;Password = sa@2019;"

    1.查询表数据：

写法1：

 var sql = new CreateSql<Users>().Select();
 var rdata = DapperExt.QueryList(sql);
写法2：
 
 var lsit= new CreateSql<Users>().Select().QueryList();

写法3：可以重载指定表名，或指定查询的字段如：

   var sql = new CreateSql<Users>("Users").Select(new string[] {"Id","UserName" });
   var rdata = DapperExt.QueryList(sql);
 或者
 
var list= new CreateSql<Users>("Users").Select(new string[] {"Id","UserName" }).
QueryList();

2.查询单个对象

var sql = new CreateSql<Users>().Select().Where("Id='843DB63F-252E-436F-B666-F4F8755793B8'");
var rdata = DapperExt.QueryList(sql);
 
或者

Var model=CreateSql<Users>().Select() 
.Where("Id='843DB63F-252E-436F-B666-F4F8755793B8'")
.QueryFirst();

3.非锁表查询（常用于事务中查询）
var sql = new CreateSql<Users>().SelectNoLock().Where("Id='843DB63F-252E-436F-B666-F4F8755793B8'"); 
var rdata = DapperExt.QueryList(sql);
或者
 
 var data= new CreateSql<Users>().SelectNoLock().Where("Id='843DB63F-252E-436F-B666-F4F8755793B8'").QueryFirst();

4.新增操作

var model = new Users() { 
Id = Guid.NewGuid(),
 Age = 20,
 Sex = "女",
 UserName = "test",
 addr = "",
 CreateTime = DateTime.Now };
   var sql = new CreateSql<Users>().Insert();
   var rdata = DapperExt.Add(sql, model);
或者
 var model = new Users() { Id = Guid.NewGuid(), Age = 20, Sex = "女", UserName = "test", addr = "", CreateTime = DateTime.Now };
 var sql = new CreateSql<Users>().Insert().Add(model);
5.修改操作

 var model = new Users() { Id = Guid.NewGuid(), Age = 20, Sex = "女", UserName = "test", addr = "", CreateTime = DateTime.Now };
var sql = new CreateSql<Users>().Updata().Where($"Id='843DB63F-252E-436F-B666-F4F8755793B8'");
var rdata = DapperExt.Modify(sql, model);
 
或者

 var model = new Users() { Id = Guid.NewGuid(), Age = 20, Sex = "女", UserName = "test", addr = "", CreateTime = DateTime.Now };
 var sql = new CreateSql<Users>().Updata().Where($"Id='843DB63F-252E-436F-B666-F4F8755793B8'").Modify(model);

6.删除操作

  var sql = new CreateSql<Users>().Delete().Where($"Id='{data.Id}'");
 
  var rdata = DapperExt.Remove(sql);
 
或者

  var sql = new CreateSql<Users>().Delete().Where($"Id=@Id");
   var rdata = DapperExt.Remove(sql,new { Id=Guid.Parse("F91BEE4E-67BE-4080-9C11-98C1A610F5C5") });


7.事务操作

说明：事务封装类：DapperTransaction，构造参数可以传入数据库连接字符串：
var dt = new DapperTransaction(DbConfig.SqlConnectString)
1.事务处理操作：

  var model = new Users() { 
Id = Guid.NewGuid(),
 Age = 20, Sex = "女",
 UserName = "test", 
addr = "", 
CreateTime = DateTime.Now };

  using (var dt = new DapperTransaction(DbConfig.SqlConnectString))
  
  {
  
var data = new CreateSql<Users>().Insert()
 
.Add(dt, model);

    model.UserName = "69855";
    
var data2 = new CreateSql<Users>().SelectNoLock()
 
.Where($"Id='{model.Id}'").QueryFirst();

    data2.UserName = "mystest";
    
 var data3 = new CreateSql<Users>().Updata().Where($"Id='{data2.Id}'")
 
.Modify(dt, data2);

   dt.Commit(); 

   }
    
---------------------------------------------------------------------------------------------------------------------------------------------------------------------
2.0.1 版本更新内容：
           1.新增拉母表达式sql解析，例如：
           
           //select
           
            var sql = new CreateSql<Users>().Select(s => new { s.Id, s.UserName }).Where(w=>w.Sex=="男").QueryList();


            var user = new Users();
            
            //insert
            new CreateSql<Users>().Insert().Add(user);

            //update
            
            new CreateSql<Users>().Updata().Where(w=>w.Id==Guid.Empty).Modify(user);

            new CreateSql<Users>().Updata(u => u.UserName).Where(w => w.Id == Guid.Empty).Modify(user);

            //delete
            
            new CreateSql<Users>().Delete().Where(w=>w.Id==Guid.Empty).Remove();
 ------------------------------------------------------------------------------------------------------------------------------------------
 2.0.2 版本更新内容：
新增DataTable返回数据方法的扩展实现。
1. public static DataTable GetDataTable(this string sql)
2. public static DataTable GetDataTable<T>(this CreateSql<T> createSql) where T : class, new()
3. public static DataTable GetDataTable(string sql, Dictionary<string, object> sqlparam)
4. public static DataTable GetDataTable<T>(this CreateSql<T> createSql, Dictionary<string, object> sqlparam) where T : class, new()
-------------------------------------------------------------------------------------------------------------------------------------------------------
2.0.3 版本更新内容：
修改拉姆达解释时部份类型无法解析的BUG.
----------------------------------------------------------------------------------------------------------------------------------------
2.0.4:版本更新BUG内容：
DbConfig.SqlConnectString 只读改为可赋值。
----------------------------------------------------------------------------------------------------------------------------------------
2.0.5:版本更新内容：
1.新增多数据库的实现
mysql，oracle，postgresql，sqlite，access
----------------------------------------------------------------------------------------------------------------------------------------
2.0.6:版本更新内容
1.优化查询条件，and,or 添加拉母达表达式。
2.优化数据库配制连接，新增DbConfig.SetSqlConnect（string con）函数，提升安全性。
--------------------------------------------------------------------------------------------------------------------------------------------------------
2.0.7:版本更新内容
1.更新运行时为.net6.
