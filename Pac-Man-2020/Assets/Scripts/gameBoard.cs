using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class gameBoard : MonoBehaviour
{
    public enum MunchSound
    {
        ClassicRetro,
        StylizedArcade,
        Realistic
    }
    public MunchSound munchSound = MunchSound.ClassicRetro;
    // board dimensions
    private static int boardWidth = 30; 
    private static int boardHeight = 30;
    public static int LifeCount = 3;
    public static int MULTIPLIER = 10; //Score added per pill.
    private static float time = 0;
    //String Names of Game Characters for various uses. 
    private string LifeName1 = "PacLife2";
    private string LifeName2 = "PacLife3";
    private GameObject lifeAsset1;
    private GameObject lifeAsset2;
    public static string Ghost1 = "Blinky";
    public static string Ghost2 = "Inky";
    public static string Ghost3 = "Clyde";
    public static string Ghost4 = "Pinky";
    public static string PacManName = "Pac-Man-Node";
    //String identifiers of UI objects.
    public static string ready = "ReadySprite";
    //Point Tracker
    public static int points = 0;
    //Delay before game starts again after Pac-Man hits a ghost.
    public static int DEATH_DELAY = 5;
    public static int PAUSE_DELAY = 1; //pause when ghost hits pacman
    public static int WAIT_DELAY = 2; //delay for death animation
    public static int playerOneLevel = 1;
	public static int playerTwoLevel = 1;
    public int totalPellets = 0;
	public static int playerOneScore = 0;
	public static int playerTwoScore = 0;
    public static bool isPlayerOneUp = true;
    public Image playerLives2;
    public Image playerLives3;
    public Text playerOneUp;
	public Text playerOneScoreText;
	public Text playerTwoScoreText;
    public Text consumedGhostScoreText;

    //Array of type GameObject initialized with board width and height
    //These are the locations that will be stored
    //We are getting the positions of the game objects and then storing them at that position in this array.
    public GameObject[,] board = new GameObject[boardWidth, boardHeight];

    public Image[] bonusImages;


    bool didSpawnBonusItem1_player1;
	bool didSpawnBonusItem2_player1;
	bool didSpawnBonusItem1_player2;
	bool didSpawnBonusItem2_player2;


    private bool munch1 = true;

    // Start is called before the first frame update
    void Start()
    {
        lifeAsset1 = GameObject.Find(LifeName1);
        lifeAsset2 = GameObject.Find(LifeName2);
        //Create an array of objects containing every objects in the scene
        Object[] objects = GameObject.FindObjectsOfType(typeof(GameObject));

        //Then iterate over that array
        //Assign each object to the variable "o"
        foreach (GameObject o in objects)
		{
            //Get the positions: 
            Vector2 pos = o.transform.position; // we use "position" (instead of "localposition") which is in the global space of Unity. 

            //Sanity check: we only want to store the objects in the array (pills, walls, etc.) not PacMan itself. 
            if (o.name != "Pac-Man-Node" && o.name != "Game" && o.name != "Maze" && o.name != "Pills" && o.name != "Nodes" && o.name != "Background" &&  o.name != "NonNodes" && o.name != "Overlay" && o.tag != "Ghost" && o.tag != "UI" && o.tag != "Base" && o.tag != "Sound" && o.name != "Canvas" && o.tag != "UIElements")
			{
               // if (o.GetComponent<Pills>() != null) {
                //    if (o.GetComponent<Pills>().isPellet || o.GetComponent<Pills>().isLargePellet) {
                //        totalPellets++;
                //    }
                //}
                //store the object o in the board array
                Debug.Log("X: " + (int)pos.x + " Y: " + (int)pos.y + " " + o.name);
                board[(int)pos.x, (int)pos.y] = o;
                Debug.Log(board[(int)pos.x, (int)pos.y]);
			} else
			{
                //just print this in case PacMan is found. 
                // Debug.Log("Found " + o.name + " at " + pos);
			}
		}

    }







    public void score()
    {
        points += MULTIPLIER;
    }


    // Update is called once per frame
    void FixedUpdate()
    {
    }

    

    public void Die() //Put the death logic here.
    {
        StartCoroutine(RepositionCharactersAndDelay());
    }
  

    public void PauseGame(float waitTime)
    {
        StartCoroutine(SuspendState(waitTime));
    }

    IEnumerator SuspendState(float waitTime)
    {
        Debug.Log("Getting Called Here");
        GameObject BackgroundSound = GameObject.Find("BackgroundSound");
        GameObject Inky = GameObject.Find(Ghost1);
        GameObject Blinky = GameObject.Find(Ghost2);
        GameObject Clyde = GameObject.Find(Ghost3);
        GameObject Pinky = GameObject.Find(Ghost4);
        GameObject PacMan = GameObject.Find(PacManName);
        GameObject readySprite = GameObject.Find(ready);
        BackgroundSound.GetComponent<AudioSource>().Stop();

        Time.timeScale = 0.0f;
        Inky.GetComponent<GhostController>().enabled = false;
        Inky.GetComponent<Animator>().enabled = false;
        Blinky.GetComponent<GhostController>().enabled = false;
        Blinky.GetComponent<Animator>().enabled = false;
        Clyde.GetComponent<GhostController>().enabled = false;
        Clyde.GetComponent<Animator>().enabled = false;
        Pinky.GetComponent<GhostController>().enabled = false;
        Pinky.GetComponent<Animator>().enabled = false;
        PacMan.GetComponent<PacManController>().enabled = false;
        PacMan.GetComponent<Animator>().enabled = false;

        Time.timeScale = 1.0f;
        yield return new WaitForSeconds(waitTime); //delay once pacman hits ghost, initiates death animation
        //Ghost contact sound/ death sound
        //Disable Scripts for death delay.
        Inky.GetComponent<GhostController>().enabled = true;
        Inky.GetComponent<Animator>().enabled = true;
        Blinky.GetComponent<GhostController>().enabled = true;
        Blinky.GetComponent<Animator>().enabled = true;
        Clyde.GetComponent<GhostController>().enabled = true;
        Clyde.GetComponent<Animator>().enabled = true;
        Pinky.GetComponent<GhostController>().enabled = true;
        Pinky.GetComponent<Animator>().enabled = true;
        PacMan.GetComponent<PacManController>().enabled = true;
        PacMan.GetComponent<Animator>().enabled = true;
        BackgroundSound.GetComponent<AudioSource>().Play();
    }

    IEnumerator RepositionCharactersAndDelay()
    {
        GameObject DeathSound = GameObject.Find("DeathSound");
        GameObject BackgroundSound = GameObject.Find("BackgroundSound");
        GameObject Inky = GameObject.Find(Ghost1);
        GameObject Blinky = GameObject.Find(Ghost2);
        GameObject Clyde = GameObject.Find(Ghost3);
        GameObject Pinky = GameObject.Find(Ghost4);
        GameObject PacMan = GameObject.Find(PacManName);
        GameObject readySprite = GameObject.Find(ready);
        
        BackgroundSound.GetComponent<AudioSource>().Stop();
        //Pause game on contact
        Time.timeScale = 0.0f;
        Inky.GetComponent<GhostController>().enabled = false;
        Inky.GetComponent<Animator>().enabled = false;
        Blinky.GetComponent<GhostController>().enabled = false;
        Blinky.GetComponent<Animator>().enabled = false;
        Clyde.GetComponent<GhostController>().enabled = false;
        Clyde.GetComponent<Animator>().enabled = false;
        Pinky.GetComponent<GhostController>().enabled = false;
        Pinky.GetComponent<Animator>().enabled = false;
        PacMan.GetComponent<PacManController>().enabled = false;
        PacMan.GetComponent<Animator>().enabled = false;

        Time.timeScale = 1.0f;
        yield return new WaitForSeconds(PAUSE_DELAY); //delay once pacman hits ghost, initiates death animation
        //Ghost contact sound/ death sound
        //Disable Scripts for death delay.
        Inky.GetComponent<GhostController>().enabled = true;
        Inky.GetComponent<Animator>().enabled = true;
        Blinky.GetComponent<GhostController>().enabled = true;
        Blinky.GetComponent<Animator>().enabled = true;
        Clyde.GetComponent<GhostController>().enabled = true;
        Clyde.GetComponent<Animator>().enabled = true;
        Pinky.GetComponent<GhostController>().enabled = true;
        Pinky.GetComponent<Animator>().enabled = true;
        //Unpause after contact
        Inky.SetActive(false);
        Blinky.SetActive(false);
        Clyde.SetActive(false);
        Pinky.SetActive(false);// not pacman yet since death animation plays once ghosts disappear 

        GameObject pacMan = GameObject.Find(PacManName);
        PacMan.GetComponent<Animator>().enabled = true;
        PacMan.GetComponent<Animator>().Play("DeathAnim", 0, 0);
        DeathSound.GetComponent<AudioSource>().Play();
        yield return new WaitForSeconds(WAIT_DELAY); // delay to play death animation
        PacMan.GetComponent<PacManController>().enabled = true;
        PacMan.GetComponent<Animator>().enabled = true;
        PacMan.SetActive(false); // now pacman disappears since animation played


        //Reposition the character and reset all temp variables to original conditions.
        Inky.GetComponent<GhostController>().refresh();
        Blinky.GetComponent<GhostController>().refresh();
        Clyde.GetComponent<GhostController>().refresh();
        PacMan.GetComponent<PacManController>().refresh();
        Pinky.GetComponent<GhostController>().refresh();

        //Add ready sprite here.
        readySprite.GetComponent<SpriteRenderer>().enabled = true;
        readySprite.GetComponent<Animator>().enabled = true;
        readySprite.GetComponent<Animator>().Play("ReadySprite", 0, 0); //reseting the animation back to the  first frame
        yield return new WaitForSeconds(DEATH_DELAY); //Death Delay
        readySprite.GetComponent<Animator>().enabled = false; //reseting the animation back to the  first frame
        readySprite.GetComponent<SpriteRenderer>().enabled = false;
        //Remove ready sprite here. 
        
        //GO -- reactivate scripts.
        Inky.SetActive(true);
        Blinky.SetActive(true);
        Clyde.SetActive(true);
        Pinky.SetActive(true);
        PacMan.SetActive(true);
        BackgroundSound.GetComponent<AudioSource>().Play();
    }

    public void munch()
    {
        switch (munchSound)
        {
            case MunchSound.ClassicRetro:
                if (munch1)
                {
                    GetComponents<AudioSource>()[0].Play();
                    munch1 = false;
                }
                else
                {
                    GetComponents<AudioSource>()[1].Play();
                    munch1 = true;
                }
                break;
            case MunchSound.StylizedArcade:
                if (munch1)
                {
                    GetComponents<AudioSource>()[2].Play();
                    munch1 = false;
                }
                else
                {
                    GetComponents<AudioSource>()[3].Play();
                    munch1 = true;
                }
                break;
            case MunchSound.Realistic:
                if (munch1)
                {
                    GetComponents<AudioSource>()[4].Play();
                    munch1 = false;
                }
                else
                {
                    GetComponents<AudioSource>()[5].Play();
                    munch1 = true;
                }
                break;
        }
    }

    public void Update()
    {
        UpdateUI();
		// CheckPelletsConsumed ();
		// CheckShouldBlink ();
		BonusItems();
        if(LifeCount >= 3) {
            lifeAsset2.GetComponent<SpriteRenderer>().enabled = true;
            lifeAsset1.GetComponent<SpriteRenderer>().enabled = true;
        } else if(LifeCount == 2) {
            lifeAsset2.GetComponent<SpriteRenderer>().enabled = false;
            lifeAsset1.GetComponent<SpriteRenderer>().enabled = true;
        } else {
            lifeAsset2.GetComponent<SpriteRenderer>().enabled = false;
            lifeAsset1.GetComponent<SpriteRenderer>().enabled = false;
        } 
        //Handle Fright Mode outside of GhostController Class
        if (GhostController.IsScared && GhostController.ScaredTimer <= GhostController.frightTime)
        {
            GhostController.ScaredTimer += Time.deltaTime;
        }
        else
        {
            GhostController.ScaredTimer = 0f;
            GhostController.IsScared = false;
        }
    }


    	void BonusItems() {

			SpawnBonusItemForPlayer (1);

	}


    void SpawnBonusItemForPlayer (int playernum) {
		if (playernum == 1) {
			if (Pills.playerOnePelletsConsumed >= 70 && Pills.playerOnePelletsConsumed < 170) {
				if (!didSpawnBonusItem1_player1) {
					didSpawnBonusItem1_player1 = true;
					SpawnBonusItemForLevel (playerOneLevel);
				}
			} else  if (Pills.playerOnePelletsConsumed >= 170) {
				if (!didSpawnBonusItem2_player1) {
					didSpawnBonusItem2_player1 = true;
					SpawnBonusItemForLevel (playerOneLevel);
				}
			}
		} else {
			if (Pills.playerTwoPelletsConsumed >= 70 && Pills.playerTwoPelletsConsumed < 170) {
				if (!didSpawnBonusItem1_player2) {
					didSpawnBonusItem1_player2 = true;
					SpawnBonusItemForLevel (playerTwoLevel);
				}
			} else  if (Pills.playerTwoPelletsConsumed >= 170) {

				if (!didSpawnBonusItem2_player2) {
					didSpawnBonusItem2_player2 = true;
					SpawnBonusItemForLevel (playerTwoLevel);
				}
			}
		}
	}


    void SpawnBonusItemForLevel (int level) {

		GameObject bonusitem = null;

		if (level == 1) {
			bonusitem = Resources.Load ("Prefabs/bonus_cherries", typeof (GameObject)) as GameObject;
		} else if (level == 2) {
			bonusitem = Resources.Load ("Prefabs/bonus_strawberry", typeof (GameObject)) as GameObject;
		} else if (level == 3) {
			bonusitem = Resources.Load ("Prefabs/bonus_peach", typeof (GameObject)) as GameObject;			
		} else if (level == 4) {
			bonusitem = Resources.Load ("Prefabs/bonus_peach", typeof (GameObject)) as GameObject;
		} else if (level == 5) {
			bonusitem = Resources.Load ("Prefabs/bonus_apple", typeof (GameObject)) as GameObject;
		} else if (level == 6) {
			bonusitem = Resources.Load ("Prefabs/bonus_apple", typeof (GameObject)) as GameObject;
		} else if (level == 7) {
			bonusitem = Resources.Load ("Prefabs/bonus_grapes", typeof (GameObject)) as GameObject;
		} else if (level == 8) {
			bonusitem = Resources.Load ("Prefabs/bonus_grapes", typeof (GameObject)) as GameObject;
		} else if (level == 9) {
			bonusitem = Resources.Load ("Prefabs/bonus_galaxian", typeof (GameObject)) as GameObject;
		} else if (level == 10) {
			bonusitem = Resources.Load ("Prefabs/bonus_galaxian", typeof (GameObject)) as GameObject;
		} else if (level == 11) {
			bonusitem = Resources.Load ("Prefabs/bonus_bell", typeof (GameObject)) as GameObject;
		} else if (level == 12) {
			bonusitem = Resources.Load ("Prefabs/bonus_bell", typeof (GameObject)) as GameObject;
		} else {
			bonusitem = Resources.Load ("Prefabs/bonus_key", typeof (GameObject)) as GameObject;
		}

		Instantiate (bonusitem);
	}



    void UpdateUI () {
		 // playerOneScoreText.text = playerOneScore.ToString ();
		 // playerTwoScoreText.text = playerTwoScore.ToString ();

		int currentLevel;
        currentLevel = playerOneLevel;

	/*	if (isPlayerOneUp) {
			currentLevel = playerOneLevel;

			if (Pills.livesPlayerOne == 3) {
			 playerLives3.enabled = true;
			 playerLives2.enabled = true;

			} else if (Pills.livesPlayerOne == 2) {
			 playerLives3.enabled = false;
			 playerLives2.enabled = true;


			} else if (Pills.livesPlayerOne == 1) {
			 playerLives3.enabled = false;
			 playerLives2.enabled = false;
			}
		} else {
			currentLevel = playerTwoLevel;

			if (Pills.livesPlayerTwo == 3) {
			playerLives3.enabled = true;
			playerLives2.enabled = true;

			} else if (Pills.livesPlayerTwo == 2) {
			playerLives3.enabled = false;
			playerLives2.enabled = true;


			} else if (Pills.livesPlayerTwo == 1) {
			playerLives3.enabled = false;
			playerLives2.enabled = false;
			}
		} */

		for (int i = 0; i < bonusImages.Length; i++)
		{
			Image li = bonusImages[i];
			li.enabled = false;
		}

		for (int i = 1; i < bonusImages.Length+1; i++)
		{
			if (currentLevel >= i) {
				Image li = bonusImages[i-1];
				li.enabled = true;
			}
		}
	}


public void StartConsumedBonusItem (GameObject bonusItem, int scoreValue) {
		Vector2 pos = bonusItem.transform.position;
		Vector2 viewPortPoint = Camera.main.WorldToViewportPoint (pos);

		// consumedGhostScoreText.GetComponent<RectTransform> ().anchorMin = viewPortPoint;
		// consumedGhostScoreText.GetComponent<RectTransform> ().anchorMax = viewPortPoint;

		// consumedGhostScoreText.text = scoreValue.ToString ();
		// consumedGhostScoreText.GetComponent<Text> ().enabled = true;

		Destroy (bonusItem.gameObject);

		StartCoroutine (ProcessConsumedBonusItem (0.75f));
	}

	IEnumerator ProcessConsumedBonusItem (float delay) {
		yield return new WaitForSeconds (delay);

		// consumedGhostScoreText.GetComponent<Text> ().enabled = false;
	}






}

      
        
    