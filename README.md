# FastSql.Core
Light and convenient ORM Framework of .netcore

Demo:

 class Program
    {
        static string sqlcon = "Data Source = 127.0.0.1;Initial Catalog = Test;User Id = sa;Password = sa@2019;";
        static void Main(string[] args)
        {
            var Sql = new CreateSql<Users>().Select().Where("ID=@ID");
            using (var conn = new SqlConnection(sqlcon))
            {
                var rdata = conn.SqlQuery<Users>(Sql.ToSqlString(),new { ID=Guid.Parse("088941FE-075A-42C6-8937-5D1A1B795ACD") });
                Console.WriteLine(rdata.ToJsonString());
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
