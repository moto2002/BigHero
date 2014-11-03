using UnityEngine;
using System.Collections;

public class WalkerPathSkill {

	private bool end = false;

	private Charactor attackone;

	private SkillConfig skillconfig;

	private bool specSign = false;

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
	
	
	public void SetSpec(bool b){
		this.specSign = b;
	}

	public bool IsEnd(){
		return end;
	}
}
