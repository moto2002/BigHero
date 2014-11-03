using UnityEngine;
using System.Collections;
using LitJson;

public class Monster : Charactor{

	
	private bool specSign = false;

	public GameObject model;

	public Animator animator;
	
	public CharModel charModel;

	public SpriteAnimation teleport;

	public SpriteRenderer spriteRenderer;

	private MoveDirection currentDirection = MoveDirection.DOWN;

	private ArrayList pathPoints;

	private int pathIndex = 0;

	private float speed;

	public int dropItemId;

	public int dropOdds;
	
	private PathPoint lastPathPoint;
	private PathPoint nextPathPoint;

	private Vector2 position;

	public Attribute attribute;

	public HPBar hpBar;

	private float skillCD = 1;
	private float normalSkillCD = 0;

	private bool dead = false;

	private bool running = true;

	private SkillConfig normalAttackSkill;

	private int skillPolicy;

	private ArrayList skills;

	private int currentSkillIndex;

	private int sumOfSkillOdds;

	public CharactorEffect effectObject;

	// Use this for initialization
	void Start () {

		this.normalAttackSkill = Config.GetInstance().GetSkillCOnfig(this.attribute.nskill);

		this.charModel.direction = this.currentDirection;
		this.charModel.Stop();
		this.charModel.gameObject.SetActive(false);
		this.hpBar.gameObject.SetActive(false);

		teleport.sprites = Resources.LoadAll<Sprite>(@"Image/Teleport");

		this.model.gameObject.transform.localPosition = new Vector2( (this.attribute.volume/2.0f - 0.5f) * Constance.GRID_GAP , -(this.attribute.volume/2.0f - 0.5f) * Constance.GRID_GAP);
	}
	
	// Update is called once per frame
	void Update () {
		if(Constance.SPEC_RUNNING == false && Constance.RUNNING == false){
			return;
		}else if(Constance.SPEC_RUNNING == true && this.specSign == false){
			return;
		}

		if(teleport != null){

			if(teleport.IsEnd() == true){
				Destroy(teleport.gameObject);
				teleport = null;
				this.charModel.Play();
				this.charModel.gameObject.SetActive(true);
				this.hpBar.gameObject.SetActive(true);
			}

			return;
		}

		this.UpdateState();

		if(this.dead == true){
			return;
		}

		this.TryMove();
		this.SortY();
		this.TryAttack();
	}

	private void SortY(){
		this.spriteRenderer.sortingOrder = -(int)(this.transform.localPosition.y * 10 + (this.attribute.volume/2.0f - 0.5f));
	}

	
	private void UpdatePosition(){

		
		int x = (int)Mathf.Round(this.transform.localPosition.x / Constance.GRID_GAP);
		int y = -(int)Mathf.Round(this.transform.localPosition.y / Constance.GRID_GAP);
		
		if(x != this.position.x || y != this.position.y){
			this.position.x = x;
			this.position.y = y;
			
			BattleControllor.UpdatePosition(this ,this.position , this.attribute.volume);
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

	private void TryMove(){

		if(this.running == false){
			return;
		}

		if(this.charModel.currentState != CharModel.State.MOVE ){
			return;
		}

		UpdatePosition();

		if(this.pathPoints == null || this.pathPoints.Count <= 1){
			return;
		}

		
		if(nextPathPoint == null){
			this.lastPathPoint = (PathPoint)this.pathPoints[0];
			this.nextPathPoint = (PathPoint)this.pathPoints[1];
			this.pathIndex = 1;
			this.charModel.direction = this.currentDirection = FindDirection(lastPathPoint , nextPathPoint);
		}


		if(this.position != nextPathPoint.point  && MoveAble() == false){
			//前方有障碍物 需要停止 
			return;
		}

		
		float toNext = GetToNextPositionDistance(nextPathPoint);
		
		float distance = Time.deltaTime * speed;

		while(distance >  toNext){
			
			move(toNext);
			distance -= toNext;

			this.lastPathPoint = (PathPoint)this.pathPoints[pathIndex];

			if(this.pathPoints.Count == ++pathIndex){
				pathIndex = 0;
			}

			this.nextPathPoint = (PathPoint)this.pathPoints[pathIndex];
			this.charModel.direction = this.currentDirection = FindDirection(lastPathPoint , nextPathPoint);
			
			toNext = GetToNextPositionDistance(nextPathPoint);
		}
		
		move(distance);
	}

	private bool MoveAble(){

		switch(this.currentDirection){
		case MoveDirection.DOWN:

			Vector2 v = nextPathPoint.point;
			v.y = v.y + this.attribute.volume - 1;

			for(int i = 0 ; i < this.attribute.volume ; i++){

				v.x = v.x + i;

				if(BattleControllor.IsMoveable(v) == false){
					return false;
				}

				if(BattleControllor.GetGameObjectsByPosition(v).Count > 0){
					return false;
				}
			}
			break;

		case MoveDirection.UP:
			
			v = nextPathPoint.point;

			for(int i = 0 ; i < this.attribute.volume ; i++){
				v.x = v.x + i;
				
				if(BattleControllor.IsMoveable(v) == false){
					return false;
				}

				if(BattleControllor.GetGameObjectsByPosition(v).Count > 0){
					return false;
				}
			}
			break;
			
			
		case MoveDirection.LEFT:
			
			v = nextPathPoint.point;
			
			for(int i = 0 ; i < this.attribute.volume ; i++){
				v.y = v.y + i;
				
				if(BattleControllor.IsMoveable(v) == false){
					return false;
				}

				if(BattleControllor.GetGameObjectsByPosition(v).Count > 0){
					return false;
				}
			}
			
			break;
			
			
		case MoveDirection.RIGHT:
			
			v = nextPathPoint.point;
			v.x = v.x + this.attribute.volume - 1;
			
			for(int i = 0 ; i < this.attribute.volume ; i++){
				v.y = v.y + i;
				
				if(BattleControllor.IsMoveable(v) == false){
					return false;
				}

				if(BattleControllor.GetGameObjectsByPosition(v).Count > 0){
					return false;
				}
			}
			
			break;
		}


		return true;
	}

	private void TryAttack(){
		this.TryNormalAttack();
		this.TrySkillAttack();
	}


	private void TryNormalAttack(){
		if(normalSkillCD > 0){
			normalSkillCD -= Time.deltaTime;
			return;
		}

		if(this.charModel.currentState == CharModel.State.ATTACK){
			return;
		}


		ArrayList points  = AttRange.GetRangeByAttType(normalAttackSkill.attack_type , this.normalAttackSkill.range ,  this.attribute.volume , this.position , this.currentDirection);
		
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

		if(skills.Count == 0){
			return;
		}

		if(this.charModel.currentState == CharModel.State.ATTACK){
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
		
		
		SkillConfig skillConfig = Config.GetInstance().GetSkillCOnfig(skillItem.skillId);
		
		this.skillCD = skillItem.cd + skillConfig.singTime;
		
		if(skillConfig == null){
			return;
		}
		
		skillConfig.b = skillItem.b;
		SkillManager.PlaySkill(this , skillConfig);
	}




	private void move(float distance){
		Vector3 position = this.transform.localPosition;
		
		
		switch(this.currentDirection){
		case MoveDirection.DOWN:
			position.y = this.transform.localPosition.y - distance;
			break;
			
		case MoveDirection.UP:
			position.y = this.transform.localPosition.y + distance;
			break;
			
			
		case MoveDirection.LEFT:
			position.x = this.transform.localPosition.x - distance;
			break;
			
		case MoveDirection.RIGHT:
			position.x = this.transform.localPosition.x + distance;
			break;
		}
		
		this.transform.localPosition = position;
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

	public void SetDirection(MoveDirection direction){
		this.currentDirection = direction;
		if(this.charModel != null)this.charModel.direction = direction;
	}
	
	public override MoveDirection GetDirection(){
		return this.currentDirection;
	}

	public void SetSpeed(float value){
		this.speed = value;
	}

	public float GetSpeed(){
		return speed;
	}

	public void SetMovePath(JsonData path){

		pathPoints = new ArrayList();

		for(int i = 0 ; i < path.Count ; i++){

			string[] p = path[i].ToString().Split(',');

			PathPoint pp = new PathPoint();

			pp.x = int.Parse(p[0]);
			pp.y = int.Parse(p[1]);
			pp.stayTime = p.Length == 3? float.Parse(p[2]) : 0;

			pathPoints.Add(pp);
		}

		if(pathPoints.Count > 0){
			if(((PathPoint)pathPoints[0]).Equals((PathPoint)pathPoints[pathPoints.Count - 1]) == false){
				for(int i = pathPoints.Count - 2 ; i > 0; i--){
					pathPoints.Add (pathPoints[i]);
				}
			}
		}
	}

	private float GetToNextPositionDistance(PathPoint pathPoint){

		Vector2 p = ToPosition(pathPoint);

		switch(this.currentDirection){
		case MoveDirection.DOWN:
		case MoveDirection.UP:
			return Mathf.Abs(p.y - this.transform.localPosition.y);
		case MoveDirection.LEFT:
		case MoveDirection.RIGHT:
			return Mathf.Abs(p.x - this.transform.localPosition.x);
		}
		
		return 0f;
	}

	private Vector2 ToPosition(PathPoint pathPoint){
		Vector2 v = new Vector2(pathPoint.x * Constance.GRID_GAP , -pathPoint.y * Constance.GRID_GAP);
		return v;
	}


	private MoveDirection FindDirection(PathPoint p1 ,PathPoint p2){
		if(p1.x == p2.x){
			if(p2.y > p1.y){
				return MoveDirection.DOWN;
			}else if(p2.y < p1.y){
				return MoveDirection.UP;
			}
		}else if(p1.y == p2.y){
			if(p2.x > p1.x){
				return MoveDirection.RIGHT;
			}else if(p2.x < p1.x){
				return MoveDirection.LEFT;
			}
		}

		return this.currentDirection;
	}

	public void SetPosition(Vector2 p){
		this.transform.localPosition = p;
	}

	public override Vector2 GetPoint(){
		return this.position;
	}

	public void setCharaterID(int id){
		this.charModel.SetID(id);
	}

	public override void PlayAttacked(){
		if(this.animator == null){
			return;
		}

		animator.SetInteger("State" , 1); 
	}


	public override void ChangeHP(float hp , bool crit){
		if(this.dead){
			return;
		}

		this.attribute.hp -= hp;
		
		if(this.attribute.hp > this.attribute.maxHp){
			this.attribute.hp = this.attribute.maxHp;
		}

		this.hpBar.SetHP(this.attribute.hp/this.attribute.maxHp);

		this.effectObject.PlayNum((int)hp , crit);
	}
	
	
	public override void StopMoving(){
		this.running = false;
	}
	
	public override void PlayMoving(){
		this.running = true;
	}
	
	
	public override int GetType(){
		return TYPE_MONSTER;
	}

	public void PlayMove(){
		if(this.charModel == null){
			return;
		}
	}

	public override void PlayAttack(){
		if(this.charModel == null){
			return;
		}
		
		this.charModel.PlayAttack();
	}

	public override void PlaySkillAttack(){
		if(this.model == null){
			return;
		}

		this.charModel.PlaySkillAttack();
	}

	public override void PlayDead(){
		if(this.dead == true){
			return;
		}

		this.dead = true;

		if(this.animator != null)this.animator.SetInteger("State" , 0);

		if(this.charModel == null){
			return;
		}

		if(this.hpBar != null)Destroy(this.hpBar.gameObject);
		
		BattleControllor.RemoveMonster(this);

		this.charModel.PlayDead();

		StartCoroutine(Remove()); 
	}


	private IEnumerator Remove(){
		yield return new WaitForSeconds(1.5f);
		Destroy(this.gameObject);

		if(this.dropItemId != null){

			if(Random.Range(0 , 10000) < this.dropOdds){
				BattleControllor.AddItem(this.dropItemId , this.transform.localPosition);
			}
		}
	}



	public override void SetPlayLock(bool b){
		if(this.model == null){
			return;
		}
		
		this.charModel.SetPlayLock(b);
	}
	
	public bool IsDead(){
		return this.dead;
	}

	
	public override Attribute GetAttribute(){
		return this.attribute;
	}

	public override bool IsInAttIndex(){
		return this.charModel.IsInAttIndex();
	}

	
	public override bool IsActive (){
		return this.dead == false;
	}


	public override void SetSpec(bool b){
		this.specSign = b;

		if(b){
			this.charModel.Play();
		}else{
			this.charModel.Stop();
		}
	}
}


class PathPoint{
	public int x;
	public int y;
	public float stayTime;

	private Vector2 p;
	public Vector2 point{
		get{
			p.x = this.x;
			p.y = this.y;

			return p;
		}
	}

	public PathPoint(){
	}


	public bool Equals(PathPoint p){
		if(p.x == this.x && p.y == this.y){
			return true;
		}

		return false;
	}


}



class SkillItem{

	public int skillId;

	public int cd;

	public int odds;

	public int b;
}


