﻿using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

namespace Completed
{
	using System.Collections.Generic;		//Allows us to use Lists.
	using UnityEngine.UI;					//Allows us to use UI.

	public class GameManager : MonoBehaviour
	{
		public float levelStartDelay = 5f;						//Time to wait before starting level, in seconds.
		public float turnDelay = 0.1f;							//Delay between each Player turn.
		//public int playerFoodPoints = 100;						//Starting value for Player food points.
		public static GameManager instance = null;				//Static instance of GameManager which allows it to be accessed by any other script.
		[HideInInspector] public bool playersTurn = true;		//Boolean to check if it's players turn, hidden in inspector but public.


		private Text levelText;									//Text to display current level number.
		private GameObject levelImage;							//Image to block out level as levels are being set up, background for levelText.
		private BoardManager boardScript;						//Store a reference to our BoardManager which will set up the level.
		private int level = 1;									//Current level number, expressed in game as "Day 1".
		private List<Enemy> enemies;							//List of all Enemy units, used to issue them move commands.
		private bool enemiesMoving;								//Boolean to check if enemies are moving.
		private bool doingSetup = true;							//Boolean to check if we're setting up board, prevent Player from moving during setup.

		public Player player;

		//Awake is always called before any Start functions
		void Awake()
		{
            //Check if instance already exists
            if (instance == null) {
				//if not, set instance to this
                instance = this;
			}

                

            //If instance already exists and it's not this:
            else if (instance != this)

                //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
                Destroy(gameObject);

			//Sets this to not be destroyed when reloading scene
			DontDestroyOnLoad(gameObject);

			//Assign enemies to a new List of Enemy objects.
			enemies = new List<Enemy>();

			//Get a component reference to the attached BoardManager script
			boardScript = GetComponent<BoardManager>();

			//Call the InitGame function to initialize the first level
			InitGame();
		}

        //this is called only once, and the paramter tell it to be called only after the scene was loaded
        //(otherwise, our Scene Load callback would be called the very first load, and we don't want that)
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static public void CallbackInitialization()
        {
            //register the callback to be called everytime the scene is loaded
			
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        //This is called each time a scene is loaded.
        static private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
			print("reload");
            instance.level++;
            instance.InitGame();
        }


		//Initializes the game for each level.
		void InitGame()
		{
			GameManager.instance.player = GameObject.Find("Player").GetComponent<Player>();
			//While doingSetup is true the player can't move, prevent player from moving while title card is up.
			doingSetup = true;

			//Get a reference to our image LevelImage by finding it by name.
			levelImage = GameObject.Find("LevelImage");

			//Get a reference to our text LevelText's text component by finding it by name and calling GetComponent.
			levelText = GameObject.Find("LevelText").GetComponent<Text>();

			//Set the text of levelText to the string "Day" and append the current level number.
			switch (level)
			{
				case 1: levelText.text = "You used to be a famous superhero \n with lasers and stuff.\n  Your archenemy 'The Shepherd' started \n transforming all the people into sheep,\n so you investigated.\n Unfortuately you now became a sheep too ...\n ...and lost all your superpowers. "; break;
				//level with eating grass, unpushable block?
				case 2: levelText.text = "Great, you ate a lot. You feel stronger now."; break;
				//level with pushing stuff (easy pushing level) //one enemy to avoid
				case 3: levelText.text = "Nice! Want a challenge now?"; break;
				// second push level
				case 4: levelText.text = "One more to go."; break;
				//level with pushing stuff (complex pushing level) //some enemies to avoid

				case 5: levelText.text = "You proved to be smart.\n Your eyes start to burn again."; 
						instance.player.hasLaser = true; break; 
						//melt ice-block
				case 6: levelText.text = "Do you think you can shoot on other things too?";
						instance.player.hasLaser = true; break;
				//hard enemy-fighting-level
				case 7:
					levelText.text = "Do you dare to take on more enemies?";
					instance.player.hasLaser = true;
						instance.player.walkOverWater = true; break;
				case 8: levelText.text = "Wow, thats a bunch of dead enemies!\n You feel much safer now, and somehow lightweighted..."; 
						instance.player.hasLaser = true;
						instance.player.walkOverWater = true; break;
						//introduction in walking over water
				case 9: levelText.text = "Wow! Can you really walk over water now?\n Try it again!"; 
						instance.player.hasLaser = true;
						instance.player.walkOverWater = true; break;
						//another walking over water
				case 10: levelText.text = "Did you think you were save? \n You must have been wrong there. \n You see 'The Shepherd' now.\n Be up for a challenge. "; 
						instance.player.hasLaser = true;
						instance.player.walkOverWater = true; 
						SoundManager.instance.PlayMusic();
						break;
				//Bossfight. Shoot at Shephard with your eyes
				case 11: levelText.text = "Congratulations. \n You got all your superpowers back \n and got revenge on 'The Shepherd'!\n But you are still a sheep...\nThe strange thing is: You dont care!\n Eating grass is great!";
						Win();                                       
						return;
				default: levelText.text = "Level " + level; break;
			}

			//Set levelImage to active blocking player's view of the game board during setup.
			levelImage.SetActive(true);

			//Call the HideLevelImage function with a delay in seconds of levelStartDelay.
			Invoke("HideLevelImage", levelStartDelay);

			//Clear any Enemy objects in our List to prepare for next level.
			enemies.Clear();

			//Call the SetupScene function of the BoardManager script, pass it current level number.
			boardScript.SetupScene(level);

		}

		public void placeExit(Vector3 pos) {
			boardScript.placeExit(pos);
		}


		//Hides black image used between levels
		void HideLevelImage()
		{
			//Disable the levelImage gameObject.                                                                                                                           
			levelImage.SetActive(false);

			//Set doingSetup to false allowing player to move again.
			doingSetup = false;
		}

		//Update is called every frame.
		void Update()
		{
			if(Input.GetKeyDown(KeyCode.Escape)) {
				levelImage.SetActive(true);
				Credits();
			}

			//Check that playersTurn or enemiesMoving or doingSetup are not currently true.
			if(playersTurn || enemiesMoving || doingSetup)

				//If any of these are true, return and do not start MoveEnemies.
				return;

			//Start moving enemies.
			StartCoroutine (MoveEnemies ());
		}

		//Call this to add the passed in Enemy to the List of Enemy objects.
		public void AddEnemyToList(Enemy script)
		{
			//Add Enemy to List enemies.
			enemies.Add(script);
		}

		public void RemoveEnemyFromList(Enemy script)
		{
			//Add Enemy to List enemies.
			enemies.Remove(script);
		}


		//GameOver is called when the player reaches 0 food points
		public void GameOver()
		{
			doingSetup = true;
			//Set levelText to display number of levels passed and game over message
			levelText.text = "After " + level + " levels, you died.";
			levelImage.SetActive(true);
			Invoke("HideLevelImage", levelStartDelay);
			GameObject.Destroy(GameObject.Find("Board"));
			instance.boardScript.SetupScene(instance.level);
		}

		public void Win()
		{
            levelText.text = "Congratulations. \n You got all your superpowers back \n and got revenge on 'The Shephard'!\n But you are still a sheep...\nThe strange thing is: You dont care!\n Eating grass is great!";
			Invoke("Credits", 10);
		}

		public void Credits() {
			levelText.lineSpacing = 1.5f;
            levelText.text = "The powerless sheep.\n \n Written in a Game-Jam 2019 by: \n Benjamin Bourgart\n Jan de Haan \n Rohland Kohlring \n Marc Buskies \n Alicia Hormann \n \n Used assets from: \n Unity Asset Store(2D-Roguelike) \n The Sheep by Daniel Eddeland \n (https://opengameart.org/content/lpc-style-farm-animals) \n Sounds generated by SFXR, collected from \n Youtube (Sheep Scream), \n OrangeFreeSounds \n (http://www.orangefreesounds.com/epic-battle-music/) \n and other places from the internet.\n \n Thanks for playing!";
			Invoke("Quit", 10);
			
		}
		

		private void Quit() {
			Application.Quit();
		}

		//Coroutine to move enemies in sequence.
		IEnumerator MoveEnemies()
		{
			//While enemiesMoving is true player is unable to move.
			enemiesMoving = true;

			//Wait for turnDelay seconds, defaults to .1 (100 ms).
			yield return new WaitForSeconds(turnDelay);

			//If there are no enemies spawned (IE in first level):
			if (enemies.Count == 0)
			{
				//Wait for turnDelay seconds between moves, replaces delay caused by enemies moving when there are none.
				yield return new WaitForSeconds(turnDelay);
			}

			//Loop through List of Enemy objects.
			for (int i = 0; i < enemies.Count; i++)
			{
				//Call the MoveEnemy function of Enemy at index i in the enemies List.
				if (enemies[i] != null)
					enemies[i].MoveEnemy ();

				//Wait for Enemy's moveTime before moving next Enemy,
				yield return new WaitForSeconds(enemies[i].moveTime);
			}
			//Once Enemies are done moving, set playersTurn to true so player can move.
			playersTurn = true;

			//Enemies are done moving, set enemiesMoving to false.
			enemiesMoving = false;
		}
		public void restartLevel()
		{
			print(instance.level);
			GameObject.Destroy(GameObject.Find("Board"));
			instance.boardScript.SetupScene(instance.level);
		}

		public void goBackLevel()
		{
			instance.level--;
			GameObject.Destroy(GameObject.Find("Board"));
			instance.boardScript.SetupScene(instance.level);
		}

	}
}
