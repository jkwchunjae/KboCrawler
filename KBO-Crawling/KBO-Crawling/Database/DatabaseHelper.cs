using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace KBO_Crawling.Database
{
	public static class DatabaseHelper
	{
		public static string ToDatabaseType(this FieldInfo member)
		{
			var lowerType = member.FieldType.Name.ToLower();
			if (lowerType == "int32") return "int";
			if (lowerType == "int64") return "bigint";
			if (lowerType == "double") return "double";
			if (lowerType == "datetime")
			{
				if (member.HasAttribute<DateAttribute>())
					return "date";
				return "datetime";
			}
			if (lowerType == "string")
			{
				if (member.HasAttribute<TextAttribute>())
					return "text";
				if (member.HasAttribute<VarcharAttribute>())
					return string.Format("varchar({0})", member.GetAttribute<VarcharAttribute>().Length);
				return "varchar(256)";
			}

			return member.FieldType.Name.ToLower();
		}

		public static Table ToTableModel<T>() where T : class
		{
			var tableTypeInfo = typeof(T);
			if (!tableTypeInfo.HasAttribute<TableAttribute>()) return null;

			var table = new Table() { Name = tableTypeInfo.Name };
			foreach (var member in tableTypeInfo.GetFields())
			{
				table.FieldList.Add(new Field()
					{
						Name = member.Name
						, Type = member.ToDatabaseType()
						, IsKey = member.HasAttribute<KeyAttribute>()
					}
				);
			}
			return table;
		}

		public static string CreateTableQuery(this Table table)
		{
			var queryBuilder = new StringBuilder();
			queryBuilder.Append(string.Format("CREATE TABLE {0}", table.Name) + Environment.NewLine);
			queryBuilder.Append("(" + Environment.NewLine);
			foreach (var elem in table.FieldList)
			{
				queryBuilder.Append(
					string.Format("\t{0} {1}{2}{3}",
						elem.Name,
						elem.Type,
						",",
						Environment.NewLine));
			}
			queryBuilder.Append(string.Format("\tPRIMARY KEY ({0}){1}",
				string.Join(", ", table.FieldList.Where(e => e.IsKey).Select(e => e.Name)),
				Environment.NewLine));

			queryBuilder.Append(")" + Environment.NewLine);
			queryBuilder.Append(@"ENGINE=InnoDB CHARACTER SET utf8 COLLATE utf8_general_ci");

			return queryBuilder.ToString();
		}
	}
}
