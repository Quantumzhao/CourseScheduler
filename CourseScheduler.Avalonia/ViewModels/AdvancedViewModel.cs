using System;
using System.Collections.Generic;
using System.Text;
using UIEngine;
using CourseScheduler.Avalonia.Model;
using CourseScheduler.Core;

namespace CourseScheduler.Avalonia.ViewModels
{
	public class AdvancedViewModel
	{
		public AdvancedViewModel()
		{
			Dashboard.ImportEntryObjects(typeof(DomainModel), typeof(Crawler), typeof(Algorithm));
		}
	}
}
