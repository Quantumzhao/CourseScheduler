using CourseScheduler.Core.DataStrucures;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using UIEngine;
using UIEngine.Core;

namespace CourseScheduler.Core
{
	public static class Crawler
	{
		public static event Action<string, string> WarningHandler;

		public static async Task<Course> GetCourse(string courseName, string termID)
		{
			Course ret;
			URL url = new URL(courseName, termID);

			var httpClient = new HttpClient();
			string html = await httpClient.GetStringAsync(url);
			var htmlDocument = new HtmlDocument();
			htmlDocument.LoadHtml(html);

			string fullName;

			List<HtmlNode> divs = null;
			divs = htmlDocument
				.DocumentNode
				.Descendants("div")
				.Where(node => node.GetAttributeValue("id", "") == courseName)
				.First()
				.Descendants("div")
				.Where(node => node.GetAttributeValue("class", "").Contains("section delivery"))
				.ToList();

			fullName = htmlDocument
				.DocumentNode
				.Descendants("span")
				.Where(node => node.GetAttributeValue("class", "") == "course-title")
				.First()
				.InnerText;
			List<Section> sections = new List<Section>();
			// Enumerate through each section
			foreach (var div in divs)
			{
				string name = div
					.Descendants("input").First()
					.ChildAttributes("value").First()
					.Value;

				var rows = div
					.Descendants("div")
					.Single(node => node.GetAttributeValue("class", "") == "class-days-container")
					.Elements("div").ToList();

				var span = div.Descendants("span");

				var instructors = span
					.Where(node => node.GetAttributeValue("class", "") == "section-instructor")
					.Select(node => node.InnerText)
					.ToList();

				int openSeats = int.Parse(span
					.Single(node => node.GetAttributeValue("class", "") == "open-seats-count")
					.InnerText);
				int waitlist = int.Parse(span
					.Where(node => node.GetAttributeValue("class", "") == "waitlist-count")
					.First()
					.InnerText);

				string instructor = instructors[0];
				for (int i = 1; i < instructors.Count(); i++)
				{
					instructor += ", ";
					instructor += instructors[i];
				}

				Dictionary<string, ClassSequence> classes = new Dictionary<string, ClassSequence>();
				// Enumerate through each class of the section
				foreach (var row in rows)
				{
					HtmlNode dayTimeGroup = row.Descendants("div").FirstOrDefault(n => n.Attributes["class"].Value.Contains("section-day-time-group"));
					if (dayTimeGroup == null)
					{
						WarningHandler?.Invoke("Warning", "Some class times are inaccessible, please contact department or instructor for details");
						break;
					}

					var enumerator = dayTimeGroup.Descendants("span").GetEnumerator();
					if (!enumerator.MoveNext()) break;

					if (enumerator.Current.GetAttributeValue("class", "") == "elms-class-message")
					{
						WarningHandler?.Invoke("Warning", "Part of the class times are only accessible through ELMS, which are skipped by the scheduler");
						break;
					}

					var positions = row.Descendants("span");
					string building = positions
						.Where(node => node.GetAttributeValue("class", "") == "building-code")
						.FirstOrDefault()?.InnerText ?? string.Empty;
					string room = positions
						.Where(node => node.GetAttributeValue("class", "") == "class-room")
						.FirstOrDefault()?.InnerText ?? string.Empty;
					Location location = new Location(building, room);

					List<DayOfWeek> days = enumerator.Current.InnerText.ToDayOfWeek();

					enumerator.MoveNext();
					Time start = enumerator.Current.InnerText;

					enumerator.MoveNext();
					Time end = enumerator.Current.InnerText;

					List<Weekday> weekdays = new List<Weekday>();
					foreach (var day in days)
					{
						weekdays.Add(new Weekday(day, new ClassSpan(start, end)));
					}

					IEnumerable<HtmlNode> possibleName = row.Descendants("div")
						.Where(node => node.GetAttributeValue("class", "") == "two columns");
					string className;
					if (possibleName.Count() != 0)
					{
						className = possibleName.First().Descendants("span").First().InnerText;
					}
					else if (!classes.ContainsKey("Lecture"))
					{
						className = "Lecture";
					}
					else
					{
						className = "Alt. Lecture";
					}

					classes.Add(className, new ClassSequence(instructor, location, weekdays.ToArray()));
				}

				sections.Add(new Section(courseName, name, openSeats, waitlist, classes.ToArray()));
			}

			ret = new Course(courseName, fullName, sections.ToArray());
			return ret;
		}

		[Visible(nameof(GetCourseSync))]
		public static Course GetCourseSync(string courseName, string termID) => GetCourse(courseName, termID).Result;

		private class URL
		{
			public URL(string courseID, string termID)
			{
				_CourseID = courseID;
				_TermID = termID;
			}

			private readonly string _CourseID;
			private readonly string _TermID;

			public static implicit operator string(URL url)
			{
				return string.Format(
					"https://app.testudo.umd.edu/soc/search?courseId={0}&sectionId=&termId={1}&_openSectionsOnly=on&creditCompare=&credits=&courseLevelFilter=ALL&instructor=&_facetoface=on&_blended=on&_online=on&courseStartCompare=&courseStartHour=&courseStartMin=&courseStartAM=&courseEndHour=&courseEndMin=&courseEndAM=&teachingCenter=ALL&_classDay1=on&_classDay2=on&_classDay3=on&_classDay4=on&_classDay5=on",
					url._CourseID,
					url._TermID);
			}
		}
	}
}
