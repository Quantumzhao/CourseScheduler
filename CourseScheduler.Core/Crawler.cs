using CourseScheduler.Core.DataStrucures;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace CourseScheduler.Core
{
	public class Crawler
	{
		//public string TermID = "202001";
		//public bool IsExcludeFC = true;

		public async Task<Course> GetCourse(string courseName, string termID)
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
				.Where(node => node.GetAttributeValue("class", "")
				.Equals("section delivery-f2f")).ToList();

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

				Dictionary<string, Class> classes = new Dictionary<string, Class>();
				// Enumerate through each class of the section
				foreach (var row in rows)
				{
					HtmlNode dayTimeGroup = row.Descendants("div").First();
					var enumerator = dayTimeGroup.Descendants("span").GetEnumerator();
					if (!enumerator.MoveNext()) break;

					var positions = row.Descendants("span");
					string building = positions
						.Where(node => node.GetAttributeValue("class", "") == "building-code")
						.Single().InnerText;
					string room = positions
						.Where(node => node.GetAttributeValue("class", "") == "class-room")
						.Single().InnerText;
					Location location = new Location(building, room);

					List<DayOfWeek> days = enumerator.Current.InnerText.ToDayOfWeek();

					enumerator.MoveNext();
					Time start = enumerator.Current.InnerText;

					enumerator.MoveNext();
					Time end = enumerator.Current.InnerText;

					List<Weekday> weekdays = new List<Weekday>();
					foreach (var day in days)
					{
						weekdays.Add(new Weekday(day, new Schedule(start, end)));
					}

					IEnumerable<HtmlNode> possibleName = row.Descendants("div")
						.Where(node => node.GetAttributeValue("class", "") == "two columns");
					string className;
					if (possibleName.Count() != 0)
					{
						className = possibleName.First().Descendants("span").First().InnerText;
					}
					else
					{
						className = "Lecture";
					}

					classes.Add(className, new Class(instructor, location, weekdays.ToArray()));
				}

				sections.Add(new Section(courseName, name, openSeats, waitlist, classes.ToArray()));
			}

			ret = new Course(courseName, fullName, sections.ToArray());
			return ret;
		}

		private class URL
		{
			public URL(string courseID, string termID)
			{
				_CourseID = courseID;
				_TermID = termID;
				//_SectionID = section;
			}

			private readonly string _CourseID;
			private readonly string _TermID;
			//private readonly string _OpenSection = string.Empty;
			//private readonly string _SectionID;

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
