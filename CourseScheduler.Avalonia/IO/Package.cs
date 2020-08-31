using CourseScheduler.Avalonia.VMInfrastructures;
using System;
using System.Collections.Generic;
using System.Text;

namespace CourseScheduler.Avalonia.IO
{
	public class Package
	{
		public Package(string name, List<ObservableTuple<string, string>> courseSectionPairs)
		{
			Name = name;
			CourseSectionPairs = courseSectionPairs;
		}

		public string Name { get; }

		public List<ObservableTuple<string, string>> CourseSectionPairs { get; }
	}
}
