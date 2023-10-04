using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delegates.Observers
{
	public delegate void StackEventHandler(object eventData);
	public class StackOperationsLogger
	{
		private readonly StackObserver stackObserver = new StackObserver();
		public void SubscribeOn<T>(ObservableStack<T> stack)
		{
			stack.StackEvent += stackObserver.HandleStackEvent ;
		}

		public string GetLog()
		{
			return stackObserver.Log.ToString();
		}
	}

	public class StackObserver  
	{
		public StringBuilder Log = new StringBuilder();

		public void HandleStackEvent(object eventData)
		{
			Log.Append(eventData);
		}
	}

	public class ObservableStack<T> 
	{
		public event StackEventHandler StackEvent;
		private readonly List<T> data = new List<T>();

		public void Push(T obj)
		{
			data.Add(obj);
			StackEvent?.Invoke(new StackEventData<T> { IsPushed = true, Value = obj });
		}

		public T Pop()
		{
			if (data.Count == 0)
				throw new InvalidOperationException();
			var result = data[data.Count - 1];
			StackEvent?.Invoke(new StackEventData<T> { IsPushed = false, Value = result });
			return result;

		}
	}


}
