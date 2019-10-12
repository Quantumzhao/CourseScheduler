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
using FirstFloor.ModernUI.Windows.Controls;

namespace CourseSelection
{
	public partial class MainUI : UserControl
	{
		public MainUI()
		{
			InitializeComponent();
			DataContext = this;

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

		private VMSet<Course> courseSet = new VMSet<Course>();
		private HashSet<Course> CourseSet_Cache = new HashSet<Course>();
		private Dictionary<string, string> semesterList = new Dictionary<string, string>();
		private Dictionary<string, bool> insList = new Dictionary<string, bool>();
		public Color[] GorgeousColors = new Color[10];
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
			var course = AddCourse(TB_CourseName.Text);

			DrawCoursePanel(course);

			UpdateView();
		}

		private void DrawCoursePanel(Course course)
		{
			if (course == null) return;

			Grid grid = new Grid();
			{
				grid.RowDefinitions.Add(new RowDefinition());
				grid.RowDefinitions.Add(new RowDefinition());
				grid.RowDefinitions.Add(new RowDefinition());
				grid.ColumnDefinitions.Add(new ColumnDefinition());
				grid.ColumnDefinitions.Add(new ColumnDefinition());
				grid.Margin = new Thickness(0, 0, 0, 10);

				var description = new TextBlock();
				{
					description.Text = course.FullName;
					description.Style = FindResource("Small") as Style;
					description.Margin = new Thickness(30, 0, 0, 0);
					Grid.SetColumn(description, 0);
					Grid.SetRow(description, 1);
				}
				grid.Children.Add(description);

				var header = new TextBlock();
				{
					header.Foreground = new SolidColorBrush(GetColor(course.Name));
					header.Style = FindResource("Heading2") as Style;
					header.Margin = new Thickness(30, 0, 0, 0);
					header.Text = course.Name;
					Grid.SetColumn(header, 0);
					Grid.SetRow(header, 0);
				}
				grid.Children.Add(header);

				var insList = new TextBlock();
				{
					insList.Margin = new Thickness(30, 0, 0, 0);
					insList.TextWrapping = TextWrapping.Wrap;
					Grid.SetColumnSpan(insList, 2);
					Grid.SetColumn(insList, 0);
					Grid.SetRow(insList, 2);
					StringBuilder stringBuilder = new StringBuilder("Instructors: ");
					var ins = course.Instructors.Keys.ToArray();
					stringBuilder.Append(ins[0]);
					for (int i = 1; i < ins.Length; i++)
					{
						stringBuilder.Append(", ");
						stringBuilder.Append(ins[i]);
					}
					insList.Text = stringBuilder.ToString();
				}
				grid.Children.Add(insList);

				var remove = new ModernButton();
				{
					remove.IconData = FindResource("Remove") as Geometry;
					Grid.SetColumn(remove, 1);
					Grid.SetRow(remove, 0);
					remove.Click += (s, be) =>
					{
						courseSet.Remove(course);
						SP_Course.Children.Clear();
						foreach (var _course in courseSet)
						{
							DrawCoursePanel(_course);
						}
						UpdateView();
						UpdateInsList();
					};
				}
				grid.Children.Add(remove);
			}
			SP_Course.Children.Add(grid);
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
			try
			{
				semesterList.Add("2019 Fall", "201908");
				semesterList.Add("2020 Spring", "202001");
			}
			catch
			{
			}
			(sender as ComboBox).ItemsSource = semesterList.Keys;
		}

		private void UpdateView(object sender = null, RoutedEventArgs e = null)
		{
			ShowCourse();
			MainCanvas.Children.Clear();
		}

		private void ShowCourse()
		{
			MainGrid.Columns.Clear();
			int index = 0;
			foreach (var course in courseSet)
			{
				var col = new System.Windows.Controls.DataGridTextColumn();
				{
					col.Header = course.Name;
					col.Binding = new Binding("[" + index.ToString() + "]");
					col.IsReadOnly = true;
					col.CanUserReorder = false;

					var style = new Style();
					{
						style.TargetType = typeof(DataGridCell);

						var setter = new Setter();
						{
							setter.Property = ToolTipProperty;

							var panel = new StackPanel();
							{
								panel.Orientation = Orientation.Vertical;

								var panel_openSeats = new StackPanel();
								{
									panel_openSeats.Orientation = Orientation.Horizontal;

									var openSeats = new TextBlock();
									{
										openSeats.Text = "Open Seats: ";
										openSeats.Style = FindResource("Emphasis") as Style;
									}
									panel_openSeats.Children.Add(openSeats);

									var num = new TextBlock();
									{
										num.SetBinding(TextBlock.TextProperty, new Binding("OpenSeats"));
									}
									panel_openSeats.Children.Add(num);

								}
								panel.Children.Add(panel_openSeats);

								var panel_waitList = new StackPanel();
								{
									panel_waitList.Orientation = Orientation.Horizontal;

									var waitList = new TextBlock();
									{
										waitList.Text = "Wait List: ";
										waitList.Style = FindResource("Emphasis") as Style;
									}
									panel_waitList.Children.Add(waitList);

									var num = new TextBlock();
									{
										num.SetBinding(TextBlock.TextProperty, new Binding("WaitList"));
									}
									panel_waitList.Children.Add(num);
								}
								panel.Children.Add(panel_waitList);
							}
							setter.Value = panel;
						};
						style.Setters.Add(setter);
					}
					col.CellStyle = style;
				}
				MainGrid.Columns.Add(col);

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

		private Course AddCourse(string courseName)
		{
			Course ret = null;
			courseName = courseName.ToUpper();

			if (!courseSet.Has(courseName))
			{
				if (CourseSet_Cache.Has(courseName))
				{
					ret = CourseSet_Cache.Get(courseName);
					courseSet.Add(ret);
				}
				else
				{
					string sem = semesterList[(string)Semester.SelectedItem];
					Course course = new Crawler() { TermID = sem }.GetCourse(courseName);
					if (course == null) return null;
					courseSet.Add(course);
					CourseSet_Cache.Add(course);
					ret = course;
					UpdateInsList();
				}
			}

			return ret;
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
			CourseSet_Cache.Clear();
			courseSet.Clear();
			SP_Course.Children.Clear();
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

		private Color GetColor(string course)
		{
			int i = courseSet.IndexOf(courseSet.Get(course));
			if (i != -1) return GorgeousColors[i];
			else return new Color();
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
							block.Stroke = new SolidColorBrush(GetColor(sections[i].Course));
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

		private void MainGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (MainGrid.SelectedItems.Count != 0)
			{
				ScheduleTimeTable(MainGrid.SelectedItems[0] as List<Section>);
			}
		}
	}

	public class VMSet<T> : HashSet<T>, INotifyCollectionChanged
	{
		private List<T> indices = new List<T>();
		public event NotifyCollectionChangedEventHandler CollectionChanged;

		public new bool Add(T newElement)
		{
			if (base.Add(newElement))
			{
				indices.Add(newElement);
				CollectionChanged?.Invoke(
					this,
					new NotifyCollectionChangedEventArgs(
						NotifyCollectionChangedAction.Add,
						newElement
					)
				);
				return true;
			}
			else
			{
				return false;
			}
		}

		public new bool Remove(T oldElement)
		{
			if (Contains(oldElement))
			{
				int index = indices.IndexOf(oldElement);
				indices.Remove(oldElement);
				base.Remove(oldElement);
				CollectionChanged?.Invoke(
					this,
					new NotifyCollectionChangedEventArgs(
						NotifyCollectionChangedAction.Remove,
						oldElement, index
					)
				);
				return true;
			}
			else
			{
				return false;
			}
		}

		public int IndexOf(T element)
		{
			if (element != null) return indices.IndexOf(element);
			else return -1;
		}
	}
}

