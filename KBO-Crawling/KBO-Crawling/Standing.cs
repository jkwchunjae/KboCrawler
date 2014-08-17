using KBO_Crawling.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KBO_Crawling
{
	[Table]
	class Standing
	{
		[Key]
		[Date]
		public DateTime Date; // 날짜
		[Key]
		[Varchar(10)]
		public string TeamName; // 팀 이름
		public int Rank; // 순위
		//public int TeamIndex; // 팀 번호
		public int Game; // 경기 수
		public int Win; // 승 수
		public int Lose; // 패 수
		public int Draw; // 무승부 수
		public double PCT; // 승률
		public double GB; // 승차
		[Varchar(20)]
		public string STRK; // 연속
		[Varchar(20)]
		public string L10; // 최근10경기

		public bool WriteInfo(string[] teamArr)
		{
			// 순위
			if (!int.TryParse(teamArr[0], out Rank)) return false;

			// 팀 이름
			TeamName = teamArr[1];

			// 경기 수
			if (!int.TryParse(teamArr[2], out Game)) return false;

			// 승
			if (!int.TryParse(teamArr[3], out Win)) return false;

			// 패
			if (!int.TryParse(teamArr[4], out Lose)) return false;

			// 무
			if (!int.TryParse(teamArr[5], out Draw)) return false;

			// 승률
			if (!double.TryParse(teamArr[6], out PCT)) return false;

			// 승차
			if (!double.TryParse(teamArr[7], out GB)) return false;

			// 연속
			STRK = teamArr[8];

			// 최근10경기
			L10 = "";

			return true;
		}
	}
}
