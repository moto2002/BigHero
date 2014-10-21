using UnityEngine;
using System.Collections;
using LitJson;

public class Follower: Charactor{

	public Object prev;
	public Follower follower;
	public CharModel model;
	public SpriteRenderer spriteRenderer;
	public HPBar hpBar;

	private Queue paths;
	private Queue directions;
	private Vector3 nextPosition;


	[HideInInspector]
	private MoveDirection currentDirection;
	private MoveDirection nextDirection;
	private Transform _transform;

	private Vector2 position;

	private GameObject _moveTarget;
	
	private float skillCD = 1f;
	private float normalSkillCD = 0f;

	private bool running = false;
	private bool dead = false;
	
	private ArrayList attackRange;
	public Attribute attribute;
	public Animator animator;

	private Queue attackTagets = new Queue();
	
	public FollowState state;
	
	private int skillPolicy;
	
	private ArrayList skills;
	
	private int currentSkillIndex;
	
	private int sumOfSkillOdds;

	private SkillConfig normalAttackSkill;

	void Start () {
		paths = new Queue();
		directions= new Queue();
		nextPosition.z = -1;

		_transform = this.gameObject.transform;

		model.direction = this.currentDirection;

		state = FollowState.IDEL;

		attackRange = new ArrayList();
		attackRange.Add(new Vector2());
		attackRange.Add(new Vector2());
		attackRange.Add(new Vector2());
		
		this.normalAttackSkill = Config.GetInstance().GetSkillCOnfig(this.attribute.nskill);

	}

	void Update () {
		
		this.spriteRenderer.sortingOrder = -(int)(this.transform.localPosition.y * 10);

		int x = (int)Mathf.Round(this._transform.localPosition.x / Constance.GRID_GAP);
		int y = -(int)Mathf.Round(this._transform.localPosition.y / Constance.GRID_GAP);

		if(x != this.position.x || y != this.position.y){
			this.position.x = x;
			this.position.y = y;

			Battle.UpdatePosition(this ,this.position);
		}

		
		if(this.dead == true){
			return;
		}


		UpdateState();
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


	private void TryAttak(){
		TryNormalAttack();
		this.TrySkillAttack();
	}
	
	
	private void TryNormalAttack(){
		if(normalAttackSkill == null){
			return;
		}

		if(normalSkillCD > 0){
			normalSkillCD -= Time.deltaTime;
			return;
		}
		
		if(this.model.currentState == CharModel.State.ATTACK){
			return;
		}
		
		
		ArrayList points  = AttRange.GetRangeByAttType(this.normalAttackSkill.attack_type , this.normalAttackSkill.range ,  this.attribute.volume , this.position);
		
		for(int i = 0 ; i < points.Count ; i++){
			ArrayList gameObjects = Battle.GetGameObjectsByPosition((Vector2)points[i]);
			
			for(int j = 0 ; j < gameObjects.Count ; j++){
				
				Charactor c = (Charactor)gameObjects[j];
				
				if(c.IsActive() == true && c.GetType() != this.GetType()){
					normalSkillCD = 1;
					SkillManager.PlaySkill(this , normalAttackSkill , c);
				}
			}
			
		}
	}


	
	private void TrySkillAttack(){
		if(this.skillCD > 0){
			this.skillCD -= Time.deltaTime;
			return;
		}
		
		if(skills.Count == 0){
			return;
		}
		
		if(this.model.currentState == CharModel.State.ATTACK){
			return;
		}
		
		SkillItem skillItem = null;
		
		if(this.skillPolicy == 1){
			skillItem = skills[currentSkillIndex] as SkillItem;
			currentSkillIndex++;
			
			if(currentSkillIndex >= skills.Count){
				currentSkillIndex = 0;
			}
		}else if(this.skillPolicy == 2){
			int odds  = Random.Range(0 , this.sumOfSkillOdds);
			
			for(int i = 0 ; i < this.skills.Count; i++){
				skillItem = (SkillItem)skills[i];
				
				if(skillItem.odds >= odds){
					break;
				}
				
				odds -= skillItem.odds;
			}
		}
		
		
		if(skillItem == null){
			this.skillCD = 1;
			return;
		}
		
		
		this.skillCD = skillItem.cd;
		
		SkillConfig skillConfig = Config.GetInstance().GetSkillCOnfig(skillItem.skillId);
		
		if(skillConfig == null){
			return;
		}
		
		skillConfig.b = skillItem.b;
		SkillManager.PlaySkill(this , skillConfig);
	}


	public void SetDirection(MoveDirection direction){
		this.currentDirection = direction;


		if(this.model != null)this.model.direction = direction;

		if(this.follower != null){
			this.follower.SetDirection(direction);
		}
	}

	public override MoveDirection GetDirection(){
		return this.currentDirection;
	}


	public void move(float distance){

		if(dead == true){
			return;
		}


		if(this.state == FollowState.WAIT_CLOSE){
			//wait the targe move


			if(this._moveTarget.tag == "Hero"){
				Hero hero = this._moveTarget.GetComponent<Hero>();

				if(hero.GetPoint() == this.position){
					this.state = FollowState.WAIT_FOLLOW;
				}
				
			}else if(this._moveTarget.tag == "Follower"){
				Follower f = this._moveTarget.GetComponent<Follower>();

				if(f.GetPoint() == this.position){
					this.state = FollowState.WAIT_FOLLOW;
				}
			}

		}else if(this.state == FollowState.WAIT_FOLLOW){
			float d = GetMoveDistance(this._moveTarget, this.gameObject);


			if(d >= Constance.GRID_GAP + 0.1f){
				this.state = FollowState.FOLLOW;
				
				if(this._moveTarget.tag == "Hero"){

					Hero hero = this._moveTarget.GetComponent<Hero>();
					this.SetDirection(hero.currentDirection);

				}else if(this._moveTarget.tag == "Follower"){

					Follower f = this._moveTarget.GetComponent<Follower>();
					this.SetDirection(f.GetDirection());
				}
				
				this.running = true;
				this.PlayAnimation();
				this._moveTarget = null;
			}else{
				return;
			}
		}


		if(this.running == false){
			//play standing animation
			return;
		}

		float d2 = GetMoveDistance(((MonoBehaviour)this.prev).gameObject, this.gameObject);

		if(d2 > Constance.GRID_GAP + 0.1f){

			if(d2 - Constance.GRID_GAP + 0.1f > 0.008f){
				distance += 0.008f;
			}else{
				distance += d2 - Constance.GRID_GAP + 0.1f;
			}
		}

		if(nextPosition.z == -1 && this.paths.Count > 0){
			this.nextPosition = (Vector3)this.paths.Dequeue();
			this.nextDirection = (MoveDirection)this.directions.Dequeue();
		}

		if(nextPosition.z == -1){
			_move(distance);
		}else{

			float toNext = GetToNextPositionDistance();

			while(distance >  toNext){

				_move(toNext);
				distance -= toNext;

				this.currentDirection = this.nextDirection;
				this.model.direction = this.currentDirection;

				if(this.paths.Count > 0){
					this.nextPosition = (Vector3)this.paths.Dequeue();
					this.nextDirection = (MoveDirection)this.directions.Dequeue();
				}

				toNext = GetToNextPositionDistance();

				if(toNext == 0){
					this.nextPosition.z = -1;
					break;
				}
			}

			_move(distance);
		}

		
		
		if(this.follower != null){
			this.follower.move (distance);
		}
	}

	private float GetToNextPositionDistance(){
		
		switch(this.currentDirection){
		case MoveDirection.DOWN:
		case MoveDirection.UP:
			return Mathf.Abs(this.nextPosition.y - this.transform.localPosition.y);
		case MoveDirection.LEFT:
		case MoveDirection.RIGHT:
			return Mathf.Abs(this.nextPosition.x - this.transform.localPosition.x);
		}
		
		return 0f;
	}


	private void _move(float distance){

		Vector3 v = this._transform.localPosition;


		switch(this.currentDirection){
		case MoveDirection.DOWN:
			v.y = this._transform.localPosition.y - distance;
			break;
		case MoveDirection.UP:
			v.y = this._transform.localPosition.y + distance;
			break;
		case MoveDirection.LEFT:
			v.x = this._transform.localPosition.x - distance;
			break;
		case MoveDirection.RIGHT:
			v.x = this._transform.localPosition.x + distance;
			break;
		}

		this._transform.localPosition = v;
	}


	public void SetFollowTarget(Hero hero){

		if(hero.follower == null){
			this._moveTarget = hero.gameObject;
			hero.follower = this;
			this.prev = hero;
		}else{
			Follower f = hero.follower;
			
			while(f.follower != null){
				f = f.follower;
			}
			this._moveTarget = f.gameObject;
			f.follower = this;
			this.prev = f;
		}
	}

	public void SetNextPosition(Vector3 position , MoveDirection direction){
		if(this.follower != null){
			this.follower.SetNextPosition (position , direction);
		}

		paths.Enqueue(position);
		directions.Enqueue(direction);
	}


	public void SetSkills(JsonData jsonSkills){
		sumOfSkillOdds = 0;
		
		this.skills = new ArrayList();
		
		for(int i = 0 ; i < jsonSkills.Count ; i++){
			SkillItem skillItem = new SkillItem();
			
			skillItem.skillId = int.Parse(jsonSkills[i]["id"].ToString());
			skillItem.cd = int.Parse(jsonSkills[i]["cd"].ToString());
			skillItem.odds = int.Parse(jsonSkills[i]["odds"].ToString());
			skillItem.b = int.Parse(jsonSkills[i]["b"].ToString());
			
			sumOfSkillOdds += skillItem.odds;
			
			this.skills.Add(skillItem);
		}
	}
	
	public void SetSkillPolicy(int policy){
		this.skillPolicy = policy;
	}

	public void SetPosition(Vector3 v){
		this.transform.localPosition = v;
	}

	public void SetCharaterID(int id){
		this.model.SetID(id);
	}
	
	public override void StopMoving(){
		this.running = false;
	}
	
	public override void PlayMoving(){
		this.running = true;
	}
	
	public override int GetType(){
		return TYPE_FOLLOWER;
	}

	public void StopAnimation(){
		this.model.Stop();
		
		if(this.follower != null){
			this.follower.StopAnimation();
		}
	}
	
	public void PlayAnimation(){
		if(this.state != FollowState.FOLLOW){
			return;
		}

		this.model.Play();

		if(this.follower != null){
			this.follower.PlayAnimation();
		}
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


	public override void PlayAttacked(){
		if(this.animator == null){
			return;
		}
		
		animator.SetInteger("State" , 1); 
	}

	public override void ChangeHP(float hp){
		this.attribute.hp -= hp;
		
		this.hpBar.SetHP(this.attribute.hp/this.attribute.maxHp);
		
		if(this.attribute.hp <= 0){
			PlayDead();
		}
	}

	public override void PlayDead(){
		dead = true;

		if(this.prev is Hero){
			((Hero)this.prev).follower = this.follower;
		}else if(this.prev is Follower){
			((Follower)this.prev).follower = this.follower;
		}

		if(this.follower != null){
			this.follower.prev = this.prev;
		}

		this.state = FollowState.DEAD;
		Battle.RemoveFollower(this);

		if(this.model == null){
			return;
		}

		this.model.PlayDead();
	}
	
	
	public override Vector2 GetPoint(){
		return this.position;
	}


	public bool isCloseToMe(Vector2 p , MoveDirection d){

		switch(d){
		case MoveDirection.UP:
			if(p.y < this.transform.localPosition.y){
				return true;
			}else{
				return false;
			}
		case MoveDirection.DOWN:
			if(p.y > this.transform.localPosition.y){
				return true;
			}else{
				return false;
			}
		case MoveDirection.LEFT:
			if(p.x > this.transform.localPosition.x){
				return true;
			}else{
				return false;
			}
		case MoveDirection.RIGHT:
			if(p.x < this.transform.localPosition.x){
				return true;
			}else{
				return false;
			}
		}

		return false;
	}

	
	public override bool IsActive (){
		if(this.state == FollowState.FOLLOW){
			return true;
		}

		return false;
	}


	public override Attribute GetAttribute(){
		return this.attribute;
	}

	public override bool IsInAttIndex(){
		return this.model.IsInAttIndex();
	}


	private float GetMoveDistance(GameObject object1 , GameObject object2){
		return Mathf.Abs(object1.transform.localPosition.x - object2.transform.localPosition.x) + Mathf.Abs(object1.transform.localPosition.y - object2.transform.localPosition.y);
	}
}






