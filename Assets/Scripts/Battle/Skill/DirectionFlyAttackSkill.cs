using UnityEngine;
using System.Collections;

public class DirectionFlyAttackSkill : Skill {

	private int effectId;

	private float distance;

	private Charactor attackOne;
	
	private Transform attackTransfrom;
	
	private bool end = false;

	private static GameObject SkillObject_pre  = (GameObject)Resources.Load("Prefabs/SkillEffect");

	private ArrayList skillObjects;

	private float speed = 6f;

	private Vector3 attackOff = Vector2.zero;

	private ArrayList directions;

	private Vector3 beginPosition;

	private SkillConfig skillConfig;

	private bool attacked = false;
	
	public DirectionFlyAttackSkill(Charactor attackOne , SkillConfig skillConfig){

		this.skillConfig = skillConfig;

		this.effectId = skillConfig.res;
		this.distance = skillConfig.range * Constance.GRID_GAP;

		this.attackOne = attackOne;
		this.attackTransfrom = this.attackOne.transform;

		attackOff = new Vector3((attackOne.GetAttribute().volume/2.0f - 0.5f) * Constance.GRID_GAP , (attackOne.GetAttribute().volume/2.0f - 0.5f) * Constance.GRID_GAP , 0);

		directions = new ArrayList();
		skillObjects = new ArrayList();

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
		this.attackOne.PlayAttack();
	}
	
	public void Update(){

		if(this.end == true){
			return;
		}

		if(attackOne.IsInAttIndex() == true && attacked == false){
			beginPosition = attackOne.transform.position + attackOff;


			for(int i = 0 ; i < directions.Count ; i++){
				MoveDirection dirction = (MoveDirection)directions[i];

				GameObject gameObject = (GameObject)MonoBehaviour.Instantiate(SkillObject_pre);
				
				SkillObject skillObject = gameObject.GetComponent<SkillObject>();
				skillObject.res = this.skillConfig.res;
				skillObject.SetSpriteOff(new Vector2(0 , 0.3f));
				
				skillObject.transform.position = attackOne.transform.position + attackOff;
				
				switch(dirction){
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

			Vector2 v = BattleUtils.PositionToGrid(skillObject.transform.position);

			ArrayList objects = Battle.GetGameObjectsByPosition(v);
			Charactor attackedOne = null;

			for(int j = 0 ; j < objects.Count ; j++){
				if((objects[j] as Charactor).GetType() == this.attackOne.GetType()){
					continue;
				}else{
					attackedOne = objects[j] as Charactor;
				}
			}


			if(attackedOne != null){
				MonoBehaviour.Destroy(skillObject.gameObject);

				float damage = Battle.Attack(attackOne.GetAttribute() , attackedOne.GetAttribute());
				
				attackedOne.ChangeHP(damage);
				
				if(attackedOne.GetAttribute().hp > 0){
					attackedOne.PlayAttacked();
				}else{
					attackedOne.PlayDead();
				}
				
				return;
			}


			float d1 = Time.deltaTime * speed;

			switch(direction){
			case MoveDirection.DOWN:
				skillObject.transform.position = skillObject.transform.position + new Vector3(0 , -d1 , 0);
				break;
			case MoveDirection.UP:
				skillObject.transform.position = skillObject.transform.position + new Vector3(0 , d1 , 0);
				break;
			case MoveDirection.LEFT:
				skillObject.transform.position = skillObject.transform.position + new Vector3(-d1 , 0 , 0);
				break;
			case MoveDirection.RIGHT:
				skillObject.transform.position = skillObject.transform.position + new Vector3(d1 , 0 , 0);
				break;
			}
		}

		
		for(int i = 0 ; i < skillObjects.Count ; i++){
			if(skillObjects[i] != null){
				return;
			}
		}

		end = true;
	}



	public bool IsEnd(){
		return end;
	}
}
