using UnityEngine;
using System.Collections;

public class LineAttackSkill : Skill {
	
	private bool end = false;
	
	private Charactor attackOne;
	
	private SkillConfig skillConfig;
	
	private static GameObject SkillObject_pre  = (GameObject)Resources.Load("Prefabs/SkillEffect");
	private static GameObject AlertBlock_pre  = (GameObject)Resources.Load("Prefabs/AlertBlock");
	
	private bool attacked = false;

	private SkillObject skillObject;

	private ArrayList range;

	private ArrayList alertBlocks;

	private float singTime;
	
	public LineAttackSkill(Charactor attackOne , SkillConfig skillConfig){
		
		this.attackOne = attackOne;
		this.skillConfig = skillConfig;

		singTime = skillConfig.singTime;
	}
	
	public void Start (){
		
		this.attackOne.PlaySkillAttack();
		this.attackOne.SetPlayLock(true);
		
		Vector2 v = BattleUtils.PositionToGrid(attackOne.transform.position.x , attackOne.transform.position.y);
		Attribute attribute = attackOne.GetAttribute();
		range = AttRange.GetRangeByAttType(skillConfig.attack_type , skillConfig.range , attribute.volume , v , attackOne.GetDirection());


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

			GameObject gameObject = (GameObject)MonoBehaviour.Instantiate(SkillObject_pre);
			
			skillObject = gameObject.GetComponent<SkillObject>();
			skillObject.res = this.skillConfig.res;
			skillObject.loop = 1;
			skillObject.spriteAnimation.renderer.sortingLayerID = 3;
			skillObject.transform.position = attackOne.transform.localPosition;

			skillObject.transform.localScale = new Vector2(attribute.volume / 4f , attribute.volume / 4f);

			switch(attackOne.GetDirection()){
			case MoveDirection.DOWN:
				skillObject.SetSpriteEulerAngles(new Vector3(0,0, 270));
				skillObject.transform.position += new Vector3(0 , -0.2f , 0);
				break;
			case MoveDirection.UP:
				skillObject.SetSpriteEulerAngles(new Vector3(0,0, 90));
				skillObject.transform.position += new Vector3(0 , 0.2f , 0);
				break;
			case MoveDirection.LEFT:
				skillObject.SetSpriteEulerAngles(new Vector3(0,0, 180));
				skillObject.transform.position += new Vector3(-0.2f , 0.2f , 0);
				break;
			case MoveDirection.RIGHT:
				skillObject.transform.position += new Vector3(0.2f, 0.2f , 0);
				break;
			}
			
			
			for(int i = 0 ; i < range.Count ; i ++){

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


		if(skillObject.IsSpritePlayEnd() == true){
			MonoBehaviour.Destroy(skillObject.gameObject);
			this.attackOne.SetPlayLock(false);
			end = true;
		}

	}
	
	public bool IsEnd(){
		return end;
	}
	
}
