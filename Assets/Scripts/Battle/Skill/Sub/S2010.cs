using UnityEngine;
using System.Collections;

public class S2010 : Skill {
	
	private bool specSign = false;
	
	private bool end = false;
	
	private Charactor attackOne;
	
	private SkillConfig skillConfig;
	
	private static GameObject SkillObject_pre  = (GameObject)Resources.Load("Prefabs/SkillEffect");
	
	private bool attacked = false;

	private ArrayList range;
	
	public S2010(Charactor attackOne , SkillConfig skillConfig){
		
		this.attackOne = attackOne;
		this.skillConfig = skillConfig;

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
		c.PlayEffect(1);
		c.ChangeHP( -((int)attackOne.GetAttribute().atk * skillConfig.demageratio) , false );
	}
	
	
	
	public void Update (){

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