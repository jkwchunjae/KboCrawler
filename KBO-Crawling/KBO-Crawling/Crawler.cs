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
            #region KBO 홈페이지에서 html code 읽어오기
            using (var client = new WebClient())
            {
               var url = string.Format("http://www.koreabaseball.com/TeamRank/TeamRank.aspx?searchDate={0}", date.ToString("yyyy-MM-dd"));
               client.Encoding = Encoding.UTF8;
               html = client.DownloadString(url);
            }
            #endregion
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
            #region 팀 순위 테이블 찾는다.
            var a = html.IndexOf("<!-- //search box -->");
            var b = html.IndexOf("<table", a);
            var c = html.IndexOf("</table>", b);
            var table = html.Substring(b, c - b + 8);
            #endregion

            #region 승차 Dictionary 를 만든다.
            var xTable = XElement.Parse(table);
            var xTbody = xTable.Element("tbody");

            foreach (var xTr in xTbody.Elements())
            {
               var teamArr = xTr.Elements().ToArray();
               dic.Add(teamArr[1].Value.GetShortEngName(), double.Parse(teamArr[7].Value));
            }
            #endregion
         }
         catch (Exception ex)
         {
            LogHelper.Log(ex);
         }

         return dic;
      }

      public bool Start(DatabaseManager DbMng, DateTime date)
      {
         #region html code 읽어오기
         var html = ReadHtml(date);
         if (html == string.Empty)
         {
            return false;
         }
         #endregion

         #region html code 로 부터 팀별 승차 현황 Dictionary 형태로 받아오기
         var dic = GetGameBehind(html);
         if (dic.Count == 0)
         {
            return false;
         }
         #endregion

         #region DB에 쓴다!
         var result = DbMng.UpdateGameBehind(date, dic);
         #endregion

         return result;
      }
   }
}
