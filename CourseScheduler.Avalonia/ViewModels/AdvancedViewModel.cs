using System;
using System.Collections.Generic;
using System.Text;
using UIEngine;
using CourseScheduler.Avalonia.Model;
using CourseScheduler.Core;
using CourseScheduler.Avalonia.VMInfrastructures;
using CourseScheduler.Core.DataStrucures;
using ReactiveUI;
using UIEngine.Nodes;

namespace CourseScheduler.Avalonia.ViewModels
{
	public class AdvancedViewModel : ViewModelBase
	{
		public AdvancedViewModel()
		{
			Dashboard.ImportEntryObjects(typeof(DomainModel), typeof(Crawler), typeof(Algorithm));
			CourseSetNode = Dashboard.GetRootNode<CollectionNode>(nameof(DomainModel.CourseSet));
		}

		private CollectionNode _CourseSetNode;
		public CollectionNode CourseSetNode
		{
			get => _CourseSetNode;
			set => this.RaiseAndSetIfChanged(ref _CourseSetNode, value);
		}
	}
}
