using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace KBO_Crawling
{
   class Crawler
   {
      public string ReadHtml(DateTime date)
      {
         var html = string.Empty;
         try
         {
            using (var client = new WebClient())
            {
               var url = string.Format("http://www.koreabaseball.com/TeamRank/TeamRank.aspx?searchDate={0}", date.ToString("yyyy-MM-dd"));
               client.Encoding = Encoding.UTF8;
               html = client.DownloadString(url);
            }
         }
         catch (Exception ex)
         {
            LogHelper.Log(ex);
         }
         return html;
      }

      public Dictionary<string, double> GetGameBehind(string html)
      {
         var dic = new Dictionary<string, double>();
         try
         {
            var a = html.IndexOf("<!-- //search box -->");
            var b = html.IndexOf("<table", a);
            var c = html.IndexOf("</table>", b);
            var table = html.Substring(b, c - b + 8);

            var xTable = XElement.Parse(table);
            var xTbody = xTable.Element("tbody");

            foreach (var xTr in xTbody.Elements())
            {
               var teamArr = xTr.Elements().ToArray();
               //Console.WriteLine("{0}, {1}", xTr.Elements().ToArray()[1].Value, xTr.Elements().ToArray()[7].Value);
               dic.Add(teamArr[1].Value.GetShortEngName(), double.Parse(teamArr[7].Value));
            }
         }
         catch (Exception ex)
         {
            LogHelper.Log(ex);
         }

         return dic;
      }

      public bool Start(DatabaseManager DbMng, DateTime date)
      {
         var html = ReadHtml(date);
         if (html == string.Empty)
         {
            return false;
         }

         var dic = GetGameBehind(html);
         if (dic.Count == 0)
         {
            return false;
         }

         return DbMng.UpdateGameBehind(date, dic);
      }
   }
}
