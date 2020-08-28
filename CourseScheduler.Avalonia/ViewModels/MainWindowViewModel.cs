using System;
using System.Collections.Generic;
using System.Text;
using System.Reactive;
using ReactiveUI;
using CourseScheduler.Avalonia.VMInfrastructures;
using CourseScheduler.Core.DataStrucures;
using CourseScheduler.Avalonia.Model;

namespace CourseScheduler.Avalonia.ViewModels
{
	public class MainWindowViewModel : ViewModelBase
	{
		public ViewModelBase _BasicsVM = new MainPageViewModel();
		public ViewModelBase BasicsVM
		{
			get => _BasicsVM;
			set => this.RaiseAndSetIfChanged(ref _BasicsVM, value);
		}
	}
}
