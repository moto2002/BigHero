using UnityEngine;
using System.Collections;

public class SoundPlayer : MonoBehaviour {

	private AudioClip nextClip;


	// Use this for initialization
	void Start () {
		DontDestroyOnLoad(this.gameObject);
	}
	
	// Update is called once per frame
	void Update () {
		if(nextClip == null){
			if(this.audio.volume < 1){
				this.audio.volume += 0.08f;
			}
			return;
		}

		if(this.audio.clip == null){
			this.audio.clip = this.nextClip;
			this.audio.Play();
			this.audio.volume = 1;

			this.nextClip = null;
			return;
		}else if(this.audio.volume > 0){
			this.audio.volume -= 0.02f;
		}else{
			this.audio.clip = this.nextClip;
			this.audio.Play();
			
			this.nextClip = null;
		}

	}


	public void Play(int sound){
		
		switch(sound){
		case 1:
			nextClip = Resources.Load<AudioClip>("Audio/Music/Main");
			break;
		case 2:
			nextClip = Resources.Load<AudioClip>("Audio/Music/Start");
			break;
		case 3:
			nextClip = Resources.Load<AudioClip>("Audio/Music/Battle1");
			break;
		}
	}
}
