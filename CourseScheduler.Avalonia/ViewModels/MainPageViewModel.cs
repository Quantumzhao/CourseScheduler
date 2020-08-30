using System;
using System.Collections.Generic;
using System.Text;
using ReactiveUI;
using System.Reactive;
using System.Collections.ObjectModel;
using CourseScheduler.Avalonia.VMInfrastructures;
using System.Net.Http;
using CourseScheduler.Core.DataStrucures;
using CourseScheduler.Avalonia.Model;
using System.Linq;
using CourseScheduler.Core;
using System.Collections.Specialized;
using System.Drawing.Text;
using System.Threading.Tasks;
using System.Threading;
using Avalonia.Media;
using CourseScheduler.Avalonia.Views;

namespace CourseScheduler.Avalonia.ViewModels
{
	public class MainPageViewModel : ViewModelBase
	{
		public MainPageViewModel()
		{
			CourseSet.CollectionChanged += (s, e) =>
			{
				if (e.Action == NotifyCollectionChangedAction.Add)
				{
					ExpandInstructorsFilter(e.NewItems[0] as Course);
				}
				else
				{
					ShrinkInstructorsFilter(e.OldItems[0] as Course);
				}
			};

			CourseSet.CollectionChanged += (s, e) => UpdateCombinations();
			this.PropertyChanged += (s, e) => AnyThesePropertiesChanged(e.PropertyName);
			ObservableTuple<bool, string>.GenericHandler += (s, e) => UpdateCombinations();
			ObservableTuple<bool, ClassSpan>.GenericHandler += (s, e) => UpdateCombinations();

			TimePeriodsFilter = new ObservableCollection<ObservableTuple<bool, ClassSpan>>
			{
				(false,new ClassSpan(new Time(6, 0), new Time(7, 0))),
				(false,new ClassSpan(new Time(7, 0), new Time(8, 0))),
				(false,new ClassSpan(new Time(8, 0), new Time(9, 0))),
				(false,new ClassSpan(new Time(9, 0), new Time(10, 0))),
				(false,new ClassSpan(new Time(10, 0), new Time(11, 0))),
				(false,new ClassSpan(new Time(11, 0), new Time(12, 0))),
				(false,new ClassSpan(new Time(12, 0), new Time(13, 0))),
				(false,new ClassSpan(new Time(13, 0), new Time(14, 0))),
				(false,new ClassSpan(new Time(17, 0), new Time(18, 0))),
				(false,new ClassSpan(new Time(18, 0), new Time(19, 0))),
				(false,new ClassSpan(new Time(19, 0), new Time(21, 0)))
			};
		}

		private string _InputCourseName;
		public string InputCourseName
		{
			get => _InputCourseName;
			set => this.RaiseAndSetIfChanged(ref _InputCourseName, value);
		}

		private bool _IsOpenSectionOnly = false;
		public bool IsOpenSectionOnly
		{
			get => _IsOpenSectionOnly;
			set => this.RaiseAndSetIfChanged(ref _IsOpenSectionOnly, value);
		}

		private bool _DoesShowFC = false;
		public bool DoesShowFC
		{
			get => _DoesShowFC;
			set => this.RaiseAndSetIfChanged(ref _DoesShowFC, value);
		}

		private string _SelectedSemester = "Fall 2020";
		public string SelectedSemester
		{
			get => _SelectedSemester;
			set => this.RaiseAndSetIfChanged(ref _SelectedSemester, value);
		}

		private List<Section> _SelectedCombination;
		public List<Section> SelectedCombination 
		{ 
			get => _SelectedCombination;
			set => this.RaiseAndSetIfChanged(ref _SelectedCombination, value);
		}

		private List<List<Section>> _Combinations = new List<List<Section>>();
		public List<List<Section>> Combinations
		{
			get => _Combinations;
			set => this.RaiseAndSetIfChanged(ref _Combinations, value);
		}

		private IEnumerable<string> _CombinationPanelHeader = new List<string>();
		public IEnumerable<string> CombinationPanelHeader
		{
			get => _CombinationPanelHeader;
			set => this.RaiseAndSetIfChanged(ref _CombinationPanelHeader, value);
		}

		public ObservableCollection<ObservableTuple<bool, string>> InstructorsFilter { get; }
			= new ObservableCollection<ObservableTuple<bool, string>>();
		public ObservableCollection<ObservableTuple<bool, ClassSpan>> TimePeriodsFilter { get; }

		public Dictionary<string, string> SemesterList { get; } = new Dictionary<string, string>
		{
			{ "Spring 2020", "202001" },
			{ "Summer 2020", "202005" },
			{ "Fall 2020", "202008" }
		};

		public ObservableSet<Course> CourseSet => DomainModel.CourseSet;

		public void AddCourse() => CourseSet.Add(AddCourseToCourseSetAndCache());

		public void RemoveCourse(Course course) => CourseSet.Remove(course);

		public void UpdateCombinations()
		{
			var blockedTimePeriods = TimePeriodsFilter.Where(t => t.E1).Select(t => t.E2);
			var blockedInstructors = InstructorsFilter.Where(t => t.E1).Select(t => t.E2);
			Combinations = Algorithm.GetPossibleCombinations
				(CourseSet.ToArray(), blockedTimePeriods, blockedInstructors, IsOpenSectionOnly, DoesShowFC);
			CombinationPanelHeader = CourseSet.Select(c => c.Name);
		}

		public void Save() => MainWindowViewModel.Instance
			.ShowSaveWindow(SelectedCombination.Select(c => (c.Course, c.Name)).ToList());

		private Course AddCourseToCourseSetAndCache()
		{
			if (string.IsNullOrWhiteSpace(InputCourseName))
			{
				return null;
			}
			MainWindowViewModel.Instance.SetLoadingState(true);
			var courseName = InputCourseName.ToUpper();
			if (string.IsNullOrWhiteSpace(courseName))
			{
				return null;
			}

			Course course = null;

			if (!CourseSet.Has(courseName))
			{
				if (DomainModel.CourseSetCache.Has(courseName))
				{
					course = DomainModel.CourseSetCache.Get(courseName);
				}
				else
				{
						course = Crawler.GetCourse(courseName, SemesterList[SelectedSemester]);
						DomainModel.CourseSetCache.Add(course);
					try
					{
					}
					catch (Exception e)
					{
						//ShowMessage(e);
						MainWindowViewModel.Instance.SetLoadingState(false);
						return null;
					}
				}

				CourseSet.Add(course);
			}

			MainWindowViewModel.Instance.SetLoadingState(false);
			return course;
		}

		private void AnyThesePropertiesChanged(string propertyName)
		{
			switch (propertyName)
			{

				case nameof(DoesShowFC):
				case nameof(IsOpenSectionOnly):
					UpdateCombinations();
					break;

				case nameof(SelectedCombination):
					MainPageView.ScheduleTimeTable(SelectedCombination);
					break;

				default:
					break;
			}
		}

		private void ExpandInstructorsFilter(Course newCourse)
		{
			if (newCourse != null)
			{
				foreach (var ins in newCourse.Instructors)
				{
					InstructorsFilter.Add((false, ins));
				}
			}
		}

		private void ShrinkInstructorsFilter(Course oldCourse)
		{
			foreach (var ins in oldCourse.Instructors)
			{
				var tup = InstructorsFilter.First(t => t.E2 == ins);
				InstructorsFilter.Remove(tup);
			}
		}

		//private void ShowMessage(Exception exception)
		//{
		//	if (exception is HttpRequestException)
		//	{
		//		MessageBoxManager.GetMessageBoxStandardWindow(
		//			"Connection error",
		//			"Testudo may be under maintenance. \nPlease retry later.\n\n" + exception.Message
		//		).Show();
		//	}
		//	else if (exception is AggregateException)
		//	{
		//		foreach (var e in (exception as AggregateException).InnerExceptions)
		//		{
		//			ShowMessage(e);
		//		}
		//	}
		//	else if (exception is InvalidOperationException)
		//	{
		//		MessageBoxManager.GetMessageBoxStandardWindow(
		//			"Invalid input",
		//			"Unable to get the course info\nPlease check if there is any typo in course name. \n\n" + exception.Message
		//		).Show();
		//	}
		//	else
		//	{
		//		MessageBoxManager.GetMessageBoxStandardWindow(
		//			"Error",
		//			exception.Message
		//		).Show();
		//	}
		//}
	}
}
