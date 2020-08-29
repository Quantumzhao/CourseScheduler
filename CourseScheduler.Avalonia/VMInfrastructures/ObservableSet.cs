using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace CourseScheduler.Avalonia.VMInfrastructures
{
	public class ObservableSet<T> : INotifyCollectionChanged, IEnumerable<T>, IEnumerable
	{
		private readonly HashSet<T> _Collection = new HashSet<T>();

		public event NotifyCollectionChangedEventHandler CollectionChanged;

		public int Count => _Collection.Count;

		public bool Contains(T element) => _Collection.Contains(element);

		public void RaiseCollectionChanged(NotifyCollectionChangedAction action, object changedObject) 
			=> CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(action, changedObject));

		public bool Add(T element)
		{
			if (_Collection.Add(element))
			{
				RaiseCollectionChanged(NotifyCollectionChangedAction.Add, element);
				return true;
			}
			else
			{
				return false;
			}
		}

		public bool Remove(T element)
		{
			if (_Collection.Remove(element))
			{
				RaiseCollectionChanged(NotifyCollectionChangedAction.Remove, element);
				return true;
			}
			else
			{
				return false;
			}
		}

		public IEnumerator<T> GetEnumerator() => _Collection.GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => _Collection.GetEnumerator();
	}
}
