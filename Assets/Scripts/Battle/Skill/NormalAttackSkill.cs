using UnityEngine;
using System.Collections;

public class NormalAttackSkill: Skill{

	private Charactor attackOne;

	private Charactor attackedOne;

	private bool end = false;

	public NormalAttackSkill(Charactor attackOne , Charactor attackedOne){
		this.attackOne = attackOne;
		this.attackedOne = attackedOne;
	}

	public void Start(){
		attackOne.PlayAttack();
	}

	public void Update(){

		if(this.end == true){
			return;
		}

		if(this.attackOne.IsInAttIndex() == false){
			return;
		}


		float damage = Battle.Attack(attackOne.GetAttribute() , attackedOne.GetAttribute());

		attackedOne.ChangeHP(damage);
		
		if(attackedOne.GetAttribute().hp > 0){
			attackedOne.PlayAttacked();
		}else{
			attackedOne.PlayDead();
		}

		end = true;
	}

	public bool IsEnd(){
		return end;
	}

}
