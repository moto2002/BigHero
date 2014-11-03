using UnityEngine;
using System.Collections;
using LitJson;

public class Follower: Charactor{
	
	private bool specSign = false;

	public Object prev;
	public Follower follower;
	public CharModel model;
	public SpriteRenderer spriteRenderer;
	public HPBar hpBar;

	public int followIndex = 0;

	public Queue paths;

	private Vector3 nextPosition;

	[HideInInspector]
	private MoveDirection currentDirection;
	private MoveDirection nextDirection;
	private Transform _transform;

	private Vector2 position;
	
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

	public CharactorEffect effectObject;

	void Start () {
		paths = new Queue();

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

			BattleControllor.UpdatePosition(this ,this.position);
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

		if(this.state != FollowState.FOLLOW){
			return;
		}
		
		if(this.model.currentState == CharModel.State.ATTACK){
			return;
		}
		
		
		ArrayList points  = AttRange.GetRangeByAttType(this.normalAttackSkill.attack_type , this.normalAttackSkill.range ,  this.attribute.volume , this.position , this.currentDirection);
		
		for(int i = 0 ; i < points.Count ; i++){
			ArrayList gameObjects = BattleControllor.GetGameObjectsByPosition((Vector2)points[i]);
			
			for(int j = 0 ; j < gameObjects.Count ; j++){
				
				Charactor c = (Charactor)gameObjects[j];
				
				if(c.IsActive() == false){
					continue;
				}
				
				if(normalAttackSkill.target > 2){
					if(c.GetType() == this.GetType()){
						normalSkillCD = normalAttackSkill.cd;
						
						normalAttackSkill.crit = attribute.crit;
						SkillManager.PlaySkill(this , normalAttackSkill , c);
						return;
					}
				}else{
					if(c.GetType() != this.GetType()){
						normalSkillCD = normalAttackSkill.cd;
						
						normalAttackSkill.crit = attribute.crit;
						SkillManager.PlaySkill(this , normalAttackSkill , c);
						return;
					}
				}
			}
			
		}
	}


	
	private void TrySkillAttack(){
		if(this.skillCD > 0){
			this.skillCD -= Time.deltaTime;
			return;
		}

		if(skills == null)return;
		if(skills.Count == 0)return;
		if(this.model.currentState == CharModel.State.ATTACK)return;
		
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


	
	public void Move(float distance){
		
		if(this.state == FollowState.DEAD){
			return;
		}

		float d = GetTotalDistance(BattleControllor.hero.transform.localPosition , this.transform.localPosition);

		if(this.state == FollowState.WAIT_FOLLOW){
			if(d < this.followIndex * Constance.GRID_GAP){
				return;
			}

			this.running = true;
			this.PlayAnimation();
			
			d = GetTotalDistance(BattleControllor.hero.transform.localPosition , this.transform.localPosition);

			this.state = FollowState.FOLLOW;
			this.running = true;
			this.PlayAnimation();
		}

		if(d > this.followIndex * Constance.GRID_GAP + 0.02){
			distance += 0.002f;
		}else if(d < this.followIndex * Constance.GRID_GAP){
			distance -= 0.002f;
		}

		if(nextPosition.z == -1 && this.paths.Count > 0){
			this.currentDirection = FindDirection();
			this.model.direction = this.currentDirection;
		}
		
		if(nextPosition.z == -1){
			_Move(distance);
		}else{
			
			float toNext = GetToNextPositionDistance();

			while(distance >  toNext){
				
				_Move(toNext);

				distance -= toNext;

				if(this.paths.Count > 0){
					this.currentDirection = FindDirection();
					this.model.direction = this.currentDirection;
				}
				
				toNext = GetToNextPositionDistance();
				
				if(toNext == 0){
					this.nextPosition.z = -1;
					this.currentDirection = BattleControllor.hero.GetDirection();
					this.model.direction = this.currentDirection;
					break;
				}
			}
			
			_Move(distance);
		}
		
		
		
		if(this.follower != null){
			this.follower.Move (distance);
		}
	}

	private float GetTotalDistance(Vector3 point1 , Vector3 point3){
		
		float distance = 0f;
		
		foreach(Vector3 point2 in paths){
			
			distance += GetDistance(point1 , point2);
			point1 = point2;
		}

		if(this.nextPosition.z != -1){
			distance += GetDistance(point1 , this.nextPosition);

			point1 =  this.nextPosition;
		}
		
		distance += GetDistance(point1 , point3);

		
		return distance;
	}
	
	
	private  float GetDistance(Vector3 v1 , Vector3 v2){
		return Mathf.Abs(v1.y - v2.y) + Mathf.Abs(v1.x - v2.x);
	}

	private float GetToNextPositionDistance(){
		
		switch(this.currentDirection){
		case MoveDirection.DOWN:
		case MoveDirection.UP:
			return Mathf.Abs(this.nextPosition.y - this.transform.position.y);
		case MoveDirection.LEFT:
		case MoveDirection.RIGHT:
			return Mathf.Abs(this.nextPosition.x - this.transform.position.x);
		}
		
		return 0f;
	}


	private MoveDirection FindDirection(){
		Vector3 next = (Vector3)this.paths.Peek();
		Vector3 localPosition = transform.localPosition;

		if(localPosition.x != next.x && localPosition.y != next.y){
			next = new Vector3(localPosition.x , next.y);
		}else{
			next = (Vector3)this.paths.Dequeue();
		}

		nextPosition = next;

		if(this.nextPosition.x == localPosition.x){
			if(this.nextPosition.y > localPosition.y){
				return MoveDirection.UP;
			}else{
				return MoveDirection.DOWN;
			}
		
		}else{
			if(this.nextPosition.x > localPosition.x){
				return MoveDirection.RIGHT;
			}else{
				return MoveDirection.LEFT;
			}
		}

		return MoveDirection.DOWN;
	}


	private void _Move(float distance){

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
			hero.follower = this;
			this.prev = hero;

			followIndex = 1;

		}else{
			Follower f = hero.follower;

			followIndex = 2;

			while(f.follower != null){
				f = f.follower;
				followIndex++;
			}


			f.follower = this;
			this.prev = f;
		}

		
		this.state = FollowState.WAIT_FOLLOW;

		Vector3 heroPosition = hero.gameObject.transform.localPosition;
		Vector3 midPosition = Vector3.zero;

		switch(hero.GetDirection()){
		case MoveDirection.DOWN:
			midPosition = new Vector3(this.transform.localPosition.x , heroPosition.y , 0);
			break;
		case MoveDirection.UP:
			midPosition = new Vector3(this.transform.localPosition.x , heroPosition.y , 0);
			break;
		case MoveDirection.LEFT:
			midPosition = new Vector3(heroPosition.x , this.transform.localPosition.y);
			break;
		case MoveDirection.RIGHT:
			midPosition = new Vector3(heroPosition.x , this.transform.localPosition.y);
			break;
		}

		
		SetNextPosition(midPosition);
		SetNextPosition(heroPosition);
	}

	public void SetNextPosition(Vector3 position){

		if(this.follower != null){
			this.follower.SetNextPosition (position);
		}

		paths.Enqueue(position);
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

	public override void ChangeHP(float hp , bool crit){
		this.attribute.hp -= hp;

		if(this.attribute.hp > this.attribute.maxHp){
			this.attribute.hp = this.attribute.maxHp;
		}
		
		this.hpBar.SetHP(this.attribute.hp/this.attribute.maxHp);
		
		if(this.attribute.hp <= 0){
			PlayDead();
		}

		this.effectObject.PlayNum((int)hp , crit);
	}

	public override void PlayDead(){
		if(this.dead == true){
			return;
		}

		if(this.animator != null)this.animator.SetInteger("State" , 0);

		dead = true;

		Follower f = this;

		while(f.follower != null){
			f.follower.followIndex -= 1;
			f = f.follower;
		}

		if(this.prev is Hero){
			((Hero)this.prev).follower = this.follower;
		}else if(this.prev is Follower){
			((Follower)this.prev).follower = this.follower;
		}

		if(this.follower != null){
			this.follower.prev = this.prev;
		}

		this.state = FollowState.DEAD;
		BattleControllor.RemoveFollower(this);

		if(this.model == null){
			return;
		}

		this.model.PlayDead();
		
		StartCoroutine(Remove()); 
	}

	
	
	private IEnumerator Remove(){
		yield return new WaitForSeconds(1.5f);
		Destroy(this.gameObject);
	}
	
	
	public override Vector2 GetPoint(){
		return this.position;
	}

	public override void SetSpec(bool b){
		this.specSign = b;

		if(b){
			this.model.Play();
		}else{
			this.model.Stop();
		}
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

	public bool HasNextPoint(){

		if(this.nextPosition.z == -1){
			return false;
		}

		return true;
	}

	public void ResetPostion(){
		paths = new Queue();
		nextPosition = new Vector3(0,0,-1);
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






