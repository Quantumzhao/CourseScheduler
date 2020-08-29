using CourseScheduler.Core.DataStrucures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CourseScheduler.Core
{
	public static class Helpers
	{
		public static List<DayOfWeek> ToDayOfWeek(this string daysofweek)
		{
			List<DayOfWeek> ret = new List<DayOfWeek>();

			for (int i = 0; i < daysofweek.Length; i++)
			{
				switch (daysofweek[i])
				{
					case 'M':
						ret.Add(DayOfWeek.Monday);
						break;

					case 'W':
						ret.Add(DayOfWeek.Wednesday);
						break;

					case 'F':
						ret.Add(DayOfWeek.Friday);
						break;

					case 'T':
						break;

					case 'u':
						ret.Add(DayOfWeek.Tuesday);
						break;

					case 'h':
						ret.Add(DayOfWeek.Thursday);
						break;

					default:
						break;
				}
			}

			return ret;
		}

		public static bool AddString(this HashSet<string> set, string s)
		{
			foreach (var item in set)
			{
				if (item.Equals(s))
				{
					return false;
				}
			}

			return set.Add(s); ;
		}

		public static string Concatenate(this IEnumerable<string> stringList)
		{
			var list = stringList.ToArray();
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < list.Length; i++)
			{
				if (i != 0) stringBuilder.Append(',');
				stringBuilder.Append(list[i]);
			}

			return stringBuilder.ToString();
		}

		public static bool Has(this IEnumerable<Course> courses, Course newCourse)
		{
			if (courses.Where(c => c.Name == newCourse.Name).Count() != 0) return true;
			else return false;
		}
		public static bool Has(this IEnumerable<Course> courses, string newCourse)
		{
			if (courses.Where(c => c.Name == newCourse).Count() != 0) return true;
			else return false;
		}
		public static Course Get(this IEnumerable<Course> courses, string newCourse)
		{
			try
			{
				return courses.Where(c => c.Name == newCourse).Single();
			}
			catch
			{
				return null;
			}
		}
	}
}
