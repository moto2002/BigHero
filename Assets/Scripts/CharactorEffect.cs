using UnityEngine;
using System.Collections;

public class CharactorEffect : MonoBehaviour {
	
	public GameObject SkillObject_pre;
	
	public GameObject hp_pop;

	public GameObject hp_pop_green;

	public ArrayList skillObjects;

	void Start () {
		skillObjects = new ArrayList();

		Invoke("CheckSkillObject" , 1);
	}

	void Update () {

		
	}

	private void CheckSkillObject(){
		foreach(SkillObject skillObject in skillObjects){
			if(skillObject.IsSpritePlayEnd() == true){
				skillObjects.Remove(skillObject);
			}
		}
	}


	public void PlayEffect(int type){

		switch(type){
		case 1:
			
			GameObject gameObject = (GameObject)MonoBehaviour.Instantiate(SkillObject_pre);
			
			SkillObject skillObject = gameObject.GetComponent<SkillObject>();
			skillObject.res = 11;
			skillObject.loop = 1;
			skillObject.spriteAnimation.sortingLayerID = 3;
			skillObject.transform.position = transform.position;
			skillObject.transform.parent = transform;
			
			skillObject.transform.localScale = new Vector2(0.25f , 0.25f);
			
			skillObject.SetSpec(true);
			skillObjects.Add(skillObject);

			break;
		}
	}

	public void PlayNum(int value , bool crit){

		if(value > 0){
			GameObject go = (GameObject)Instantiate(this.hp_pop);
			HpPop hpPop = go.GetComponent<HpPop>();
			go.transform.parent = this.transform;
			go.transform.localPosition = new Vector3(0, 0.2f ,0);
			hpPop.SetValue(value);
			hpPop.SetCrit(crit);
		}else if(value < 0){
			GameObject go = (GameObject)Instantiate(this.hp_pop_green);
			HpPop hpPop = go.GetComponent<HpPop>();
			go.transform.parent = this.transform;
			go.transform.localPosition = new Vector3(0, 0.2f ,0);
			hpPop.SetValue(value);
			hpPop.SetCrit(crit);
		}
	}
}
