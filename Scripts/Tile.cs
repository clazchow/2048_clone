using UnityEngine;
using System.Collections;

public class Tile : MonoBehaviour {
	public int value;
	public bool hasMerged = false;
	public bool playAnim;
	public bool win;
	private GUIStyle style = new GUIStyle();
	private Vector3 thisPosition = new Vector3(0,0,0);
	private SpriteRenderer spriteRen;
	private Rect rect = new Rect(0, 0, 30, 30);
	private Animator animator;
	// Use this for initialization
	void Start () {
		spriteRen = GetComponent<SpriteRenderer> ();
		animator = GetComponent<Animator> ();
		style.fontSize = 32;
		style.normal.textColor = Color.white;
	}
	
	// Update is called once per frame
	void Update () {
		if (value == 4) {
			spriteRen.color = Color.red;
		} else if (value == 8 || value == 256){
			spriteRen.color = Color.green;
		} else if (value == 16 || value == 512){
			spriteRen.color = Color.grey;
		} else if (value == 32 || value == 1024){
			spriteRen.color = Color.black;
		} else if (value == 64){
			spriteRen.color = Color.cyan;
		} else if (value == 128){
			spriteRen.color = Color.blue;
		}

		if (value == 2048) {
			win = true;
		} else {
			win = false;
		}

		if (playAnim) {
			animator.SetTrigger("merge");
			playAnim = false;
		}
	}

	Vector3 WorldSpaceToScreenSpace(Vector3 worldcoordiante){
		Vector3 screencoordiante = Camera.main.WorldToScreenPoint (worldcoordiante);
		screencoordiante.x = screencoordiante.x - 25;
		screencoordiante.y = Screen.height - screencoordiante.y - 15;
		return screencoordiante; 
	}

	void OnGUI(){
		rect.position = WorldSpaceToScreenSpace (transform.position);
		GUI.Label(rect, value.ToString(), style);
		if (win) {
			GUI.Label(new Rect((Screen.width/2 - 100), (Screen.height/7*6), 200, 20), "Congrats YOU WON!!", style);
		}
		//rect.center = new Vector3 (transform.position.x, transform.position.y, transform.position.z);
	}
}
