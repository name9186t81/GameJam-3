using Architecture;
using System;
using System.Collections.Generic;

namespace Core
{
	public static class ServiceLocator
	{
		private static Dictionary<Type, object> _services = new Dictionary<Type, object>();

		public static void ClearAll(CreateEssentialServicesElement creator)
        {
			if(creator == null)
				throw new ArgumentNullException(nameof(creator));

			_services.Clear();
        }

		public static void Register<T>(T instance) where T : class, IService
		{
			_services.Add(typeof(T), instance);
		}

		public static bool TryGet<T>(out T instance) where T : class, IService
		{
			if (_services.TryGetValue(typeof(T), out object obj))
			{
				instance = obj as T;
				return true;
			}
			instance = default;
			return false;
		}

		public static T Get<T>() where T : class, IService
		{
			if (_services.TryGetValue(typeof(T), out object obj))
			{
				return (T)obj;
			}
			return null;
		}

		public static bool TrySet<T>(T val) where T : class, IService
		{
			bool res = _services.ContainsKey(typeof(T));
			if (res) _services[typeof(T)] = val;
			return res;
		}
	}
}