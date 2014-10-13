using UnityEngine;
using System.Collections;

public class BattleUtils{
	
	public static Vector2 GridToPosition(Vector2 v){
		return GridToPosition((int)v.x , (int)v.y);
	}
	
	public static Vector2 GridToPosition(int x , int y ){
		Vector2 v = new Vector3(x * Constance.GRID_GAP , -y * Constance.GRID_GAP );
		
		return v;
	}

	
	
	public static Vector2 PositionToGrid(Vector2 v){
		return PositionToGrid(v.x , v.y);
	}
	
	
	public static Vector2 PositionToGrid(float x , float y ){
		Vector2 v = new Vector2((int)Mathf.Round(x / Constance.GRID_GAP), -(int)Mathf.Round(y / Constance.GRID_GAP));
		return v;
	}

}
