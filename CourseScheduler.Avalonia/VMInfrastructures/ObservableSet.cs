using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace CourseScheduler.Avalonia.VMInfrastructures
{
	public class ObservableSet<T> : INotifyCollectionChanged
	{
		private readonly HashSet<T> _Collection = new HashSet<T>();

		public event NotifyCollectionChangedEventHandler CollectionChanged;

		public int Count => _Collection.Count;

		public bool Contains(T element) => _Collection.Contains(element);

		public bool Add(T element)
		{
			if (_Collection.Add(element))
			{
				CollectionChanged?.Invoke(this, 
					new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, element));
				return true;
			}
			else
			{
				return false;
			}
		}

		public void RaiseCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) 
			=> CollectionChanged?.Invoke(sender, e);

		public T[] ToArray() => _Collection.ToArray();
	}
}
