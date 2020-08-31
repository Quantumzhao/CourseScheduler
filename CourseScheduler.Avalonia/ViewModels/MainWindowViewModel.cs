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
			_OtherVM = new OtherViewModel(_BasicsVM);
		}

		public static MainWindowViewModel Instance { get; private set; }

		public MainPageViewModel _BasicsVM = new MainPageViewModel();
		public MainPageViewModel BasicsVM
		{
			get => _BasicsVM;
			set => this.RaiseAndSetIfChanged(ref _BasicsVM, value);
		}

		private OtherViewModel _OtherVM;
		public OtherViewModel OtherVM
		{
			get => _OtherVM;
			set => this.RaiseAndSetIfChanged(ref _OtherVM, value);
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

		private bool _DoesShowMsgBox;
		public bool DoesShowMsgBox
		{
			get => _DoesShowMsgBox;
			set => this.RaiseAndSetIfChanged(ref _DoesShowMsgBox, value);
		}

		private string _MsgBoxTitle;
		public string MsgBoxTitle
		{
			get => _MsgBoxTitle;
			set => this.RaiseAndSetIfChanged(ref _MsgBoxTitle, value);
		}

		private string _MsgBoxText;
		public string MsgBoxText 
		{ 
			get => _MsgBoxText;
			set => this.RaiseAndSetIfChanged(ref _MsgBoxText, value);
		}

		public void SetLoadingState(bool b)
		{
			DoesShowProgRing = b;
			DoesShowWindowMask = b;
		}

		public static void ShowMessageBox(string title, string text)
		{
			Instance.DoesShowMsgBox = true;
			Instance.DoesShowWindowMask = true;
			Instance.MsgBoxTitle = title;
			Instance.MsgBoxText = text;
		}

		public void Ok()
		{
			DoesShowMsgBox = false;
			DoesShowWindowMask = false;
		}
	}
}
