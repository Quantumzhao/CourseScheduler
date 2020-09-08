using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace CourseScheduler.Avalonia.Views
{
	public class AdvancedView : UserControl
	{
		public AdvancedView()
		{
			this.InitializeComponent();
		}

		private void InitializeComponent()
		{
			AvaloniaXamlLoader.Load(this);
		}
	}
}
