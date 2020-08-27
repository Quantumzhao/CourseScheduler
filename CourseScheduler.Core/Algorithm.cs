using System;
using System.Linq;
using System.Net.Http;
using HtmlAgilityPack;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using CourseScheduler.Core.DataStrucures;

namespace CourseScheduler
{
	public static class Algorithm
	{
		public static List<List<Section>> GetPossibleCombinations(Course[] dataset, 
			bool isOpenSecOnly = false, bool isShowFC = false)
		{
			if (dataset.Length == 0) return new List<List<Section>>();

			List<List<Section>> combinations = new List<List<Section>>();
			//Section.IsShowFC = isShowFC;
			//Section.IsOpenSectionOnly = isOpenSecOnly;
			foreach (var section in dataset[0].Sections)
			{
				if (section.IsAvailable)
				{
					combinations.Add(new List<Section>() { section });
				}
			}
			for (int i = 1; i < dataset.Length; i++)
			{
				UpdateCombinations(ref combinations, dataset[i]);
			}

			return combinations;
		}

		private static void UpdateCombinations(ref List<List<Section>> possibilities, Course newCourse)
		{
			var newPossibilities = new List<List<Section>>();

			foreach (var newSection in newCourse.Sections)
			{
				for (int i = 0; i < possibilities.Count; i++)
				{
					if (newSection.IsAvailable && !possibilities[i].IsOverlap(newSection))
					{
						List<Section> sections = new List<Section>(possibilities[i]);
						sections.Add(newSection);
						newPossibilities.Add(sections);
					}
				}
			}
			possibilities = newPossibilities;
		}

		private static bool IsOverlap(this List<Section> possibility, Section newSection)
		{
			foreach (var section in possibility)
			{
				if (section.IsOverlap(newSection))
				{
					return true;
				}
			}

			return false;
		}

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

		public static bool Has(this HashSet<Course> courses, Course newCourse)
		{
			if (courses.Where(c => c.Name == newCourse.Name).Count() != 0) return true;
			else return false;
		}
		public static bool Has(this HashSet<Course> courses, string newCourse)
		{
			if (courses.Where(c => c.Name == newCourse).Count() != 0) return true;
			else return false;
		}
		public static Course Get(this HashSet<Course> courses, string newCourse)
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
