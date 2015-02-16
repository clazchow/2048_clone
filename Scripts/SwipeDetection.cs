using UnityEngine;
using System.Collections;

public class SwipeDetection : MonoBehaviour {
	private Vector2 initialPos;
	private Vector2 posDiff;

	void Update() {
		if (Input.GetMouseButtonDown (0)) {
			//print("Touchbegan!");
			initialPos = Input.mousePosition;
		}
		
		if (Input.GetMouseButtonUp (0)) {
			posDiff = (Vector2)Input.mousePosition - initialPos;
			print(posDiff);
			if (posDiff.y > 225){
				print("UP");
				SendMessage("AddRandomTile");
			} else if (posDiff.y < -225){
				print ("DOWN");
				SendMessage("AddRandomTile");
			} else if (posDiff.x > 225){
				print("RIGHT");
				SendMessage("AddRandomTile");
			} else if (posDiff.x < -225){
				print("LEFT");
				SendMessage("AddRandomTile");
			}
		}
	}
//		void Update () {
//		if (Input.touchCount > 0) {
//			print("touching");
//			Touch touch = Input.GetMouseButtonDown(0);
//
//			switch (touch.phase){
//
//			case TouchPhase.Began:
//				initialPos = touch.position;
//				break;
//
//			case TouchPhase.Ended:
//				float swipeDistVert = (new Vector3(0, touch.position.y, 0) - new Vector3(0, initialPos.y, 0)).magnitude;
//				if (swipeDistVert > minSwipeDistY){
//					float swipeValue = Mathf.Sign(touch.position.y - initialPos.y);
//					if (swipeValue > 0){
//						Debug.Log("UP");
//					} else if (swipeValue < 0){
//						Debug.Log("Down");
//					}
//				}
//				break;
//			}
//		}
//	}

}
