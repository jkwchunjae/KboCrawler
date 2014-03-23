using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KBO_Crawling
{
   class Standing
   {
      public DateTime Date;
      public int Rank;
      public string TeamName;
      public int TeamIndex;
      public int Game;
      public int Win;
      public int Lose;
      public int Draw;
      public double PCT;
      public double GB;
      public string STRK;
      public string L10;

      public bool WriteInfo(string[] teamArr)
      {
         int tmp;
         double dbl;

         #region 순위
         if (int.TryParse(teamArr[0], out tmp))
         {
            Rank = tmp;
         }
         else
         {
            return false;
         }
         #endregion

         #region 구단 이름
         TeamName = teamArr[1];
         #endregion

         #region 경기 수
         if (int.TryParse(teamArr[2], out tmp))
         {
            Game = tmp;
         }
         else
         {
            return false;
         }
         #endregion

         #region 승
         if (int.TryParse(teamArr[3], out tmp))
         {
            Win = tmp;
         }
         else
         {
            return false;
         }
         #endregion

         #region 패
         if (int.TryParse(teamArr[4], out tmp))
         {
            Lose = tmp;
         }
         else
         {
            return false;
         }
         #endregion

         #region 무
         if (int.TryParse(teamArr[5], out tmp))
         {
            Draw = tmp;
         }
         else
         {
            return false;
         }
         #endregion

         #region 승률
         if (double.TryParse(teamArr[6], out dbl))
         {
            PCT = dbl;
         }
         else
         {
            return false;
         }
         #endregion

         #region 승차
         if (double.TryParse(teamArr[7], out dbl))
         {
            GB = dbl;
         }
         else
         {
            return false;
         }
         #endregion

         #region 연속
         STRK = teamArr[8];
         #endregion

         #region 최근10경기
         L10 = "";
         #endregion

         return true;
      }
   }
}
