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

		public readonly string Name;
		public readonly string FullName;
		public readonly HashSet<Section> Sections;
		public Dictionary<string, bool> Instructors
		{
			get
			{
				var dic = new Dictionary<string, bool>();
				foreach (var section in Sections)
				{
					foreach (var ins in section.Instructors)
					{
						if (!dic.ContainsKey(ins.Key))
						{
							dic.Add(ins.Key, ins.Value);
						}
					}
				}
				return dic;
			}
		}

		public void ExcludeInstructor(string name, bool isExclude)
		{
			foreach (var section in Sections)
			{
				section.Instructors[name] = !isExclude;
			}
		}
	}
}
