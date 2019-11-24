using UnityEngine;
using System;
using System.Collections.Generic; 		//Allows us to use Lists.
using Random = UnityEngine.Random; 		//Tells Random to use the Unity Engine random number generator.

namespace Completed

{

	public class BoardManager : MonoBehaviour
	{
		// Using Serializable allows us to embed a class with sub properties in the inspector.
		[Serializable]
		public class Count
		{
			public int minimum; 			//Minimum value for our Count class.
			public int maximum; 			//Maximum value for our Count class.


			//Assignment constructor.
			public Count (int min, int max)
			{
				minimum = min;
				maximum = max;
			}
		}


		public int columns = 8; 										//Number of columns in our game board.
		public int rows = 8;											//Number of rows in our game board.
		public Count wallCount = new Count (5, 9);						//Lower and upper limit for our random number of walls per level.
		public Count foodCount = new Count (1, 5);						//Lower and upper limit for our random number of food items per level.
		public GameObject exit;											//Prefab to spawn for exit.
        public GameObject[] exitDecorations;
		public GameObject[] floorTiles;									//Array of floor prefabs.
		public GameObject[] wallTiles;									//Array of wall prefabs.
		public GameObject[] foodTiles;									//Array of food prefabs.
		public GameObject[] enemyTiles;									//Array of enemy prefabs.
		public GameObject[] outerWallTiles;								//Array of outer tile prefabs.

		public GameObject[] boxTiles;
		public GameObject unmovableBox;
		public GameObject ice;
		public GameObject water;

		public GameObject shepherd;
		private Transform boardHolder;									//A variable to store a reference to the transform of our Board object.
		private List <Vector3> gridPositions = new List <Vector3> ();	//A list of possible locations to place tiles.

		private int[,] boardArray;


		//Clears our list gridPositions and prepares it to generate a new board.
		void InitialiseList ()
		{
			//Clear our list gridPositions.
			gridPositions.Clear ();

			//Loop through x axis (columns).
			for(int x = 1; x < columns-1; x++)
			{
				//Within each column, loop through y axis (rows).
				for(int y = 1; y < rows-1; y++)
				{
					//At each index add a new Vector3 to our list with the x and y coordinates of that position.
					gridPositions.Add (new Vector3(x, y, 0f));
				}
			}
		}


		//Sets up the outer walls and floor (background) of the game board.

		int[,] flipArray(int[,] inputarr) {
			int[,] flipped = (int[,])inputarr.Clone();

			for (int i = 0; i < inputarr.GetLength(0); i++) {
				for (int j = 0; j < inputarr.GetLength(1); j++) {
					flipped[i,j] = inputarr[inputarr.GetLength(0)-1-i, j];
				}
			}
			return flipped;
		}

		void BoardSetup (int level)
		{
			//Instantiate Board and set boardHolder to its transform.
			boardHolder = new GameObject ("Board").transform;

			/**
			-2 = start
			-1 = exit
			0 = ground
			1 = wall
			2 = grass
			3 = box
			4 = enemy
			5 = notMovableBox
			6 = ice
			7 = water
			42 = shepherd
			**/
			switch ( level)
			{
				case 1: boardArray = new int[,]{{1,1,1,1,1,1,1,1,1,1},
								{1,-2,5,0,5,0,5,0,0,1},
								{1,0,0,5,0,5,0,5,0,1},
								{1,0,2,2,2,2,2,2,-1,1},
								{1,1,1,1,1,1,1,1,1,1}};
							break;


				case 2: boardArray = new int[,]{{1,1,1,1,1,1,1,1,1,1},
												{1,0,0,0,1,0,0,0,-1,1},
												{1,0,0,0,3,0,0,0,0,1},
												{1,0,0,0,7,0,0,0,0,1},
												{1,0,0,0,3,0,0,0,0,1},
												{1,0,0,0,7,0,0,0,0,1},
												{1,0,0,0,3,0,0,0,0,1},
												{1,0,0,0,7,0,0,0,0,1},
												{1,-2,0,0,3,0,0,0,0,1},
												{1,1,1,1,1,1,1,1,1,1}};
							break;

				case 3: boardArray = new int[,]{{1,1,1,1,1,1,1,1,1,1},
												{1,0,1,0,0,1,0,0,-1,1},
												{1,0,3,0,0,1,0,1,0,1},
												{1,3,1,1,0,1,0,0,1,1},
												{1,0,0,1,0,0,3,3,0,1},
												{1,0,0,1,0,1,1,0,0,1},
												{1,1,3,1,0,0,0,0,0,1},
												{1,0,0,1,0,1,0,0,0,1},
												{1,-2,0,1,0,1,0,0,0,1},
												{1,1,1,1,1,1,1,1,1,1}};
							break;
				case 4: boardArray = new int[,]{{1,1,1,1,1,1,1,1,1,1},
												{1,0,0,0,3,3,0,0,-1,1},
												{1,0,3,0,3,0,0,0,0,1},
												{1,3,0,3,3,3,0,0,0,1},
												{1,0,3,3,0,3,3,0,0,1},
												{1,3,3,0,3,0,0,0,0,1},
												{1,0,3,3,0,0,0,0,0,1},
												{1,0,0,3,3,0,0,7,0,1},
												{1,-2,0,3,0,3,0,0,0,1},
												{1,1,1,1,1,1,1,1,1,1}};
							break;

				case 5: boardArray = new int[,]{{1,1,1,1,1,1,1,1,1,1},
												{1,0,0,0,0,0,0,0,-1,1},
												{1,0,0,1,0,0,0,0,0,1},
												{1,0,4,1,0,1,1,1,1,1},
												{1,1,0,0,0,0,0,0,0,1},
												{1,1,0,0,0,0,4,0,0,1},
												{1,0,0,1,0,0,0,0,1,1},
												{1,0,0,0,1,1,1,0,0,1},
												{1,-2,0,0,0,0,0,0,0,1},
												{1,1,1,1,1,1,1,1,1,1}};
							break;

				case 7: boardArray = new int[,]{{1,1,1,1,1,1,1,1,1,1},
												{1,0,0,7,7,0,0,0,-1,1},
												{1,1,1,1,7,7,0,0,0,1},
												{1,7,7,0,0,7,7,7,7,1},
												{1,0,7,7,0,0,1,1,1,1},
												{1,0,0,7,7,7,7,7,0,1},
												{1,0,0,0,3,1,0,7,7,1},
												{1,0,0,0,0,1,0,0,0,1},
												{1,-2,0,0,0,1,0,0,0,1},
												{1,1,1,1,1,1,1,1,1,1}};
							break;

				case 8: boardArray = new int[,]{{1,1,1,1,1,1,1,1,1,1},
												{1,0,0,0,0,7,0,0,-1,1},
												{1,7,0,4,0,7,0,0,0,1},
												{1,7,0,0,0,7,7,0,0,1},
												{1,7,7,0,0,7,7,7,7,1},
												{1,0,7,0,7,7,0,0,7,1},
												{1,0,7,7,7,0,0,0,0,1},
												{1,0,0,7,0,0,0,4,0,1},
												{1,-2,0,7,0,0,0,0,0,1},
												{1,1,1,1,1,1,1,1,1,1}};
							break;

				case 9: boardArray = new int[,]{{1,1,1,1,1,1,1,1,1,1},
												{1,-2,0,0,0,0,0,0,0,1},
												{1,0,0,0,0,0,0,0,0,1},
												{1,0,0,0,0,0,0,0,0,1},
												{1,0,0,0,0,0,0,0,0,1},
												{1,0,0,0,0,0,0,0,0,1},
												{1,0,0,0,0,0,0,0,0,1},
												{1,0,0,0,0,0,0,0,0,1},
												{1,0,0,0,0,0,0,0,42,1},
												{1,1,1,1,1,1,1,1,1,1}};
							break;
				default: boardArray = new int[,]{{1,1,1,1,1,1,1,1,1,1},
								{1,2,0,0,1,0,0,0,0,1},
								{1,0,1,0,1,0,1,0,0,1},
								{1,0,1,0,1,0,1,1,0,1},
								{1,0,1,0,1,0,1,0,0,1},
								{1,0,1,0,1,0,1,0,0,1},
								{1,0,1,0,1,0,1,0,0,1},
								{1,0,1,0,1,0,1,0,0,1},
								{1,0,1,0,0,0,1,0,-1,1},
								{1,1,1,1,1,1,1,1,1,1}};
							break;
			}

			boardArray = flipArray(boardArray);

			for(int i = -1; i < boardArray.GetLength(0)-1; i++) {
				for (int j = -1; j < boardArray.GetLength(1)-1; j++) {
					if (boardArray[i+1,j+1] == 0) {
						GameObject toInstantiate = floorTiles[Random.Range (0,floorTiles.Length)];
						GameObject instance = Instantiate (toInstantiate, new Vector3 (j, i, 0f), Quaternion.identity) as GameObject;
						instance.transform.SetParent (boardHolder);
					} else if(boardArray[i+1,j+1] == 1) {
						var wallAdjacency = getWallAdjacency(new BoardArray(boardArray), i + 1, j + 1);
						GameObject toInstantiate = outerWallTiles[wallAdjacency.toIndex()];
						GameObject instance = Instantiate (toInstantiate, new Vector3 (j, i, 0f), Quaternion.identity) as GameObject;
						instance.transform.SetParent (boardHolder);
					} else if(boardArray[i+1,j+1] == -1) {
                        GameObject exitDecoration = null;
                        switch (level)
                        {
                            case 1:
                                exitDecoration = exitDecorations[0];
                                break;
                            case 4:
                                exitDecoration = exitDecorations[1];
                                break;
                        }

						GameObject toInstantiate = floorTiles[Random.Range (0,floorTiles.Length)];
						GameObject instance = Instantiate (toInstantiate, new Vector3 (j, i, 0f), Quaternion.identity) as GameObject;
						instance.transform.SetParent (boardHolder);
						instance = Instantiate (exit, new Vector3 (j, i, 0f), Quaternion.identity);
						instance.transform.SetParent (boardHolder);
                        if(exitDecoration != null) {
                            instance = Instantiate(exitDecoration, new Vector3(j, i, 0f), Quaternion.identity);
                            instance.transform.SetParent(boardHolder);
                        }
					} else if(boardArray[i+1,j+1] == -2) {
						GameObject toInstantiate = floorTiles[Random.Range (0,floorTiles.Length)];
						GameObject instance = Instantiate (toInstantiate, new Vector3 (j, i, 0f), Quaternion.identity) as GameObject;
						instance.transform.SetParent (boardHolder);
						GameManager.instance.player.gameObject.transform.position = new Vector3(j, i, 0);
					} else {
						GameObject toInstantiate = floorTiles[Random.Range (0,floorTiles.Length)];
						GameObject instance = Instantiate (toInstantiate, new Vector3 (j, i, 0f), Quaternion.identity) as GameObject;
						instance.transform.SetParent (boardHolder);
						switch(boardArray[i+1,j+1]) {
						case 2: toInstantiate = foodTiles[0]; break;
						case 3: toInstantiate = boxTiles[0]; break;
						case 4: toInstantiate = enemyTiles[0]; break;
						case 5: toInstantiate = unmovableBox; break;
						case 6: toInstantiate = ice; break;
						case 7: toInstantiate = water; break;
						case 42: toInstantiate = shepherd; break;
						default: toInstantiate = floorTiles[Random.Range (0,floorTiles.Length)]; break;
						}
						instance = Instantiate (toInstantiate, new Vector3 (j, i, 0f), Quaternion.identity) as GameObject;
						instance.transform.SetParent (boardHolder);
					}
				}
			}
			//Loop along x axis, starting from -1 (to fill corner) with floor or outerwall edge tiles.
			/*for(int x = -1; x < columns + 1; x++)
			{
				//Loop along y axis, starting from -1 to place floor or outerwall tiles.
				for(int y = -1; y < rows + 1; y++)
				{
					//Choose a random tile from our array of floor tile prefabs and prepare to instantiate it.
					GameObject toInstantiate = floorTiles[Random.Range (0,floorTiles.Length)];

					//Check if we current position is at board edge, if so choose a random outer wall prefab from our array of outer wall tiles.
					if(x == -1 || x == columns || y == -1 || y == rows)
						toInstantiate = outerWallTiles [Random.Range (0, outerWallTiles.Length)];

					//Instantiate the GameObject instance using the prefab chosen for toInstantiate at the Vector3 corresponding to current grid position in loop, cast it to GameObject.
					GameObject instance =
						Instantiate (toInstantiate, new Vector3 (x, y, 0f), Quaternion.identity) as GameObject;

					//Set the parent of our newly instantiated object instance to boardHolder, this is just organizational to avoid cluttering hierarchy.
					instance.transform.SetParent (boardHolder);
				}
			}*/
		}

		WallAdjacency getWallAdjacency(BoardArray boardArray, int x, int y) {
			var tile = boardArray.get(x, y);
			if(tile == null) {
				return null;
			}
			var left = boardArray.get(x - 1, y) == 1;
			var right = boardArray.get(x + 1, y) == 1;
			var bottom = boardArray.get(x, y - 1) == 1;
			var top = boardArray.get(x, y + 1) == 1;
			return new WallAdjacency { left = left, right = right, bottom = bottom, top = top };
		}



		//RandomPosition returns a random position from our list gridPositions.
		Vector3 RandomPosition ()
		{
			//Declare an integer randomIndex, set it's value to a random number between 0 and the count of items in our List gridPositions.
			int randomIndex = Random.Range (0, gridPositions.Count);

			//Declare a variable of type Vector3 called randomPosition, set it's value to the entry at randomIndex from our List gridPositions.
			Vector3 randomPosition = gridPositions[randomIndex];

			//Remove the entry at randomIndex from the list so that it can't be re-used.
			gridPositions.RemoveAt (randomIndex);

			//Return the randomly selected Vector3 position.
			return randomPosition;
		}


		//LayoutObjectAtRandom accepts an array of game objects to choose from along with a minimum and maximum range for the number of objects to create.
		void LayoutObjectAtRandom (GameObject[] tileArray, int minimum, int maximum)
		{
			//Choose a random number of objects to instantiate within the minimum and maximum limits
			int objectCount = 3;//Random.Range (minimum, maximum+1);

			//Instantiate objects until the randomly chosen limit objectCount is reached
			for(int i = 0; i < objectCount; i++)
			{
				//Choose a position for randomPosition by getting a random position from our list of available Vector3s stored in gridPosition
				Vector3 randomPosition = RandomPosition();
				int rand_x = 0;
				int rand_y = 0;
				while (boardArray[rand_y,rand_x] == 1) {
					rand_x = Random.Range (0, boardArray.GetLength(0));
					rand_y = Random.Range (0, boardArray.GetLength(1));
				}


				//Choose a random tile from tileArray and assign it to tileChoice
				GameObject tileChoice = tileArray[Random.Range (0, tileArray.Length)];

				//Instantiate tileChoice at the position returned by RandomPosition with no change in rotation
				Instantiate(tileChoice, new Vector3(rand_x, rand_y, 0), Quaternion.identity);
			}
		}


		//SetupScene initializes our level and calls the previous functions to lay out the game board
		public void SetupScene (int level)
		{
			//Creates the outer walls and floor.
			BoardSetup (level);

			//Reset our list of gridpositions.
			InitialiseList ();

			//Instantiate a random number of wall tiles based on minimum and maximum, at randomized positions.
			//LayoutObjectAtRandom (wallTiles, wallCount.minimum, wallCount.maximum);

			//Instantiate a random number of food tiles based on minimum and maximum, at randomized positions.
			//LayoutObjectAtRandom (foodTiles, foodCount.minimum, foodCount.maximum);

			//Determine number of enemies based on current level number, based on a logarithmic progression
			int enemyCount = (int)Mathf.Log(level, 2f);

			//Instantiate a random number of enemies based on minimum and maximum, at randomized positions.
			//LayoutObjectAtRandom (enemyTiles, enemyCount, enemyCount);

			//Instantiate the exit tile in the upper right hand corner of our game board
			//Instantiate (exit, new Vector3 (columns - 1, rows - 1, 0f), Quaternion.identity);
		}
	}

	class BoardArray {
		public BoardArray(int[,] array2d) {
			this.array2d = array2d;
		}

		public int? get(int x, int y) {
			if(!(0 <= x && x < this.width && 0 <= y && y < this.height)) {
				return null;
			} else {
				return this.array2d[x,y];
			}
		}

		public int width {
			get {
				return this.array2d.GetLength(0);
			}
		}

		public int height {
			get {
				return this.array2d.GetLength(1);
			}
		}

		private int[,] array2d;
	}

	class WallAdjacency {
		public bool left;
		public bool right;
		public bool top;
		public bool bottom;

		public int toIndex() {
			return (this.bottom ? 8 : 0)
				+ (this.top ? 4 : 0)
				+ (this.right ? 2 : 0)
				+ (this.left ? 1 : 0);
		}
	}
}
