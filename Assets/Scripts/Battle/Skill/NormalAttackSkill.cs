using UnityEngine;
using System.Collections;

public class NormalAttackSkill: Skill{
	
	private bool specSign = false;

	private Charactor attackOne;

	private Charactor attackedOne;

	private bool end = false;

	private SkillConfig skillConfig;

	public NormalAttackSkill(Charactor attackOne , SkillConfig skillConfig ,  Charactor attackedOne){
		this.attackOne = attackOne;
		this.attackedOne = attackedOne;

		this.skillConfig = skillConfig;
	}

	public void Start(){

		if(attackedOne == null){

			ArrayList points  = AttRange.GetRangeByAttType(skillConfig.attack_type , this.skillConfig.range ,  this.attackOne.GetAttribute().volume , this.attackOne.GetPoint() , this.attackOne.GetDirection());
			
			for(int i = 0 ; i < points.Count ; i++){
				ArrayList gameObjects = BattleControllor.GetGameObjectsByPosition((Vector2)points[i]);
				
				for(int j = 0 ; j < gameObjects.Count ; j++){
					Charactor c = (Charactor)gameObjects[j];

					if(c.IsActive() == true && c.GetType() != attackOne.GetType()){
						this.attackedOne = c;
					}
				}
			}

		}

		if(attackedOne != null){
			attackOne.PlayAttack();
		}else{
			end = true;
		}
	}

	public void Update(){
		if(Constance.SPEC_RUNNING == false && Constance.RUNNING == false){
			return;
		}else if(Constance.SPEC_RUNNING == true && this.specSign == false){
			return;
		}
		
		
		if(attackOne.IsActive() == false){
			this.end = true;
			return;
		}


		if(this.end == true){
			return;
		}

		if(this.attackOne.IsInAttIndex() == false){
			return;
		}
		
		bool crit = BattleControllor.Crit(skillConfig.crit);
		float damage = BattleControllor.Attack(attackOne.GetAttribute() , attackedOne.GetAttribute() , skillConfig.demageratio , skillConfig.b , crit);

		attackedOne.ChangeHP(damage , crit);
		
		if(attackedOne.GetAttribute().hp > 0){
			attackedOne.PlayAttacked();
		}else{
			attackedOne.PlayDead();
		}


		attackOne.audio.clip = Resources.Load<AudioClip>("Audio/Skill/7");
		if(attackOne.audio.clip != null){
			attackOne.audio.Play();
		}

		end = true;
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
