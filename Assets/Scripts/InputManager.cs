using UnityEngine;
using System.Collections;

public class InputManager : MonoBehaviour {


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

		switch(direction){
		case FingerGestures.SwipeDirection.Down:
			Battle.hero.direction = MoveDirection.DOWN;
			break;
		case FingerGestures.SwipeDirection.Up:
			Battle.hero.direction = MoveDirection.UP;
			break;
		case FingerGestures.SwipeDirection.Right:
			Battle.hero.direction = MoveDirection.RIGHT;
			break;
		case FingerGestures.SwipeDirection.Left:
			Battle.hero.direction = MoveDirection.LEFT;
			break;
		}
	}

}
