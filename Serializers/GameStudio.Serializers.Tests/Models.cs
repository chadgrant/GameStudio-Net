using System;
using System.Collections.Generic;
using System.Text;

namespace GameStudio.Serializers.Tests
{
	public enum TestEnum
	{
		NegativeOne = -1,
		Zero = 0,
		One = 1
	}

	[Serializable]
	public class ClassWithDate
	{
		public ClassWithDate()
		{
		}

		public ClassWithDate(DateTime date)
		{
			Date = date;
		}

		public DateTime Date { get; set; }
	}

	[Serializable]
	public class ClassWithNullableDate
	{
		public ClassWithNullableDate()
		{
		}

		public ClassWithNullableDate(DateTime? date)
		{
			Date = date;
		}

		public DateTime? Date { get; set; }
	}

	[Serializable]
	public class ClassWithTestEnum
	{
		public TestEnum TestEnum { get; set; }
	}
}
