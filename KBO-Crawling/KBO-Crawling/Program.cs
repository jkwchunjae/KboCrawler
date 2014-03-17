using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace KBO_Crawling
{
   class Program
   {
      static void Main(string[] args)
      {
         if ((args.Count() >= 1 && args[0] == "cmd") || (args.Count() < 5))
         {
            Console.WriteLine("args : Host Port Database UserId Password Range BeginDate EndDate (Absolute Style)");
            Console.WriteLine("args : Host Port Database UserId Password [Date...]");
            Console.WriteLine("Date : Relative -x");
            Console.WriteLine("       Absolute yyyymmdd");
            Console.WriteLine("       None - Auto Crawl");
            return;
         }

         var DbMng = new DatabaseManager()
                        {
                           Host = args[0]
                           , Port = args[1]
                           , Database = args[2]
                           , UserId = args[3]
                           , Password = args[4]
                        };

         var crawler = new Crawler();
         if (args.Count() == 8 && args[5]=="Range")
         {
            #region 범위로 값 읽어오기
            var beginDate = int.Parse(args[6]).ToDateTime();
            var endDate = int.Parse(args[7]).ToDateTime();
            if (beginDate.ToInt() == 19890201 || endDate.ToInt() == 19890201)
            {
               LogHelper.Log("Range {0} - {1} : Fail", args[6], args[7]);
               return;
            }
            for (var date = beginDate; date != endDate.AddDays(1); date = date.AddDays(1))
            {
               var isSuccess = crawler.Start(DbMng, date);
               LogHelper.Log("{0} : {1}", date.ToInt(), (isSuccess ? "Success" : "Fail"));
            }
            #endregion
         }
         else if (args.Count() > 5)
         {
            #region 특정 일 값 읽어오기 (절대참조, 상대참조 둘 다 가능)
            foreach (var arg in args.Where((e, i) => i >= 5))
            {
               int diff;
               if (int.TryParse(arg, out diff))
               {
                  var date = Utils.GetDate(diff);
                  if (date.ToInt() == 19890201)
                  {
                     LogHelper.Log(arg + " : Fail");
                     continue;
                  }
                  var isSuccess = crawler.Start(DbMng, date);
                  LogHelper.Log("{0}({1}) : {2}", arg, date.ToInt(), (isSuccess ? "Success" : "Fail"));
               }
               else
               {
                  LogHelper.Log(arg + " : Fail");
               }
            }
            #endregion
         }
         else
         {
            #region 10분 주기로 어제, 오늘 값 읽어오기
            while (true)
            {
               try
               {
                  var date = DateTime.Now;
                  var isSuccess = crawler.Start(DbMng, date);
                  LogHelper.Log("{0} : {1}", date.ToInt(), (isSuccess ? "Success" : "Fail"));
                  date = DateTime.Now.AddDays(-1);
                  isSuccess = crawler.Start(DbMng, date);
                  LogHelper.Log("{0} : {1}", date.ToInt(), (isSuccess ? "Success" : "Fail"));
               }
               catch (Exception ex)
               {
                  LogHelper.Log(ex);
               }
               Thread.Sleep(new TimeSpan(0, 10, 0));
            }
            #endregion
         }
      }
   }
}
