using UnityEngine;
using System.Collections;

public class InputManager : MonoBehaviour {

	private bool press = false;

	private Vector3 startPoint;

	private Vector3 lastPoint;

	private float holdTime;

	void Update(){

		if(BattleControllor.hero == null){
			return;
		}

		if(BattleControllor.state == BattleState.STATE_TO_NEXT){
			return;
		}

		if (Input.GetKeyDown (KeyCode.W)) {
			//up
			BattleControllor.hero.direction = MoveDirection.UP;
		}else if (Input.GetKeyDown (KeyCode.S)) {
			//down
			BattleControllor.hero.direction = MoveDirection.DOWN;
		}else if (Input.GetKeyDown (KeyCode.A)) {
			//left
			BattleControllor.hero.direction = MoveDirection.LEFT;
		}else if (Input.GetKeyDown (KeyCode.D)) {
			//right
			BattleControllor.hero.direction = MoveDirection.RIGHT;
		}

		if(Input.GetMouseButtonDown(0)){
			press = true;
			startPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		}
		
		if(Input.GetMouseButtonUp(0)){
			press = false;
		}

		if(press){
			Vector3 nowPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);


			if(Vector3.Distance(lastPoint , nowPoint) < 0.05f){
				holdTime += Time.deltaTime;

				if(holdTime > 0.1f){
					startPoint = nowPoint;
					holdTime = 0f;
				}
			}else{
				lastPoint = nowPoint;
				holdTime = 0f;
			}

			
			if(Vector3.Distance(startPoint , nowPoint) < 0.2f){
				return;
			}

			float x = nowPoint.x - startPoint.x;
			float y = nowPoint.y - startPoint.y;

			if(Mathf.Abs(x) > Mathf.Abs(y)){

				if(x > 0){
					BattleControllor.hero.direction = MoveDirection.RIGHT;
				}else{
					BattleControllor.hero.direction = MoveDirection.LEFT;
				}

			}else{

				if(y > 0){
					BattleControllor.hero.direction = MoveDirection.UP;
				}else{
					BattleControllor.hero.direction = MoveDirection.DOWN;
				}
			}
		}



//		float x = Input.acceleration.x;
//		float y = Input.acceleration.y;
//
//
//		if(Mathf.Abs(x) > Mathf.Abs(y)){
//			if(x > 0){
//				Battle.hero.direction = MoveDirection.RIGHT;
//			}else{
//				Battle.hero.direction = MoveDirection.LEFT;
//			}
//
//		}else{
//			if(y > 0){
//				Battle.hero.direction = MoveDirection.UP;
//			}else{
//				Battle.hero.direction = MoveDirection.DOWN;
//			}
//		}





//		if(Input.GetMouseButtonDown(0)){   
//			
//			Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
//
//
//
//			Vector3 heroPosition = Battle.hero.transform.position;
//
//			if(Mathf.Abs(heroPosition.x - mouseWorldPosition.x) > Mathf.Abs(heroPosition.y - mouseWorldPosition.y)){
//				if(mouseWorldPosition.x > heroPosition.x){
//					Battle.hero.direction = MoveDirection.RIGHT;
//				}else{
//					Battle.hero.direction = MoveDirection.LEFT;
//				}
//
//			}else{
//				if(mouseWorldPosition.y > heroPosition.y){
//					Battle.hero.direction = MoveDirection.UP;
//				}else{
//					Battle.hero.direction = MoveDirection.DOWN;
//				}
//			}
//		}   
	}

	// Use this for initialization
	void OnEnable()
	{
		//启动时调用，这里开始注册手势操作的事件。
		//上、下、左、右、四个方向的手势滑动
		FingerGestures.OnFingerSwipe += OnFingerSwipe;
	}
	
	void OnDisable()
	{
		FingerGestures.OnFingerSwipe -= OnFingerSwipe;
	}


	//上下左右四方方向滑动手势操作
	void OnFingerSwipe( int fingerIndex, Vector2 startPos, FingerGestures.SwipeDirection direction, float velocity )
	{
		//结果是 Up Down Left Right 四个方向
		Debug.Log("OnFingerSwipe " + direction + " with finger " + fingerIndex);
		if(BattleControllor.hero == null){
			return;
		}
		
		switch(direction){
		case FingerGestures.SwipeDirection.Down:
			BattleControllor.hero.direction = MoveDirection.DOWN;
			break;
		case FingerGestures.SwipeDirection.Up:
			BattleControllor.hero.direction = MoveDirection.UP;
			break;
		case FingerGestures.SwipeDirection.Right:
			BattleControllor.hero.direction = MoveDirection.RIGHT;
			break;
		case FingerGestures.SwipeDirection.Left:
			BattleControllor.hero.direction = MoveDirection.LEFT;
			break;
		}
	}

}
