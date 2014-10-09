using UnityEngine;
using System.Collections;

public class DirectionFlyAttackSkill : Skill {
	
	private Charactor attackOne;
	
	private Transform attackTransfrom;

	private Transform skillTransfrom;
	
	private bool end = false;

	private static GameObject SkillObject_pre  = (GameObject)Resources.Load("Prefabs/SkillEffect");

	private SkillObject skillObject;

	private float speed = 6f;

	private Vector3 off = Vector2.zero;
	
	public DirectionFlyAttackSkill(Charactor attackOne , int effectId , float distance){
		this.attackOne = attackOne;

		this.attackTransfrom = this.attackOne.transform;
	}
	
	public void Start(){

		GameObject gameObject = (GameObject)MonoBehaviour.Instantiate(SkillObject_pre);
		
		skillObject = gameObject.GetComponent<SkillObject>();
		skillObject.transform.position = attackOne.transform.position;

		skillTransfrom = skillObject.transform;

		switch(attackOne.GetDirection()){
		case MoveDirection.DOWN:
			break;
		case MoveDirection.UP:
			break;
		case MoveDirection.LEFT:
			break;
		case MoveDirection.RIGHT:
			break;
		}
	}
	
	public void Update(){

		if(this.end == true){
			return;
		}

		float d1 = Time.deltaTime * speed;

	}



	public bool IsEnd(){
		return end;
	}
}
