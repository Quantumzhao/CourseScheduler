using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace UIEngine.Avalonia.Views
{
	public class MethodBox : UserControl
	{
		public MethodBox()
		{
			this.InitializeComponent();
		}

		private void InitializeComponent()
		{
			AvaloniaXamlLoader.Load(this);
		}
	}
}
