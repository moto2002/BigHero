using UnityEngine;
using System.Collections;

public class Test : MonoBehaviour {

	// Use this for initialization
	void Start () {

		Texture2D testT = Resources.Load<Texture2D>("Image/Model/1/w");
		
		int w = testT.width/4;
		int h = testT.height/4;

		Sprite [] sprites = new Sprite[16];

		for(int i = 0 ; i < 4 ; i++){
			for(int j = 0 ; j < 4 ; j++){
				Rect r = new Rect();
				
				r.x = i * w;
				r.y = 3 * h;
				r.width = w;
				r.height = h;
				
				Vector2 p = new Vector2();
				
				p.x = 0.5f;
				p.y = 0.3f;
				
				Sprite s = Sprite.Create(testT , r ,p);
				
				sprites[i] = s;
				
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
