using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace CourseScheduler.Avalonia.VMInfrastructures
{
	public class VMTuple<T1, T2> : INotifyPropertyChanged
	{
		public VMTuple(T1 e1, T2 e2)
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

		public static implicit operator VMTuple<T1, T2>(System.Tuple<T1, T2> tuple) => new VMTuple<T1, T2>(tuple.Item1, tuple.Item2);
	}
}
