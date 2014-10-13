using UnityEngine;
using System.Collections;

public class WalkerPathSkill : Skill {

	private bool end = false;

	private Charactor attackone;

	private SkillConfig skillconfig;

	public WalkerPathSkill(Charactor attackone , SkillConfig skillconfig){
		this.attackone = attackone;
		this.skillconfig = skillconfig;


	}

	// Use this for initialization
	public void Start () {
	
	}
	
	// Update is called once per frame
	public void Update () {
	
	}

	public bool IsEnd(){
		return end;
	}
}
