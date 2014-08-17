﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Data;
using KBO_Crawling.Database;

namespace KBO_Crawling
{
	class DatabaseManager
	{
		public string Host { set; get; }
		public string Port { set; get; }
		public string Database { set; get; }
		public string UserId { set; get; }
		public string Password { set; get; }

		private string _dbConnectionString 
		{
			get
			{
				return string.Format("Server={0};Port={1};Database={2};Uid={3};Pwd={4};charset=utf8;", Host, Port, Database, UserId, Password);
			}
		}

		public bool UpdateStanding(DateTime date, List<Standing> standing)
		{
			bool result = false;
			try
			{
				foreach (var teamInfo in standing)
				{
					#region SQL Query 만들기
					var query = string.Empty;
					if (ExistDate(date, teamInfo.TeamName)) //DB 해당 날짜가 존재하는지 체크
					{
						#region Update SQL Query 만들기
						query = "update standing set rank=@rank, game=@game, win=@win, lose=@lose, draw=@draw, PCT=@PCT, GB=@GB, STRK=@STRK "
								+ " where date = @date and teamName = @teamName;";
								//+ " where date = @date and teamIndex = (select teamIndex from team where teamNameKOR = @teamName);";
						#endregion
					}
					else
					{
						#region Insert SQL Query 만들기
						query = "insert into standing (date, rank, teamName, game, win, lose, draw, PCT, GB, STRK) "
								+ " values (@date, @rank, @teamName, @game, @win, @lose, @draw, @PCT, @GB, @STRK) ;";
								//+ " values (@date, @rank, (select teamIndex from team where teamNameKOR = @teamName), @game, @win, @lose, @draw, @PCT, @GB, @STRK) ;";
						#endregion
					}
					#endregion

					#region Query 실행
					using (var conn = new MySqlConnection(_dbConnectionString))
					{
						conn.Open();
						var cmd = new MySqlCommand(query, conn);

						cmd.Parameters.AddWithValue("@date", date.ToString("yyyy-MM-dd"));
						cmd.Parameters.AddWithValue("@teamName", teamInfo.TeamName);
						cmd.Parameters.AddWithValue("@rank", teamInfo.Rank);
						cmd.Parameters.AddWithValue("@game", teamInfo.Game);
						cmd.Parameters.AddWithValue("@win", teamInfo.Win);
						cmd.Parameters.AddWithValue("@lose", teamInfo.Lose);
						cmd.Parameters.AddWithValue("@draw", teamInfo.Draw);
						cmd.Parameters.AddWithValue("@PCT", teamInfo.PCT);
						cmd.Parameters.AddWithValue("@GB", teamInfo.GB);
						cmd.Parameters.AddWithValue("@STRK", teamInfo.STRK);


						var affectedRow = cmd.ExecuteNonQuery();
						result = (affectedRow == 1);
					}
					#endregion
				}
			}
			catch (Exception ex)
			{
				LogHelper.Log(ex);
			}
			return result;
		}

		private bool ExistDate(DateTime date, string teamName)
		{
			try
			{
				#region SQL Query 만들기
				var query = "select count(1) as cnt from standing "
							+ " where date = @date and teamName = @teamName;";
							//+ " where date = @date and teamIndex = (select teamIndex from team where teamNameKOR = @teamName);";
				#endregion

				#region Query 실행
				using (var conn = new MySqlConnection(_dbConnectionString))
				{
					conn.Open();
					var cmd = new MySqlCommand(query, conn);
					cmd.Parameters.AddWithValue("@date", date.ToString("yyyy-MM-dd"));
					cmd.Parameters.AddWithValue("@teamName", teamName);
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

		public void SyncTable<T>() where T: class
		{
			var table = DatabaseHelper.ToTableModel<T>();
			if (IsTableExist(table.Name)) return;

			var query = table.CreateTableQuery();
			ExecuteNonQuery(query);
		}

		public bool IsTableExist(string tableName)
		{
			var tableQuery = string.Format(
				"SELECT count(*) FROM information_schema.tables WHERE table_schema = '{0}' AND table_name = '{1}'",
				Database, tableName);
			var result = ExecuteReader(tableQuery);
			return (long)result.Rows[0][0] > 0;
		}

		public DataTable ExecuteReader(string query)
		{
			var data = new DataTable();
			using (var conn = new MySqlConnection(_dbConnectionString))
			{
				conn.Open();
				using (var adapter = new MySqlDataAdapter(query, conn))
				{
					adapter.Fill(data);
				}
			}
			return data;
		}

		public void ExecuteNonQuery(string query)
		{
			using (var conn = new MySqlConnection(_dbConnectionString))
			{
				conn.Open();
				using (var command = new MySqlCommand(query, conn))
				{
					command.ExecuteNonQuery();
				}
			}
		}
	}
}
