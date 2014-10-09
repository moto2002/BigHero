using UnityEngine;
using System.Collections;

public class CharModel : SpriteAnimation {
	
	private Sprite[] moveUp;
	private Sprite[] moveDown;
	private Sprite[] moveRight;
	private Sprite[] moveLeft;
	
	private Sprite[] attUp;
	private Sprite[] attDown;
	private Sprite[] attRight;
	private Sprite[] attLeft;

	private Sprite[] attedUp;
	private Sprite[] attedDown;
	private Sprite[] attedRight;
	private Sprite[] attedLeft;
	
	private Sprite[] deadUp;
	private Sprite[] deadDown;
	private Sprite[] deadRight;
	private Sprite[] deadLeft;
	
	private Sprite[] skillUp;
	private Sprite[] skillDown;
	private Sprite[] skillRight;
	private Sprite[] skillLeft;

	[HideInInspector]
	public int model;

	private float deadTime = 0;

	public enum State{
		MOVE,
		STOP,
		DEAD,
		ATTACK,
		ATTACKED,
		SKILL
	}


	private State _currentState;
	[HideInInspector]
	public State currentState{
		set{
			_currentState = value;

			switch(value){
				case State.MOVE:
				Move ();
				break;
				case State.ATTACK:
				PlayAttack (false);
				break;
			}
		}
		get{
			return this._currentState;
		}
	}


	private MoveDirection _direction;
	[HideInInspector]
	public MoveDirection direction{
		set{
			this._direction = value;

			switch(this._currentState){
			case State.MOVE:
				this.Move();
				break;
			}
		}
		get{
			return this._direction;
		}
	}


	// Use this for initialization
	public void Start () {
		base.Start();
	}

	public void SetID(int id){
		Sprite [] modelSprites = Resources.LoadAll<Sprite>(@"Image/Model/" + id + "_w");

		if(modelSprites != null){
			moveUp = new Sprite[4];
			moveDown = new Sprite[4];
			moveLeft = new Sprite[4];
			moveRight = new Sprite[4];
			
			moveDown[0] = modelSprites[0];
			moveDown[1] = modelSprites[1];
			moveDown[2] = modelSprites[2];
			moveDown[3] = modelSprites[3];
			
			moveLeft[0] = modelSprites[4];
			moveLeft[1] = modelSprites[5];
			moveLeft[2] = modelSprites[6];
			moveLeft[3] = modelSprites[7];
			
			moveUp[0] = modelSprites[8];
			moveUp[1] = modelSprites[9];
			moveUp[2] = modelSprites[10];
			moveUp[3] = modelSprites[11];
			
			moveRight[0] = modelSprites[12];
			moveRight[1] = modelSprites[13];
			moveRight[2] = modelSprites[14];
			moveRight[3] = modelSprites[15];
		}
		
		modelSprites = Resources.LoadAll<Sprite>(@"Image/Model/" + id + "_a");
		if(modelSprites != null){

			attUp = new Sprite[4];
			attDown = new Sprite[4];
			attLeft = new Sprite[4];
			attRight = new Sprite[4];
			attDown[0] = modelSprites[0];
			attDown[1] = modelSprites[1];
			attDown[2] = modelSprites[2];
			attDown[3] = modelSprites[3];
			attLeft[0] = modelSprites[4];
			attLeft[1] = modelSprites[5];
			attLeft[2] = modelSprites[6];
			attLeft[3] = modelSprites[7];
			attUp[0] = modelSprites[8];
			attUp[1] = modelSprites[9];
			attUp[2] = modelSprites[10];
			attUp[3] = modelSprites[11];
			attRight[0] = modelSprites[12];
			attRight[1] = modelSprites[13];
			attRight[2] = modelSprites[14];
			attRight[3] = modelSprites[15];

		}


		
		modelSprites = Resources.LoadAll<Sprite>(@"Image/Model/" + id + "_d");
		if(modelSprites != null){
			
			deadUp = new Sprite[4];
			deadDown = new Sprite[4];
			deadLeft = new Sprite[4];
			deadRight = new Sprite[4];
			deadDown[0] = modelSprites[0];
			deadDown[1] = modelSprites[1];
			deadDown[2] = modelSprites[2];
			deadDown[3] = modelSprites[3];
			deadLeft[0] = modelSprites[4];
			deadLeft[1] = modelSprites[5];
			deadLeft[2] = modelSprites[6];
			deadLeft[3] = modelSprites[7];
			deadUp[0] = modelSprites[8];
			deadUp[1] = modelSprites[9];
			deadUp[2] = modelSprites[10];
			deadUp[3] = modelSprites[11];
			deadRight[0] = modelSprites[12];
			deadRight[1] = modelSprites[13];
			deadRight[2] = modelSprites[14];
			deadRight[3] = modelSprites[15];
			
		}

		currentState = State.MOVE;
		this.Move();
	}
	
	// Update is called once per frame
	void Update () {

		base.Update();
		base.fps = 8;

		if(base.index == 3 && (this.currentState == State.ATTACK ||
			   this.currentState == State.ATTACKED ||
			   this.currentState == State.SKILL
			   )){
			this.Move();
		}

		if(base.index == 3 && this.currentState == State.DEAD){
			this.Stop();
		}

		if( this.currentState == State.DEAD ){
			deadTime += Time.deltaTime;

			if(deadTime > 1.5){
				Destroy(this.gameObject.transform.parent.gameObject);
			}
		}
	}



	public void Move(){
		_currentState = State.MOVE;

		switch(this.direction){
		case MoveDirection.UP:
			this.sprites = this.moveUp;
			break;
		case MoveDirection.DOWN:
			this.sprites = this.moveDown;
			break;
		case MoveDirection.LEFT:
			this.sprites = this.moveLeft;
			break;
		case MoveDirection.RIGHT:
			this.sprites = this.moveRight;
			break;
		}
	}


	public void PlayAttack(bool b = true){
		
		_currentState = State.ATTACK;

		if(b == true){
			base.index = 0;
		}

		switch(this.direction){
		case MoveDirection.UP:
			this.sprites = this.attUp;
			break;
		case MoveDirection.DOWN:
			this.sprites = this.attDown;
			break;
		case MoveDirection.LEFT:
			this.sprites = this.attLeft;
			break;
		case MoveDirection.RIGHT:
			this.sprites = this.attRight;
			break;
		}
	}


	public void PlayDead(){
		base.index = 0;
		_currentState = State.DEAD;
		
		switch(this.direction){
		case MoveDirection.UP:
			this.sprites = this.deadUp;
			break;
		case MoveDirection.DOWN:
			this.sprites = this.deadDown;
			break;
		case MoveDirection.LEFT:
			this.sprites = this.deadLeft;
			break;
		case MoveDirection.RIGHT:
			this.sprites = this.deadRight;
			break;
		}
	}

	void LaterUpdate(){

	}

	public bool IsInAttIndex(){

		if(this._currentState == State.ATTACK && base.index == 2){
			return true;
		}

		return false;
	}

}








	