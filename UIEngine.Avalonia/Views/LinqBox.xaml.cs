using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace UIEngine.Avalonia.Views
{
	public class LinqBox : UserControl
	{
		public LinqBox()
		{
			this.InitializeComponent();
		}

		private void InitializeComponent()
		{
			AvaloniaXamlLoader.Load(this);
		}
	}
}
