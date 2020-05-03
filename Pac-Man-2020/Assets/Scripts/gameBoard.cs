using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
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
    public static int LifeCount = 2;
    public static int maxLife = 3;
    private static int minLife = 0;
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
    public static int WAIT_DELAY = 2; //delay for death animation

    //Array of type GameObject initialized with board width and height
    //These are the locations that will be stored
    //We are getting the positions of the game objects and then storing them at that position in this array.
    public GameObject[,] board = new GameObject[boardWidth, boardHeight];

    private bool munch1 = true;

    // Start is called before the first frame update
    void Start()
    {
        points = 0;
        LifeCount = 2;
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
            if (o.name != "Pac-Man-Node" && o.name != "Game" && o.name != "Maze" && o.name != "Pills" && o.name != "Nodes" && o.name != "Background" &&  o.name != "NonNodes" && o.name != "Overlay" && o.tag != "Ghost" && o.tag != "UI" && o.tag != "Base" && o.tag != "Sound")
			{
                // if (o.GetComponent<Pills>() != null) {
                //    if (o.GetComponent<Pills>().isPellet || o.GetComponent<Pills>().isLargePellet) {
                //        totalPellets++;
                //    }
                // }
                //store the object o in the board array
               // Debug.Log("X: " + (int)pos.x + " Y: " + (int)pos.y + " " + o.name);
                board[(int)pos.x, (int)pos.y] = o;
                //Debug.Log(board[(int)pos.x, (int)pos.y]);
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
        //yield return new WaitForSeconds(PAUSE_DELAY);
        PauseGame(WAIT_DELAY); //delay once pacman hits ghost, initiates death animation
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
        BackgroundSound.GetComponent<AudioSource>().Stop();
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

    private void Update()
    {
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
        //Handle GAME OVER 
        if(LifeCount == minLife){
            SceneManager.LoadScene("GameOver");
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
}
