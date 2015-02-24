using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Game_Management : MonoBehaviour {
	public GameObject grid;
	public GameObject tile;
	public int[,] Area;
	public int gridX, gridY;
	public int currentTileAmount = 0;
	public int score;

	private RaycastHit2D realTarget;
	private Game_State gameState;
	private bool gameover;
	private static Vector3 horizontalRay = new Vector3(.51f, 0, 0);
	private static Vector3 verticalRay = new Vector3(0, .51f, 0);
	
	private enum Game_State {
		invalid_state = -1,
		Loaded,
		WaitForPlayer,
		Checking,
		GameOver
	}

	void Awake () {
		Area = new int[gridX, gridY];
		for (int x = 0; x < gridX; x++) {
			for (int y = 0; y < gridY; y++) {
				GameObject go = Instantiate(grid, new Vector3(x, y, 0), Quaternion.identity) as GameObject;
			}
		}
	}

	void Start () {
		gameState = Game_State.Loaded;
	}
		
	void Update () {
		if (gameState == Game_State.Loaded) {
			gameState = Game_State.WaitForPlayer;
			AddRandomTile();
			AddRandomTile();
		} else if (gameState == Game_State.WaitForPlayer){

		} else if (gameState == Game_State.Checking){
			if(!GameOverCheck()){
				AddRandomTile();
				Debug.Log("New Turn");
				resetTilesState();
				gameState = Game_State.WaitForPlayer;
			} else{
				gameState = Game_State.GameOver;
			}
		} else if (gameState == Game_State.GameOver){
			gameover = true;
			//throw new UnityException("GameOver");
		}

		if (currentTileAmount == gridX * gridY) {
			if(GameOverCheck()){
				gameover = true;
			}
		}
	}

	void OnGUI(){
		GUI.Label(new Rect(400, 20, 50, 20), "SCORE:");
		GUI.Label(new Rect(450, 20, 50, 20), score.ToString());
		if(gameover){
			GUI.Label(new Rect((Screen.width/2), (Screen.height/2), 200, 20), "Sorry You Lost");
			GameObject camera = GameObject.FindGameObjectWithTag("MainCamera");
			SwipeDetection swipe = camera.GetComponent<SwipeDetection>();
			swipe.enabled = false;
		}
	}

	private static Vector2 GridToWorldPoint(int x, int y){
		return new Vector2 (x, y);
	}

	private static Vector2 WorldToGridPoint(float x, float y){
		return new Vector2 (x, y);
	}

	private static int FloatToInt(float fnum){
		int inum = (int)fnum;
		return inum;
	}

	private GameObject GetObjectAtGridPosition(int x, int y) {
		// return the objects found at certain coordinates. At the moment only return object with tag "Tile"
		RaycastHit2D[] hits = Physics2D.RaycastAll(GridToWorldPoint (x, y), -Vector2.right, 0.05f);
		foreach (RaycastHit2D hit in hits) {
			if (hit.collider.tag == "Tile"){
				realTarget = hit;
			}
		}
		if (realTarget != null) {
			return realTarget.collider.gameObject;
		} else {
			throw new UnityException("Unable to find gameObject at" +x+","+y);
		}
	}

	void AddRandomTile(){
		if (currentTileAmount >= (gridX * gridY)) {
			gameover = true;
		}
		int x = Random.Range (0, gridX);
		int y = Random.Range (0, gridY);
		bool exist = false; // boolean checker to see if there is already a tile
		while (!exist) {
			if(Area[x,y] == 0){ // if there is no tile 
				Area[x,y] = 1;
				exist = true;
				Vector2 worldposition = GridToWorldPoint(x,y);
				Tile tileEx = tile.GetComponent<Tile>();
				if (Random.value >= 0.9f){
					tileEx.value = 4;
				} else {
					tileEx.value = 2;
				}
				GameObject obj = Instantiate(tile, worldposition, transform.rotation) as GameObject;
				currentTileAmount++; 
			}
		x++; // If there is a tile then move to next tile
			// Move on to the next avaliable tile
			if (x >= gridX){
				y++;
				x = 0;
			}
			if ( y >= gridY){
				y = 0;
			}
		}
	}

	bool MoveToUp(){
		bool done = false;
		bool moved = false;
		for (int x = 0; x < gridX; x++) { // from left to right
			for (int y = gridY - 1; y >= 0; y--){
				if (Area[x, y] == 0){
					Debug.Log("not found at "+x+","+y);
					continue;
				}
				bool stopped = false;
				bool tileExist = false;
				bool gridExist = false;
				
				GameObject thisTile = GetObjectAtGridPosition(x,y);
				GameObject thatTile = GetObjectAtGridPosition(x,y);
				while(!stopped){
					RaycastHit2D[] hits = Physics2D.RaycastAll(thisTile.transform.position + verticalRay, Vector2.up, 0.1f);
					foreach (RaycastHit2D hit in hits){
						if(hit.collider.tag == "Tile"){
							tileExist = true;
							thatTile = hit.collider.gameObject;
						} else if (hit.collider.tag == "Grid"){
							gridExist = true;
						}
					}
					
					if (tileExist && gridExist){
						print ("Up is Occupied");
						Tile currentTile = thisTile.GetComponent<Tile>();
						Tile nextTile = thatTile.GetComponent<Tile>();
						if (currentTile.value == nextTile.value && nextTile.hasMerged == false && currentTile.hasMerged == false){
							MergeTiles(thisTile, nextTile.gameObject);
							moved = true;
							tileExist = false;
							gridExist = false;
						} else {
							stopped = true;
						}
					} else if (!tileExist && gridExist){
						print ("nothing upwards");
						Vector3 targetPosDif = new Vector3(0, 1, 0);
						Area[FloatToInt(thisTile.transform.position.x), FloatToInt(thisTile.transform.position.y)] = 0;
						thisTile.transform.position += targetPosDif;
						Area[FloatToInt(thisTile.transform.position.x), FloatToInt(thisTile.transform.position.y)] = 1;
						gridExist = false;
						moved = true;
					} else if (!tileExist && !gridExist){
						print ("Arrived Position");
						stopped = true; 
					}
				}
			}
		}

		if (moved) {
			gameState = Game_State.Checking;
		}
		return done = true;
	}

	bool MoveToDown(){
		bool moved = false;
		bool done = false;
		for (int x = 0; x < gridX; x++) { // from down to up
			for (int y = 1; y < gridY; y++){
				if (Area[x, y] == 0){
					Debug.Log("not found at "+x+","+y);
					continue;
				}
				bool stopped = false;
				bool tileExist = false;
				bool gridExist = false;
				
				GameObject thisTile = GetObjectAtGridPosition(x,y);
				GameObject thatTile = GetObjectAtGridPosition(x,y);
				while(!stopped){
					RaycastHit2D[] hits = Physics2D.RaycastAll(thisTile.transform.position - verticalRay, -Vector2.up, 0.1f);
					foreach (RaycastHit2D hit in hits){
						if(hit.collider.tag == "Tile"){
							tileExist = true;
							thatTile = hit.collider.gameObject;
						} else if (hit.collider.tag == "Grid"){
							gridExist = true;
						}
					}
					
					if (tileExist && gridExist){
						print ("Down is Occupied");
						Tile currentTile = thisTile.GetComponent<Tile>();
						Tile nextTile = thatTile.GetComponent<Tile>();
						if (currentTile.value == nextTile.value && nextTile.hasMerged == false && currentTile.hasMerged == false){
							MergeTiles(thisTile, nextTile.gameObject);
							moved = true;
							tileExist = false;
							gridExist = false;
						} else {
							stopped = true;
						}
					} else if (!tileExist && gridExist){
						print ("nothing downwards");
						Vector3 targetPosDif = new Vector3(0, 1, 0);
						Area[FloatToInt(thisTile.transform.position.x), FloatToInt(thisTile.transform.position.y)] = 0;
						thisTile.transform.position -= targetPosDif;
						Area[FloatToInt(thisTile.transform.position.x), FloatToInt(thisTile.transform.position.y)] = 1;
						gridExist = false;
						moved = true;
					} else if (!tileExist && !gridExist){
						print ("Arrived Position");
						stopped = true; 
					}
				}
			}
		}
		if(moved){
			gameState = Game_State.Checking;
		}
		return done = true;
	}

	bool MoveToLeft(){
		bool done = false;
		bool moved = false;
		for (int x = 1; x < gridX; x++) { // from left to right
			for (int y = 0; y < gridY; y++){
				if (Area[x, y] == 0){
					Debug.Log("not found at "+x+","+y);
					continue;
				}
				bool stopped = false;
				bool tileExist = false;
				bool gridExist = false;

				GameObject thisTile = GetObjectAtGridPosition(x,y);
				GameObject thatTile = GetObjectAtGridPosition(x,y);
				while(!stopped){
					RaycastHit2D[] hits = Physics2D.RaycastAll(thisTile.transform.position - horizontalRay, -Vector2.right, 0.1f);
					foreach (RaycastHit2D hit in hits){
						if(hit.collider.tag == "Tile"){
							tileExist = true;
							thatTile = hit.collider.gameObject;
						} else if (hit.collider.tag == "Grid"){
							gridExist = true;
						}
					}

					if (tileExist && gridExist){
						print ("Left is Occupied");
						Tile currentTile = thisTile.GetComponent<Tile>();
						Tile nextTile = thatTile.GetComponent<Tile>();
						if (currentTile.value == nextTile.value && nextTile.hasMerged == false && currentTile.hasMerged == false){
							MergeTiles(thisTile, nextTile.gameObject);
							moved = true;
							tileExist = false;
							gridExist = false;
						} else {
							stopped = true;
						}
					} else if (!tileExist && gridExist){
						print ("nothing at left");
						Vector3 targetPosDif = new Vector3(1, 0, 0);
						Area[FloatToInt(thisTile.transform.position.x), FloatToInt(thisTile.transform.position.y)] = 0;
						thisTile.transform.position -= targetPosDif;
						Area[FloatToInt(thisTile.transform.position.x), FloatToInt(thisTile.transform.position.y)] = 1;
						gridExist = false;
						moved = true;
					} else if (!tileExist && !gridExist){
						print ("Arrived Position");
						stopped = true; 
					}
				}
			}
		}
		if(moved){
			gameState = Game_State.Checking;
		}
		return done = true;
	}

	bool MoveToRight(){
		bool done = false;
		bool moved = false;
		for (int x = gridX - 1; x >= 0; x--) { // from right to left
			for (int y = 0; y < gridY; y++){
				if (Area[x, y] == 0){
					Debug.Log("not found at "+x+","+y);
					continue;
				}
				bool stopped = false;
				bool tileExist = false;
				bool gridExist = false;
				
				GameObject thisTile = GetObjectAtGridPosition(x,y);
				GameObject thatTile = GetObjectAtGridPosition(x,y);
				while(!stopped){
					RaycastHit2D[] hits = Physics2D.RaycastAll(thisTile.transform.position + horizontalRay, Vector2.right, 0.1f);
					foreach (RaycastHit2D hit in hits){
						if(hit.collider.tag == "Tile"){
							tileExist = true;
							thatTile = hit.collider.gameObject;
						} else if (hit.collider.tag == "Grid"){
							gridExist = true;
						}
					}
					
					if (tileExist && gridExist){
						print ("Right is Occupied");
						Tile currentTile = thisTile.GetComponent<Tile>();
						Tile nextTile = thatTile.GetComponent<Tile>();
						if (currentTile.value == nextTile.value && nextTile.hasMerged == false && currentTile.hasMerged == false){
							MergeTiles(thisTile, nextTile.gameObject);
							moved = true;
							tileExist = false;
							gridExist = false;
						} else {
							stopped = true;
						}
					} else if (!tileExist && gridExist){
						print ("nothing at right");
						Vector3 targetPosDif = new Vector3(1, 0, 0);
						Area[FloatToInt(thisTile.transform.position.x), FloatToInt(thisTile.transform.position.y)] = 0;
						thisTile.transform.position += targetPosDif;
						Area[FloatToInt(thisTile.transform.position.x), FloatToInt(thisTile.transform.position.y)] = 1;
						gridExist = false;
						moved = true;
					} else if (!tileExist && !gridExist){
						print ("Arrived Position");
						stopped = true; 
					}
				}
			}
		}
		if(moved){
			gameState = Game_State.Checking;
		}
		return done = true;
	}

	void MergeTiles(GameObject thisTile, GameObject thatTile){
		Area[FloatToInt(thisTile.transform.position.x), FloatToInt(thisTile.transform.position.y)] = 0; // de-register original tile
		Destroy (thisTile); // destroy original tile 
		Tile nextTile = thatTile.GetComponent<Tile> (); // this is the tile on the way
		nextTile.value = nextTile.value * 2;
		score += nextTile.value;
		nextTile.hasMerged = true;
		nextTile.playAnim = true;
		currentTileAmount -= 1;
	}

	void resetTilesState(){
		Object[] objs = GameObject.FindGameObjectsWithTag("Tile");
		foreach (GameObject obj in objs) {
			Tile eachtile = obj.GetComponent<Tile>();
			eachtile.hasMerged = false;
		}
	}

	public void resetGame(){
		Application.LoadLevel(0);
	}

	bool GameOverCheck(){
		if (currentTileAmount < gridX * gridY) {
			return false;
		}

		for (int x = 0; x < gridX -1; x++) {
			for (int y = 0; y < gridY -1; y++){
				Tile currentTile = GetObjectAtGridPosition(x,y).GetComponent<Tile>();
				Tile rightTile = GetObjectAtGridPosition(x+1,y).GetComponent<Tile>();
				Tile upTile = GetObjectAtGridPosition(x,y+1).GetComponent<Tile>();

				if(x != gridX - 1 && currentTile.value == rightTile.value){ // if now isn't on the last x row
					return false;
				} else if (y != gridY - 1 && currentTile.value == upTile.value){ // if now isn't on the last y column
					return false;
				}
			}
		}
		return true;
	}
}
