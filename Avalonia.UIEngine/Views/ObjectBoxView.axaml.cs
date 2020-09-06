using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.UIEngine.ViewModels;

namespace Avalonia.UIEngine.Views
{
	public class ObjectBoxView : UserControl
	{
		public ObjectBoxView()
		{
			this.InitializeComponent();
			//this.DataContextChanged += (s, e) => ((s as ObjectBoxView).DataContext as ObjectBoxViewModel).View = (s as ObjectBoxView);
		}

		private void InitializeComponent()
		{
			AvaloniaXamlLoader.Load(this);
		}
	}
}
