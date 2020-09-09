using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using CourseScheduler.Core.DataStrucures;
using System.Collections.ObjectModel;

namespace UIEngine.Avalonia.Views
{
	public class CourseBox : Box
	{
		public CourseBox()
		{
			this.InitializeComponent();
			this.FindControl<ListBox>("MainListBox").SelectionChanged += OnSelectionChanged;
			_NextControl = this.FindControl<ContentControl>("NextControl");
		}

		private void InitializeComponent()
		{
			AvaloniaXamlLoader.Load(this);
		}

		public static readonly StyledProperty<ObjectNode> CourseNodeProperty 
			= AvaloniaProperty.Register<CourseBox, ObjectNode>(nameof(CourseNode));
		public ObjectNode CourseNode 
		{
			get => GetValue(CourseNodeProperty);
			set => SetValue(CourseNodeProperty, value);
		}

		private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (e.AddedItems.Count != 0)
			{
				_NextControl.Content = new SectionBox() { SectionNode = e.AddedItems[0] as ObjectNode };
			}
		}
	}
}
