using UnityEngine;
using System.Collections;
using LitJson;

public class Monster : Charactor{


	public GameObject model;

	public CharModel charModel;


	public Animator animator;

	public SpriteRenderer spriteRenderer;

	private MoveDirection currentDirection = MoveDirection.DOWN;

	private ArrayList pathPoints;

	private int pathIndex = 0;

	private float speed;
	
	private PathPoint lastPathPoint;
	private PathPoint nextPathPoint;

	private Vector2 position;

	public Attribute attribute;

	public HPBar hpBar;

	private float attackCD = 1;
	
	public GameObject hp_pop;

	private Queue attackTagets = new Queue();
	private bool dead = false;


	private bool running = true;


	private SkillConfig normalAttackSkill;

	// Use this for initialization
	void Start () {
		this.normalAttackSkill = Config.GetInstance().GetSkillCOnfig(0);

		this.charModel.direction = this.currentDirection;

		this.attribute.volume = 2;

		this.model.gameObject.transform.localPosition = new Vector2( (this.attribute.volume/2.0f - 0.5f) * Constance.GRID_GAP , -(this.attribute.volume/2.0f - 0.5f) * Constance.GRID_GAP);
	}
	
	// Update is called once per frame
	void Update () {
		if(Constance.RUNNING == false){
			return;
		}

		
		this.UpdateState();

		if(this.dead == true){
			return;
		}

		this.TryMove();
		//this.TryAttak();
	}
	
	private void UpdatePosition(){

		this.spriteRenderer.sortingOrder = -(int)(this.transform.localPosition.y * 10 + (this.attribute.volume/2.0f - 0.5f));
		
		int x = (int)Mathf.Round(this.transform.localPosition.x / Constance.GRID_GAP);
		int y = -(int)Mathf.Round(this.transform.localPosition.y / Constance.GRID_GAP);
		
		if(x != this.position.x || y != this.position.y){
			this.position.x = x;
			this.position.y = y;
			
			Battle.UpdatePosition(this ,this.position , this.attribute.volume);
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

		if(this.charModel.currentState == CharModel.State.ATTACK || attackCD < 1){
			return;
		}

		UpdatePosition();

		if(this.pathPoints == null || this.pathPoints.Count == 0){
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

				if(Battle.IsMoveable(v) == false){
					return false;
				}

				if(Battle.GetGameObjectsByPosition(v).Count > 0){
					return false;
				}
			}
			break;

		case MoveDirection.UP:
			
			v = nextPathPoint.point;

			for(int i = 0 ; i < this.attribute.volume ; i++){
				v.x = v.x + i;
				
				if(Battle.IsMoveable(v) == false){
					return false;
				}

				if(Battle.GetGameObjectsByPosition(v).Count > 0){
					return false;
				}
			}
			break;
			
			
		case MoveDirection.LEFT:
			
			v = nextPathPoint.point;
			
			for(int i = 0 ; i < this.attribute.volume ; i++){
				v.y = v.y + i;
				
				if(Battle.IsMoveable(v) == false){
					return false;
				}

				if(Battle.GetGameObjectsByPosition(v).Count > 0){
					return false;
				}
			}
			
			break;
			
			
		case MoveDirection.RIGHT:
			
			v = nextPathPoint.point;
			v.x = v.x + this.attribute.volume - 1;
			
			for(int i = 0 ; i < this.attribute.volume ; i++){
				v.y = v.y + i;
				
				if(Battle.IsMoveable(v) == false){
					return false;
				}

				if(Battle.GetGameObjectsByPosition(v).Count > 0){
					return false;
				}
			}
			
			break;
		}


		return true;
	}

	private void TryAttak(){

		if(attackTagets.Count > 0){

			this.charModel.PlayAttack();
			this.attackCD = 0;
			
			while(attackTagets.Count > 0){
				Charactor charactor = (Charactor)attackTagets.Dequeue();
				
				SkillManager.PlaySkill(this , charactor , normalAttackSkill);
			}
			
		}

		
		if(this.attackCD < 1){
			this.attackCD += Time.deltaTime;
			return;
		}

		
		ArrayList points  = AttRange.GetRange(AttRange.TYPE_CORSS , this.normalAttackSkill.range , this.attribute.volume, this.position);
		
		for(int i = 0 ; i < points.Count ; i++){
			ArrayList followers = Battle.GetFollowersByPoint((Vector2)points[i]);
			
			if(followers.Count > 0){
				
				for(int j = 0 ; j < followers.Count  ; j++){

					Follower f = (Follower)followers[j];

					if(f.state == FollowState.FOLLOW){
						attackTagets.Enqueue(followers[j]);
					}
				}
			}

			if(Battle.hero.GetPoint() == (Vector2)points[i] && Battle.hero.IsDead() == false){
				attackTagets.Enqueue(Battle.hero);
			}
		}

		if(attackTagets.Count > 0){
			this.charModel.PlayAttack();
			this.attackCD = 0;
		}
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


	public void SetDirection(MoveDirection direction){
		this.currentDirection = direction;
		if(this.charModel != null)this.charModel.direction = direction;
	}
	
	public MoveDirection GetDirection(){
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

	public Vector2 GetPoint(){
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


	public override void ChangeHP(float hp){

		this.attribute.hp -= hp;

		this.hpBar.SetHP(this.attribute.hp/this.attribute.maxHp);

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


	public void PlayMove(){
		if(this.charModel == null){
			return;
		}
	}

	public override void PlayAttack(){

		if(this.animator == null){
			return;
		}
		
		animator.SetInteger("State" , 1); 
	}

	public override void PlayDead(){
		this.dead = true;

		if(this.charModel == null){
			return;
		}

		if(this.hpBar != null)Destroy(this.hpBar.gameObject);
		
		Battle.RemoveMonster(this);

		this.charModel.PlayDead();
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




