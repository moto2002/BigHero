using UnityEngine;
using System.Collections;

public class SpriteAnimation : MonoBehaviour {

	public Sprite[] ss;

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


	public bool autoDestroy = false;

	public int fps;

	/// <summary>
	/// 循环的次数，如果为0，则无限循环
	/// </summary>
	public int loopTimes;

	private int curLoopTimes;
	
	private SpriteRenderer spriteRenderer;

	[HideInInspector]
	public int index;

	private float curTime;
	private float changeTime;
	
	private bool running = true;
	private bool specSign = false;


	private int _sortingLayerID;

	public int sortingLayerID{
		set{
			this._sortingLayerID = value;
		}
		get{
			return this._sortingLayerID;
		}
	}


	private int _sortingOrder;
	public int sortingOrder{
		set{
			this._sortingOrder = value;
		}
		get{
			return this._sortingOrder;
		}
	}
	// Use this for initialization
	public void Start () {
		index = 0;
		changeTime = 1 / (float)fps;
		curTime = 0;
		curLoopTimes = 0;

		
		spriteRenderer = GetComponent<SpriteRenderer>();

		if(this._sortingLayerID != 0){
			spriteRenderer.sortingLayerID = this._sortingLayerID;
		}

		if(this._sortingOrder != 0){
			spriteRenderer.sortingOrder = this._sortingOrder;
		}

		if(ss.Length > 0){
			this.sprites = ss;
		}
	}
	
	// Update is called once per frame
	public void Update () {

		if(Constance.SPEC_RUNNING == false && Constance.RUNNING == false){
			return;
		}else if(Constance.SPEC_RUNNING == true && this.specSign == false){
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
			curLoopTimes++;
			index = 0;
		}


		if(loopTimes > 0 && curLoopTimes == loopTimes){
			index = this._sprites.Length - 1;
			this.Stop();

			if(autoDestroy){
				Destroy(this.gameObject);
			}
		}

		this.spriteRenderer.sprite = this._sprites[index];
	}


	public void Play(){
		running = true;
	}


	public void Stop(){
		running = false;
	}

	public bool IsEnd(){
		if(loopTimes > 0 && curLoopTimes == loopTimes){
			return true;
		}

		return false;
	}

	public void SetSpec(bool b){
		this.specSign = b;

		if(b == true){
			this.spriteRenderer.sortingLayerID = 7;
		}else{
			this.spriteRenderer.sortingLayerID = 1;
		}
	}
}
