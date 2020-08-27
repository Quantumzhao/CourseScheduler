using System.Collections.Generic;

namespace CourseScheduler.Core.DataStrucures
{
	public class Class
	{
		public Class(string instructor, Location location, params Weekday[] weekdays)
		{
			Instructor = instructor;
			Location = location;
			Weekdays = new HashSet<Weekday>(weekdays);
		}

		public readonly HashSet<Weekday> Weekdays;
		public readonly Location Location;
		public readonly string Instructor;

		public bool IsOverlap(Class another)
		{
			foreach (var myDay in Weekdays)
			{
				foreach (var anotherDay in another.Weekdays)
				{
					if (myDay.IsOverlap(anotherDay))
					{
						return true;
					}
				}
			}

			return false;
		}
	}
}
