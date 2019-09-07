using System;
using System.Linq;
using System.Text;
using System.Windows;
using System.Threading;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Navigation;
using System.Collections.Generic;
using System.Windows.Media.Imaging;
using System.Collections.ObjectModel;

namespace CourseSelection
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private List<List<Section>> combinations = new List<List<Section>>();
		private HashSet<string> courseSet = new HashSet<string>();

		public Color[] GorgeousColors = new Color[10];

		public MainWindow()
		{
			InitializeComponent();

			GorgeousColors[0] = Color.FromRgb(96, 96, 183);
			GorgeousColors[1] = Color.FromRgb(96, 183, 145);
			GorgeousColors[2] = Color.FromRgb(255, 171, 70);
			GorgeousColors[3] = Color.FromRgb(255, 113, 77);
			GorgeousColors[4] = Color.FromRgb(194, 150, 214);
			GorgeousColors[5] = Color.FromRgb(174, 229, 235);
			// Not so gorgeous underneath ↓
			GorgeousColors[6] = Color.FromRgb(224, 224, 224);
			GorgeousColors[7] = Color.FromRgb(192, 192, 192);
			GorgeousColors[8] = Color.FromRgb(160, 160, 160);
			GorgeousColors[9] = Color.FromRgb(128, 128, 128);
		}

		private void Button_Click_AddCourse(object sender, RoutedEventArgs e)
		{
			//TextBlock textBlock = new TextBlock();
			//string text = Course_TextBox.Text.ToUpper();
			//textBlock.Text = text;
			//if (!courseSet.Add(text)) return;

			//DockPanel dockPanel = new DockPanel();
			//dockPanel.Children.Add(textBlock);
			//dockPanel.Margin = new Thickness(0, 10, 0, 0);


			//Button remove = new Button();
			//remove.Content = "Remove";
			//remove.Click += (button_sender, button_e) => 
			//{
			//	List.Children.Remove(dockPanel);
			//	courseSet.Remove(text);
			//};
			//dockPanel.Children.Add(remove);
			//DockPanel.SetDock(remove, Dock.Right);
			//remove.HorizontalAlignment = HorizontalAlignment.Right;

			//List.Children.Add(dockPanel);

			string courseName = Course_TextBox.Text;

			bool isOpenSectionOnly = IsOpenSectionOnly.IsChecked ?? false;
			bool isExcludeFC = IsExcludeFC.IsChecked ?? false;

			Course course = new Crawler().GetCourse(courseName,isOpenSectionOnly, isExcludeFC);

			Expander CourseInfo = new Expander();
			CourseInfo.Margin = new Thickness(10, 10, 10, 0);
			CourseInfo.Header = courseName;

			var stackPanel = new StackPanel();
			{
				var subtitle = new TextBlock();
				{
					subtitle.Text = course.FullName;
				}
				stackPanel.Children.Add(subtitle);

				var sectionPanel = new StackPanel();
				{
					sectionPanel.Margin = new Thickness(0, 20, 0, 0);
					foreach (var section in course.Sections)
					{
						var sectionInfo = new StackPanel();
						{

						}
						sectionPanel.Children.Add(sectionInfo);
					}
				}
				stackPanel.Children.Add(sectionPanel);
			}
			CourseInfo.Content = stackPanel;
		}

		private void Button_Click_Refresh(object sender, RoutedEventArgs e)
		{
			CmbView.Columns.Clear();

			List<Course> newCourses = new List<Course>();
			var crawler = new Crawler();
			bool isOpenSectionOnly = IsOpenSectionOnly.IsChecked ?? false;
			bool isExcludeFC = IsExcludeFC.IsChecked ?? true;

			int index = 0;
			foreach (var course in courseSet)
			{
				var result = crawler.GetCourse(course, isOpenSectionOnly, isExcludeFC);
				if (result == null) return;
				try
				{
					newCourses.Add(result);
				}
				catch (Exception)
				{
					MessageBox.Show(
						"Course Initialization Error",
						"Unable to add a course", 
						MessageBoxButton.OK, 
						MessageBoxImage.Error
					);
					return;
				}

				CmbView.Columns.Add(
					new GridViewColumn() {
						Header = course,
						DisplayMemberBinding = new Binding("[" + index.ToString() + "]")
					}
				);

				index++;
			}
			combinations = Algorithm.GetPossibleCombinations(newCourses.ToArray());
			Combinations.ItemsSource = combinations;

			if (combinations.Count != 0)
			{
				ScheduleTimeTable(combinations[0]);
			}
			else
			{
				MainCanvas.Children.Clear();
			}
		}

		private void ScheduleTimeTable(List<Section> sections)
		{
			MainCanvas.Children.Clear();

			for (int i = 0; i < sections.Count; i++)
			{
				foreach (var myClass in sections[i].Classes.Values)
				{
					foreach (var weekday in myClass.Weekdays)
					{
						foreach (var timePeriod in weekday.TimePeriod)
						{
							Rectangle block = new Rectangle();
							block.StrokeThickness = 5;
							block.Stroke = new SolidColorBrush(GorgeousColors[i]);
							block.Height = 5;
							Canvas.SetTop(block, 10.5 + 26 * ((int)weekday.Day - 1));

							/* Suppose a time table of width 24 * 60 
							 * (which is the total number of minutes in a day)
							 * Settle all of the classes in this table
							 * And then cut off the time period before 0600 and after 2300 
							 * (there is no class at this moment)
							 * And resize the table so that it can fit into display area
							 * (and also keep a margin of 30 at the left and 10 at the right)*/
							double absWidth = timePeriod.SpanInMinute();
							double absLeft = timePeriod.Start.ToMinute();
							block.Width = MainCanvas.ActualWidth / (17 * 60) * absWidth;
							Canvas.SetLeft(block, MainCanvas.ActualWidth / (17 * 60) * (absLeft - 360));

							MainCanvas.Children.Add(block);
						}
					}
				}
			}
		}

		private void Combinations_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			try
			{
				ScheduleTimeTable(Combinations.SelectedItems[0] as List<Section>);
			}
			catch { }
		}

		private void MainCanvas_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			double ratio = e.NewSize.Width / (e.PreviousSize.Width);
			if (ratio < 0) ratio = 1;
			foreach (UIElement rectangle in MainCanvas.Children)
			{
				if (rectangle is Rectangle)
				{
					Canvas.SetLeft(rectangle, ratio * Canvas.GetLeft(rectangle));
					((Rectangle)rectangle).Width *= ratio;
				}
			}
		}
	}
}
