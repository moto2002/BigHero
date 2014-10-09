using UnityEngine;
using System.Collections;

public class AttRange {

	public const int TYPE_CORSS = 1;

	public const int TYPE_RECT = 2;

	public static ArrayList GetRange(int type , int range , int volume , Vector2 zorePoint){

		switch(type){
		case TYPE_CORSS:
			return CrossRange(range , volume, zorePoint);
		case TYPE_RECT:
			return RectRange(range , volume , zorePoint);
		}

		return new ArrayList();
	}



	private static ArrayList CrossRange(int range , int volume , Vector2 zorePoint){

		ArrayList rangs = new ArrayList();
		
		//Down
		for(int d1 = 0 ; d1 < range ; d1++){
			for(int v1 = 0 ; v1 < volume ; v1 ++){
				rangs.Add(new Vector2(zorePoint.x + v1 , zorePoint.y + d1 + volume));
			}
		}
		
		//Up
		for(int d2 = 0 ; d2 < range ; d2++){
			for(int v2 = 0 ; v2 < volume ; v2 ++){
				rangs.Add(new Vector2(zorePoint.x + v2 , zorePoint.y - 1 - d2));
			}
		}
		
		
		//left
		for(int d3 = 0 ; d3 < range ; d3++){
			for(int v3 = 0 ; v3 < volume ; v3 ++){
				rangs.Add(new Vector2(zorePoint.x - d3 - 1  , zorePoint.y + v3));
			}
		}
		
		//Right
		for(int d4 = 0 ; d4 < range ; d4++){
			for(int v4 = 0 ; v4 < volume ; v4 ++){
				rangs.Add(new Vector2(zorePoint.x + d4 + volume  , zorePoint.y + v4));
			}
		}
		return rangs;
	}

	
	private static ArrayList RectRange(int range , int volume , Vector2 zeroPoint){
		
		ArrayList rangs = new ArrayList();
		
		for(int i = (int)zeroPoint.x - range ; i < (int)zeroPoint .x + range + volume ; i++){
			for (int j = (int)zeroPoint.y - range ; j < (int)zeroPoint.y + range + volume ; j++){
				
				if( i >= zeroPoint.x && i < zeroPoint.x + volume && j >= zeroPoint.y && j < zeroPoint.y + volume){
					continue;
				}
				
				rangs.Add(new Vector2(i ,j));
			}
		}
		
		return rangs;
	}

}







