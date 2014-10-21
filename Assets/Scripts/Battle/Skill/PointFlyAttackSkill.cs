using UnityEngine;
using System.Collections;

public class PointFlyAttackSkill : Skill {
	
	private Charactor attackOne;
	
	private Charactor attackedOne;


	private Transform attackTransfrom;
	private Transform attackedTransfrom;
	private Transform skillTransfrom;
	
	private bool end = false;

	private static GameObject SkillObject_pre  = (GameObject)Resources.Load("Prefabs/SkillEffect");

	private SkillObject skillObject;

	private float speed = 6f;
	
	private Vector3 attackedOff = Vector2.zero;
	private Vector3 attackOff = Vector2.zero;

	private SkillConfig skillConfig;
	
	public PointFlyAttackSkill(Charactor attackOne , SkillConfig skillConfig){
		this.attackOne = attackOne;

		ArrayList points  = AttRange.GetRangeByAttType(skillConfig.attack_type , skillConfig.range ,  attackOne.GetAttribute().volume , attackOne.GetPoint() , attackOne.GetDirection());
		
		for(int i = 0 ; i < points.Count ; i++){
			ArrayList objects = Battle.GetGameObjectsByPosition((Vector2)points[i]);
			
			for(int j = 0 ; j < objects.Count ; j++){
				Charactor c = objects[j] as Charactor;

				if(c.GetType() != this.attackOne.GetType() && c.IsActive() == true){
					this.attackedOne = objects[j] as Charactor;
					break;
				}
			}
		}

		if(attackedOne == null){
			this.end = true;
			return;
		}

		this.attackTransfrom = this.attackOne.transform;
		this.attackedTransfrom = this.attackedOne.transform;

		this.skillConfig = skillConfig;

		attackOff = new Vector3((attackOne.GetAttribute().volume/2.0f - 0.5f) * Constance.GRID_GAP , (attackOne.GetAttribute().volume/2.0f - 0.5f) * Constance.GRID_GAP , 0);
		attackedOff = new Vector3((attackedOne.GetAttribute().volume/2.0f - 0.5f) * Constance.GRID_GAP , (attackedOne.GetAttribute().volume/2.0f - 0.5f) * Constance.GRID_GAP , 0);
	}
	
	public void Start(){
		if(this.end == true){
			return;
		}

		this.attackOne.PlaySkillAttack();
	}
	
	public void Update(){

		if(this.end == true){
			return;
		}

		if(skillObject == null && this.attackOne.IsInAttIndex()){

			GameObject gameObject = (GameObject)MonoBehaviour.Instantiate(SkillObject_pre);
			
			skillObject = gameObject.GetComponent<SkillObject>();
			skillObject.spriteAnimation.renderer.sortingLayerID = 3;
			skillObject.res = skillConfig.res;
			skillObject.transform.position = attackOne.transform.position + attackOff;
			
			skillTransfrom = skillObject.transform;
			
			skillTransfrom.eulerAngles = new Vector3(0,0,GetAngle()); 
		}

		if(skillObject == null){
			return;
		}

		float d1 = Time.deltaTime * speed;
		float d2 = Vector3.Distance(skillTransfrom.position , attackedTransfrom.position + attackedOff);


		if(d1 > d2){
			MonoBehaviour.Destroy(this.skillObject.gameObject);
			this.end = true;

			float damage = Battle.Attack(attackOne.GetAttribute() , attackedOne.GetAttribute());
			
			attackedOne.ChangeHP(damage);
			
			if(attackedOne.GetAttribute().hp > 0){
				attackedOne.PlayAttacked();
			}else{
				attackedOne.PlayDead();
			}

			return;
		}

		skillTransfrom.eulerAngles = new Vector3(0 , 0 , GetAngle()); 
		skillTransfrom.position = Vector3.MoveTowards(skillTransfrom.position , attackedTransfrom.position + attackedOff , d1);
	}


	private float GetAngle(){

		return Mathf.Atan2(attackedTransfrom.position.y + attackedOff.y  - skillTransfrom.position.y , attackedTransfrom.position.x - skillTransfrom.position.x + attackedOff.y) * 180 / Mathf.PI ;
	}
	
	public bool IsEnd(){
		return end;
	}
}
