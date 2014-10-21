using UnityEngine;
using System.Collections;

public class AttRange {
	

	public static ArrayList GetRangeByAttType(int type , int range , int volume , Vector2 zeroPoint , MoveDirection direction = MoveDirection.UP){

		switch(type){
		case 1:
			return HalfRectRange(range , volume , zeroPoint , direction);
			break;
		case 2:
			return LineRange(range , volume , zeroPoint , direction);
			break;
		case 3:
			return RectRange(range , volume , zeroPoint);
			break;
		case 4:
			return SubRectRange(range , volume , zeroPoint);
			break;
		case 5:
			return SectorRange(range , volume , zeroPoint , direction);
			break;
		case 6:
			return RectRange(range , volume , zeroPoint);
			break;
		case 7:
			return RectRange(range , volume , zeroPoint);
			break;
		case 8:
			return RectRange(range , volume , zeroPoint);
			break;
		case 9:
			return HalfRectRange(range , volume , zeroPoint , direction);
			break;
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
				
//				if( i >= zeroPoint.x && i < zeroPoint.x + volume && j >= zeroPoint.y && j < zeroPoint.y + volume){
//					continue;
//				}
				
				rangs.Add(new Vector2(i ,j));
			}
		}
		
		return rangs;
	}


	private static ArrayList SubRectRange(int range , int volume , Vector2 zeroPoint){

		int minx = (int)zeroPoint.x - range;
		int maxx = (int)zeroPoint.x + range + volume;
		int miny = (int)zeroPoint.y - range;;
		int maxy = (int)zeroPoint.y + range + volume;

		ArrayList rangs = new ArrayList();


		for(int i = 0; i < Battle.v ; i++){
			for(int j = 0 ; j < Battle.h ; j++){

				if(j >= minx && j <= maxx && i >= miny && i <= maxy){
					continue;
				}

				rangs.Add(new Vector2(j ,i));
			}
		}

		return rangs;
	}


	private static ArrayList SectorRange(int range , int volume, Vector2 zeroPoint , MoveDirection direction){
		
		ArrayList rangs = new ArrayList();

		switch(direction){
		case MoveDirection.LEFT:
			//left
			for(int i = 0 ; i < range ; i++){
				
				int x = (int)zeroPoint.x - 1 - i;
				
				for(int j = 0 ; j < volume + i * 2 ; j++){
					
					int y = (int)zeroPoint.y - i + j;
					
					rangs.Add(new Vector2(x ,y));
				}
			}
			break;
		case MoveDirection.UP:
			for(int i = 0 ; i < range ; i++){
				
				int y = (int)zeroPoint.y - 1 - i;
				
				for(int j = 0 ; j < volume + i * 2 ; j++){
					
					int x = (int)zeroPoint.x - i + j ;
					
					rangs.Add(new Vector2(x ,y));
				}
			}
			break;
		case MoveDirection.RIGHT:
			for(int i = 0 ; i < range ; i++){
				
				int x = (int)zeroPoint.x + volume + i;
				
				for(int j = 0 ; j < volume + i * 2 ; j++){
					
					int y = (int)zeroPoint.y - i + j;
					
					rangs.Add(new Vector2(x ,y));
				}
			}
			break;
		case MoveDirection.DOWN:
			for(int i = 0 ; i < range ; i++){
				
				int y = (int)zeroPoint.y + volume + i;
				
				for(int j = 0 ; j < volume + i * 2 ; j++){
					
					int x = (int)zeroPoint.x - i + j ;
					
					rangs.Add(new Vector2(x ,y));
				}
			}
			break;
		}

		return rangs;
	}


	private static ArrayList HalfRectRange(int range , int volume, Vector2 zeroPoint , MoveDirection direction){
		ArrayList rangs = new ArrayList();

		switch(direction){
		case MoveDirection.LEFT:
			//left
			for(int i = 0 ; i < range ; i++){
				
				int x = (int)zeroPoint.x - i - 1;
				
				for(int j = 0 ; j < volume + range * 2; j++){
					int y = (int)zeroPoint.y - range + j;
					
					rangs.Add(new Vector2(x ,y));
				}
			}
			break;
		case MoveDirection.UP:
			for(int i = 0 ; i < range ; i++){
				
				int y = (int)zeroPoint.y - 1 - i;
				
				for(int j = 0 ; j < volume + range * 2 ; j++){
					
					int x = (int)zeroPoint.x - range + j;
					
					rangs.Add(new Vector2(x ,y));
				}
			}
			break;
		case MoveDirection.RIGHT:
			for(int i = 0 ; i < range + volume ; i++){
				
				int x = (int)zeroPoint.x + i;
				
				for(int j = 0 ; j < volume + range * 2 ; j++){
					
					int y = (int)zeroPoint.y - range + j;
					
					rangs.Add(new Vector2(x ,y));
				}
			}
			break;
		case MoveDirection.DOWN:
			for(int i = 0 ; i < range + volume ; i++){
				
				int y = (int)zeroPoint.y + i;
				
				for(int j = 0 ; j < volume + range * 2 ; j++){
					
					int x = (int)zeroPoint.x - range + j ;
					
					rangs.Add(new Vector2(x ,y));
				}
			}
			break;
		}

		return rangs;
	}


	private static ArrayList LineRange(int range , int volume, Vector2 zeroPoint , MoveDirection direction){
		ArrayList rangs = new ArrayList();

		switch(direction){
		case MoveDirection.LEFT:
			//left
			for(int i = 0 ; i < range ; i++){

				int x = (int)zeroPoint.x - i - 1;

				for(int j = 0 ; j < volume; j++){
					int y = (int)zeroPoint.y + j;
					
					rangs.Add(new Vector2(x ,y));
				}
			}
			break;
		case MoveDirection.UP:
			for(int i = 0 ; i < range ; i++){
				
				int y = (int)zeroPoint.y - 1 - i;
				
				for(int j = 0 ; j < volume ; j++){
					
					int x = (int)zeroPoint.x + j;
					
					rangs.Add(new Vector2(x ,y));
				}
			}
			break;
		case MoveDirection.RIGHT:
			for(int i = 0 ; i < range ; i++){
				
				int x = (int)zeroPoint.x + volume + i;
				
				for(int j = 0 ; j < volume ; j++){
					
					int y = (int)zeroPoint.y + j;
					
					rangs.Add(new Vector2(x ,y));
				}
			}
			break;
		case MoveDirection.DOWN:
			for(int i = 0 ; i < range ; i++){
				
				int y = (int)zeroPoint.y + volume + i;
				
				for(int j = 0 ; j < volume ; j++){
					
					int x = (int)zeroPoint.x + j ;
					
					rangs.Add(new Vector2(x ,y));
				}
			}
			break;
		}

		return rangs;
	}

}







