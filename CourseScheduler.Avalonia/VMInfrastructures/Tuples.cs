using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace CourseScheduler.Avalonia.VMInfrastructures
{
	public class ObservableTuple<T1, T2> : INotifyPropertyChanged
	{
		public ObservableTuple(T1 e1, T2 e2)
		{
			E1 = e1;
			E2 = e2;
		}

		public event PropertyChangedEventHandler PropertyChanged;

		private T1 _E1;
		public T1 E1 
		{ 
			get => _E1;
			set
			{
				if (_E1 == null || !_E1.Equals(value))
				{
					_E1 = value;
					PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(E1)));
				}
			}
		}

		private T2 _E2;
		public T2 E2
		{
			get => _E2;
			set
			{
				if (E2 == null || !_E2.Equals(value))
				{
					_E2 = value;
					PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(E2)));
				}
			}
		}

		public static implicit operator ObservableTuple<T1, T2>((T1, T2) tuple) => new ObservableTuple<T1, T2>(tuple.Item1, tuple.Item2);
	}
}
