using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using CourseScheduler.Core.DataStrucures;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using CourseScheduler.Avalonia.Converters;
using CourseScheduler.Avalonia.Model;
using CourseScheduler.Core;

namespace CourseScheduler.Avalonia.Views
{
	public class MainPageView : UserControl
	{
		private static MainPageView _Instance;
		public MainPageView()
		{
			this.InitializeComponent();
			_Instance = this;
		}

		private void InitializeComponent()
		{
			AvaloniaXamlLoader.Load(this);
		}

		// This is magic, I don't want to touch it anymore
		public static void ScheduleTimeTable(List<Section> sections)
		{
			var canvas = _Instance.FindControl<Canvas>("MainCanvas");
			canvas.Children.Clear();
			if (sections == null)
			{
				return;
			}

			for (int i = 0; i < sections.Count; i++)
			{
				foreach (var myClass in sections[i].ClassSequences.Values)
				{
					foreach (var weekday in myClass.Weekdays)
					{
						foreach (var timePeriod in weekday.ClassSpansInSingleDay)
						{
							Rectangle block = new Rectangle();
							block.StrokeThickness = 5;
							block.Stroke = CourseToColorConverter.Singleton.Convert(DomainModel.CourseSet
								.Get(sections[i].Course), typeof(SolidColorBrush), null, null) as SolidColorBrush;
							block.Height = 5;
							Canvas.SetTop(block, 10.5 + 26 * ((int)weekday.Day - 1));

							/* Suppose a time table of width 24 * 60 
							 * (which is the total number of minutes in a day)
							 * Settle all of the classes in this table
							 * And then cut off the time period before 0600 and after 2300 
							 * (there is no class at this moment)
							 * And resize the table so that it can fit into display area*/
							double absWidth = timePeriod.SpanInMinute();
							double absLeft = timePeriod.Start.ToMinute();
							block.Width = canvas.Width / (17 * 60) * absWidth;
							Canvas.SetLeft(block, canvas.Width / (17 * 60) * (absLeft - 360));

							canvas.Children.Add(block);
						}
					}
				}
			}
		}
	}
}
