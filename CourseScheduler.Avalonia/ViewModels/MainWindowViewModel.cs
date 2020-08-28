using System;
using System.Collections.Generic;
using System.Text;
using System.Reactive;
using ReactiveUI;

namespace CourseScheduler.Avalonia.ViewModels
{
	public class MainWindowViewModel : ViewModelBase
	{
		public ViewModelBase _Content = new MainPageViewModel();
		public ViewModelBase Content
		{
			get => _Content;
			set => this.RaiseAndSetIfChanged(ref _Content, value);
		}
	}
}
