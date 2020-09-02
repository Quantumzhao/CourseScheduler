using Avalonia;
using Avalonia.Controls.Shapes;
using Avalonia.Platform;
using CourseScheduler.Avalonia.VMInfrastructures;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CourseScheduler.Avalonia.IO
{
	public static class Communicator
	{
		private static string _BaseUrl = AppDomain.CurrentDomain.BaseDirectory;

		public static List<Package> LoadFromFile(string url)
		{
			url = _BaseUrl + url;
			if (!File.Exists(url))
			{
				File.Create(url).Close();
			}
			var src = File.ReadAllLines(url).ToList();
			src.Add("\n");

			var ret = new List<Package>();
			Package package;
			string name = string.Empty;
			List<ObservableTuple<string, string>> pairs = new List<ObservableTuple<string, string>>();

			for (int i = 0; i < src.Count; i++)
			{
				// is the end of package
				if (string.IsNullOrWhiteSpace(src[i]))
				{
					package = new Package(name, pairs);
					ret.Add(package);
					pairs = new List<ObservableTuple<string, string>>();
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

		public static void SaveToFile(IEnumerable<Package> packages, string url)
		{
			var lines = new List<string>();

			foreach (var pkg in packages)
			{
				lines.Add(pkg.Name);

				foreach (var pair in pkg.CourseSectionPairs)
				{
					lines.Add($"{pair.E1} {pair.E2}");
				}

				lines.Add(string.Empty);
			}

			lines.RemoveAt(lines.Count - 1);

			using (var stream = File.Create(_BaseUrl + url))
			{
				using (var writer = new StreamWriter(stream))
				{
					for (int i = 0; i < lines.Count; i++)
					{
						writer.WriteLine(lines[i]);
					}
				}
			}
		}
	}
}
