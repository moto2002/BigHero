using UnityEngine;
using System.Collections;

public class GuanItemIcon : MonoBehaviour {
	
	private int copyId;

	public CopyConfig copyConfig{
		set{
			copyId = value.id;
			
			CopyInfo info = UserInfoMgr.GetInstance().GetCopyInfo(copyId);
			int finishid = UserInfoMgr.GetInstance().finishCopyId;
			
			
			this.GetComponent<UIButton>().isEnabled = false;
			
			start1.gameObject.SetActive(false);
			start2.gameObject.SetActive(false);
			start3.gameObject.SetActive(false);
			
			enable = false;
			
			if(info != null){
				enable = true;
				this.GetComponent<UIButton>().isEnabled = true;
				
				if(info.star > 0){
					start1.gameObject.SetActive(true);
				}
				
				if(info.star > 1){
					start2.gameObject.SetActive(true);
				}
				
				if(info.star > 2){
					start3.gameObject.SetActive(true);
				}
				
			}else if(Config.GetInstance().GetNextCopyConfig(finishid).id == copyId){
				enable = true;
				this.GetComponent<UIButton>().isEnabled = true;
			}
		}
	}

	public GameObject icon;
	public GameObject start1;
	public GameObject start2;
	public GameObject start3;


	private bool enable;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
	}


	void OnClick(){
		if(enable){
			BattleControllor.copyId = copyId;
			Application.LoadLevel(2);
		}
	}
}
