using Avalonia;
using Avalonia.Controls;
using Avalonia.Data.Converters;
using Avalonia.Media;
using CourseScheduler.Core.DataStrucures;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace CourseScheduler.Avalonia.Converters
{
	public class CourseToColorConverter : IValueConverter
	{
		public static CourseToColorConverter Singleton { get; } = new CourseToColorConverter();
		public Dictionary<Course, SolidColorBrush> CourseColorMap { get; } = new Dictionary<Course, SolidColorBrush>();

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var course = value as Course;
			if (!CourseColorMap.ContainsKey(course))
			{
				if (!(Application.Current.FindResource($"ColorTag{CourseColorMap.Count}") is SolidColorBrush brush))
				{
					brush = new SolidColorBrush(GenerateNewColor());
				}

				CourseColorMap.Add(course, brush);
				return brush;
			}
			else
			{
				return CourseColorMap[course];
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}

		private Color GenerateNewColor()
		{
			return Color.FromUInt32(0xff000000 | (uint)new Random().Next(0, 0xffffff));
		}
	}
}
