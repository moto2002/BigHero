using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour {

	public Battle battle;

	private Hero hero;

	void Start () {

	}


	void Update () {
		FollowHero();
	}


	private void FollowHero(){
		
		if(Battle.hero == null){
			return;
		}
		
		float w = battle.ground.h;
		float h = battle.ground.v;
		
		if(w == 0 && h == 0){
			return;
		}
		
		float x = Battle.hero.transform.position.x;
		float y = Battle.hero.transform.position.y;

		if(x < 4 * Constance.GRID_GAP){
			x = 4 * Constance.GRID_GAP;
		}

		if(x > (w - 4) * Constance.GRID_GAP){
			x = (w - 4) * Constance.GRID_GAP;
		}

		
		if(-y < 6 * Constance.GRID_GAP){
			y = - 6 * Constance.GRID_GAP;
		}
		
		if(-y > (h - 6) * Constance.GRID_GAP){
			y = -(h - 6) * Constance.GRID_GAP;
		}
		
		this.transform.position = new Vector3(x , y , -20);
	}
}
