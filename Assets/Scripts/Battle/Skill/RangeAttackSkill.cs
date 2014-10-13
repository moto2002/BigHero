using UnityEngine;
using System.Collections;

public class RangeAttackSkill : Skill {

	private bool end = false;

	private Charactor attackOne;

	private SkillConfig skillConfig;
	
	private static GameObject SkillObject_pre  = (GameObject)Resources.Load("Prefabs/SkillEffect");

	private ArrayList skillObjects;

	private bool attacked = false;

	public RangeAttackSkill(Charactor attackOne , SkillConfig skillConfig){

		this.attackOne = attackOne;
		this.skillConfig = skillConfig;

		skillObjects = new ArrayList();

	}

	public void Start (){

		this.attackOne.PlayAttack();


	}
	
	public void Update (){
		if(end == true){
			return;
		}


		if(this.attackOne.IsInAttIndex() == true && attacked == false){
			Vector2 v = BattleUtils.PositionToGrid(attackOne.transform.position.x , attackOne.transform.position.y);
			
			Attribute attribute = attackOne.GetAttribute();
			
			ArrayList points = AttRange.GetRange(AttRange.TYPE_RECT , skillConfig.range , attribute.volume , v);

			//random
			ArrayList range = new ArrayList();
			if(skillConfig.param1 > 0){

				for(int i = 0 ; i < skillConfig.param1 ; i++){
					object o = points[Random.Range(0 , points.Count)];

					if(range.Contains(o) == false){
						range.Add(o);
					}
				}
			}else{
				range = points;
			}

			
			for(int i = 0 ; i < range.Count ; i ++){
				
				GameObject gameObject = (GameObject)MonoBehaviour.Instantiate(SkillObject_pre);
				
				SkillObject skillObject = gameObject.GetComponent<SkillObject>();
				skillObject.res = this.skillConfig.res;
				skillObject.loop = 1;
				
				skillObject.transform.position = BattleUtils.GridToPosition((Vector2)range[i]);
				
				skillObjects.Add(skillObject);
				
				ArrayList objects = Battle.GetGameObjectsByPosition((Vector2)range[i]);
				
				for(int j = 0 ; j < objects.Count ; j++){
					Charactor c = objects[j] as Charactor;
					
					if(c.GetType() != this.attackOne.GetType()){
						
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

			attacked = true;
		}

		if(attacked == false){
			return;
		}

		for(int i = 0 ; i < skillObjects.Count ; i++){

			SkillObject skillObject  = skillObjects[i] as SkillObject;

			if(skillObject.IsSpritePlayEnd() == true){
				MonoBehaviour.Destroy(skillObject.gameObject);

				skillObjects.Remove(skillObjects);
				end = true;
			}
		}
	}
	
	public bool IsEnd(){
		return end;
	}

}
