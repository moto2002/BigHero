using UnityEngine;
using System.Collections;

public class HealSkill  {
	
	private bool specSign = false;
	
	private bool end = false;
	
	private Charactor attackOne;
	
	private SkillConfig skillConfig;
	
	private static GameObject SkillObject_pre  = (GameObject)Resources.Load("Prefabs/SkillEffect");
	
	private bool attacked = false;

	private ArrayList skillObjects;
	
	private ArrayList range;
	
	public HealSkill(Charactor attackOne , SkillConfig skillConfig){
		
		this.attackOne = attackOne;
		this.skillConfig = skillConfig;

		skillObjects = new ArrayList();
	}


	
	public void Start (){

		
		if(attackOne.GetType() == Charactor.TYPE_HERO){
			this.attackOne.PlaySkillAttack();
			
			for(int i = 0 ; i < BattleControllor.followers.Count ; i++){
				Charactor c = (Charactor)BattleControllor.followers[i];
				
				if(c.IsActive() == false){
					continue;
				}
				
				PlayHeal(c);
			}
			
			PlayHeal(BattleControllor.hero);


		}else{
			this.attackOne.PlaySkillAttack();

			for(int i = 0 ; i < BattleControllor.monsters.Count ; i++){
				Charactor c = (Charactor)BattleControllor.monsters[i];
				
				if(c.IsActive() == false){
					continue;
				}
				
				PlayHeal(c);
			}
		}
	}

	
	
	
	private void PlayHeal(Charactor c){
		
		GameObject gameObject = (GameObject)MonoBehaviour.Instantiate(SkillObject_pre);
		
		SkillObject skillObject = gameObject.GetComponent<SkillObject>();
		skillObject.res = 11;
		skillObject.loop = 1;
		skillObject.spriteAnimation.sortingLayerID = 3;
		skillObject.transform.position = c.transform.position;
		skillObject.transform.parent = c.transform;

		skillObject.transform.localScale = new Vector2(0.25f , 0.25f);

		skillObject.SetSpec(this.specSign);
		
		skillObjects.Add(skillObject);

		c.ChangeHP( -((int)attackOne.GetAttribute().atk * skillConfig.demageratio) , false );
	}


	
	public void Update (){
		if(Constance.SPEC_RUNNING == false && Constance.RUNNING == false){
			return;
		}else if(Constance.SPEC_RUNNING == true && this.specSign == false){
			return;
		}
		
		
		if(end == true){
			return;
		}


		if(skillObjects.Count == 0){
			end = true;
			return;
		}

		if(((SkillObject)skillObjects[0]).IsSpritePlayEnd() == true){
			this.attackOne.SetPlayLock(false);
			end = true;

			for(int i = 0 ; i < skillObjects.Count ; i++){
				SkillObject skObject = (SkillObject)skillObjects[i];

				MonoBehaviour.Destroy(skObject.gameObject);
			}
		}
	}
	
	
	public void SetSpec(bool b){
		this.specSign = b;
	}
	
	public bool IsEnd(){
		return end;
	}
	
}