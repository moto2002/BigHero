using UnityEngine;
using System.Collections;

public class S1006 : Skill {
	
	private bool specSign = false;

	private int effectId;

	private float distance;

	private Charactor attackOne;
	
	private Transform attackTransfrom;
	
	private bool end = false;

	private static GameObject SkillObject_pre  = (GameObject)Resources.Load("Prefabs/SkillEffect");
	private static GameObject AlertBlock_pre  = (GameObject)Resources.Load("Prefabs/AlertBlock");

	private ArrayList skillObjects;

	private float speed = 6f;

	private Vector3 attackOff = Vector2.zero;

	private ArrayList directions;

	private Vector3 beginPosition;

	private SkillConfig skillConfig;

	private ArrayList range;
	
	private ArrayList alertBlocks;

	private bool attacked = false;
	
	private float singTime;

	
	public S1006(Charactor attackOne , SkillConfig skillConfig){

		this.skillConfig = skillConfig;
		this.singTime = skillConfig.singTime;

		this.effectId = skillConfig.res;
		this.distance = skillConfig.range * Constance.GRID_GAP;

		this.attackOne = attackOne;
		this.attackTransfrom = this.attackOne.transform;

		attackOff = new Vector3((attackOne.GetAttribute().volume/2.0f - 0.5f) * Constance.GRID_GAP , (attackOne.GetAttribute().volume/2.0f - 0.5f) * Constance.GRID_GAP , 0);

		directions = new ArrayList();
		skillObjects = new ArrayList();
		alertBlocks = new ArrayList();

		switch(skillConfig.param1){
		case 0:
			directions.Add(attackOne.GetDirection());
			break;
		case 1:
			directions.Add((MoveDirection)(((int)attackOne.GetDirection() + 5) % 4));
			directions.Add((MoveDirection)(((int)attackOne.GetDirection() + 3) % 4));
			break;
		case 2:
			directions.Add(attackOne.GetDirection());
			directions.Add((MoveDirection)(((int)attackOne.GetDirection() + 2) % 4));
			break;
		case 3:
			directions.Add(attackOne.GetDirection());
			directions.Add((MoveDirection)(((int)attackOne.GetDirection() + 1) % 4));
			directions.Add((MoveDirection)(((int)attackOne.GetDirection() + 2) % 4));
			directions.Add((MoveDirection)(((int)attackOne.GetDirection() + 3) % 4));
			break;
		default:
			directions.Add(attackOne.GetDirection());
			break;
		}



	}
	
	public void Start(){
		this.attackOne.PlaySkillAttack();

		if(this.singTime > 0){

			this.attackOne.SetPlayLock(true);
			
			Vector2 v = BattleUtils.PositionToGrid(attackOne.transform.position.x , attackOne.transform.position.y);
			
			Attribute attribute = attackOne.GetAttribute();
			this.range = AttRange.GetRangeByAttType(skillConfig.attack_type , skillConfig.range , attribute.volume , v);

			
			for(int i = 0 ; i < range.Count ; i ++){
				
				GameObject gameObject = (GameObject)MonoBehaviour.Instantiate(AlertBlock_pre);
				gameObject.transform.position = BattleUtils.GridToPosition((Vector2)range[i]);
				
				alertBlocks.Add(gameObject);
			}
		}
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

		singTime -= Time.deltaTime;
		
		if(singTime > 0){
			return;
		}

		
		
		while(alertBlocks.Count > 0){
			GameObject gameObject = alertBlocks[0] as GameObject;
			MonoBehaviour.Destroy(gameObject);
			
			alertBlocks.RemoveAt(0);
		}



		if(attackOne.IsInAttIndex() == true && attacked == false){
			Attribute attribute = attackOne.GetAttribute();
			beginPosition = attackOne.transform.position + attackOff;


			for(int i = 0 ; i < directions.Count ; i++){
				MoveDirection dirction = (MoveDirection)directions[i];

				GameObject gameObject = (GameObject)MonoBehaviour.Instantiate(SkillObject_pre);
				gameObject.transform.localScale = new Vector3(attribute.volume / 4f , attribute.volume / 4f , 0);
				
				SkillObject skillObject = gameObject.GetComponent<SkillObject>();
				skillObject.res = 9;
				if(this.specSign == true){
					skillObject.spriteAnimation.renderer.sortingLayerID = 7;
				}else{
					skillObject.spriteAnimation.renderer.sortingLayerID = 1;
				}
				
				skillObject.transform.position = attackOne.transform.position + attackOff;
				
				switch(dirction){
				case MoveDirection.DOWN:
					skillObject.SetSpriteEulerAngles(new Vector3(0,0, 270));
					break;
				case MoveDirection.UP:
					skillObject.SetSpriteEulerAngles(new Vector3(0,0, 90));
					break;
				case MoveDirection.LEFT:
					skillObject.SetSpriteOff(new Vector2(0 , 0.15f));
					skillObject.gameObject.transform.localScale += new Vector3(-skillObject.gameObject.transform.localScale.x * 2 , 0, 0);
					break;
				case MoveDirection.RIGHT:
					skillObject.SetSpriteOff(new Vector2(0 , 0.15f));
					break;
				}

				skillObjects.Add(skillObject);
			}

			attacked = true;
		}

		if(attacked == false){
			return;
		}


		for(int i = 0 ; i < skillObjects.Count ; i++){
			SkillObject skillObject = (SkillObject)skillObjects[i];
			MoveDirection direction = (MoveDirection)directions[i];

			if(skillObject == null){
				continue;
			}

			if(Vector3.Distance(beginPosition , skillObject.transform.position) > distance){
				MonoBehaviour.Destroy(skillObject.gameObject);
				skillObjects[i] = null;
				continue;
			}


			float d1 = Time.deltaTime * speed;

			Vector3 newPosition = Vector3.zero;
			
			switch(direction){
			case MoveDirection.DOWN:
				newPosition = skillObject.transform.position + new Vector3(0 , -d1 , 0);
				break;
			case MoveDirection.UP:
				newPosition = skillObject.transform.position + new Vector3(0 , d1 , 0);
				break;
			case MoveDirection.LEFT:
				newPosition = skillObject.transform.position + new Vector3(-d1 , 0 , 0);
				break;
			case MoveDirection.RIGHT:
				newPosition = skillObject.transform.position + new Vector3(d1 , 0 , 0);
				break;
			}

			bool inNewGrid = false;

			if(BattleUtils.PositionToGrid(newPosition) != BattleUtils.PositionToGrid(skillObject.transform.position)){
				//in diffrenet grid
				inNewGrid = true;
			}

			skillObject.transform.position = newPosition;

			if(inNewGrid == false){
				continue;
			}


			//Try attack
			Vector2 v = BattleUtils.PositionToGrid(skillObject.transform.position);

			ArrayList objects = BattleControllor.GetGameObjectsByPosition(v);
			Charactor attackedOne = null;

			for(int j = 0 ; j < objects.Count ; j++){
				Charactor c = objects[j] as Charactor;

				if(c.GetType() != this.attackOne.GetType() && c.IsActive() == true){
					attackedOne = c;
					break;
				}
			}


			if(attackedOne != null){
				bool crit = BattleControllor.Crit(skillConfig.crit);
				float damage = BattleControllor.Attack(attackOne.GetAttribute() , attackedOne.GetAttribute() , skillConfig.demageratio , skillConfig.b , crit);

				attackedOne.ChangeHP(damage , crit);
				
				if(attackedOne.GetAttribute().hp > 0){
					attackedOne.PlayAttacked();
				}else{
					attackedOne.PlayDead();
				}
				
				return;
			}



		}

		
		for(int i = 0 ; i < skillObjects.Count ; i++){
			if(skillObjects[i] != null){
				return;
			}
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
