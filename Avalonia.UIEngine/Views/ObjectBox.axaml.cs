using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using UIEngine;

namespace Avalonia.UIEngine.Views
{
	public class ObjectBox : UserControl
	{
		public ObjectBox()
		{
			this.InitializeComponent();
		}

		private void InitializeComponent()
		{
			AvaloniaXamlLoader.Load(this);
		}

		public static readonly StyledProperty<ObjectNode> ObjectNodeProperty 
			= AvaloniaProperty.Register<ObjectBox, ObjectNode>(nameof(ObjectNode));
		public ObjectNode ObjectNode
		{
			get => GetValue(ObjectNodeProperty);
			set => SetValue(ObjectNodeProperty, value);
		}
	}
}
