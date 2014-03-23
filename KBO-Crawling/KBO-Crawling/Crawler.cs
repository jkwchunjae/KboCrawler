using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using HtmlAgilityPack;
using System.IO;

namespace KBO_Crawling
{
   class Crawler
   {
      public string ReadHtml(DateTime date)
      {
         #region KBO 홈페이지에서 html code 읽어오기
         var html = string.Empty;
         try
         {
            var url = string.Format("http://www.koreabaseball.com/TeamRank/TeamRank.aspx?searchDate={0}", date.ToString("yyyy-MM-dd"));
            var webRequest = (HttpWebRequest)WebRequest.Create(url);
            webRequest.UserAgent = "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 6.0)";
            webRequest.CookieContainer = new CookieContainer();
            webRequest.AllowAutoRedirect = true;
            webRequest.Timeout = 2000;

            using (var webResponse = (HttpWebResponse)webRequest.GetResponse())
            using (var reader = new StreamReader(webResponse.GetResponseStream(), Encoding.UTF8))
            {
               var rawHtml = reader.ReadToEnd();
               var statusCode = webResponse.StatusCode;

               html = rawHtml;
            }
         }
         catch (Exception ex)
         {
            LogHelper.Log(ex);
         }
         #endregion
         return html;
      }

      public List<Standing> GetGameBehind(string html)
      {
         var standing = new List<Standing>();
         try
         {
            #region 팀 순위 테이블 찾는다.
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);
            var table = htmlDoc.GetElementbyId("xtable1");
            #endregion

            #region 승차 List<Standing> 에 값을 채운다.

            foreach (var xTr in table.SelectSingleNode("tbody").SelectNodes("tr"))
            {
               var teamArr = xTr.SelectNodes("td").Select(e => e.InnerText).ToArray();
               var teamInfo = new Standing();
               teamInfo.WriteInfo(teamArr);
               standing.Add(teamInfo);
            }
            #endregion
         }
         catch (Exception ex)
         {
            LogHelper.Log(ex);
         }

         return standing;
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
         var standing = GetGameBehind(html);
         if (standing.Count == 0)
         {
            return false;
         }
         #endregion

         #region DB에 쓴다!
         var result = DbMng.UpdateStanding(date, standing);
         #endregion

         return result;
      }
   }
}
