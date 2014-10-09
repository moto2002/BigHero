using UnityEngine;
using System.Collections;

public class SpriteAnimation : MonoBehaviour {
	
	private Sprite[] _sprites;
	public Sprite[] sprites{
		set{

			if(spriteRenderer == null){
				spriteRenderer = GetComponent<SpriteRenderer>();
			}

			if(value != _sprites && value != null && value.Length > 0){
				spriteRenderer.sprite = value[0];
			}
			_sprites = value;
		}
		get{
			return _sprites;
		}
	}



	public int fps;

	/// <summary>
	/// 循环的次数，如果为0，则无限循环
	/// </summary>
	public int loopTimes;
	
	private SpriteRenderer spriteRenderer;

	[HideInInspector]
	public int index;

	private float curTime;
	private float changeTime;


	private bool running = true;
	
	// Use this for initialization
	public void Start () {
		index = 0;
		changeTime = 1 / (float)fps;
		curTime = 0;
		
		spriteRenderer = GetComponent<SpriteRenderer>();
	}
	
	// Update is called once per frame
	public void Update () {

		if(Constance.RUNNING == false){
			return;
		}

		if(running == false){
			return;
		}

		if(this._sprites == null || this._sprites.Length == 0){
			return;
		}

		curTime += Time.deltaTime;

		if(curTime < this.changeTime){
			return;
		}

		curTime -= this.changeTime;

		index++;

		if(index >= this._sprites.Length){
			index = 0;
		}


		this.spriteRenderer.sprite = this._sprites[index];
	}


	public void Play(){
		running = true;
	}


	public void Stop(){
		running = false;
	}
}
