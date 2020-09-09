using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System;
using System.Collections.Generic;
using System.Text;

namespace UIEngine.Avalonia
{
	public abstract class AbstractCollectionBox : Box
	{
		public static readonly StyledProperty<CollectionNode> ItemsProperty
			= AvaloniaProperty.Register<AbstractCollectionBox, CollectionNode>(nameof(Items));
		public CollectionNode Items
		{
			get => GetValue(ItemsProperty);
			set => SetValue(ItemsProperty, value);
		}

		protected void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (e.AddedItems.Count != 0)
			{
				_NextControl.Content = null;
				NextNode = e.AddedItems[0] as Node;
			}
		}
	}
}
