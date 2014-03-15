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
         if (args.Count() > 5)
         {
            foreach (var arg in args.Where((e, i) => i >= 5))
            {
               int diff;
               if (int.TryParse(arg, out diff))
               {
                  var date = Utils.GetDate(diff);
                  if (date.ToInt() == 19890201) continue;
                  var isSuccess = crawler.Start(DbMng, date);
                  LogHelper.Log("{0}({1}) : {2}", arg, date.ToInt(), (isSuccess?"Success":"Fail"));
               }
               else
               {
                  LogHelper.Log(arg + " : Fail");
               }
            }
         }
         else
         {
            while (true)
            {
               try
               {
                  var date = DateTime.Now;
                  var isSuccess = crawler.Start(DbMng, date);
                  LogHelper.Log("{0} : {1}", date.ToInt(), (isSuccess ? "Success" : "Fail"));
               }
               catch (Exception ex)
               {
                  LogHelper.Log(ex);
               }
               Thread.Sleep(new TimeSpan(0, 10, 0));
            }
         }
      }
   }
}
