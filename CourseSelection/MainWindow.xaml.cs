using System;
using System.Linq;
using System.Text;
using System.Windows;
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
using System.Collections.Specialized;
using System.Windows.Controls.Primitives;

namespace CourseSelection
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private List<List<Section>> combinations = new List<List<Section>>();
		private VMSet<Course> courseSet = new VMSet<Course>();
		private HashSet<Course> courseCache = new HashSet<Course>();
		private VMSet<string> instructors = new VMSet<string>();
		private VMSet<string> excludedInstructors = new VMSet<string>();

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

			courseSet.CollectionChanged += (cs, ce) =>
			{
				switch (ce.Action)
				{
					case NotifyCollectionChangedAction.Add:
						foreach (var i in (ce.NewItems[0] as Course).Instructors)
						{
							instructors.Add(i);
						}
						break;

					case NotifyCollectionChangedAction.Remove:
						foreach (var i in (ce.OldItems[0] as Course).Instructors)
						{
							instructors.Remove(i);
						}
						break;
				}
			};
		}

		private void Button_Click_AddCourse(object sender, RoutedEventArgs e)
		{
			// init calc
			(sender as Button).IsEnabled = false;
			Log.UpdateStatus("Connecting to Testudo...");

			string courseName = Course_TextBox.Text.ToUpper();

			bool isOpenSectionOnly = IsOpenSectionOnly.IsChecked ?? false;
			bool isExcludeFC = IsExcludeFC.IsChecked ?? false;

			Course course;
			if (courseCache.Has(courseName))
			{
				course = courseCache.Get(courseName);
			}
			else
			{
				course = new Crawler().GetCourse(courseName,isOpenSectionOnly, isExcludeFC);
				courseCache.Add(course);
			}

			if (courseSet.Has(course)) goto Finalize;
			courseSet.Add(course);
			ShowSections();

			Grid grid = new Grid();
			{
				var col1 = new ColumnDefinition();
				col1.Width = GridLength.Auto;
				grid.ColumnDefinitions.Add(col1);

				var col2 = new ColumnDefinition();
				grid.ColumnDefinitions.Add(col2);

				var col3 = new ColumnDefinition();
				col3.Width = GridLength.Auto;
				grid.ColumnDefinitions.Add(col3);

				Label courseInfo = new Label();
				{
					courseInfo.Content = courseName;
					courseInfo.Foreground = new SolidColorBrush(GetColor(courseName));
					courseInfo.FontWeight = FontWeights.Bold;
					courseInfo.FontSize = 15;
					Grid.SetColumn(courseInfo, 0);
					courseSet.CollectionChanged += (cs, ce) =>
					{
						courseInfo.Foreground = new SolidColorBrush(GetColor(courseName));
					};
				}
				grid.Children.Add(courseInfo);

				Label instructor = new Label();
				{
					instructor.Content = course.Instructors.Concatenate();
					Grid.SetColumn(instructor, 1);
					instructor.VerticalAlignment = VerticalAlignment.Center;
					instructor.HorizontalAlignment = HorizontalAlignment.Center;
				}
				grid.Children.Add(instructor);

				Button remove_Button = new Button();
				{
					remove_Button.Background = Brushes.Transparent;
					remove_Button.BorderBrush = Brushes.Transparent;

					remove_Button.Content = "";
					remove_Button.FontFamily = new FontFamily("Segoe MDL2 Assets");

					remove_Button.Width = double.NaN;
					remove_Button.Height = double.NaN;
					remove_Button.HorizontalAlignment = HorizontalAlignment.Right;
					remove_Button.VerticalAlignment = VerticalAlignment.Center;

					remove_Button.Click += (bs, be) =>
					{
						List.Children.Remove(grid);
						courseSet.Remove(course);
						ShowSections();
					};
					Grid.SetColumn(remove_Button, 2);
				}
				grid.Children.Add(remove_Button);
			}
			List.Children.Add(grid);

			// fin calc
			Finalize:
			(sender as Button).IsEnabled = true;
			Log.UpdateStatus("Complete");
		}

		private void ShowSections()
		{
			CmbView.Columns.Clear();

			int index = 0;
			foreach (var course in courseSet)
			{
				CmbView.Columns.Add(
					new GridViewColumn()
					{
						Header = course.Name,
						DisplayMemberBinding = new Binding("[" + index.ToString() + "]")
					}
				);

				index++;
			}
			combinations = Algorithm.GetPossibleCombinations(courseSet.ToArray());
			Combinations.ItemsSource = combinations;
			Count.Content = combinations.Count;

			MainCanvas.Children.Clear();
			if (combinations.Count != 0)
			{
				ScheduleTimeTable(combinations[0]);
			}
		}

		private void Button_Click_Refresh(object sender, RoutedEventArgs e)
		{
			combinations = Algorithm.GetPossibleCombinations(courseSet.ToArray());
			Combinations.ItemsSource = null;
			Combinations.ItemsSource = combinations;
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

		private void Combinations_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			ScheduleTimeTable(Combinations.SelectedItems[0] as List<Section>);
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

		private void Instructor_ComboBox_Initialized(object sender, EventArgs e)
		{
			var me = sender as ComboBox;

			me.ItemsSource = instructors;
			excludedInstructors.CollectionChanged += (cs, ce) =>
			{
				switch (ce.Action)
				{
					case NotifyCollectionChangedAction.Add:
						if (me.SelectedItem == null)
						{
							break;
						}
						Label label = new Label();
						{
							label.Margin = new Thickness(0, 0, 10, 10);
							label.Background = new SolidColorBrush(Color.FromRgb(89, 117, 201));
							label.Foreground = Brushes.White;

							StackPanel stackPanel = new StackPanel();
							{
								stackPanel.Orientation = Orientation.Horizontal;

								TextBlock name = new TextBlock();
								{
									name.Text = me.SelectedItem as string;
								}
								stackPanel.Children.Add(name);

								Button remove = new Button();
								{
									remove.Background = Brushes.Transparent;
									remove.BorderBrush = Brushes.Transparent;
									remove.Foreground = Brushes.White;

									remove.Content = "";
									remove.FontFamily = new FontFamily("Segoe MDL2 Assets");

									remove.Width = double.NaN;
									remove.Height = double.NaN;
									remove.VerticalAlignment = VerticalAlignment.Center;
									remove.Margin = new Thickness(5, 0, 0, 0);
									remove.Padding = new Thickness(0, 1, 0, 0);

									remove.Click += (bs, be) =>
									{
										ExIns_Panel.Children.Remove(label);
										excludedInstructors.Remove(name.Text);
									};
								}
								stackPanel.Children.Add(remove);
							}
							label.Content = stackPanel;
						}
						ExIns_Panel.Children.Add(label);

						(me.ItemsSource as VMSet<string>).Remove(me.SelectedItem as string);
						break;

					case NotifyCollectionChangedAction.Remove:
						(me.ItemsSource as VMSet<string>).Add(ce.OldItems[0] as string);
						break;
				}
			};
		}

		private void Instructor_ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			excludedInstructors.Add((sender as ComboBox).SelectedItem as string);
		}

		private void StatusBar_Initialized(object sender, EventArgs e)
		{
			Log.StatusBar = sender as StatusBar;
		}

		private Color GetColor(string course)
		{
			int i = courseSet.IndexOf(courseSet.Get(course));
			if (i != -1) return GorgeousColors[i];
			else return new Color();
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

	public static class Log
	{
		private static StatusBar statusBar;
		public static StatusBar StatusBar
		{
			get => statusBar;
			set
			{
				statusBar = value;
				Status = StatusBar.FindName("Status") as TextBlock;
			}
		}
		public static TextBlock Status { get; private set; }
		public static void UpdateStatus(string status)
		{
			Status.Text = status;
		}
	}
}
