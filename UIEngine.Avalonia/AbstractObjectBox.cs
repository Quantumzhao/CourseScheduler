using Avalonia;
using System;
using System.Collections.Generic;
using System.Text;

namespace UIEngine.Avalonia
{
	public abstract class AbstractObjectBox : Box
	{
		public static readonly StyledProperty<ObjectNode> ObjectNodeProperty
			= AvaloniaProperty.Register<AbstractObjectBox, ObjectNode>(nameof(ObjectNode));
		public ObjectNode ObjectNode
		{
			get => GetValue(ObjectNodeProperty);
			set => SetValue(ObjectNodeProperty, value);
		}
	}
}
