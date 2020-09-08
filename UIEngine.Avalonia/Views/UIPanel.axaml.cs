using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System.Collections.Generic;
using UIEngine;

namespace UIEngine.Avalonia.Views
{
	public class UIPanel : UserControl
	{
		public UIPanel()
		{
			this.InitializeComponent();
			//DataContext = this;
		}

		private void InitializeComponent()
		{
			AvaloniaXamlLoader.Load(this);
		}

		public static readonly StyledProperty<IEnumerable<Node>> ItemsProperty 
			= AvaloniaProperty.Register<UIPanel, IEnumerable<Node>>(nameof(Items));
		public IEnumerable<Node> Items
		{
			get => GetValue(ItemsProperty);
			set => SetValue(ItemsProperty, value);
		}

		public static readonly StyledProperty<Node> StartProperty
			= AvaloniaProperty.Register<UIPanel, Node>(nameof(Start));
		public Node Start
		{
			get => GetValue(StartProperty);
			set => SetValue(StartProperty, value);
		}

		public static readonly StyledProperty<bool> IsExpandedProperty
			= AvaloniaProperty.Register<UIPanel, bool>(nameof(IsExpanded));
		public bool IsExpanded
		{
			get => GetValue(IsExpandedProperty);
			set => SetValue(IsExpandedProperty, value);
		}
	}
}
