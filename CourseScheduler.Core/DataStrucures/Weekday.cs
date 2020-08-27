using System;
using System.Collections.Generic;
using System.Text;

namespace CourseScheduler.Core.DataStrucures
{
	public class Weekday
	{
		public Weekday(DayOfWeek day, params Schedule[] timePeriod)
		{
			Day = day;
			TimePeriod = new HashSet<Schedule>(timePeriod);
		}

		public readonly DayOfWeek Day;
		public readonly HashSet<Schedule> TimePeriod;

		public bool IsOverlap(Weekday another)
		{
			if (Day != another.Day)
			{
				return false;
			}

			foreach (var mySchedule in TimePeriod)
			{
				foreach (var anotherSchedule in another.TimePeriod)
				{
					foreach (var time in MainUI.TimePeriod.Keys)
					{
						if (MainUI.TimePeriod[time] && time.IsOverlap(anotherSchedule))
						{
							return true;
						}
					}

					if (mySchedule.IsOverlap(anotherSchedule))
					{
						return true;
					}
				}
			}

			return false;
		}
	}
}
