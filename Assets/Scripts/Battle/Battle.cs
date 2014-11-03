using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using LitJson;

public class Battle : MonoBehaviour {

	public Ground ground;

	public SpriteRenderer specMask;

	public SpecSkillEffect spectEffect;
	
	public GameObject exitPoint;
	public GameObject enterPoint;

	public GameObject exitEffect_pre;
	
	private Skill [] skilInSpec;
	
	private Charactor[] charInSpec;

	void Start () {
		exitPoint.gameObject.SetActive(false);
		enterPoint.gameObject.SetActive(false);

		BattleControllor.battle = this;
	}

	void Update () {
		if(Constance.SPEC_RUNNING == true){
			CheckSpecState();
		}


		BattleControllor.Update();
	}

	public void playSkill(){
		if(BattleControllor.hero == null){
			return;
		}
		
		StartCoroutine(PlaySpecSkillEffect()); 
	}
	
	
	private IEnumerator PlaySpecSkillEffect(){
		
		Skill skill = BattleControllor.hero.playSkill(0);
		
		if(skill != null){
			Constance.SPEC_RUNNING = true;
			Constance.RUNNING = false;
			
			Color color = new Color();
			color.a = 0.5f;
			specMask.color = color;
			
			spectEffect.Active(1);
			yield return new WaitForSeconds(1.5f);
			
			spectEffect.Hide();
			IntoSpecTime(new Charactor[]{BattleControllor.hero} , new Skill[]{skill}); 
		}
	}
	
	
	public void IntoSpecTime(Charactor[] charactors , Skill [] skills){
		
		for(int i = 0 ; i < charactors.Length ; i++){
			Charactor c = charactors[i];
			c.SetSpec(true);
		}
		
		for(int i = 0 ; i < skills.Length ; i++){
			Skill skill = skills[i];
			skill.SetSpec(true);
		}
		
		skilInSpec = skills;
		charInSpec = charactors;
	}

	
	private void CheckSpecState(){
		if(skilInSpec == null){
			return;
		}
		
		for(int i = 0 ; i < skilInSpec.Length ; i++){
			Skill skill = skilInSpec[i];
			
			if(skill.IsEnd() == false){
				return;
			}
		}
		
		Constance.SPEC_RUNNING = false;
		Constance.RUNNING = true;
		
		Color color = new Color();
		color.a = 0f;
		specMask.color = color;
		
		for(int i = 0 ; i < charInSpec.Length ; i++){
			Charactor c = charInSpec[i];
			c.SetSpec(false);
		}
		
		charInSpec = null;
		skilInSpec = null;
	}


	public void AnimationPlay(){
		Constance.RUNNING = true;
	}

	public void AnimationStop(){
		Constance.RUNNING = false;
	}

	public void X1(){
		Time.timeScale = 1;
	}

	public void X2(){
		Time.timeScale = 2;
	}

	public void HideEnterPoint(){
		SetEnterPoint(false);
	}


	public void SetExitPoint(bool b){
		exitPoint.gameObject.SetActive(b);
	}


	public void SetEnterPoint(bool b){
		enterPoint.gameObject.SetActive(b);
	}

	public void PlayExitEffect(){
		GameObject go = (GameObject)Instantiate(exitEffect_pre);

		SpriteAnimation exitEffect = go.GetComponent<SpriteAnimation>();
		exitEffect.autoDestroy = true;

		go.transform.position = exitPoint.transform.position;
	}
}

















