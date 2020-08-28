using CourseScheduler.Avalonia.VMInfrastructures;
using CourseScheduler.Core.DataStrucures;
using System;
using System.Collections.Generic;
using System.Text;

namespace CourseScheduler.Avalonia.Model
{
	public static class DomainModel
	{
		public static readonly ObservableSet<Course> CourseSetCache = new ObservableSet<Course>();
		public static readonly ObservableSet<Course> CourseSet = new ObservableSet<Course>();
	}
}
