using UnityEngine;
using System.Collections;

public class HPBar : MonoBehaviour {


	public GameObject hpBar;

	void Start () {
	}

	void Update () {
	}

	public void SetHP(float v){
		if(hpBar == null){
			return;
		}

		if(v < 0 ){
			v = 0;
		}

		hpBar.transform.localScale = new Vector2(v , 1);
	}

}
