using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace CourseScheduler.Avalonia.Views
{
	public class OtherView : UserControl
	{
		public OtherView()
		{
			this.InitializeComponent();
		}

		private void InitializeComponent()
		{
			AvaloniaXamlLoader.Load(this);
		}
	}
}
