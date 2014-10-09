using UnityEngine;
using System.Collections;
using LitJson;
using System.Collections.Generic;

public class Hero : Charactor{
	
	public float speed = 0.5f;

	public Follower follower;


	[HideInInspector]
	public Vector2 nextPosition = new Vector2(0,0); 
	
	[HideInInspector]
	public float attackCD = 1f;

	private Queue attackTagets = new Queue();

	private Transform _transform;

	public CharModel model;
	public SpriteRenderer spriteRenderer;
	public Animator animator;

	public HPBar hpBar;

	private bool running = true;
	private bool dead = false;

	private Vector2 position;

	private ArrayList attackRange;

	public Attribute attribute;

	public GameObject hp_pop;

	
	[HideInInspector]
	public MoveDirection _currentDirection;
	public MoveDirection currentDirection{
		set{
			this.model.direction = value;
			this._currentDirection = value;
		}
		get{
			
			return this._currentDirection;
		}
	}
	
	private MoveDirection _direction;
	public MoveDirection direction{
		set{
//			if(value == MoveDirection.DOWN && currentDirection == MoveDirection.UP){
//				return;
//			}
//			
//			
//			if(value == MoveDirection.UP && currentDirection == MoveDirection.DOWN){
//				return;
//			}
//			
//			
//			if(value == MoveDirection.RIGHT && currentDirection == MoveDirection.LEFT){
//				return;
//			}
//			
//			
//			if(value == MoveDirection.LEFT && currentDirection == MoveDirection.RIGHT){
//				return;
//			}
			
			this._direction = value;
		}
		get{return _direction;}
	}

	private SkillConfig normalAttackSkill;

	private ArrayList skills;


	// Use this for initialization
	void Start () {

		if(this.follower != null){
			this.follower.SetDirection(this.direction);
		}

		switch(this.direction){
		case MoveDirection.DOWN:
			nextPosition.y -= 1;
			break;
		case MoveDirection.UP:
			nextPosition.y += 1;
			break;
		case MoveDirection.LEFT:
			nextPosition.x -= 1;
			break;
		case MoveDirection.RIGHT:
			nextPosition.x += 1;
			break;
		}

		this.model.direction = this.currentDirection;

		_transform = this.transform;

		UpdatePosition();

		attackRange = new ArrayList();
		attackRange.Add(new Vector2());
		attackRange.Add(new Vector2());
		attackRange.Add(new Vector2());


		skills = new ArrayList();

		normalAttackSkill = Config.GetInstance().GetSkillCOnfig(0);
	}
	
	// Update is called once per frame
	void Update () {

		if(this.dead == true){
			return;
		}

		FindFollower();
		UpdateState();
		TryMove();
		TryAttak();
	}


	private void FindFollower(){
		ArrayList followers = Battle.GetFollowersByPoint(this.position);
		
		foreach(Follower follower in followers){
			
			if(this.position == follower.GetPoint()){
				if(follower.state == FollowState.IDEL){

					follower.state = FollowState.WAIT_CLOSE;
					follower.SetFollowTarget(this);
				}
			}
		}
	}


	private void UpdateState(){
		if(this.animator == null){
			return;
		}

		if( this.animator.GetCurrentAnimatorStateInfo(0).IsName("Shake") && this.animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.1f){
			this.animator.SetInteger("State" , 0);
		}
	}


	private void UpdatePosition(){

		this.spriteRenderer.sortingOrder = -(int)(this.transform.localPosition.y * 10);
		
		int x = (int)Mathf.Round(this.transform.localPosition.x / Constance.GRID_GAP);
		int y = -(int)Mathf.Round(this.transform.localPosition.y / Constance.GRID_GAP);
		
		if(x != this.position.x || y != this.position.y){
			this.position.x = x;
			this.position.y = y;
			
			Battle.UpdatePosition(this ,this.position);
		}
	}


	private void TryMove(){
		if (Input.GetKeyDown (KeyCode.W)) {
			//up
			this.direction = MoveDirection.UP;
		}else if (Input.GetKeyDown (KeyCode.S)) {
			//down
			this.direction = MoveDirection.DOWN;
		}else if (Input.GetKeyDown (KeyCode.A)) {
			//left
			this.direction = MoveDirection.LEFT;
		}else if (Input.GetKeyDown (KeyCode.D)) {
			//right
			this.direction = MoveDirection.RIGHT;
		}
		
		if(Constance.RUNNING == false){
			return;
		}
		
		if(this.running == false){
			return;
		}


		this.nextPosition = GetNextPosition();
		
		Vector2 nextPoint = BattleUtils.PositionToGrid(this.nextPosition.x , this.nextPosition.y);
		
		
		if(Battle.IsMoveable(nextPoint) == false){
			if(this.currentDirection != this.direction){
				this.currentDirection = this.direction;
				
				if(this.follower != null)follower.SetNextPosition(this._transform.localPosition , this.direction);
			}
			return;
		}
		
		//下个格子中的物体
		ArrayList monsterList = Battle.GetMonstersByPoint(nextPoint);
		
		if(this.position != nextPoint && monsterList.Count > 0){
			//前方有障碍物 需要停止
			
			if(this.currentDirection != this.direction){
				this.currentDirection = this.direction;
				
				if(this.follower != null)follower.SetNextPosition(this._transform.localPosition , this.direction);
			}
			
			return;
		}
		
		
		float moveDistance = Time.deltaTime * speed;
		
		float toNextPositionDistance = GetToNextPositionDistance();
		
		if(toNextPositionDistance > moveDistance){
			//not to point
			Move(moveDistance);
		}else{
			
			if(this.follower != null){
				follower.SetNextPosition(this.nextPosition , this.direction);
			}
			
			Move(toNextPositionDistance);
			
			this.currentDirection = this.direction;
			Move(moveDistance - toNextPositionDistance);
		}
		
		if(follower != null){
			follower.move(moveDistance);
		}
		

		UpdatePosition();
	}


	public void Move(float distance){

		Vector3 position = this._transform.localPosition;

		
		switch(this.currentDirection){
			case MoveDirection.DOWN:
			position.y = this._transform.localPosition.y - distance;
				break;

			case MoveDirection.UP:
			position.y = this._transform.localPosition.y + distance;
				break;


			case MoveDirection.LEFT:
			position.x = this._transform.localPosition.x - distance;
				break;

			case MoveDirection.RIGHT:
			position.x = this._transform.localPosition.x + distance;
				break;
		}

		this._transform.localPosition = position;
	}


	private void TryAttak(){
		NormalAttack();
	}


	private void NormalAttack(){
		if(attackTagets.Count > 0){
			this.model.PlayAttack();
			this.attackCD = 0;
			
			while(attackTagets.Count > 0){
				Charactor charactor = (Charactor)attackTagets.Dequeue();
				
				SkillManager.PlaySkill(this , charactor, normalAttackSkill);
			}
			
		}
		
		if(this.attackCD < 1){
			this.attackCD += Time.deltaTime;
			return;
		}
		
		ArrayList points  = AttRange.GetRange(AttRange.TYPE_RECT , this.normalAttackSkill.range ,  this.attribute.volume , this.position);
		
		for(int i = 0 ; i < points.Count ; i++){
			ArrayList monsters = Battle.GetMonstersByPoint((Vector2)points[i]);
			
			for(int j = 0 ; j < monsters.Count ; j++){
				
				if(attackTagets.Contains(monsters[j]) == false){
					attackTagets.Enqueue(monsters[j]);
				}
			}
		}
	}



	public void playSkill(int index){
		this.attackCD = 0;

		SkillConfig skillConfig = Config.GetInstance().GetSkillCOnfig(3);

		SkillManager.PlaySkill(this , null , skillConfig);
	}


	private Vector2 GetNextPosition(){

		nextPosition.x = this._transform.localPosition.x ;
		nextPosition.y = this._transform.localPosition.y ;

		switch(this.currentDirection){
			case MoveDirection.UP:

				if(nextPosition.y >= 0){
					nextPosition.y = this._transform.localPosition.y - this._transform.localPosition.y % Constance.GRID_GAP + Constance.GRID_GAP;
				}else{
					nextPosition.y = this._transform.localPosition.y - this._transform.localPosition.y % Constance.GRID_GAP;
				}
				
				
				break;
			case MoveDirection.DOWN:
				if(nextPosition.y >= 0){
					nextPosition.y = this._transform.localPosition.y - this._transform.localPosition.y % Constance.GRID_GAP;
				}else{
					nextPosition.y = this._transform.localPosition.y - this._transform.localPosition.y % Constance.GRID_GAP - Constance.GRID_GAP;
				}

				
				break;
			case MoveDirection.LEFT:
				if(nextPosition.x >= 0){
					nextPosition.x = this._transform.localPosition.x - this._transform.localPosition.x % Constance.GRID_GAP;
				}else{
					nextPosition.x = this._transform.localPosition.x - this._transform.localPosition.x % Constance.GRID_GAP - Constance.GRID_GAP;
				}
				break;
			case MoveDirection.RIGHT:

				if(nextPosition.x >= 0){
					nextPosition.x = this._transform.localPosition.x - this._transform.localPosition.x % Constance.GRID_GAP + Constance.GRID_GAP;
				}else{
					nextPosition.x = this._transform.localPosition.x - this._transform.localPosition.x % Constance.GRID_GAP;
				}

				break;
		}

		return nextPosition;
	}


	private float GetToNextPositionDistance(){

		switch(this.currentDirection){
			case MoveDirection.DOWN:
			case MoveDirection.UP:
				return Mathf.Abs(this.nextPosition.y - this._transform.localPosition.y);
			case MoveDirection.LEFT:
			case MoveDirection.RIGHT:
				return Mathf.Abs(this.nextPosition.x - this._transform.localPosition.x);
		}

		return 0f;
	}


	public void SetStartPosition(int x , int y){
		this.transform.localPosition = BattleUtils.GridToPosition(x , y);
		UpdatePosition();
	}
	
	
	
	public void SetChararerID(int id){
		this.model.SetID(id);
	}

	public void StopAnimation(){
		this.model.Stop();

		if(this.follower != null){
			this.follower.StopAnimation();
		}
	}

	public void PlayAnimation(){
		this.model.Play();

		
		if(this.follower != null){
			this.follower.PlayAnimation();
		}
	}

	public override void PlayAttacked(){
		if(this.animator == null){
			return;
		}
		
		animator.SetInteger("State" , 1); 
	}

	
	
	public override void PlayAttack(){
		if(this.model == null){
			return;
		}
		
		this.model.PlayAttack();
	}


	public override void PlayDead(){
		Battle.HeroDead(this);
		this.dead = true;

		Destroy(this.hpBar.gameObject);

		if(this.model == null){
			return;
		}


		this.model.PlayDead();
	}

	public override void ChangeHP(float hp){
		this.attribute.hp -= hp;
		
		this.hpBar.SetHP(this.attribute.hp/this.attribute.maxHp);
		
		if(this.attribute.hp <= 0){
			Battle.HeroDead(this);
		}

		GameObject go = (GameObject)Instantiate(this.hp_pop);
		HpPop hpPop = go.GetComponent<HpPop>();
		go.transform.parent = this.transform;
		go.transform.localPosition = new Vector3(0,0,0);
		hpPop.SetValue(hp);
	}


	public override void StopMoving(){
		this.running = false;
	}

	public override void PlayMoving(){
		this.running = true;
	}


	public bool IsInPosition(Vector2 p){
		if(p.x == this.position.x && p.y == this.position.y){
			return true;
		}

		return false;
	}

	public Vector2 GetPoint(){
		return this.position;
	}

	public bool IsDead(){
		return this.dead;
	}

	public override MoveDirection GetDirection(){
		return this.currentDirection;
	}

	
	public override Attribute GetAttribute(){
		return this.attribute;
	}

	public override bool IsInAttIndex(){
		return this.model.IsInAttIndex();
	}
}





















