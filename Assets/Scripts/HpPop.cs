using UnityEngine;
using System.Collections;

public class HpPop : MonoBehaviour {

	public Sprite [] nums;

	private bool crit = false;

	private float liveTime = 0.5f;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		liveTime -= Time.deltaTime;

		if(liveTime < 0){
			Destroy(this.gameObject);
			return;
		}

		this.transform.position = this.transform.position + new Vector3(0 , 0.7f * Time.deltaTime, 0);


		if(crit == true && liveTime < 0.4f && liveTime > 0.2f){
			this.transform.localScale = this.transform.localScale + new Vector3(8f * Time.deltaTime , 8f * Time.deltaTime, 0);
		}
	}

	public void SetValue(float value){

		value = Mathf.Abs(value);

		string s = ((int)value).ToString();
		
		float off = 0;

		ArrayList sps = new ArrayList();
		
		for(int i = 0 ; i < s.Length; i++){
			int k = int.Parse(s[i] + "");
			
			GameObject go = new GameObject();
			go.AddComponent("SpriteRenderer");
			go.transform.parent = this.transform;
			
			SpriteRenderer sp = go.GetComponent<SpriteRenderer>();

			sp.sortingLayerID = 2;
			sp.sprite = nums[k];
			
			go.transform.localPosition = new Vector2(off , 0);

			sps.Add(go);

			off += sp.sprite.bounds.size.x/2f + 0.05f;
		}


		for(int i = 0 ; i < sps.Count ; i++){
			GameObject go = (GameObject)sps[i];

			go.transform.localPosition -= new Vector3(off/2 ,0 , 0);
		}
	}

	public void SetCrit(bool crit){
		this.crit = crit;
	}
}
