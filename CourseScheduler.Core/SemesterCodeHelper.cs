using System;
using System.Collections.Generic;
using System.Text;

namespace CourseScheduler.Core
{
	public static class SemesterCodeHelper
	{
		private static readonly Dictionary<int, (string, string)> _SemesterCodeDictionary
			= new Dictionary<int, (string, string)>
			{
				{ 0, ("Winter", "12") },
				{ 1, ("Spring", "01") },
				{ 2, ("Summer", "05") },
				{ 3, ("Fall", "08") }
			};

		public static (string, Dictionary<string, string>) GetSemesterList()
		{
			var now = DateTime.Now;
			var currentSemester = GetCurrentSemester(now);
			var currentYear = now.Year;
			var ret = new Dictionary<string, string>();

			var startingSemester = currentSemester - 1;
			for (int i = startingSemester; i < startingSemester + 4; i++)
			{
				int year;
				int semester;
				if (i < 0)
				{
					year = currentYear - 1;
					semester = i + 4;
				}
				else if (i >= 4)
				{
					year = currentYear + 1;
					semester = i - 4;
				}
				else
				{
					year = currentYear;
					semester = i;
				}

				var semesterCode = $"{year}{_SemesterCodeDictionary[semester].Item2}";
				ret.Add(ToSemesterName(semester, year), semesterCode);
			}

			return (ToSemesterName(currentSemester, currentYear), ret);
		}

		private static string ToSemesterName(int semester, int year) 
			=> $"{_SemesterCodeDictionary[semester].Item1} {year}";

		private static int GetCurrentSemester(DateTime now)
		{
			var currentMonth = now.Month;
			switch (currentMonth)
			{
				case 12:
					return 0;

				case 1:
				case 2:
				case 3:
				case 4:
					return 1;

				case 5:
				case 6:
				case 7:
					return 2;

				case 8:
				case 9:
				case 10:
				case 11:
					return 3;

				default:
					throw new ArgumentOutOfRangeException();
			}
		}
	}
}
