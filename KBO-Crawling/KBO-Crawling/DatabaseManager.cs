using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace KBO_Crawling
{
   class DatabaseManager
   {
      public string Host { set; get; }
      public string Port { set; get; }
      public string Database { set; get; }
      public string UserId { set; get; }
      public string Password { set; get; }

      public MySqlConnection GetDbConnection()
      {
         var dbInfo = string.Format("Server={0};Port={1};Database={2};Uid={3};Pwd={4};charset=utf8;", Host, Port, Database, UserId, Password);
         var conn = new MySqlConnection(dbInfo);

         try
         {
            conn.Open();
         }
         catch
         {
            if (conn != null)
            {
               conn.Close();
            }
            conn = null;
         }
         return conn;
      }

      public bool UpdateGameBehind(DateTime date, Dictionary<string, double> GbDic)
      {
         bool result = false;
         try
         {
            #region SQL Query 만들기
            var query = string.Empty;
            if (ExistDate(date)) //DB 해당 날짜가 존재하는지 체크
            {
               #region Update SQL Query 만들기
               //update
               var str = string.Empty;
               foreach (var team in GbDic)
               {
                  str += string.Format("{0}=@{1}, ", team.Key, team.Key);
               }
               //query = "update gamebehind set SS=@SS, SK=@SK, LT=@LT, KA=@KA, DS=@DS, LG=@LG, HH=@HH, NX=@NX, NC=@NC where datee=@date;";
               query = "update gamebehind set " + str.Substring(0, str.Length - 2) + " where datee=@date;";
               #endregion
            }
            else
            {
               #region Insert SQL Query 만들기
               //insert
               var str = string.Empty;
               foreach (var team in GbDic)
               {
                  str += string.Format(", @{0}", team.Key);
               }
               //query = "insert into gamebehind (datee, SS, SK, LT, KA, DS, LG, HH, NX, NC) values(@date, @SS, @SK, @LT, @KA, @DS, @LG, @HH, @NX, @NC);";
               query = "insert into gamebehind (datee" + str.Replace("@", "") + ") values(@date" + str + ");";
               #endregion
            }
            #endregion

            #region Query 실행
            using (var conn = GetDbConnection())
            {
               var cmd = new MySqlCommand(query, conn);
               cmd.Parameters.AddWithValue("@date", date.ToString("yyyy-MM-dd"));
               foreach (var team in GbDic)
               {
                  cmd.Parameters.AddWithValue("@" + team.Key, team.Value);
               }
               //cmd.Parameters.AddWithValue("@SS", GbDic["SS"]);

               var affectedRow = cmd.ExecuteNonQuery();
               result = (affectedRow == 1);
            }
            #endregion
         }
         catch (Exception ex)
         {
            LogHelper.Log(ex);
         }
         return result;
      }

      private bool ExistDate(DateTime date)
      {
         try
         {
            #region SQL Query 만들기
            var query = "select count(1) as cnt from gamebehind where datee = @date;";
            #endregion

            #region Query 실행
            using (var conn = GetDbConnection())
            {
               var cmd = new MySqlCommand(query, conn);
               cmd.Parameters.AddWithValue("@date", date.ToString("yyyy-MM-dd"));
               var reader = cmd.ExecuteReader();
               if (reader.Read())
               {
                  var cnt = reader.GetInt32("cnt");
                  return !(cnt == 0);
               }
            }
            #endregion
         }
         catch (Exception ex)
         {
            LogHelper.Log(ex);
         }
         return false;
      }
   }
}
