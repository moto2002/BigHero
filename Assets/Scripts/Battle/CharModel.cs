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
	
	private bool stateLock = false;

	public enum State{
		MOVE,
		STOP,
		DEAD,
		ATTACK,
		ATTACKED,
		SKILL
	}

	//1 normal att
	//2 skill att
	private int attType = 1;


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
				if(attType == 1){
					PlayAttack (false);
				}else{
					PlaySkillAttack(false);
				}
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
		base.fps = 8;

		Texture2D texture2d = Resources.Load<Texture2D>("Image/Model/" + id + "/w");

		if(texture2d != null){
			moveUp = new Sprite[4];
			moveDown = new Sprite[4];
			moveLeft = new Sprite[4];
			moveRight = new Sprite[4];

			int w = texture2d.width / 4;
			int h = texture2d.height / 4;

			for(int i = 0 ; i < 4 ; i++){
				for(int j = 0 ; j < 4 ; j++){
					Rect r = new Rect();

					r.x = j * w;
					r.y = i * h;
					r.width = w;
					r.height = h;
					
					Vector2 p = new Vector2();
					
					p.x = 0.5f;
					p.y = 0.3f;
					
					Sprite s = Sprite.Create(texture2d , r ,p);
					
					if(i == 0){
						moveRight[j] = s;
					}else if(i == 1){
						moveUp[j] = s;
					}else if(i == 2){
						moveLeft[j] = s;
					}else if(i == 3){
						moveDown[j] = s;
					}
					
				}
			}
		}

		
		texture2d = Resources.Load<Texture2D>("Image/Model/" + id + "/a");
		if(texture2d != null){

			attUp = new Sprite[4];
			attDown = new Sprite[4];
			attLeft = new Sprite[4];
			attRight = new Sprite[4];

			int w = texture2d.width / 4;
			int h = texture2d.height / 4;
			
			for(int i = 0 ; i < 4 ; i++){
				for(int j = 0 ; j < 4 ; j++){
					Rect r = new Rect();

					r.x = j * w;
					r.y = i * h;
					r.width = w;
					r.height = h;
					
					Vector2 p = new Vector2();
					
					p.x = 0.5f;
					p.y = 0.3f;
					
					Sprite s = Sprite.Create(texture2d , r ,p);
					
					if(i == 0){
						attRight[j] = s;
					}else if(i == 1){
						attUp[j] = s;
					}else if(i == 2){
						attLeft[j] = s;
					}else if(i == 3){
						attDown[j] = s;
					}
				}
			}

		}


		
		texture2d = Resources.Load<Texture2D>("Image/Model/" + id + "/d");
		if(texture2d != null){
			
			deadUp = new Sprite[4];
			deadDown = new Sprite[4];
			deadLeft = new Sprite[4];
			deadRight = new Sprite[4];
			
			
			int w = texture2d.width / 4;
			int h = texture2d.height / 4;
			
			for(int i = 0 ; i < 4 ; i++){
				for(int j = 0 ; j < 4 ; j++){
					Rect r = new Rect();
					
					r.x = j * w;
					r.y = i * h;
					r.width = w;
					r.height = h;
					
					Vector2 p = new Vector2();
					
					p.x = 0.5f;
					p.y = 0.3f;
					
					Sprite s = Sprite.Create(texture2d , r ,p);
					
					if(i == 0){
						deadRight[j] = s;
					}else if(i == 1){
						deadUp[j] = s;
					}else if(i == 2){
						deadLeft[j] = s;
					}else if(i == 3){
						deadDown[j] = s;
					}
				}
			}
		}


		
		
		
		texture2d = Resources.Load<Texture2D>("Image/Model/" + id + "/s");
		if(texture2d != null){
			
			skillUp = new Sprite[4];
			skillDown = new Sprite[4];
			skillLeft = new Sprite[4];
			skillRight = new Sprite[4];
			
			
			int w = texture2d.width / 4;
			int h = texture2d.height / 4;
			
			for(int i = 0 ; i < 4 ; i++){
				for(int j = 0 ; j < 4 ; j++){
					Rect r = new Rect();
					
					r.x = j * w;
					r.y = i * h;
					r.width = w;
					r.height = h;
					
					Vector2 p = new Vector2();
					
					p.x = 0.5f;
					p.y = 0.3f;
					
					Sprite s = Sprite.Create(texture2d , r ,p);
					
					if(i == 0){
						skillRight[j] = s;
					}else if(i == 1){
						skillUp[j] = s;
					}else if(i == 2){
						skillLeft[j] = s;
					}else if(i == 3){
						skillDown[j] = s;
					}
				}
			}
		}

		currentState = State.MOVE;
		this.Move();
	}
	
	// Update is called once per frame
	void Update () {

		base.Update();

		if(stateLock == true){
			return;
		}

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

		attType = 1;
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


	public void PlaySkillAttack(bool b = true){
		attType = 2;

		_currentState = State.ATTACK;

		if(b == true){
			base.index = 0;
		}
		
		switch(this.direction){
		case MoveDirection.UP:
			this.sprites = this.skillUp;
			break;
		case MoveDirection.DOWN:
			this.sprites = this.skillDown;
			break;
		case MoveDirection.LEFT:
			this.sprites = this.skillLeft;
			break;
		case MoveDirection.RIGHT:
			this.sprites = this.skillRight;
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

	public void SetPlayLock(bool b){
		this.stateLock = b;
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








	