using CourseScheduler.Avalonia.IO;
using CourseScheduler.Avalonia.VMInfrastructures;
using CourseScheduler.Core.DataStrucures;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive.PlatformServices;
using System.Text;

namespace CourseScheduler.Avalonia.ViewModels
{
	public class OtherViewModel : ViewModelBase
	{
		public OtherViewModel(MainPageViewModel mainPage)
		{
			_MainPage = mainPage;
			_MainPage.PropertyChanged += (s, e) =>
			{
				if (e.PropertyName == nameof(MainPageViewModel.SelectedCombination))
				{
					this.RaisePropertyChanged(nameof(SelectedConfiguration));
					this.RaisePropertyChanged(nameof(DoesShowSelectedConfiguration));
				}
			};

			LoadAllPackages();
		}

		private readonly MainPageViewModel _MainPage;

		private string _ConfigurationName;
		public string ConfigurationName
		{
			get => _ConfigurationName;
			set => this.RaiseAndSetIfChanged(ref _ConfigurationName, value);
		}

		private Package _SelectedPackage;
		public Package SelectedPackage 
		{ 
			get => _SelectedPackage;
			set => this.RaiseAndSetIfChanged(ref _SelectedPackage, value);
		}

		public bool DoesShowSelectedConfiguration => SelectedConfiguration != null;
		public List<Section> SelectedConfiguration => _MainPage.SelectedCombination;

		public ObservableCollection<Package> LoadedPackages { get; private set; }

		public void Save()
		{
			if (string.IsNullOrWhiteSpace(ConfigurationName) && ConfigurationName.Contains(' '))
			{
				MainWindowViewModel.ShowMessageBox("Configuration not saved",
					"Please make sure the name does not contain whitespaces");
				return;
			}

			var pkg = new Package(ConfigurationName, SelectedConfiguration
				.Select(s => new ObservableTuple<string, string>(s.Course, s.Name)).ToList());
			LoadedPackages.Add(pkg);

			if (Flush())
			{
				ConfigurationName = string.Empty;
				MainWindowViewModel.ShowMessageBox("Information", "Current configuration saved");
			}
		}

		public void RemoveSelection()
		{
			LoadedPackages.Remove(SelectedPackage);
			if (Flush())
			{
				MainWindowViewModel.ShowMessageBox("Information", "Successfully removed selection");
			}
		}

		public void Load()
		{
			_MainPage.CourseSet.Clear();
			SelectedPackage.CourseSectionPairs.ForEach(t => _MainPage.AddCourseToCourseSetAndCache(t.E1));

			var combinations = _MainPage.Combinations;
			var sections = SelectedPackage.CourseSectionPairs.Select(t => t.E2).ToList();
			List<Section> query = null;

			for (int j = 0; j < combinations.Count; j++)
			{
				var isSatisfied = true;

				for (int i = 0; i < combinations[j].Count; i++)
				{
					isSatisfied &= combinations[j][i].Name == sections[i];
				}

				if (isSatisfied)
				{
					query = combinations[j];
					goto RESUME;
				}
			}

			RESUME:

			if (query != null)
			{
				_MainPage.SelectedCombination = query;
				if (query.Any(s => s.OpenSeats == 0))
				{
					MainWindowViewModel.ShowMessageBox("Successful", "And all sections still have open seats");
				}
				else
				{
					MainWindowViewModel.ShowMessageBox("Information", "Some sections have no open seats left");
				}
			}
			else
			{
				MainWindowViewModel.ShowMessageBox("Sections not found", "Perhaps some sections are no longer available");
			}
		}

		private bool Flush()
		{
			try
			{
				Communicator.SaveToFile(LoadedPackages, "SavedCourses.txt");
				return true;
			}
			catch (IOException)
			{
				MainWindowViewModel.ShowMessageBox("Error", "Cannot access the saves");
				return false;
			}
			catch (Exception e)
			{
				MainWindowViewModel.ShowMessageBox("Error", e.Message);
				return false;
			}
		}

		private void LoadAllPackages() => LoadedPackages =
			new ObservableCollection<Package>(Communicator.LoadFromFile("SavedCourses.txt"));
	}
}
