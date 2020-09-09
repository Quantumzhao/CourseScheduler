using System.Collections.Generic;
using System.Linq;
using UIEngine;

namespace CourseScheduler.Core.DataStrucures
{
	public class ClassSequence : IVisible
	{
		public ClassSequence(string instructor, Location location, params Weekday[] weekdays)
		{
			Instructor = instructor;
			Location = location;
			Weekdays = new HashSet<Weekday>(weekdays);
		}

		[Visible(nameof(Weekdays))]
		public HashSet<Weekday> Weekdays { get; }
		[Visible(nameof(Location))]
		public Location Location { get; }
		[Visible(nameof(Instructor))]
		public string Instructor { get; }

		public string Name => Header;
		public string Description => string.Empty;
		public string Header => "Instructors, Location and Weekdays";

		public bool IsOverlap(ClassSequence another)
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
