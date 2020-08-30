using System;
using System.Collections.Generic;
using System.Text;
using System.Reactive;
using ReactiveUI;
using CourseScheduler.Avalonia.VMInfrastructures;
using CourseScheduler.Core.DataStrucures;
using CourseScheduler.Avalonia.Model;
using CourseScheduler.Avalonia.IO;

namespace CourseScheduler.Avalonia.ViewModels
{
	public class MainWindowViewModel : ViewModelBase
	{
		public MainWindowViewModel()
		{
			Instance = this;
		}

		private List<(string, string)> _DataToBeSaved;

		public static MainWindowViewModel Instance { get; private set; }

		public ViewModelBase _BasicsVM = new MainPageViewModel();
		public ViewModelBase BasicsVM
		{
			get => _BasicsVM;
			set => this.RaiseAndSetIfChanged(ref _BasicsVM, value);
		}

		private bool _DoesShowProgRing;
		public bool DoesShowProgRing
		{
			get => _DoesShowProgRing;
			set => this.RaiseAndSetIfChanged(ref _DoesShowProgRing, value);
		}

		private bool _DoesShowWindowMask;
		public bool DoesShowWindowMask
		{
			get => _DoesShowWindowMask;
			set => this.RaiseAndSetIfChanged(ref _DoesShowWindowMask, value);
		}

		private bool _DoesShowPopup;
		public bool DoesShowPopup
		{
			get => _DoesShowPopup;
			set => this.RaiseAndSetIfChanged(ref _DoesShowPopup, value);
		}

		private bool _DoesShowInputBox;
		public bool DoesShowInputBox
		{
			get => _DoesShowInputBox;
			set => this.RaiseAndSetIfChanged(ref _DoesShowInputBox, value);
		}

		private string _InputBoxText;
		public string InputBoxText
		{
			get => _InputBoxText;
			set => this.RaiseAndSetIfChanged(ref _InputBoxText, value);
		}

		private string _InputBoxPrompt = "";
		public string InputBoxPrompt 
		{ 
			get => _InputBoxPrompt;
			set => this.RaiseAndSetIfChanged(ref _InputBoxPrompt, value);
		}

		public void SetLoadingState(bool b)
		{
			DoesShowProgRing = b;
			DoesShowWindowMask = b;
		}

		public void SetSaveWindowStatus(bool b)
		{
			DoesShowWindowMask = b;
			DoesShowPopup = b;
		}

		public void ShowSaveWindow(List<(string, string)> dataToBeSaved)
		{
			DoesShowPopup = true;
			DoesShowInputBox = true;
			_DataToBeSaved = dataToBeSaved;
		}

		public void SubmitSaving()
		{
			if (DoesShowInputBox)
			{
				if (!InputBoxText.Contains(" "))
				{
					var pkg = new Package(InputBoxText, _DataToBeSaved);
					Communicator.SaveToFile(pkg, "/Assets/SavedCourses.txt");
					InputBoxPrompt = string.Empty;
				}
				else
				{
					InputBoxPrompt = "Name string cannot contain whitespace";
				}
			}

			SetSaveWindowStatus(false);
		}
	}
}
