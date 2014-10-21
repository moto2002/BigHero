using UnityEngine;
using System.Collections;
using System.Timers;

public class FireWallAttackSkill : Skill {
	
	private Charactor attackOne;

	private SkillConfig skillConfig;

	private bool end;

	private ArrayList skillObjects;

	private bool attacked = false;
	
	private static GameObject SkillObject_pre  = (GameObject)Resources.Load("Prefabs/SkillEffect");

	private float time;

	private float passTime;

	/**
	 * 
	 * param1 num of firewall
	 * param2 time of firewall
	 * 
	 */
	public FireWallAttackSkill(Charactor attackOne , SkillConfig skillConfig){
		this.skillConfig = skillConfig;
		this.attackOne = attackOne;

		this.skillObjects = new ArrayList();

		time = skillConfig.param2;
	}

	// Use this for initialization
	public void Start () {
		
		this.attackOne.PlaySkillAttack();
		this.attackOne.SetPlayLock(true);
		
		Attribute attribute = attackOne.GetAttribute();


	}
	
	// Update is called once per frame
	public void Update () {
		if(this.end == true){
			return;
		}


		if(this.attackOne.IsInAttIndex() == true && attacked == false){
			buildFirewall();
			attacked = true;
		}

		if(attacked == false){
			return;
		}

		passTime += Time.deltaTime;
		time -= Time.deltaTime;

		if(time <= 0){
			end = true;

			for(int i = 0 ; i < skillObjects.Count ; i++){
				SkillObject skillObject = (SkillObject)skillObjects[i];

				MonoBehaviour.Destroy(skillObject.gameObject);
			}
		}

		if(passTime >= 0.5f){
			passTime -= 0.5f;
			 
			this.TryAttack();
		}
	}


	private void buildFirewall(){
		int firewallNum = skillConfig.param1;
		
		Vector2 zeroPoint = new Vector2(Random.Range(0,Battle.h) , Random.Range(0,Battle.v));
		
		ArrayList points = new ArrayList();
		
		switch(Random.Range(1 , 3)){
		case 1:
			//up
			
			for(int i = 1 ; i < firewallNum ; i++){
				Vector2 point = zeroPoint;
				
				if(zeroPoint.y + i > Battle.v){
					i = - (firewallNum - i);
					firewallNum = 0;
				}
				
				point.y = zeroPoint.y + i;
				
				points.Add(point);
			}
			
			break;
		case 2:
			//right
			
			for(int i = 1 ; i < firewallNum ; i++){
				Vector2 point = zeroPoint;
				
				if(zeroPoint.x + i > Battle.h){
					i = - (firewallNum - i);
					firewallNum = 0;
				}
				
				point.x = zeroPoint.x + i;
				
				points.Add(point);
			}
			
			break;
		}
		
		for(int i = 0 ; i < points.Count; i++){
			Vector2 point = (Vector2)points[i];
			
			GameObject gameObject = (GameObject)MonoBehaviour.Instantiate(SkillObject_pre);
			
			SkillObject skillObject = gameObject.GetComponent<SkillObject>();
			skillObject.res = this.skillConfig.res;
			skillObject.loop = 0;
			
			skillObject.transform.position = BattleUtils.GridToPosition(point);
			
			skillObjects.Add(skillObject);
		}
	}


	private void TryAttack(){

		for(int i = 0 ; i < skillObjects.Count ; i++){
			SkillObject skillObject = (SkillObject)skillObjects[i];

			Vector2 v = BattleUtils.PositionToGrid(skillObject.transform.position);

			ArrayList gameObjects = Battle.GetGameObjectsByPosition(v);

			for(int j = 0 ; j < gameObjects.Count; j++){

				Charactor c = (Charactor)gameObjects[j];

				if(c.GetType() !=  this.attackOne.GetType() && c.IsActive() == true){

					float damage = Battle.Attack(attackOne.GetAttribute() , c.GetAttribute());
					
					c.ChangeHP(damage);
					
					if(c.GetAttribute().hp > 0){
						c.PlayAttacked();
					}else{
						c.PlayDead();
					}
				}
			}
		}
	}
	
	public bool IsEnd(){
		return end;
	}
}
