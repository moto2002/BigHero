using UnityEngine;
using System.Collections;

public class iLog : MonoBehaviour {

	static string pDocument = "";
	
	public static void log (string s) {
		pDocument += "\n" + s;
	}
	void OnGUI () {
		//GUI.TextField (new Rect (10, 10, Screen.width-10, Screen.height-10), pDocument);
	}
	
	
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
