using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace UIEngine.Avalonia.Views
{
	public class ObjectBox : AbstractObjectBox
	{
		public ObjectBox()
		{
			this.InitializeComponent();
			_NextControl = this.FindControl<ContentControl>("NextControl");
		}

		private void InitializeComponent()
		{
			AvaloniaXamlLoader.Load(this);
		}

		public void InstantiateNext(Node nextNode)
		{
			if (nextNode is ObjectNode objectNode)
			{
				_NextControl.Content = ToSpecificBox(objectNode);
			}
		}
	}
}
