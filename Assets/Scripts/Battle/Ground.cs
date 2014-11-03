using UnityEngine;
using System.Collections;
using LitJson;


public class Ground : MonoBehaviour {

	public SpriteRenderer groudBlock;

	private Sprite [] sprites ;

	[HideInInspector]
	public int v;
	[HideInInspector]
	public int h;

	void Start () {
		if(sprites == null){
			sprites = Resources.LoadAll<Sprite>(@"Image/Ground/Ground");
		}
	}

	public void drawGround(JsonData mapGridConfig){
		SpriteRenderer sr;

		v = mapGridConfig.Count;
		h = mapGridConfig[0].Count;

		for (int i = 0; i < v; i++) {
			for(int j = 0 ; j < h ; j++){

				sr = (SpriteRenderer)Instantiate (groudBlock);
				
				sr.transform.position = new Vector3(j * Constance.GRID_GAP + this.transform.position.x , -i * Constance.GRID_GAP + this.transform.position.y , 0);
				sr.transform.parent = this.transform;

				if(sprites == null){
					sprites = Resources.LoadAll<Sprite>(@"Image/Ground/Ground");
				}

				sr.sprite = sprites[(int)mapGridConfig[i][j]];
				Color c = sr.color;
				c.a = 0f;
				sr.color = c;
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}


	public void Clear(){
		for(int i = 0 ; i < this.transform.childCount; i++){
			Destroy(this.transform.GetChild(i).gameObject);
		}
	}
}
