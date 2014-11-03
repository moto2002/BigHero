using UnityEngine;
using System.Collections;

public class SpecSkillEffect : MonoBehaviour {
	
	public GameObject bg;
	
	public SpriteRenderer heroImage;
	
	public SpriteRenderer skillName;


	private Vector3 bgStart;

	private Vector3 heroStart;

	private Vector3 nameStart;


	// Use this for initialization
	void Start () {
		Hide();

		this.gameObject.transform.position = Vector3.zero;

		bgStart = bg.transform.position;
		heroStart = heroImage.transform.position;
		nameStart = skillName.transform.position;
	}
	
	// Update is called once per frame
	void Update () {

		bg.transform.position += new Vector3(4 * Time.deltaTime, 0 , 0);


		if(heroImage.transform.position.x - (-0.93f) > 0.01){
			heroImage.transform.position += new Vector3(- 10 * Time.deltaTime, 0 , 0);
		}

		
		if(1.7f - skillName.transform.position.x > 0.01){
			skillName.transform.position += new Vector3(10 * Time.deltaTime, 0 , 0);
		}
	}

	public void Active(int skillId){
		this.gameObject.SetActive(true);
		this.gameObject.transform.position = Vector3.zero;

		bg.transform.position = bgStart;
		heroImage.transform.position = heroStart;
		skillName.transform.position = nameStart;
	}

	public void Hide(){
		this.gameObject.SetActive(false);

	}
}
