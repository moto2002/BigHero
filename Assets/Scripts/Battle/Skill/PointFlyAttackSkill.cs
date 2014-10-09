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
	
	public PointFlyAttackSkill(Charactor attackOne , Charactor attackedOne , int effectId){
		this.attackOne = attackOne;
		this.attackedOne = attackedOne;

		this.attackTransfrom = this.attackOne.transform;
		this.attackedTransfrom = this.attackedOne.transform;

		attackOff = new Vector3((attackOne.GetAttribute().volume/2.0f - 0.5f) * Constance.GRID_GAP , (attackOne.GetAttribute().volume/2.0f - 0.5f) * Constance.GRID_GAP , 0);
		attackedOff = new Vector3((attackedOne.GetAttribute().volume/2.0f - 0.5f) * Constance.GRID_GAP , (attackedOne.GetAttribute().volume/2.0f - 0.5f) * Constance.GRID_GAP , 0);
	}
	
	public void Start(){

		GameObject gameObject = (GameObject)MonoBehaviour.Instantiate(SkillObject_pre);
		
		skillObject = gameObject.GetComponent<SkillObject>();
		skillObject.transform.position = attackOne.transform.position + attackOff;

		skillTransfrom = skillObject.transform;

		skillTransfrom.eulerAngles = new Vector3(0,0,GetAngle()); 
	}
	
	public void Update(){

		if(this.end == true){
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
