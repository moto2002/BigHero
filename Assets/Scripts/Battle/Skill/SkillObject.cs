using UnityEngine;
using System.Collections;

public class SkillObject : MonoBehaviour {

	public SpriteAnimation spriteAnimation;
	
	private bool specSign = false;

	[HideInInspector]
	public int res;
	
	[HideInInspector]
	public int loop = 0;

	[HideInInspector]
	public int sound = 0;
	// Use this for initialization
	void Start () {
		spriteAnimation.sprites = Resources.LoadAll<Sprite>(@"Image/Effect/" + res);
		spriteAnimation.loopTimes = loop;

		
		spriteAnimation.SetSpec(specSign);

		if(sound != 0){
			this.audio.clip = Resources.Load<AudioClip>("Audio/Skill/" + sound);
			if(this.audio.clip != null){
				this.audio.Play();
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
		this.spriteAnimation.renderer.sortingOrder = -(int)(this.transform.localPosition.y * 10);
	}

	public void SetSpriteOff(Vector3 v){
		spriteAnimation.transform.position += v;
	}

	public void SetSpriteEulerAngles(Vector3 v){
		spriteAnimation.transform.eulerAngles = v;
	}

	public bool IsSpritePlayEnd(){
		return spriteAnimation.IsEnd();
	}
	
	public void SetSpec(bool b){
		specSign = b;
	}

}
