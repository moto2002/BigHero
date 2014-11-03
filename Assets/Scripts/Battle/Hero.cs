using UnityEngine;
using System.Collections;
using LitJson;
using System.Collections.Generic;

public class Hero : Charactor{
	
	
	private bool specSign = false;

	public float speed = 0.7f;

	public Follower follower;

	[HideInInspector]
	public Vector2 nextPosition = new Vector2(0,0); 
	
	[HideInInspector]
	public float attackCD = 1f;
	
	private float skillCD = 1;

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

	public CharactorEffect effectObject;
	
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

	public MoveDirection direction;

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

		normalAttackSkill = Config.GetInstance().GetSkillCOnfig(1001);
	}
	
	// Update is called once per frame
	void Update () {
		if( Constance.RUNNING == false){
			return;
		}

		if(this.dead == true){
			return;
		}

		UpdateState();
		SortY();

		if(BattleControllor.state != BattleState.STATE_BATTLING){
			return;
		}

		if(skillCD > 0){
			skillCD -= Time.deltaTime;
		}

		UpdatePosition();
		TryMove();
		TryAttak();
	}

	

	private void UpdateState(){
		if(this.animator == null){
			return;
		}

		if( this.animator.GetCurrentAnimatorStateInfo(0).IsName("Shake") && this.animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.1f){
			this.animator.SetInteger("State" , 0);
		}
	}


	private void SortY(){
		this.spriteRenderer.sortingOrder = -(int)(this.transform.localPosition.y * 10);
	}


	private void UpdatePosition(){
		
		int x = (int)Mathf.Round(this.transform.localPosition.x / Constance.GRID_GAP);
		int y = -(int)Mathf.Round(this.transform.localPosition.y / Constance.GRID_GAP);
		
		if(x != this.position.x || y != this.position.y){
			this.position.x = x;
			this.position.y = y;
			
			BattleControllor.UpdatePosition(this ,this.position);

			FindFollower();
			TryPickUpItem();
		}
	}

	private void FindFollower(){
		ArrayList followers = BattleControllor.GetFollowersByPoint(this.position);

		if(followers == null){
			return;
		}
		
		foreach(Follower follower in followers){
			
			if(follower.state == FollowState.IDEL){
				follower.SetFollowTarget(this);
			}
		}
	}
	
	
	private void TryPickUpItem(){
		ArrayList items = BattleControllor.GetItemByPoint(this.position);
		
		foreach(Item item in items){
			
			if(this.position == item.GetPoint()){
				item.Pick(this);
			}
		}
	}
	

	private void TryMove(){
		if(Constance.RUNNING == false){
			return;
		}
		
		if(this.running == false){
			return;
		}

		this.nextPosition = GetNextPosition();
		
		Vector2 nextPoint = BattleUtils.PositionToGrid(this.nextPosition.x , this.nextPosition.y);
		
		
		if(BattleControllor.IsMoveable(nextPoint) == false){
			if(this.currentDirection != this.direction){
				this.currentDirection = this.direction;
				
				if(this.follower != null)follower.SetNextPosition(this._transform.localPosition);
			}
			return;
		}
		
		//下个格子中的物体
		ArrayList monsterList = BattleControllor.GetMonstersByPoint(nextPoint);
		
		if(this.position != nextPoint && monsterList.Count > 0){
			//前方有障碍物 需要停止
			
			if(this.currentDirection != this.direction){
				this.currentDirection = this.direction;
				
				if(this.follower != null)follower.SetNextPosition(this._transform.localPosition);
			}
			
			return;
		}
		
		
		float moveDistance = Time.deltaTime * speed;
		
		if(this.currentDirection != this.direction){
			this.currentDirection = this.direction;
			
			if(this.follower != null)follower.SetNextPosition(this._transform.localPosition);
		}


		Move(moveDistance);

		
		if(follower != null){
			follower.Move(moveDistance);
		}
		

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
		if(this.attackCD > 0){
			this.attackCD -= Time.deltaTime;
			return;
		}
		
		ArrayList points  = AttRange.GetRangeByAttType(normalAttackSkill.attack_type , normalAttackSkill.range ,  this.attribute.volume , this.position , this.currentDirection);
		
		for(int i = 0 ; i < points.Count ; i++){
			ArrayList gameObjects = BattleControllor.GetGameObjectsByPosition((Vector2)points[i]);
			
			for(int j = 0 ; j < gameObjects.Count ; j++){
				
				Charactor c = (Charactor)gameObjects[j];
				
				if(c.IsActive() == false){
					continue;
				}
				
				if(normalAttackSkill.target > 2){
					if(c.GetType() == this.GetType()){
						attackCD = normalAttackSkill.cd;
						
						normalAttackSkill.crit = attribute.crit;
						SkillManager.PlaySkill(this , normalAttackSkill , c);
						return;
					}
				}else{
					if(c.GetType() != this.GetType()){
						attackCD = normalAttackSkill.cd;
						
						normalAttackSkill.crit = attribute.crit;
						SkillManager.PlaySkill(this , normalAttackSkill , c);
						return;
					}
				}
			}
		}
	}



	public Skill playSkill(int index){
		if(skillCD > 0){
			return null;
		}

		SkillConfig skillConfig = Config.GetInstance().GetSkillCOnfig(1006);
		this.skillCD = 2;

		return SkillManager.PlaySkill(this , skillConfig);
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


	public override void PlaySkillAttack(){
		if(this.model == null){
			return;
		}
		
		this.model.PlaySkillAttack();
	}


	public override void PlayDead(){
		if(this.dead == true){
			return;
		}
		
		if(this.animator != null)this.animator.SetInteger("State" , 0);

		BattleControllor.HeroDead(this);
		this.dead = true;

		Destroy(this.hpBar.gameObject);

		if(this.model == null){
			return;
		}


		this.model.PlayDead();
	}

	public override void ChangeHP(float hp , bool crit){
		this.attribute.hp -= hp;
		
		if(this.attribute.hp > this.attribute.maxHp){
			this.attribute.hp = this.attribute.maxHp;
		}

		this.hpBar.SetHP(this.attribute.hp/this.attribute.maxHp);
		
		if(this.attribute.hp <= 0){
			BattleControllor.HeroDead(this);
		}

		this.effectObject.PlayNum((int)hp , crit);
	}


	public override void StopMoving(){
		this.running = false;
	}

	public override void PlayMoving(){
		this.running = true;
	}

	public override int GetType(){
		return TYPE_HERO;
	}

	public bool IsInPosition(Vector2 p){
		if(p.x == this.position.x && p.y == this.position.y){
			return true;
		}

		return false;
	}

	public override Vector2 GetPoint(){
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

	public override bool IsActive (){
		return this.dead == false;
	}
	
	public override void SetSpec(bool b){
		this.specSign = b;
		this.model.SetSpec(b);

	}


	public override void PlayEffect(int type){
		effectObject.PlayEffect(type);
	}
}





















