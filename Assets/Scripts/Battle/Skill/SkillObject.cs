using UnityEngine;
using System.Collections;

public class SkillObject : MonoBehaviour {

	public SpriteAnimation spriteAnimation;

	private SpriteRenderer spriteRenderer;

	[HideInInspector]
	public int res;
	
	[HideInInspector]
	public int loop = 0;
	// Use this for initialization
	void Start () {
		spriteAnimation.sprites = Resources.LoadAll<Sprite>(@"Image/Effect/" + res);
		spriteAnimation.loopTimes = loop;
	}
	
	// Update is called once per frame
	void Update () {
		this.spriteAnimation.renderer.sortingOrder = -(int)(this.transform.localPosition.y * 10);
	}

	public void SetSpriteOff(Vector2 v){
		spriteAnimation.transform.position = v;
	}

	public void SetSpriteEulerAngles(Vector3 v){
		spriteAnimation.transform.eulerAngles = v;
	}

	public bool IsSpritePlayEnd(){
		return spriteAnimation.IsEnd();
	}
}
