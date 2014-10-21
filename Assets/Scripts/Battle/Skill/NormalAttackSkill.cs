using UnityEngine;
using System.Collections;

public class NormalAttackSkill: Skill{

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

			ArrayList points  = AttRange.GetRangeByAttType(skillConfig.attack_type , this.skillConfig.range ,  this.attackOne.GetAttribute().volume , this.attackOne.GetPoint());
			
			for(int i = 0 ; i < points.Count ; i++){
				ArrayList gameObjects = Battle.GetGameObjectsByPosition((Vector2)points[i]);
				
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

		if(this.end == true){
			return;
		}

		if(this.attackOne.IsInAttIndex() == false){
			return;
		}

		float damage = Battle.Attack(attackOne.GetAttribute() , attackedOne.GetAttribute());

		attackedOne.ChangeHP(damage);
		
		if(attackedOne.GetAttribute().hp > 0){
			attackedOne.PlayAttacked();
		}else{
			attackedOne.PlayDead();
		}

		end = true;
	}

	public bool IsEnd(){
		return end;
	}

}
