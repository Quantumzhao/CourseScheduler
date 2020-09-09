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

		public CollectionNode SectionsNode { get; private set; }

		public void SetSectionsNode(ObjectNode node)
		{
			SectionsNode = node["Sections"] as CollectionNode;
		}

		private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
		{

		}
	}
}
