using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace UIEngine.Avalonia.Views
{
	public class SectionBox : ObjectBox
	{
		public SectionBox()
		{
			this.InitializeComponent();
		}

		private void InitializeComponent()
		{
			AvaloniaXamlLoader.Load(this);
		}

		public static readonly StyledProperty<ObjectNode> SectionNodeProperty 
			= AvaloniaProperty.Register<SectionBox, ObjectNode>(nameof(SectionNode));
		public ObjectNode SectionNode 
		{
			get => GetValue(SectionNodeProperty);
			set => SetValue(SectionNodeProperty, value);
		}
	}
}
