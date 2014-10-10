using UnityEngine;
using System.Collections;

public class DirectionFlyAttackSkill : Skill {

	private int effectId;

	private float distance;

	private Charactor attackOne;
	
	private Transform attackTransfrom;

	private Transform skillTransfrom;
	
	private bool end = false;

	private static GameObject SkillObject_pre  = (GameObject)Resources.Load("Prefabs/SkillEffect");

	private SkillObject skillObject;

	private float speed = 6f;

	private Vector3 attackOff = Vector2.zero;

	private MoveDirection dirction;

	private Vector3 beginPosition;

	private SkillConfig skillConfig;
	
	public DirectionFlyAttackSkill(Charactor attackOne , SkillConfig skillConfig){

		this.skillConfig = skillConfig;

		this.effectId = skillConfig.res;
		this.distance = skillConfig.range * Constance.GRID_GAP;

		this.attackOne = attackOne;
		this.attackTransfrom = this.attackOne.transform;


		attackOff = new Vector3((attackOne.GetAttribute().volume/2.0f - 0.5f) * Constance.GRID_GAP , (attackOne.GetAttribute().volume/2.0f - 0.5f) * Constance.GRID_GAP , 0);
	}
	
	public void Start(){
		this.attackOne.PlayAttack();
	}
	
	public void Update(){

		if(this.end == true){
			return;
		}

		if(attackOne.IsInAttIndex() == true && skillObject == null){
			GameObject gameObject = (GameObject)MonoBehaviour.Instantiate(SkillObject_pre);
			
			skillObject = gameObject.GetComponent<SkillObject>();
			skillObject.res = this.skillConfig.res;
			skillObject.SetSpriteOff(new Vector2(0 , 0.3f));
			
			skillObject.transform.position = attackOne.transform.position + attackOff;
			
			skillTransfrom = skillObject.transform;
			
			beginPosition = skillTransfrom.position;
			
			dirction = attackOne.GetDirection();
			
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
		}

		if(skillObject == null){
			return;
		}

		if(Vector3.Distance(beginPosition , skillTransfrom.position) > distance){
			MonoBehaviour.Destroy(this.skillObject.gameObject);
			end = true;
		}

		Vector2 v = BattleUtils.PositionToGrid(this.skillTransfrom.position.x , this.skillTransfrom.position.y);

		ArrayList objects = Battle.GetGameObjectsByPosition(v);

		Charactor attackedOne = null;

		for(int i = 0 ; i < objects.Count ; i++){
			if(objects[i] as Charactor == this.attackOne){
				continue;
			}else{
				attackedOne = objects[i] as Charactor;
			}
		}


		if(attackedOne != null){
			MonoBehaviour.Destroy(this.skillObject.gameObject);
			end = true;

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

		switch(dirction){
		case MoveDirection.DOWN:
			skillTransfrom.position = skillTransfrom.position + new Vector3(0 , -d1 , 0);
			break;
		case MoveDirection.UP:
			skillTransfrom.position = skillTransfrom.position + new Vector3(0 , d1 , 0);
			break;
		case MoveDirection.LEFT:
			skillTransfrom.position = skillTransfrom.position + new Vector3(-d1 , 0 , 0);
			break;
		case MoveDirection.RIGHT:
			skillTransfrom.position = skillTransfrom.position + new Vector3(d1 , 0 , 0);
			break;
		}

	}



	public bool IsEnd(){
		return end;
	}
}
