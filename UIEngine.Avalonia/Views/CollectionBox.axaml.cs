using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using UIEngine;

namespace UIEngine.Avalonia.Views
{
	public class CollectionBox : ObjectBox
	{
		public CollectionBox()
		{
			this.InitializeComponent();
		}

		private void InitializeComponent()
		{
			AvaloniaXamlLoader.Load(this);
		}

		public static readonly StyledProperty<CollectionNode> ItemsProperty 
			= AvaloniaProperty.Register<CollectionBox, CollectionNode>(nameof(Items));
		public CollectionNode Items 
		{
			get => GetValue(ItemsProperty);
			set => SetValue(ItemsProperty, value);
		}
	}
}
