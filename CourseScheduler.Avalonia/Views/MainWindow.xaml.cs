using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Markup.Xaml;
using Avalonia.ProgressRing;
using CourseScheduler.Avalonia.ViewModels;
using System;
using System.Collections.Generic;

namespace CourseScheduler.Avalonia.Views
{
	public class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
		}

		private void InitializeComponent()
		{
			AvaloniaXamlLoader.Load(this);
		}
	}
}
