using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CourseScheduler.Avalonia.IO
{
	public static class Communicator
	{
		public static List<Package> LoadFromFile(string url)
		{
			var src = File.ReadAllLines(url);
			var ret = new List<Package>();
			Package package;
			string name = string.Empty;
			List<(string, string)> pairs = new List<(string, string)>();

			for (int i = 0; i < src.Length; i++)
			{
				// is the end of package
				if (string.IsNullOrWhiteSpace(src[i]))
				{
					package = new Package(name, pairs);
					ret.Add(package);
					pairs = new List<(string, string)>();
				}
				// is a name
				else if (!src[i].Contains(" "))
				{
					name = src[i];
				}
				// is a pair
				else
				{
					var stringPair = src[i].Split();
					pairs.Add((stringPair[0], stringPair[1]));
				}
			}

			return ret;
		}

		public static void SaveToFile(Package package, string url)
		{
			var lines = new List<string>
			{
				package.Name
			};

			foreach (var pair in package.CourseSectionPairs)
			{
				lines.Add($"{pair.Item1} {pair.Item2}");
			}

			lines.Add("\n");
			File.WriteAllLines(url, lines);
		}
	}

	public class Package
	{
		public Package(string name, List<(string, string)> courseSectionPairs)
		{
			Name = name;
			CourseSectionPairs = courseSectionPairs;
		}

		public readonly string Name;

		public readonly List<(string, string)> CourseSectionPairs;
	}
}
