using UnityEngine;
using System.Collections;

public class Item : MonoBehaviour {

	public SpriteRenderer spriteRenderer;

	[HideInInspector]
	public ItemConfig itemConfig;

	private bool playAnimation = false;

	public float s = 0.8f;

	private Vector3 rendererPosition;


	private Vector2 point;

	void Start () {
		playAnimation = true;

		rendererPosition = spriteRenderer.transform.localPosition;

		spriteRenderer.sortingOrder = -(int)(this.transform.localPosition.y * 10);

		point = BattleUtils.PositionToGrid(this.transform.localPosition);
	}
	

	void Update () {

		if(playAnimation == false){
			return;
		}

		if(s < -0.8f){
			playAnimation = false;
			this.spriteRenderer.transform.localPosition = rendererPosition;
			return;
		}

		this.spriteRenderer.transform.localPosition += new Vector3(0 , s * Time.deltaTime , 0);

		s -= 0.05f;
	}


	public void SetItemId(int id){
		spriteRenderer.sprite = Resources.Load<Sprite>("Image/Item/" + id);
	}

	public Vector2 GetPoint(){
		return point;
	}

	public bool Pick(Hero hero){

		Attribute attribute = hero.GetAttribute();

		if(itemConfig.addhp > 0){
			float addhp = (-attribute.maxHp * itemConfig.addhp) / 10000;
			hero.ChangeHP( addhp , false);
			hero.PlayEffect(1);

			Follower f = hero.follower;

			while(f != null){
				f.ChangeHP(addhp , false);
				f = f.follower;
			}
		}
		
		if(itemConfig.addpow > 0){
		}
		
		if(itemConfig.atk > 0){
		}

		BattleControllor.RemoveItem(this);
		
		return true;
	}
}
