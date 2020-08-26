using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace CourseSchedulerAtAvalonia.Views
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
