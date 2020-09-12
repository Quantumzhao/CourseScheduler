using System;
using System.Collections.Generic;
using System.Text;
using UIEngine;
using UIEngine.Core;

namespace CourseScheduler.Core.DataStrucures
{
	public class Weekday
	{
		public Weekday(DayOfWeek day, params ClassSpan[] timePeriod)
		{
			Day = day;
			ClassSpansInSingleDay = new HashSet<ClassSpan>(timePeriod);
		}

		[Visible(nameof(Day))]
		public DayOfWeek Day { get; }
		[Visible(nameof(ClassSpansInSingleDay))]
		public HashSet<ClassSpan> ClassSpansInSingleDay { get; }

		public bool IsOverlap(Weekday another)
		{
			if (Day != another.Day)
			{
				return false;
			}

			foreach (var myClassSpan in ClassSpansInSingleDay)
			{
				foreach (var anotherClassSpan in another.ClassSpansInSingleDay)
				{
					//foreach (var time in MainUI.TimePeriod.Keys)
					//{
					//	if (MainUI.TimePeriod[time] && time.IsOverlap(anotherClassSpan))
					//	{
					//		return true;
					//	}
					//}

					if (myClassSpan.IsOverlap(anotherClassSpan))
					{
						return true;
					}
				}
			}

			return false;
		}
	}
}
