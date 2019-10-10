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

namespace CourseSelection
{
	/// <summary>
	/// Interaction logic for MainUI.xaml
	/// </summary>
	public partial class MainUI : UserControl
	{
		public MainUI()
		{
			InitializeComponent();
		}

		private HashSet<Course> courseSet = new HashSet<Course>();
		private HashSet<Course> CourseSet_Cache = new HashSet<Course>();

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
				SemesterPanel.Visibility = Visibility.Visible;
				Expander.IconData = Application.Current.Resources["Expand"] as Geometry;
			}
			else
			{
				MUIB_NoInstructors.Visibility = Visibility.Collapsed;
				MUIB_NoTime.Visibility = Visibility.Collapsed;
				CB_OpenSectionOnly.Visibility = Visibility.Collapsed;
				CB_ShowFC.Visibility = Visibility.Collapsed;
				AcademicYearPanel.Visibility = Visibility.Collapsed;
				SemesterPanel.Visibility = Visibility.Collapsed;
				Expander.IconData = Application.Current.Resources["Collapse"] as Geometry;
			}
		}

		private void MUIB_NoInstructors_Checked(object sender, RoutedEventArgs e)
		{
			LB_NoInstructors.Visibility = Visibility.Visible;
			var insList = new List<string>();
			foreach (var course in courseSet)
			{
				foreach (var ins in course.Instructors)
				{
					insList.Add(ins);
				}
			}
			LB_NoInstructors.ItemsSource = insList;
		}

		private void MUIB_NoInstructors_LostFocus(object sender, RoutedEventArgs e)
		{
			if (!LB_NoInstructors.IsFocused)
			{
				MUIB_NoInstructors.IsChecked = false;
			}
		}

		private void MUIB_NoInstructors_Unchecked(object sender, RoutedEventArgs e)
		{
			LB_NoInstructors.Visibility = Visibility.Collapsed;
		}

		private void MUIB_Add_Click(object sender, RoutedEventArgs e)
		{
			string courseName = TB_CourseName.Text.ToUpper();
			if (CourseSet_Cache.Has(courseName))
			{
				courseSet.Add(CourseSet_Cache.Get(courseName));
			}
			else
			{
				int year = (int)Year.SelectedItem;
				string sem = Semester.SelectedItem as string;
				string termID = null;
				switch (sem)
				{
					case "Spring":
						termID = year.ToString() + "01";
						break;

					case "Summer":
						termID = year.ToString() + "05";
						break;

					case "Fall":
						termID = year.ToString() + "08";
						break;

					case "Winter":
						termID = (year - 1).ToString() + "12";
						break;

					default:
						break;
				}

				Course course = new Crawler() { TermID = termID }.GetCourse(courseName);
				courseSet.Add(course);
				CourseSet_Cache.Add(course);
			}

			ShowCourse();
		}

		private void Year_Loaded(object sender, RoutedEventArgs e)
		{
			var yearList = new List<int>();
			var now = DateTime.Now.Year;
			yearList.Add(now - 1);
			yearList.Add(now);
			yearList.Add(now + 1);
			(sender as ComboBox).ItemsSource = yearList;
		}

		private void Semester_Loaded(object sender, RoutedEventArgs e)
		{
			var semesterList = new List<string>();
			semesterList.Add("Spring");
			semesterList.Add("Summer");
			semesterList.Add("Fall");
			semesterList.Add("Winter");
			(sender as ComboBox).ItemsSource = semesterList;
		}

		private void ShowCourse()
		{

		}
	}
}

