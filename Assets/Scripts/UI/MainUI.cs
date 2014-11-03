using UnityEngine;
using System.Collections;

public class MainUI : MonoBehaviour {

	void Start(){
		SoundManager.PlayMusic(1);
	}

	public void GotoGuanSence(){
		Application.LoadLevel(1);
	}
}
