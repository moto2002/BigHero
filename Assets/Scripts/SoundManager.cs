using UnityEngine;
using System.Collections;

public class SoundManager : MonoBehaviour {

	public GameObject soundPlayer_pre;
	
	private static SoundPlayer soundPlayer;

	private static int currentSound;

	// Use this for initialization
	void Start () {

		if(soundPlayer == null){
			soundPlayer = ((GameObject)Instantiate(soundPlayer_pre)).GetComponent<SoundPlayer>();
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public static void PlayMusic(int sound){

		currentSound = sound;

		if(soundPlayer == null){
			return;
		}

		soundPlayer.Play(sound);

	}
}
