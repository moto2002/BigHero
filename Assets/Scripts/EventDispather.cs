using UnityEngine;
using System.Collections;
using System.Collections.Generic;



public class EventDispather  {

	private static Dictionary<string , HashSet<EventListener>> eventListener;

	public delegate void EventListener(GameEvent e);

	public static void AddEventListener(string eventName , EventListener e){
		if(eventListener == null){
			eventListener = new Dictionary<string , HashSet<EventListener>>();
		}

		HashSet<EventListener> listeners;


		eventListener.TryGetValue(eventName , out listeners);

		if(listeners == null){
			listeners = new  HashSet<EventListener>();
			eventListener[eventName] = listeners;
		}

		listeners.Add(e);
	}

	
	
	public static void DispatchEvent(string eventName){
		DispatchEvent(new GameEvent(eventName));
	}

	public static void DispatchEvent(GameEvent e){
		if(eventListener == null){
			eventListener = new Dictionary<string , HashSet<EventListener>>();
		}

		string eventName = e.eventName;

		HashSet<EventListener> listeners;
		eventListener.TryGetValue(eventName , out listeners);

		if(listeners == null){
			return;
		}
		
		foreach(EventListener listener in listeners){
			listener(e);
		}
	}

	public static void RemoveEventListener(){
	}
}
