using System;
using System.Linq;
using System.Net.Http;
using HtmlAgilityPack;
using System.Collections.Generic;
using System.Windows;
using System.Text;
using System.Threading.Tasks;

namespace CourseScheduler
{
	public static class Algorithm
	{
		public static List<List<Section>> GetPossibleCombinations(Course[] dataset, 
			bool isOpenSecOnly = false, bool isShowFC = false)
		{
			if (dataset.Length == 0) return new List<List<Section>>();

			List<List<Section>> combinations = new List<List<Section>>();
			Section.IsShowFC = isShowFC;
			Section.IsOpenSectionOnly = isOpenSecOnly;
			foreach (var section in dataset[0].Sections)
			{
				if (section.isAvailable)
				{
					combinations.Add(new List<Section>() { section });
				}
			}
			for (int i = 1; i < dataset.Length; i++)
			{
				UpdateCombinations(ref combinations, dataset[i]);
			}

			return combinations;
		}

		private static void UpdateCombinations(ref List<List<Section>> possibilities, Course newCourse)
		{
			var newPossibilities = new List<List<Section>>();

			foreach (var newSection in newCourse.Sections)
			{
				for (int i = 0; i < possibilities.Count; i++)
				{
					if (newSection.isAvailable && !possibilities[i].IsOverlap(newSection))
					{
						List<Section> sections = new List<Section>(possibilities[i]);
						sections.Add(newSection);
						newPossibilities.Add(sections);
					}
				}
			}
			possibilities = newPossibilities;
		}

		private static bool IsOverlap(this List<Section> possibility, Section newSection)
		{
			foreach (var section in possibility)
			{
				if (section.IsOverlap(newSection))
				{
					return true;
				}
			}

			return false;
		}

		public static List<DayOfWeek> ToDayOfWeek(this string daysofweek)
		{
			List<DayOfWeek> ret = new List<DayOfWeek>();

			for (int i = 0; i < daysofweek.Length; i++)
			{
				switch (daysofweek[i])
				{
					case 'M':
						ret.Add(DayOfWeek.Monday);
						break;

					case 'W':
						ret.Add(DayOfWeek.Wednesday);
						break;

					case 'F':
						ret.Add(DayOfWeek.Friday);
						break;

					case 'T':
						break;

					case 'u':
						ret.Add(DayOfWeek.Tuesday);
						break;

					case 'h':
						ret.Add(DayOfWeek.Thursday);
						break;

					default:
						break;
				}
			}

			return ret;
		}

		public static bool AddString(this HashSet<string> set, string s)
		{
			foreach (var item in set)
			{
				if (item.Equals(s))
				{
					return false;
				}
			}
			
			return set.Add(s); ;
		}

		public static string Concatenate(this IEnumerable<string> stringList)
		{
			var list = stringList.ToArray();
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < list.Length; i++)
			{
				if (i != 0) stringBuilder.Append(',');
				stringBuilder.Append(list[i]);
			}

			return stringBuilder.ToString();
		}

		public static bool Has(this HashSet<Course> courses, Course newCourse)
		{
			if (courses.Where(c => c.Name == newCourse.Name).Count() != 0) return true;
			else return false;
		}
		public static bool Has(this HashSet<Course> courses, string newCourse)
		{
			if (courses.Where(c => c.Name == newCourse).Count() != 0) return true;
			else return false;
		}
		public static Course Get(this HashSet<Course> courses, string newCourse)
		{
			try
			{
				return courses.Where(c => c.Name == newCourse).Single();
			}
			catch
			{
				return null;
			}
		}
	}

	public class Crawler
	{
		public string TermID = "202001";
		//public bool IsExcludeFC = true;

		public async Task<Course> GetCourse(string courseName)
		{
			Course ret;
			URL url = new URL(courseName, TermID);

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
			public URL(string courseID, string termID, string section = "")
			{
				CourseID = courseID;
				TermID = termID;
				sectionID = section;
			}

			private string CourseID;
			private string TermID;
			private string OpenSection = "";
			private string sectionID;

			public static implicit operator string(URL url)
			{
				return string.Format(
					"https://app.testudo.umd.edu/soc/search?courseId={0}&sectionId=&termId={1}&_openSectionsOnly=on&creditCompare=&credits=&courseLevelFilter=ALL&instructor=&_facetoface=on&_blended=on&_online=on&courseStartCompare=&courseStartHour=&courseStartMin=&courseStartAM=&courseEndHour=&courseEndMin=&courseEndAM=&teachingCenter=ALL&_classDay1=on&_classDay2=on&_classDay3=on&_classDay4=on&_classDay5=on", 
					url.CourseID, 
					url.TermID, 
					url.OpenSection);
			}
		}
	}

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

	public class Section
	{
		public Section(string course, string name, int openSeats, int waitList, params KeyValuePair<string, Class>[] classes)
		{
			Course = course;
			Name = name;
			OpenSeats = openSeats;
			WaitList = waitList;

			foreach (var myClass in classes)
			{
				Classes.Add(myClass.Key, myClass.Value);
				if (!Instructors.ContainsKey(myClass.Value.Instructor))
				{
					Instructors.Add(myClass.Value.Instructor, true);
				}
			}
		}

		public static bool IsOpenSectionOnly;
		public static bool IsShowFC;

		public bool isAvailable
		{
			get
			{
				if (OpenSeats == 0 && IsOpenSectionOnly)
				{
					return false;
				}

				if (Name[0] == 'F' && !IsShowFC)
				{
					return false;
				}

				foreach (var @class in Classes.Values)
				{
					if (!Instructors[@class.Instructor])
					{
						return false;
					}
				}

				foreach (var @class in Classes.Values)
				{
					foreach (var weekday in @class.Weekdays)
					{
						foreach (var time in weekday.TimePeriod)
						{
							foreach (var item in MainUI.TimePeriod.Keys)
							{
								if (MainUI.TimePeriod[item] && item.IsOverlap(time))
								{
									return false;
								}
							}
						}
					}
				}

				return true;
			}
		}

		public readonly Dictionary<string, Class> Classes = new Dictionary<string, Class>();
		public readonly string Course;
		public string Name { get; set; }
		public int OpenSeats { get; set; }
		public int WaitList { get; set; }

		public Dictionary<string, bool> Instructors = new Dictionary<string, bool>();

		public bool IsOverlap(Section another)
		{
			foreach (var myClass in Classes.Values)
			{
				foreach (var anotherClass in another.Classes.Values)
				{
					if (myClass.IsOverlap(anotherClass))
					{
						return true;
					}
				}
			}

			return false;
		}

		public override string ToString()
		{
			return Name;
		}
	}

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

	public class Schedule
	{
		public Schedule(Time start, Time end)
		{
			Start = start;
			End = end;
		}

		public readonly Time Start;
		public readonly Time End;

		public int SpanInMinute()
		{
			return End.ToMinute() - Start.ToMinute();
		}

		public bool IsOverlap(Schedule another)
		{
			if (another.Start > Start && another.Start < End)
			{
				return true;
			}
			else if (another.End > Start && another.End < End)
			{
				return true;
			}
			else if (another.Start <= Start && another.End >= End)
			{
				return true;
			}
			else
			{
				return false;
			}
		}
	}

	public struct Time
	{
		public Time(string time)
		{
			if (time.Contains(':'))
			{
				string[] hm = time.Split(':');
				int tempHour = int.Parse(hm[0]);
				int minute = int.Parse(hm[1].Substring(0, 2));
				if (hm[1][2] == 'p' && tempHour != 12)
				{
					tempHour += 12;
				}
				Hour = tempHour;
				Minute = minute;
			}
			else
			{
				Hour = int.Parse(time.Substring(0, 2));
				Minute = int.Parse(time.Substring(2, 2));
			}
		}
		public Time(int hour, int minute)
		{
			Hour = hour;
			Minute = minute;
		}

		public static implicit operator Time(string time)
		{
			return new Time(time);
		}

		public int Hour;
		public int Minute;

		public int ToMinute()
		{
			return Hour * 60 + Minute;
		}

		public static bool operator <(Time time1, Time time2)
		{
			if (time1.Hour < time2.Hour)
			{
				return true;
			}
			else if (time1.Hour > time2.Hour)
			{
				return false;
			}
			else
			{
				if (time1.Minute < time2.Minute)
				{
					return true;
				}
				else
				{
					return false;
				}
			}
		}
		public static bool operator >(Time time1, Time time2)
		{
			if (time1.Hour > time2.Hour)
			{
				return true;
			}
			else if (time1.Hour < time2.Hour)
			{
				return false;
			}
			else
			{
				if (time1.Minute > time2.Minute)
				{
					return true;
				}
				else
				{
					return false;
				}
			}
		}
		public static bool operator <=(Time time1, Time time2)
		{
			return time1 < time2 || time1 == time2;
		}
		public static bool operator >=(Time time1, Time time2)
		{
			return time1 > time2 || time1 == time2;
		}

		public static bool operator ==(Time time1, Time time2)
		{
			return time1.Hour == time2.Hour && time1.Minute == time2.Minute;
		}

		public static bool operator !=(Time time1, Time time2)
		{
			return time1.Hour != time2.Hour || time1.Minute != time2.Minute;
		}

		public static Time operator +(Time time, int minute)
		{
			int tempMin = time.Minute + minute;
			if (tempMin / 60 == 1)
			{
				return new Time(time.Hour + 1, tempMin - 60);
			}
			else
			{
				return new Time(time.Hour, tempMin);
			}
		}

		public override bool Equals(object obj)
		{
			return obj is Time time &&
				   Hour == time.Hour &&
				   Minute == time.Minute;
		}

		public override int GetHashCode()
		{
			var hashCode = 510674192;
			hashCode = hashCode * -1521134295 + Hour.GetHashCode();
			hashCode = hashCode * -1521134295 + Minute.GetHashCode();
			return hashCode;
		}
	}

	public struct Location
	{
		public Location(string building, string room)
		{
			Building = building;
			Room = room;
		}

		public string Building;
		public string Room;
	}
}
