using System;
using System.Collections.Generic;
using System.Text;

namespace CourseScheduler.Core.DataStrucures
{
	public class Course
	{
		public Course(string name, string fullName, params Section[] sections)
		{
			Name = name;
			FullName = fullName;
			Sections = new HashSet<Section>(sections);
		}

		public string Name { get; }
		public string FullName { get; }
		public HashSet<Section> Sections { get; }
		public string InstructorsString => Instructors.Concatenate();
		public List<string> Instructors
		{
			get
			{
				var list = new List<string>();
				foreach (var section in Sections)
				{
					foreach (var ins in section.Instructors)
					{
						if (!list.Contains(ins))
						{
							list.Add(ins);
						}
					}
				}
				return list;
			}
		}
	}
}
