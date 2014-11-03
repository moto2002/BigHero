using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIManager : MonoBehaviour {


	public GameObject[] uilist;

	public Dictionary<string ,GameObject> ui = new Dictionary<string, GameObject>();
	// Use this for initialization
	void Start () {
		for(int i = 0 ; i < uilist.Length ; i++){
			string name = ((GameObject)uilist[i]).name;

			ui[name] = (GameObject)uilist[i];
		}

		ShowSingleUI("ControllUI");
		
		EventDispather.AddEventListener(EventName.UI_SHOW ,OnShowUI);
		EventDispather.AddEventListener(EventName.UI_SHOW_SINGLE ,OnShowUI);

		SoundManager.PlayMusic(3);
	}


	public void BackToGuan(){
		HideAll();
		Application.LoadLevel(1);
	}

	private void OnShowUI(GameEvent e){
		ShowUI(e.data.ToString());
	}


	public void ShowSingleUI(string name){
		HideAll();
		ShowUI(name);
	}


	public void ShowUI(string name){
		GameObject go;

		ui.TryGetValue(name , out go);

		if(go != null){
			go.SetActive(true);
		}

	}

	public void HideUI(string name){
		GameObject go;
		
		ui.TryGetValue(name , out go);
		
		if(go != null){
			go.SetActive(false);
		}
	}

	public void HideAll(){
		for(int i = 0 ; i < uilist.Length ; i++){
			((GameObject)uilist[i]).SetActive(false);
		}
	}
}
