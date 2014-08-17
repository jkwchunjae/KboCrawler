using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KBO_Crawling.Database
{
	public class Table
	{
		public string Name;
		public List<Field> FieldList = new List<Field>();
	}

	public class Field
	{
		public string Name;
		public string Type;
		public bool IsKey;
	}
}
