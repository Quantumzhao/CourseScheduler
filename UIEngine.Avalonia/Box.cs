using Avalonia;
using Avalonia.Controls;
using System;
using System.Collections.Generic;
using System.Text;
using UIEngine.Avalonia.Views;
using UIEngine.Nodes;

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

		protected static Box ToSpecificBox(ObjectNode nextNode)
		{
			Box ret;

			//if (nextNode.IsTypeOf<Course>())
			//{
			//	ret = new CourseBox() { CourseNode = nextNode };
			//}
			//else 
			if (nextNode is CollectionNode collectionNode)
			{
				ret = new CollectionBox() { Items = collectionNode };
			}
			else
			{
				ret = new ObjectBox() { ObjectNode = nextNode };
			}

			return ret;
		}
	}
}
