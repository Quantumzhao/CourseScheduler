using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Collections.Specialized;

namespace CourseSelection
{
	public partial class MainUI : UserControl
	{
		public MainUI()
		{
			InitializeComponent();
			DataContext = this;
		}

		private HashSet<Course> courseSet = new HashSet<Course>();
		private HashSet<Course> CourseSet_Cache = new HashSet<Course>();
		private Dictionary<string, string> semesterList = new Dictionary<string, string>();
		private Dictionary<string, bool> insList = new Dictionary<string, bool>();
		public static TimeDictionary TimePeriod { get; set; } = new TimeDictionary();

		private bool isOpenSecOnly => CB_OpenSectionOnly.IsChecked ?? false;
		private bool isShowFC => CB_ShowFC.IsChecked ?? false;

		private void Expander_Click(object sender, RoutedEventArgs e)
		{
			if (MUIB_NoInstructors.Visibility == Visibility.Collapsed)
			{
				MUIB_NoInstructors.Visibility = Visibility.Visible;
				MUIB_NoTime.Visibility = Visibility.Visible;
				CB_OpenSectionOnly.Visibility = Visibility.Visible;
				CB_ShowFC.Visibility = Visibility.Visible;
				AcademicYearPanel.Visibility = Visibility.Visible;
				Expander.IconData = Application.Current.Resources["Expand"] as Geometry;
			}
			else
			{
				MUIB_NoInstructors.Visibility = Visibility.Collapsed;
				MUIB_NoTime.Visibility = Visibility.Collapsed;
				CB_OpenSectionOnly.Visibility = Visibility.Collapsed;
				CB_ShowFC.Visibility = Visibility.Collapsed;
				AcademicYearPanel.Visibility = Visibility.Collapsed;
				Expander.IconData = Application.Current.Resources["Collapse"] as Geometry;
			}
		}

		private void MUIB_NoInstructors_Checked(object sender, RoutedEventArgs e)
		{
			LB_NoInstructors.Visibility = Visibility.Visible;
		}

		private void MUIB_NoInstructors_Unchecked(object sender, RoutedEventArgs e)
		{
			LB_NoInstructors.Visibility = Visibility.Collapsed;
		}

		private void MUIB_Add_Click(object sender, RoutedEventArgs e)
		{
			AddCourse(TB_CourseName.Text);

			UpdateView();
		}

		private void Semester_Loaded(object sender, RoutedEventArgs e)
		{
			//var now = DateTime.Now;
			//var year = now.Year;
			//var month = now.Month;
			//switch (month)
			//{
			//	case var exp when now >= new DateTime(year, 1, 27) && now < new DateTime(year, 5, 23):
			//		semesterList.Add(year.ToString() + " Spring", year.ToString() + "01");
			//		break;

			//	case var exp when now >= new DateTime(year, 5, 23) && now < new DateTime(year, 8, 21):
			//		semesterList.Add(year.ToString() + " Summer", year.ToString() + "05");
			//		break;

			//	case var exp when now >= new DateTime(year, 8, 21) && now < new DateTime(year, 12, 18);
			//		semesterList.Add(year.ToString() + " Fall", year.ToString() + "08");
			//		break;

			//	default:
			//		break;
			//}
			semesterList.Add("2019 Fall", "201908");
			semesterList.Add("2020 Spring", "202001");
			(sender as ComboBox).ItemsSource = semesterList.Keys;
		}

		private void UpdateView(object sender = null, RoutedEventArgs e = null)
		{
			ShowCourse();
		}

		private void ShowCourse()
		{
			MainGrid.Columns.Clear();
			int index = 0;
			foreach (var course in courseSet)
			{
				MainGrid.Columns.Add(
					new DataGridTextColumn()
					{
						Header = course.Name,
						Binding = new Binding("[" + index.ToString() + "]"),
						IsReadOnly = true,
						CanUserReorder = false						
					}
				);

				index++;
			}
			var combinations = Algorithm.GetPossibleCombinations(courseSet.ToArray(), isOpenSecOnly, isShowFC);
			Count.Text = combinations.Count.ToString();
			MainGrid.ItemsSource = combinations;
		}

		private void UpdateInsList()
		{
			var set = new HashSet<CheckBox>();
			foreach (var course in courseSet)
			{
				foreach (var ins in course.Instructors)
				{
					var cb = new CheckBox();
					cb.Content = ins.Key;
					cb.IsChecked = !ins.Value;
					cb.Checked += (s, e) =>
					{
						course.ExcludeInstructor(ins.Key, true);
						UpdateView();
					};
					cb.Unchecked += (s, e) =>
					{
						course.ExcludeInstructor(ins.Key, false);
						UpdateView();
					};
					set.Add(cb);
				}
			}
			LB_NoInstructors.ItemsSource = set;
		}

		private void AddCourse(string courseName)
		{
			courseName = courseName.ToUpper();
			if (CourseSet_Cache.Has(courseName))
			{
				courseSet.Add(CourseSet_Cache.Get(courseName));
			}
			else
			{
				string sem = semesterList[(string)Semester.SelectedItem];
				//string termID = null;
				//switch (sem)
				//{
				//	case "Spring":
				//		termID = year.ToString() + "01";
				//		break;

				//	case "Summer":
				//		termID = year.ToString() + "05";
				//		break;

				//	case "Fall":
				//		termID = year.ToString() + "08";
				//		break;

				//	case "Winter":
				//		termID = (year - 1).ToString() + "12";
				//		break;

				//	default:
				//		break;
				//}

				Course course = new Crawler() { TermID = sem }.GetCourse(courseName);
				courseSet.Add(course);
				CourseSet_Cache.Add(course);
				UpdateInsList();
			}
		}

		private void MUIB_NoInstructors_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			if (!(bool)e.NewValue)
			{
				LB_NoInstructors.Visibility = Visibility.Collapsed;
			}
		}

		private void Semester_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			var courseList = courseSet.Select(c => c.Name).ToArray();
			CourseSet_Cache = new HashSet<Course>();
			courseSet = new HashSet<Course>();
			foreach (var name in courseList)
			{
				AddCourse(name);
			}
			UpdateView();
		}

		private void MUIB_NoTime_Unchecked(object sender, RoutedEventArgs e)
		{
			LB_NoTime.Visibility = Visibility.Collapsed;
		}

		private void MUIB_NoTime_Checked(object sender, RoutedEventArgs e)
		{
			LB_NoTime.Visibility = Visibility.Visible;
		}

		private void MUIB_NoTime_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			if (!(bool)e.NewValue)
			{
				MUIB_NoTime.Visibility = Visibility.Collapsed;
			}
		}

		public class TimeDictionary : Dictionary<Schedule, bool>
		{
			public bool this[int index]
			{
				get
				{
					return this.ToArray()[index].Value;
				}
				set
				{
					this[this.ToArray()[index].Key] = value;
				}
			}
		}

		private void LB_NoTime_Initialized(object sender, EventArgs e)
		{
			TimePeriod.Add(new Schedule(new Time(6, 0), new Time(7, 0)), false);
			TimePeriod.Add(new Schedule(new Time(7, 0), new Time(8, 0)), false);
			TimePeriod.Add(new Schedule(new Time(8, 0), new Time(9, 0)), false);
			TimePeriod.Add(new Schedule(new Time(9, 0), new Time(10, 0)), false);
			TimePeriod.Add(new Schedule(new Time(10, 0), new Time(11, 0)), false);
			TimePeriod.Add(new Schedule(new Time(11, 0), new Time(12, 0)), false);
			TimePeriod.Add(new Schedule(new Time(12, 0), new Time(13, 0)), false);
			TimePeriod.Add(new Schedule(new Time(13, 0), new Time(14, 0)), false);
			TimePeriod.Add(new Schedule(new Time(17, 0), new Time(18, 0)), false);
			TimePeriod.Add(new Schedule(new Time(18, 0), new Time(19, 0)), false);
			TimePeriod.Add(new Schedule(new Time(19, 0), new Time(21, 0)), false);
		}
	}
}

