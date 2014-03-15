﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KBO_Crawling
{
   public static class Utils
   {
      private static Dictionary<string, string> engNameDic = new Dictionary<string, string>();

      static Utils()
      {
         engNameDic.Add("삼성", "SS");
         engNameDic.Add("넥센", "NX");
         engNameDic.Add("LG", "LG");
         engNameDic.Add("롯데", "LT");
         engNameDic.Add("KIA", "KA");
         engNameDic.Add("두산", "DS");
         engNameDic.Add("SK", "SK");
         engNameDic.Add("NC", "NC");
         engNameDic.Add("한화", "HH");
         engNameDic.Add("현대", "NX");
      }

      public static DateTime ToDateTime(this int date)
      {
         return new DateTime(date / 10000, (date / 100) % 100, date % 100);
      }

      public static int ToInt(this DateTime date)
      {
         return date.Year * 10000 + date.Month * 100 + date.Day;
      }

      public static DateTime GetDate(int diff)
      {
         var date = DateTime.Now;
         if (diff == 0)
         {
            date = DateTime.Now;
         }
         else if (diff < 0)
         {
            date = DateTime.Now.AddDays(diff);
         }
         else if (diff < 9999999)
         {
            LogHelper.Log(diff + " : Fail");
            date = new DateTime(1989, 2, 1);
         }
         else
         {
            try
            {
               date = diff.ToDateTime();
            }
            catch (Exception ex)
            {
               LogHelper.Log(ex);
               date = new DateTime(1989, 2, 1);
            }
         }
         return date;
      }

      public static void Dump(this object obj)
      {
         Console.WriteLine(obj.ToString());
      }

      public static string GetShortEngName(this string teamName)
      {
         if (engNameDic.ContainsKey(teamName))
         {
            return engNameDic[teamName];
         }
         else
         {
            LogHelper.Log("Unknown TeamName : {0}", teamName);
            return "??";
         }
      }
   }
}
