using CourseScheduler.Avalonia.VMInfrastructures;
using CourseScheduler.Core.DataStrucures;
using System;
using System.Collections.Generic;
using System.Text;
using UIEngine;

namespace CourseScheduler.Avalonia.Model
{
	public static class DomainModel
	{
		public static readonly ObservableSet<Course> CourseSetCache = new ObservableSet<Course>();
		[Visible(nameof(CourseSet), name:nameof(CourseSet))]
		public static ObservableSet<Course> CourseSet { get; } = new ObservableSet<Course>();
	}
}
