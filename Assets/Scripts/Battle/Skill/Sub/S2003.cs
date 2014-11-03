using UnityEngine;
using System.Collections;

public class S2003 : Skill {
	
	
	private bool specSign = false;

	private bool end = false;

	private Charactor attackOne;

	private SkillConfig skillConfig;
	
	private static GameObject SkillObject_pre  = (GameObject)Resources.Load("Prefabs/SkillEffect");
	private static GameObject AlertBlock_pre  = (GameObject)Resources.Load("Prefabs/AlertBlock");

	private ArrayList skillObjects;
	
	private ArrayList range;
	
	private ArrayList alertBlocks;

	private bool attacked = false;

	private bool subtract = false;
	
	private float singTime;

	public S2003(Charactor attackOne , SkillConfig skillConfig){

		this.attackOne = attackOne;
		this.skillConfig = skillConfig;
		this.subtract = subtract;
		
		singTime = skillConfig.singTime;

		skillObjects = new ArrayList();
	}

	public void Start (){
		this.attackOne.PlaySkillAttack();
		this.attackOne.SetPlayLock(true);
		
		Vector2 v = BattleUtils.PositionToGrid(attackOne.transform.position.x , attackOne.transform.position.y);
		
		Attribute attribute = attackOne.GetAttribute();
		this.range = AttRange.GetRangeByAttType(skillConfig.attack_type , skillConfig.range , attribute.volume , v);

		
		alertBlocks = new ArrayList();
		
		for(int i = 0 ; i < range.Count ; i ++){
			
			GameObject gameObject = (GameObject)MonoBehaviour.Instantiate(AlertBlock_pre);
			gameObject.transform.position = BattleUtils.GridToPosition((Vector2)range[i]);
			
			alertBlocks.Add(gameObject);
		}
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


		singTime -= Time.deltaTime;
		
		if(singTime > 0){
			return;
		}

		
		while(alertBlocks.Count > 0){
			GameObject gameObject = alertBlocks[0] as GameObject;
			MonoBehaviour.Destroy(gameObject);
			
			alertBlocks.RemoveAt(0);
		}

		if(this.attackOne.IsInAttIndex() == true && attacked == false){
			Vector2 v = BattleUtils.PositionToGrid(attackOne.transform.position.x , attackOne.transform.position.y);
			
			Attribute attribute = attackOne.GetAttribute();
			
			for(int i = 0 ; i < range.Count ; i ++){
				
				GameObject gameObject = (GameObject)MonoBehaviour.Instantiate(SkillObject_pre);
				
				SkillObject skillObject = gameObject.GetComponent<SkillObject>();
				skillObject.res = 7;
				skillObject.loop = 1;
				skillObject.transform.localScale = new Vector3(0.5f, 0.5f , 0);
				
				if(i == 0){
					skillObject.sound = 1;
				}

				skillObject.transform.position = BattleUtils.GridToPosition((Vector2)range[i]);
				
				skillObjects.Add(skillObject);
				
				ArrayList objects = BattleControllor.GetGameObjectsByPosition((Vector2)range[i]);
				
				for(int j = 0 ; j < objects.Count ; j++){
					Charactor c = objects[j] as Charactor;
					
					if(c.GetType() != this.attackOne.GetType() && c.IsActive() == true){
						
						bool crit = BattleControllor.Crit(skillConfig.crit);
						float damage = BattleControllor.Attack(attackOne.GetAttribute() , c.GetAttribute() , skillConfig.demageratio , skillConfig.b , crit);
						
						c.ChangeHP(damage , crit);

//						if(skillConfig.sound2 != 0){
//							AudioClip ac = Resources.Load<AudioClip>("Audio/Skill/" + skillConfig.sound2);
//							c.audio.clip = ac;
//							c.audio.Play();
//						}
						
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
				this.attackOne.SetPlayLock(false);
			}
		}
	}
	
	
	public void SetSpec(bool b){
		this.specSign = b;
	}

	public bool IsEnd(){
		return end;
	}

	public void Clean(){
		
		while(alertBlocks.Count > 0){
			GameObject gameObject = alertBlocks[0] as GameObject;
			MonoBehaviour.Destroy(gameObject);
			
			alertBlocks.RemoveAt(0);
		}
		
		while(skillObjects.Count > 0){
			GameObject gameObject = skillObjects[0] as GameObject;
			MonoBehaviour.Destroy(gameObject);
			
			skillObjects.RemoveAt(0);
		}
	}

}
