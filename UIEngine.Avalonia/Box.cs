using Avalonia;
using Avalonia.Controls;
using System;
using System.Collections.Generic;
using System.Text;
using UIEngine.Avalonia.Views;

namespace UIEngine.Avalonia
{
	public abstract class Box : UserControl
	{
		protected ContentControl _NextControl;

		public static StyledProperty<Node> NextNodeProperty 
			= AvaloniaProperty.Register<Box, Node>(nameof(NextNode));
		public Node NextNode
		{
			get => GetValue(NextNodeProperty);
			set => SetValue(NextNodeProperty, value);
		}
	}
}
