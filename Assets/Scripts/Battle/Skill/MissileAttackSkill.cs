using UnityEngine;
using System.Collections;

public class MissileAttackSkill  {
	
	private bool specSign = false;

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
		return false;
	}
}
