using UnityEngine;
using System.Collections;

public class SectorRangeAttackSkill {
	
	private bool specSign = false;

	private Charactor attackOne;

	private SkillConfig skillConfig;

	private bool end;

	private static GameObject SkillObject_pre  = (GameObject)Resources.Load("Prefabs/SkillEffect");

	private bool attacked = false;

	private SkillObject skillObject;

	public SectorRangeAttackSkill(Charactor attackOne , SkillConfig skillConfig){
		this.attackOne = attackOne;
		this.skillConfig = skillConfig;
	}

	public void Start () {
		
		this.attackOne.PlaySkillAttack();
		this.attackOne.SetPlayLock(true);
	}


	public void Update () {
		if(Constance.SPEC_RUNNING == false && Constance.RUNNING == false){
			return;
		}else if(Constance.SPEC_RUNNING == true && this.specSign == false){
			return;
		}

		if(end == true){
			return;
		}
		
		
		if(this.attackOne.IsInAttIndex() == true && attacked == false){
			Vector2 v = BattleUtils.PositionToGrid(attackOne.transform.position.x , attackOne.transform.position.y);
			
			Attribute attribute = attackOne.GetAttribute();
			
			MoveDirection direction = attackOne.GetDirection();

			ArrayList range = AttRange.GetRangeByAttType(skillConfig.attack_type , skillConfig.range , attribute.volume , v , direction);

			GameObject gameObject = (GameObject)MonoBehaviour.Instantiate(SkillObject_pre);
			
			skillObject = gameObject.GetComponent<SkillObject>();
			skillObject.res = this.skillConfig.res;
			skillObject.loop = 1;
			
			skillObject.transform.position = attackOne.transform.position;

			switch(direction){
			case MoveDirection.DOWN:
				skillObject.SetSpriteEulerAngles(new Vector3(0,0, 270));
				break;
			case MoveDirection.UP:
				skillObject.SetSpriteEulerAngles(new Vector3(0,0, 90));
				break;
			case MoveDirection.LEFT:
				skillObject.SetSpriteEulerAngles(new Vector3(0,0, 180));
				break;
			case MoveDirection.RIGHT:
				break;
			}

			
			for(int i = 0 ; i < range.Count ; i ++){
				
				ArrayList objects = BattleControllor.GetGameObjectsByPosition((Vector2)range[i]);
				
				for(int j = 0 ; j < objects.Count ; j++){
					Charactor c = objects[j] as Charactor;
					
					if(c.GetType() != this.attackOne.GetType() && c.IsActive() == true){
						
						bool crit = BattleControllor.Crit(skillConfig.crit);
						float damage = BattleControllor.Attack(attackOne.GetAttribute() , c.GetAttribute() , skillConfig.demageratio , skillConfig.b , crit);
						
						c.ChangeHP(damage, crit);
						
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
		

			
		if(skillObject.IsSpritePlayEnd() == true){
			MonoBehaviour.Destroy(skillObject.gameObject);

			end = true;
		}

	}
	
	public void SetSpec(bool b){
		this.specSign = b;
	}


	public bool IsEnd(){
		return end;
	}
}
