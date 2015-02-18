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
	private static Vector3 horizontalRay = new Vector3(0.51f, 0, 0);
	private static Vector3 verticalRay = new Vector3(0, .51f, 0);
	
	private enum game_State {
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

	}
		
	void Update () {

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
//		RaycastHit2D hit = Physics2D.Raycast(GridToWorldPoint (x, y), -Vector2.right, 0.05f);
//		if (hit.collider.tag == "Tile") {
//			return hit.collider.gameObject;
//		} else {
//			throw new UnityException("Unable to find gameObject at" +x+","+y);
//		}
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
			throw new UnityException("Game Over");
			currentTileAmount = 0;
			Object[] Tiles = GameObject.FindGameObjectsWithTag("Tile");
			foreach( Object Tile in Tiles){
				Destroy(Tile);
			}
		}
		int x = Random.Range (0, gridX);
		int y = Random.Range (0, gridY);
		bool exist = false; // boolean checker to see if there is already a tile
		while (!exist) {
			if(Area[x,y] == 0){ // if there is no tile 
				Area[x,y] = 1;
				exist = true;
				Vector2 worldposition = GridToWorldPoint(x,y);
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

	void MoveToUp(){
		for (int x = 0; x < gridX; x++) { // from up to down
			for (int y = gridY - 1; y >= 0; y--){
				if (Area[x, y] == 0){
					Debug.Log("not found at "+x+","+y);
					continue;
				}
				bool stopped = false;
				GameObject thisTile = GetObjectAtGridPosition(x,y);
				while(!stopped){
					RaycastHit2D hit = Physics2D.Raycast (thisTile.transform.position + verticalRay, Vector2.up, 0.1f);
					if (hit) {
						if(hit.collider.tag == "Tile"){
							print ("Up is Occupied");
							Tile nextTile = hit.collider.gameObject.GetComponent<Tile>();
							Tile currentTile = thisTile.GetComponent<Tile>();
							if (currentTile.value == nextTile.value && nextTile.hasMerged == false && currentTile.hasMerged == false){
								MergeTiles(thisTile, hit.collider.gameObject);
							} else {
								stopped = true;
							}
						} else if (hit.collider.tag == "Grid"){
							print ("nothing upwards");
							Vector3 targetPosDif = new Vector3(0, 1, 0);
							Area[FloatToInt(thisTile.transform.position.x), FloatToInt(thisTile.transform.position.y)] = 0;
							thisTile.transform.position += targetPosDif;
							Area[FloatToInt(thisTile.transform.position.x), FloatToInt(thisTile.transform.position.y)] = 1;
						}
					} else {
						print ("Arrived Position");
						stopped = true;
					}
				}
			}
		}
	}

	void MoveToDown(){
		for (int x = 0; x < gridX; x++) { // from down to up
			for (int y = 1; y < gridY; y++){
				if (Area[x, y] == 0){
					Debug.Log("not found at "+x+","+y);
					continue;
				}
				bool stopped = false;
				GameObject thisTile = GetObjectAtGridPosition(x,y);
				while(!stopped){
					RaycastHit2D hit = Physics2D.Raycast (thisTile.transform.position - verticalRay, -Vector2.up, 0.1f);
					if (hit) {
						if(hit.collider.tag == "Tile"){
							print ("Down is Occupied");
							Tile nextTile = hit.collider.gameObject.GetComponent<Tile>();
							Tile currentTile = thisTile.GetComponent<Tile>();
							if (currentTile.value == nextTile.value && nextTile.hasMerged == false && currentTile.hasMerged == false){
								MergeTiles(thisTile, hit.collider.gameObject);
							} else {
								stopped = true;
							}
						} else if (hit.collider.tag == "Grid"){
							print ("nothing downwards");
							Vector3 targetPosDif = new Vector3(0, 1, 0);
							Area[FloatToInt(thisTile.transform.position.x), FloatToInt(thisTile.transform.position.y)] = 0;
							thisTile.transform.position -= targetPosDif;
							Area[FloatToInt(thisTile.transform.position.x), FloatToInt(thisTile.transform.position.y)] = 1;
						}
					} else {
						print ("Arrived Position");
						stopped = true;
					}
				}
			}
		}
	}

	void MoveToLeft(){
		for (int x = 1; x < gridX; x++) { // from left to right
			for (int y = 0; y < gridY; y++){
				if (Area[x, y] == 0){
					Debug.Log("not found at "+x+","+y);
					continue;
				}
				bool stopped = false;
				GameObject thisTile = GetObjectAtGridPosition(x,y);
				while(!stopped){
					RaycastHit2D hit = Physics2D.Raycast (thisTile.transform.position - horizontalRay, -Vector2.right, 0.1f);
					if (hit) {
						if(hit.collider.tag == "Tile"){
							print ("Left is Occupied");
							Tile nextTile = hit.collider.gameObject.GetComponent<Tile>();
							Tile currentTile = thisTile.GetComponent<Tile>();
							if (currentTile.value == nextTile.value && nextTile.hasMerged == false && currentTile.hasMerged == false){
								MergeTiles(thisTile, hit.collider.gameObject);
							} else {
								stopped = true;
							}
						} else if (hit.collider.tag == "Grid"){
							print ("nothing at left");
							Vector3 targetPosDif = new Vector3(1, 0, 0);
							Area[FloatToInt(thisTile.transform.position.x), FloatToInt(thisTile.transform.position.y)] = 0;
							thisTile.transform.position -= targetPosDif;
							Area[FloatToInt(thisTile.transform.position.x), FloatToInt(thisTile.transform.position.y)] = 1;
						}
					} else {
						print ("Arrived Position");
						stopped = true;
					}
				}
			}
		}
	}

	void MoveToRight(){
		for (int x = gridX - 1; x >= 0; x--) { // from right to left
			for (int y = 0; y < gridY; y++){
				if (Area[x, y] == 0){
					Debug.Log("not found at "+x+","+y);
					continue;
				}
				bool stopped = false;
				GameObject thisTile = GetObjectAtGridPosition(x,y);
				while(!stopped){
					RaycastHit2D hit = Physics2D.Raycast (thisTile.transform.position + horizontalRay, Vector2.right, 0.1f);
					if (hit) {
						if(hit.collider.tag == "Tile"){
							print ("Right is Occupied");
							Tile nextTile = hit.collider.gameObject.GetComponent<Tile>();
							Tile currentTile = thisTile.GetComponent<Tile>();
							if (currentTile.value == nextTile.value && nextTile.hasMerged == false && currentTile.hasMerged == false){
								MergeTiles(thisTile, hit.collider.gameObject);
							} else {
								stopped = true;
							}
						} else if (hit.collider.tag == "Grid"){
							print ("nothing at right");
							Vector3 targetPosDif = new Vector3(1, 0, 0);
							Area[FloatToInt(thisTile.transform.position.x), FloatToInt(thisTile.transform.position.y)] = 0;
							thisTile.transform.position += targetPosDif;
							Area[FloatToInt(thisTile.transform.position.x), FloatToInt(thisTile.transform.position.y)] = 1;
						}
					} else {
						print ("Arrived Position");
						stopped = true;
					}
				}
			}
		}
	}

	void MergeTiles(GameObject thisTile, GameObject thatTile){
		Area[FloatToInt(thisTile.transform.position.x), FloatToInt(thisTile.transform.position.y)] = 0;
		Destroy (thisTile);
		Tile nextTile = thatTile.GetComponent<Tile> ();
		nextTile.value = nextTile.value * 2;
		score += nextTile.value;
		nextTile.hasMerged = true;
		currentTileAmount -= 1;
	}

	bool GameOverCheck(){
		return false;
	}
}
