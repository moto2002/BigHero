using UnityEngine;
using System.Collections;

public class SkillObject : MonoBehaviour {

	public SpriteAnimation spriteAnimation;

	[HideInInspector]
	public int res;
	// Use this for initialization
	void Start () {
		spriteAnimation.sprites = Resources.LoadAll<Sprite>(@"Image/Effect/" + res);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void SetSpriteOff(Vector2 v){
		spriteAnimation.transform.position = v;
	}

	public void SetSpriteEulerAngles(Vector3 v){
		spriteAnimation.transform.eulerAngles = v;
	}
}
