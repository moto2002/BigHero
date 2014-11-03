using UnityEngine;
using System.Collections;

public class GuanUI : MonoBehaviour {


	// Use this for initialization
	void Start () {
		SoundManager.PlayMusic(2);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void BackToMain(){
		Application.LoadLevel(0);
	}
}
