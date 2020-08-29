using System;
using System.Collections.Generic;
using System.Text;
using ReactiveUI;
using System.Reactive;
using System.Collections.ObjectModel;
using CourseScheduler.Avalonia.VMInfrastructures;
using System.Net.Http;
using MessageBox.Avalonia;
using CourseScheduler.Core.DataStrucures;
using CourseScheduler.Avalonia.Model;
using System.Linq;
using CourseScheduler.Core;
using System.Collections.Specialized;
using System.Drawing.Text;
using System.Threading.Tasks;
using System.Threading;
using Avalonia.Media;

namespace CourseScheduler.Avalonia.ViewModels
{
	public class MainPageViewModel : ViewModelBase
	{
		public MainPageViewModel()
		{
		}

		private string _CourseName;
		public string CourseName
		{
			get => _CourseName;
			set => this.RaiseAndSetIfChanged(ref _CourseName, value);
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

		public ObservableCollection<ObservableTuple<bool, string>> InstructorsFilter { get; } 
			= new ObservableCollection<ObservableTuple<bool, string>>();
		public ObservableCollection<ObservableTuple<bool, ClassSpan>> TimePeriodsFilter { get; }
			= new ObservableCollection<ObservableTuple<bool, ClassSpan>>
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

		public Dictionary<string, string> SemesterList { get; } = new Dictionary<string, string>
		{
			{ "Spring 2020", "202001" },
			{ "Summer 2020", "202005" },
			{ "Fall 2020", "202008" }
		};

		public ObservableSet<Course> CourseSet => DomainModel.CourseSet;

		public async void AddCourse()
		{
			async Task<Course> AddCourseToCourseSetAndCache()
			{
				var courseName = CourseName?.ToUpper();
				if (string.IsNullOrWhiteSpace(courseName))
				{
					return null;
				}

				Course course;

				if (!CourseSet.Has(courseName))
				{
					if (DomainModel.CourseSetCache.Has(courseName))
					{
						course = DomainModel.CourseSetCache.Get(courseName);
					}
					else
					{
						try
						{
							course = await Crawler.GetCourse(courseName, SemesterList[SelectedSemester]);
							DomainModel.CourseSetCache.Add(course);
						}
						catch (Exception e)
						{
							ShowMessage(e);
							return null;
						}
					}

					CourseSet.Add(course);
					CourseSet.RaiseCollectionChanged(NotifyCollectionChangedAction.Add, course);
					return course;
				}
				else
				{
					return null;
				}
			}

			void UpdateInstrctorsFilter(Course newCourse)
			{
				if (newCourse != null)
				{
					foreach (var ins in newCourse.Instructors)
					{
						InstructorsFilter.Add((false, ins));
					}
				}
			}

			var newCourse = await AddCourseToCourseSetAndCache();
			UpdateInstrctorsFilter(newCourse);

			var blockedTimePeriods = TimePeriodsFilter.Where(t => t.E1).Select(t => t.E2);
			var blockedInstructors = InstructorsFilter.Where(t => t.E1).Select(t => t.E2);
			var combinations = Algorithm.GetPossibleCombinations
				(CourseSet.ToArray(), blockedTimePeriods, blockedInstructors, IsOpenSectionOnly, DoesShowFC);
		}

		private void ShowMessage(Exception exception)
		{
			if (exception is HttpRequestException)
			{
				MessageBoxManager.GetMessageBoxStandardWindow(
					"Connection error",
					"Testudo may be under maintenance. \nPlease retry later.\n\n" + exception.Message
				).Show();
			}
			else if (exception is AggregateException)
			{
				foreach (var e in (exception as AggregateException).InnerExceptions)
				{
					ShowMessage(e);
				}
			}
			else if (exception is InvalidOperationException)
			{
				MessageBoxManager.GetMessageBoxStandardWindow(
					"Invalid input",
					"Unable to get the course info\nPlease check if there is any typo in course name. \n\n" + exception.Message
				).Show();
			}
			else
			{
				MessageBoxManager.GetMessageBoxStandardWindow(
					"Error",
					exception.Message
				).Show();
			}
		}
	}
}
