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
				SendMessage("MoveToUp");
			} else if (posDiff.y < -225){
				print ("DOWN");
				SendMessage("MoveToDown");
			} else if (posDiff.x > 225){
				print("RIGHT");
				SendMessage("MoveToRight");
			} else if (posDiff.x < -225){
				print("LEFT");
				SendMessage("MoveToLeft");
			}
		}
//		if(Input.GetKeyDown("r")){
//			SendMessage("AddRandomTile");
//		}
	}
}
