using UnityEngine;
using System.Collections;

public class Game_Management : MonoBehaviour {
	public GameObject grid;
	public GameObject tile;
	public GameObject[] ObjList;
	public int[,] Area;
	public int gridX, gridY;
	public int currentTileAmount = 0;

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
				// go.transform.parent = this.transform;
			}
		}
		//gameObject.transform.position = new Vector3(0f, 0f, 0f);
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

	void AddRandomTile(){
		if (currentTileAmount >= (gridX * gridY)) {
			//throw new UnityException("Game Over");
			currentTileAmount = 0;
			Object[] Tiles = GameObject.FindGameObjectsWithTag("Tile");
			foreach( Object Tile in Tiles){
				Destroy(Tile);
			}
		}
		int id = Random.Range (1, 4);
		int x = Random.Range (0, gridX);
		int y = Random.Range (0, gridY);
		bool exist = false; // boolean checker to see if there is already a tile
		while (!exist) {
			if(Area[x,y] == 0){ // if there is no tile 
				exist = true;
				Area[x,y] = id;
				Vector2 worldposition = GridToWorldPoint(x,y);
				GameObject obj;
				obj = Instantiate(tile, worldposition, transform.rotation) as GameObject;
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

	}

	void MoveToDown(){
		
	}

	void MoveToLeft(){
		for (int x = 1; x < gridX; x++) {
			for (int y = gridY - 1; y >= 0; y--){
				if (Area[x, y] == 0){
					continue;
				}

			}
		}
	}

	void MoveToRight(){
		
	}
}
