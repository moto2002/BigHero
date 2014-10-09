using UnityEngine;
using System.Collections;

public class GameEvent {

	
	public GameEvent(){
	}

	public GameEvent(string name){
		this.eventName = name;
	}

	public GameEvent(string name , object data){
		this.eventName = name;
		this.data = data;
	}
	public object data;
	public string eventName;
}
