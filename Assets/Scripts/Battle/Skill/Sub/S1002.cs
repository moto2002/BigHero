using UnityEngine;
using System.Collections;

public class S1002 : Skill {
	
	private bool specSign = false;
	
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
	
	private Vector3 targetPoint;
	
	public S1002(Charactor attackOne , SkillConfig skillConfig){
		this.attackOne = attackOne;
		
		ArrayList points  = AttRange.GetRangeByAttType(skillConfig.attack_type , skillConfig.range ,  attackOne.GetAttribute().volume , attackOne.GetPoint() , attackOne.GetDirection());
		
		for(int i = 0 ; i < points.Count ; i++){
			ArrayList objects = BattleControllor.GetGameObjectsByPosition((Vector2)points[i]);
			
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
		if(Constance.SPEC_RUNNING == false && Constance.RUNNING == false){
			return;
		}else if(Constance.SPEC_RUNNING == true && this.specSign == false){
			return;
		}
		
		
		if(this.end == true){
			return;
		}
		
		
		if(attackedOne.IsActive() == true){
			targetPoint = attackedTransfrom.position + attackedOff;
		}
		
		if(skillObject == null && this.attackOne.IsInAttIndex()){
			
			if(attackOne.IsActive() == false){
				end = true;
				return;
			}
			
			GameObject gameObject = (GameObject)MonoBehaviour.Instantiate(SkillObject_pre);
			
			skillObject = gameObject.GetComponent<SkillObject>();
			skillObject.spriteAnimation.renderer.sortingLayerID = 3;
			skillObject.res = 1;
			skillObject.transform.position = attackOne.transform.position + attackOff;
			skillObject.sound = 3;
			
			skillTransfrom = skillObject.transform;
			
			skillTransfrom.eulerAngles = new Vector3(0,0,GetAngle()); 
		}
		
		if(skillObject == null){
			return;
		}
		
		float d1 = Time.deltaTime * speed;
		float d2 = Vector3.Distance(skillTransfrom.position , targetPoint);
		
		
		if(d1 > d2){
			MonoBehaviour.Destroy(this.skillObject.gameObject);
			this.end = true;
			
			bool crit = BattleControllor.Crit(skillConfig.crit);
			float damage = BattleControllor.Attack(attackOne.GetAttribute() , attackedOne.GetAttribute() , skillConfig.demageratio , skillConfig.b , crit);
			
			attackedOne.ChangeHP(damage , crit);
			

			AudioClip ac = Resources.Load<AudioClip>("Audio/Skill4/");
			attackedOne.audio.clip = ac;
			attackedOne.audio.Play();

			
			if(attackedOne.GetAttribute().hp > 0){
				attackedOne.PlayAttacked();
			}else{
				attackedOne.PlayDead();
			}
			
			return;
		}
		
		skillTransfrom.eulerAngles = new Vector3(0 , 0 , GetAngle()); 
		skillTransfrom.position = Vector3.MoveTowards(skillTransfrom.position , targetPoint , d1);
	}
	
	
	private float GetAngle(){
		
		return Mathf.Atan2(targetPoint.y  - skillTransfrom.position.y , targetPoint.x - skillTransfrom.position.x + attackedOff.y) * 180 / Mathf.PI ;
	}
	
	
	public void SetSpec(bool b){
		this.specSign = b;
	}
	
	public bool IsEnd(){
		return end;
	}

	public void Clean(){

	}
}
