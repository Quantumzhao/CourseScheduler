using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schedule.Code
{
	public class Algorithm
	{
		public static IEnumerable<object> Collection;
		public IEnumerable<PropertyInfo> GetProperties(object entryPoint)
		{
			return entryPoint.GetType().GetProperties();
		}

		public IEnumerable<object> Select(CustomFunctionBuilder functionBuilder)
		{
			foreach (var item in Collection)
			{
				yield return functionBuilder.Invoke();
			}
		}
	}

	public class CustomFunctionBuilder
	{
		private TypedDictionary tempVariables = new TypedDictionary();

		private Dictionary<string, object> executionSequence =
			new Dictionary<string, object>();

		public CustomFunctionBuilder() { }
		public CustomFunctionBuilder(
			TypedDictionary parameters,
			KeyValuePair<string, Func<object>> function = new KeyValuePair<string, Func<object>>())
		{
			AddVariable(parameters);
			if (function.Value != null)
			{
				AddFunction(function.Key, function.Value);
			}
		}
		public CustomFunctionBuilder(
			TypedDictionary parameters,
			KeyValuePair<string, Action> function = new KeyValuePair<string, Action>())
		{
			AddVariable(parameters);
			if (function.Value != null)
			{
				AddFunction(function.Key, function.Value);
			}
		}
		public CustomFunctionBuilder(
			TypedDictionary parameters,
			KeyValuePair<string, CustomFunctionBuilder> function
			= new KeyValuePair<string, CustomFunctionBuilder>())
		{
			AddVariable(parameters);
			if (function.Value != null)
			{
				AddFunction(function.Key, function.Value);
			}
		}

		/// <summary>
		///		Add one local variable to the wrapped function
		/// </summary>
		/// <param name="name"> Name of the parameter</param>
		/// <param name="data"> The object data of the parameter</param>
		/// <typeparam name="T"></typeparam>
		public void AddVariable<T>(string name, T data)
			=> tempVariables.Add(name, data);

		/// <summary>
		///		Add multiple variables at once to the wrapped function
		/// </summary>
		/// <param name="parameters">
		///		The variable list, containing names and data of each variable respectively
		///	</param>
		public void AddVariable(TypedDictionary parameters)
			=> tempVariables.Concat(parameters);

		/// <summary>
		///		Add one subprocedure or function to the wrapped function
		/// </summary>
		/// <param name="name">
		///		<para>The name of the function</para>
		///		<para>
		///			NOTE: If the function has a return value, 
		///			the return value will be stored inside the 
		///			<code>CustomizedFunctionWrapper</code> typed instance, 
		///			and its name is the same as the function. 
		///		</para>
		/// </param>
		/// <param name="method"></param>
		public void AddFunction(string name, Func<object> function)
			=> executionSequence.Add(name, function);
		public void AddFunction(string name, Action function)
			=> executionSequence.Add(name, function);
		public void AddFunction(string name, CustomFunctionBuilder function)
			=> executionSequence.Add(name, function);

		/// <summary>
		///		To invoke the wrapped function
		/// </summary>
		/// <returns>
		///		If the wrapped function is void type, then return null
		///		Otherwise a meaningful return value
		/// </returns>
		public object Invoke()
		{
			KeyValuePair<string, object> tempResult = new KeyValuePair<string, object>();

			foreach (KeyValuePair<string, object> function in executionSequence)
			{
				tempResult = new KeyValuePair<string, object>
				(
					function.Key,
					function
						.Value
						.GetType()
						.GetMethod("Invoke")
						.Invoke
						(
							function.Value,
							new object[] { }
						)
				);

				if (tempResult.Value != null)
					tempVariables.Add(tempResult.Key, tempResult.Value);
			}
			return tempResult.Value;
		}

		/// <summary>
		///		The generic version of the indexer
		/// </summary>
		/// <typeparam name="T">The type of the return value</typeparam>
		/// <param name="name">The name of the <c>tempVariable</c>, which is used to find it</param>
		/// <returns>The requested variable</returns>
		public T GetTempVariable<T>(string name)
			=> tempVariables.GetVariable<T>(name);

		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="name"></param>
		/// <param name="variable"></param>
		public void SetTempVariable<T>(string name, T variable)
			=> tempVariables.SetVariable<T>(name, variable);

		/// <summary>
		///		The Indexer. This version is a shortcut of <c>GetTempVariable<c/>. 
		/// </summary>
		/// <param name="name">The name of the <c>tempVariable</c>, which is used to find it</param>
		/// <returns>The requested variable</returns>
		public object this[string name]
		{
			get => tempVariables.GetVariable<object>(name);
			set => tempVariables.SetVariable<object>(name, value);
		}

		/// <summary>
		///		Show the invocation sequence
		/// </summary>
		/// <returns>A list of the names of functions ordered by their invocation</returns>
		public IEnumerable<string> ShowInvocationOrder()
		{
			foreach (KeyValuePair<string, object> function in executionSequence)
				yield return function.Key;
		}

		public class TypedDictionary
		{
			private List<object> typedKeyValuePairList = new List<object>();

			public void Add<T>(string name, T data)
				=> typedKeyValuePairList.Add(new TypedKeyValuePair<T>(name, data));

			public T GetVariable<T>(string name)
				=> (
					(TypedKeyValuePair<T>)typedKeyValuePairList
						.Where(v => ((TypedKeyValuePair<T>)v).Name == name)
						.Single()
				).ObjectData;

			public void SetVariable<T>(string name, T data)
				=> (
					(TypedKeyValuePair<T>)typedKeyValuePairList
						.Where(v => ((TypedKeyValuePair<T>)v).Name == name)
						.Single()
				).ObjectData = data;

			public void Concat(TypedDictionary another)
				=> typedKeyValuePairList.Concat(another.typedKeyValuePairList);

			private int Count => typedKeyValuePairList.Count;

			private class TypedKeyValuePair<T>
			{
				public string Name { get; set; }
				public T ObjectData { get; set; }
				public Type objectType { get; set; }

				public TypedKeyValuePair(string name, T data)
				{
					Name = name;
					ObjectData = data;
					objectType = typeof(T);
				}
			}
		}
	}
}
