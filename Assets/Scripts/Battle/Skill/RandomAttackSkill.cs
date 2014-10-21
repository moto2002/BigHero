using UnityEngine;
using System.Collections;


//unfinshed
public class RandomAttackSkill : Skill {

	private bool end = false;
	
	private Charactor attackOne;
	
	private SkillConfig skillConfig;
	
	private static GameObject SkillObject_pre  = (GameObject)Resources.Load("Prefabs/SkillEffect");
	private static GameObject AlertBlock_pre  = (GameObject)Resources.Load("Prefabs/AlertBlock");
	
	private ArrayList skillObjects;
	
	private bool attacked = false;

	private int num;

	private float time;

	private ArrayList range;

	private ArrayList alertBlocks;
	
	private float singTime;

	public RandomAttackSkill(Charactor attackOne , SkillConfig skillConfig){
		
		this.attackOne = attackOne;
		this.skillConfig = skillConfig;
		this.singTime = skillConfig.singTime;

		this.num = skillConfig.param1;
		this.time = skillConfig.param2;
		
		skillObjects = new ArrayList();
		
	}

	public void Start (){
		
		this.attackOne.PlaySkillAttack();
		this.attackOne.SetPlayLock(true);

		
		Vector2 v = BattleUtils.PositionToGrid(attackOne.transform.position.x , attackOne.transform.position.y);
		Attribute attribute = attackOne.GetAttribute();
		ArrayList r = AttRange.GetRangeByAttType(skillConfig.attack_type , skillConfig.range , attribute.volume , v , attackOne.GetDirection());

		this.range = new ArrayList();

		for(int i = 0 ; i < this.num ; i++){
			int index = Random.Range(0 , r.Count);

			v = (Vector2)r[index];

			if(v.x >= Battle.h || v.x < 0 || v.y >= Battle.v || v.y < 0){
				i--;
				continue;
			}

			range.Add(r[index]);
		}

		r = null;

		alertBlocks = new ArrayList();
		
		for(int i = 0 ; i < range.Count ; i ++){
			
			GameObject gameObject = (GameObject)MonoBehaviour.Instantiate(AlertBlock_pre);
			gameObject.transform.position = BattleUtils.GridToPosition((Vector2)range[i]);
			
			alertBlocks.Add(gameObject);
		}
	}
	
	public void Update (){
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
			Attribute attribute = attackOne.GetAttribute();
			
			for(int i = 0 ; i < range.Count ; i ++){
				
				GameObject gameObject = (GameObject)MonoBehaviour.Instantiate(SkillObject_pre);
				
				SkillObject skillObject = gameObject.GetComponent<SkillObject>();
				skillObject.res = this.skillConfig.res;
				skillObject.loop = 1;
				skillObject.transform.localScale = new Vector3(0.5f, 0.5f , 0);
				
				skillObject.transform.position = BattleUtils.GridToPosition((Vector2)range[i]);
				
				skillObjects.Add(skillObject);
				
				ArrayList objects = Battle.GetGameObjectsByPosition((Vector2)range[i]);
				
				for(int j = 0 ; j < objects.Count ; j++){
					Charactor c = objects[j] as Charactor;
					
					if(c.GetType() != this.attackOne.GetType() && c.IsActive() == true){
						
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
	
	public bool IsEnd(){
		return end;
	}
}
