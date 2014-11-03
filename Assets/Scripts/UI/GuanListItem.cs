using UnityEngine;
using System.Collections;

public class GuanListItem : MonoBehaviour {

	public int index;

	public GuanItemIcon [] itemIcons;

	// Use this for initialization
	void Start () {

		for(int i = 0 ; i < itemIcons.Length ; i++){
			GuanItemIcon guanItemIcon = itemIcons[i];
			guanItemIcon.copyConfig = Config.GetInstance().GetCopyConfigByIndex(i);
		}
	}
	
	// Update is called once per frame
	void Update () {

	}


	void OnClick(){

	}


}
