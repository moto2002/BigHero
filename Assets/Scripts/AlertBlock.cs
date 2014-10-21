using UnityEngine;
using System.Collections;

public class AlertBlock : MonoBehaviour {

	// Use this for initialization
	void Start () {
		SpriteAnimation sp = this.GetComponent<SpriteAnimation>();

		sp.sprites =  Resources.LoadAll<Sprite>(@"Image/AlertBlock");
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
