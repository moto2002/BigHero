using UnityEngine;
using System.Collections;

public class HpPop : MonoBehaviour {

	private Animator animator;

	public Sprite [] nums;

	// Use this for initialization
	void Start () {
		this.animator = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
		if(this.animator == null){
			return;
		}

		if(this.animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.6){
			Destroy(this.gameObject);
		}
	}

	public void SetValue(float value){
		string s = value.ToString();
		
		float off = 0;
		
		for(int i = 0 ; i < s.Length; i++){
			int k = int.Parse(s[i] + "");
			
			GameObject go = new GameObject();
			go.AddComponent("SpriteRenderer");
			go.transform.parent = this.transform;
			
			SpriteRenderer sp = go.GetComponent<SpriteRenderer>();

			sp.sortingLayerID = 2;
			
			sp.sprite = nums[k];
			
			go.transform.localPosition = new Vector2(off - 0.2f , 0);
			
			off += sp.sprite.bounds.size.x/2 + 0.05f;
		}
	}
}
