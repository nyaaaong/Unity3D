using System;
using System.Collections.Generic;
using UnityEngine;

namespace DevionGames
{
	public class EventHandler : MonoBehaviour
	{
		private static Dictionary<string, Delegate> m_GlobalEvents;
		private static Dictionary<object, Dictionary<string, Delegate>> m_Events;

		static EventHandler()
		{
			EventHandler.m_GlobalEvents = new Dictionary<string, Delegate>();
			EventHandler.m_Events = new Dictionary<object, Dictionary<string, Delegate>>();
		}

		public static void Execute(string eventName)
		{
			if (EventHandler.GetDelegate(eventName) is System.Action mDelegate)
			{
				mDelegate();
			}
		}

		public static void Execute(object obj, string eventName)
		{
			if (EventHandler.GetDelegate(obj, eventName) is System.Action mDelegate)
			{
				mDelegate();
			}
		}

		public static void Execute<T1>(string eventName, T1 arg1)
		{
			if (EventHandler.GetDelegate(eventName) is Action<T1> mDelegate)
			{
				mDelegate(arg1);
			}
		}

		public static void Execute<T1>(object obj, string eventName, T1 arg1)
		{
			if (EventHandler.GetDelegate(obj, eventName) is Action<T1> mDelegate)
			{
				mDelegate(arg1);
			}
		}

		public static void Execute<T1, T2>(string eventName, T1 arg1, T2 arg2)
		{
			if (EventHandler.GetDelegate(eventName) is Action<T1, T2> mDelegate)
			{
				mDelegate(arg1, arg2);
			}
		}

		public static void Execute<T1, T2>(object obj, string eventName, T1 arg1, T2 arg2)
		{
			if (EventHandler.GetDelegate(obj, eventName) is Action<T1, T2> mDelegate)
			{
				mDelegate(arg1, arg2);
			}
		}

		public static void Execute<T1, T2, T3>(string eventName, T1 arg1, T2 arg2, T3 arg3)
		{
			if (EventHandler.GetDelegate(eventName) is Action<T1, T2, T3> mDelegate)
			{
				mDelegate(arg1, arg2, arg3);
			}
		}

		public static void Execute<T1, T2, T3>(object obj, string eventName, T1 arg1, T2 arg2, T3 arg3)
		{
			if (EventHandler.GetDelegate(obj, eventName) is Action<T1, T2, T3> mDelegate)
			{
				mDelegate(arg1, arg2, arg3);
			}
		}

		public static void Register(string eventName, System.Action handler)
		{
			EventHandler.Register(eventName, (Delegate)handler);
		}

		public static void Register(object obj, string eventName, System.Action handler)
		{
			EventHandler.Register(obj, eventName, (Delegate)handler);
		}


		public static void Register<T1>(string eventName, Action<T1> handler)
		{
			EventHandler.Register(eventName, (Delegate)handler);
		}

		public static void Register<T1>(object obj, string eventName, Action<T1> handler)
		{
			EventHandler.Register(obj, eventName, (Delegate)handler);
		}

		public static void Register<T1, T2>(string eventName, Action<T1, T2> handler)
		{
			EventHandler.Register(eventName, (Delegate)handler);
		}

		public static void Register<T1, T2>(object obj, string eventName, Action<T1, T2> handler)
		{
			EventHandler.Register(obj, eventName, (Delegate)handler);
		}

		public static void Register<T1, T2, T3>(string eventName, Action<T1, T2, T3> handler)
		{
			EventHandler.Register(eventName, (Delegate)handler);
		}

		public static void Register<T1, T2, T3>(object obj, string eventName, Action<T1, T2, T3> handler)
		{
			EventHandler.Register(obj, eventName, (Delegate)handler);
		}

		public static void Unregister(string eventName, System.Action handler)
		{
			EventHandler.Unregister(eventName, (Delegate)handler);
		}

		public static void Unregister(object obj, string eventName, System.Action handler)
		{
			EventHandler.Unregister(obj, eventName, (Delegate)handler);
		}

		public static void Unregister<T1>(string eventName, Action<T1> handler)
		{
			EventHandler.Unregister(eventName, (Delegate)handler);
		}

		public static void Unregister<T1>(object obj, string eventName, Action<T1> handler)
		{
			EventHandler.Unregister(obj, eventName, (Delegate)handler);
		}

		public static void Unregister<T1, T2>(string eventName, Action<T1, T2> handler)
		{
			EventHandler.Unregister(eventName, (Delegate)handler);
		}

		public static void Unregister<T1, T2>(object obj, string eventName, Action<T1, T2> handler)
		{
			EventHandler.Unregister(obj, eventName, (Delegate)handler);
		}

		public static void Unregister<T1, T2, T3>(string eventName, Action<T1, T2, T3> handler)
		{
			EventHandler.Unregister(eventName, (Delegate)handler);
		}

		public static void Unregister<T1, T2, T3>(object obj, string eventName, Action<T1, T2, T3> handler)
		{
			EventHandler.Unregister(obj, eventName, (Delegate)handler);
		}

		private static void Register(string eventName, Delegate handler)
		{
			Delegate mDelegate;
			if (!EventHandler.m_GlobalEvents.TryGetValue(eventName, out mDelegate))
			{
				EventHandler.m_GlobalEvents.Add(eventName, handler);
			}
			else
			{
				EventHandler.m_GlobalEvents[eventName] = Delegate.Combine(mDelegate, handler);
			}
		}

		private static void Register(object obj, string eventName, Delegate handler)
		{
			if (obj == null) return;
			Dictionary<string, Delegate> mEvents;
			Delegate mDelegate;
			if (!EventHandler.m_Events.TryGetValue(obj, out mEvents))
			{
				mEvents = new Dictionary<string, Delegate>();
				EventHandler.m_Events.Add(obj, mEvents);
			}
			if (!mEvents.TryGetValue(eventName, out mDelegate))
			{
				mEvents.Add(eventName, handler);
			}
			else
			{
				mEvents[eventName] = Delegate.Combine(mDelegate, handler);
			}
		}


		private static void Unregister(string eventName, Delegate handler)
		{
			Delegate mDelegate;
			if (EventHandler.m_GlobalEvents.TryGetValue(eventName, out mDelegate))
			{
				EventHandler.m_GlobalEvents[eventName] = Delegate.Remove(mDelegate, handler);
			}
		}

		private static void Unregister(object obj, string eventName, Delegate handler)
		{
			if (obj == null) return;
			Dictionary<string, Delegate> mEvents;
			Delegate mDelegate;
			if (EventHandler.m_Events.TryGetValue(obj, out mEvents) && mEvents.TryGetValue(eventName, out mDelegate))
			{
				mEvents[eventName] = Delegate.Remove(mDelegate, handler);
			}
		}

		private static Delegate GetDelegate(string eventName)
		{
			Delegate mDelegate;
			if (EventHandler.m_GlobalEvents.TryGetValue(eventName, out mDelegate))
			{
				return mDelegate;
			}
			return null;
		}

		private static Delegate GetDelegate(object obj, string eventName)
		{
			Dictionary<string, Delegate> mEvents;
			Delegate mDelegate;
			if (EventHandler.m_Events.TryGetValue(obj, out mEvents) && mEvents.TryGetValue(eventName, out mDelegate))
			{
				return mDelegate;
			}
			return null;
		}
	}
}