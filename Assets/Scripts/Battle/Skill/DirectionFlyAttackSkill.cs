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
	
	public DirectionFlyAttackSkill(Charactor attackOne , int effectId , float distance){

		this.effectId = effectId;
		this.distance = distance;

		this.attackOne = attackOne;
		this.attackTransfrom = this.attackOne.transform;

		attackOff = new Vector3((attackOne.GetAttribute().volume/2.0f - 0.5f) * Constance.GRID_GAP , (attackOne.GetAttribute().volume/2.0f - 0.5f) * Constance.GRID_GAP , 0);
	}
	
	public void Start(){

		GameObject gameObject = (GameObject)MonoBehaviour.Instantiate(SkillObject_pre);
		
		skillObject = gameObject.GetComponent<SkillObject>();
		skillObject.transform.position = attackOne.transform.position + attackOff;

		skillTransfrom = skillObject.transform;

		beginPosition = skillTransfrom.position;

		dirction = attackOne.GetDirection();

		switch(dirction){
		case MoveDirection.DOWN:
			skillTransfrom.eulerAngles = new Vector3(0,0, 270); 
			break;
		case MoveDirection.UP:
			skillTransfrom.eulerAngles = new Vector3(0,0, 90); 
			break;
		case MoveDirection.LEFT:
			skillTransfrom.eulerAngles = new Vector3(0,0, 180); 
			break;
		case MoveDirection.RIGHT:
			break;
		}
	}
	
	public void Update(){

		if(this.end == true){
			return;
		}

		if(Vector3.Distance(beginPosition , skillTransfrom.position) > distance){
			MonoBehaviour.Destroy(this.skillObject.gameObject);
			end = true;
		}

		//BattleUtils.PositionToGrid();

		//Battle.GetGameObjectsByPosition();

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
