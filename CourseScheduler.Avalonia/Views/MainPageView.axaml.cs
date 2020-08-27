using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace CourseScheduler.Avalonia.Views
{
	public class MainPageView : UserControl
	{
		public MainPageView()
		{
			this.InitializeComponent();
		}

		private void InitializeComponent()
		{
			AvaloniaXamlLoader.Load(this);
		}
	}
}
