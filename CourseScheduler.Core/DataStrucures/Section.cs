using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CourseScheduler.Core.DataStrucures
{
	public class Section
	{
		public Section(string course, string name, int openSeats, int waitList, params KeyValuePair<string, ClassSequence>[] classTypesAndSchedules)
		{
			Course = course;
			Name = name;
			OpenSeats = openSeats;
			WaitList = waitList;

			foreach (var myClass in classTypesAndSchedules)
			{
				ClassSequences.Add(myClass.Key, myClass.Value);
				if (!Instructors.Contains(myClass.Value.Instructor))
				{
					Instructors.Add(myClass.Value.Instructor);
				}
			}
		}

		public readonly Dictionary<string, ClassSequence> ClassSequences = new Dictionary<string, ClassSequence>();
		public readonly string Course;
		public string Name { get; set; }
		public int OpenSeats { get; set; }
		public int WaitList { get; set; }

		public List<string> Instructors = new List<string>();

		public override string ToString() => Name;

		public bool IsOverlap(Section another)
		{
			foreach (var myClass in ClassSequences.Values)
			{
				foreach (var anotherClass in another.ClassSequences.Values)
				{
					if (myClass.IsOverlap(anotherClass))
					{
						return true;
					}
				}
			}

			return false;
		}

		public bool IsAvailable(bool isOpenSectionOnly, bool doesShowFC, IEnumerable<ClassSpan> allClassSpanConstrains, IEnumerable<string> allInstructorsConstrains)
		{
			if (OpenSeats == 0 && isOpenSectionOnly)
			{
				return false;
			}

			if (Name[0] == 'F' && !doesShowFC)
			{
				return false;
			}

			if (Instructors.Intersect(allInstructorsConstrains).Count() != 0)
			{
				return false;
			}

			foreach (var @class in ClassSequences.Values)
			{
				foreach (var weekday in @class.Weekdays)
				{
					foreach (var classSpan in weekday.ClassSpansInSingleDay)
					{
						foreach (var anotherClassSpan in allClassSpanConstrains)
						{
							if (anotherClassSpan.IsOverlap(classSpan))
							{
								return false;
							}
						}
					}
				}
			}

			return true;
		}
	}
}
