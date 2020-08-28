using System;
using System.Collections.Generic;
using System.Text;
using ReactiveUI;
using System.Reactive;
using System.Collections.ObjectModel;
using CourseScheduler.Avalonia.VMInfrastructures;

namespace CourseScheduler.Avalonia.ViewModels
{
	public class MainPageViewModel : ViewModelBase
	{
		private string _CourseName;
		public string CourseName
		{
			get => _CourseName;
			set => this.RaiseAndSetIfChanged(ref _CourseName, value);
		}

		private bool _IsOpenSectionOnly = false;
		public bool IsOpenSectionOnly
		{
			get => _IsOpenSectionOnly;
			set => this.RaiseAndSetIfChanged(ref _IsOpenSectionOnly, value);
		}

		private bool _DoesShowFC = false;
		public bool DoesShowFC
		{
			get => _DoesShowFC;
			set => this.RaiseAndSetIfChanged(ref _DoesShowFC, value);
		}

		public ObservableSet<string> Instructors { get; } = new ObservableSet<string>();

		public void AddCourse()
		{

		}
	}
}
