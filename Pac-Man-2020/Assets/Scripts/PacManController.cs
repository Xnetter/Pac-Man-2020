using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class PacManController : ControllerNodes
{
    public bool randomMovement = false;
    private Direction facing = Direction.Left; // 0 = left, 1 = right, 2 = down, 3 = up;
    private static float BUFFER_PILL_TIME = .45f;//Amount of time each pill adds to the pill munching duration length.

    public int totalPellets=0;
    //public static gameBoard instance;
    //public int level = 1;
    int allPellets = 30;
    public static int cruisePellets = 140;
    
    //private int cruiseFactor = 10 ;
    //public int speedy = GhostController.speed;

   //float Spd = GameObject.Find("Blinky").GetComponent<GhostController>().speed;
    private int ghostCounter = 1; //counts ghosts being eaten
    private int ghostPoints = 200; //value of first ghost (goes up exp) 


    public override void Start()
    {
        isPacMan = true;
        if (randomMovement)
        {
            this.canReverse = false;
        }
        facing = Direction.Left;
        transform.position = startPosition;//PAC-MAN MUST START ON A NODE FOR NOW.
        Node current = getNodeAtPosition(transform.position);//Get node at this position.
        if (current != null)
        {
            currentNode = current;
            // Debug.Log(currentNode);
        }

        direction = Vector2.left;//Auto start.
        ChangePosition(direction);
        transform.rotation = Quaternion.Euler(0, 0, 180); //Face left.
    }
    public override void Update()
    {
        if (!randomMovement)
            CheckInput();//Disallows diagonal or conflicting movements.
        else randomInput();

        Move();//Move, or act on gathered user  input.

        Flip();//Update orientation using current direction data.

        ConsumePellet();  //Run to see if pill to be consumed.

        StopChewing();//Check if not moving to stop chewing animation.
    }

    void Flip() // We are using Quaternions as a very temporary solution -- later, we will use animation frames instead of actually modifying the transform.
    {
        Quaternion rotater = transform.localRotation;
        switch (direction.normalized.x) // Using the unit vector so I can switch on exact cases.
        {
            case -1: // velocity is to the left
                if (facing != Direction.Left)
                {
                    rotater.eulerAngles = new Vector3(0, 0, 180);
                    facing = (Direction)0;
                }
                break;
            case 1: // velocity is to the right
                if (facing != Direction.Right)
                {
                    rotater.eulerAngles = new Vector3(0, 0, 0);
                    facing = Direction.Right;
                }
                break;
        }
        switch (direction.normalized.y)
        {
            case -1: // velocity is down.
                if (facing != Direction.Down)
                {
                    rotater.eulerAngles = new Vector3(0, 0, 270);
                    facing = Direction.Down;
                }
                break;
            case 1: // velocity is up.
                if (facing != Direction.Up)
                {
                    rotater.eulerAngles = new Vector3(0, 0, 90);
                    facing = Direction.Up;
                }
                break;
        }
        transform.localRotation = rotater;
    }

    void StopChewing()
    {
        if (direction == Vector2.zero)
        {
            GetComponent<Animator>().enabled = false;
            GetComponent<SpriteRenderer>().sprite = idle; //Uncomment this, and set the graphic you want Pac-Man to stop on.
        }
        /*if(totalPellets==30) {
          GetComponent<Animator>().enabled = false;
          GetComponent<SpriteRenderer>().sprite = nextLevel;
        }*/

        else
        {
            GetComponent<Animator>().enabled = true;
        }
    }

    void ConsumePellet()
    {
        GameObject o = GetTileAtPosition(transform.position);  //pellet object created with correct coordinates

        if (o != null)
        {
            Pills tile = o.GetComponent<Pills>(); //gets pill information
            if (tile != null)
            {
                if (!tile.Consumed && (tile.isPellet || tile.isLargePellet))
                { //tile has visible pellet and is a pellet of some form
                    o.GetComponent<SpriteRenderer>().enabled = false; //make oill invisible
                    tile.Consumed = true; //update system
                    GameObject temp = GameObject.Find("Game");//get the game object.
                    gameBoard game = temp.GetComponent<gameBoard>();//get the game state
                    game.score();//score
                    game.munch();

                    totalPellets++;

                    if (totalPellets == allPellets){ // allPellets = 191

                      GhostController.canCruise = false;

                      GetComponent<Animator>().enabled = false;
                      GetComponent<SpriteRenderer>().sprite = nextLevel;


                      GameObject.Find("Game").GetComponent<gameBoard>().LevelUp();
                    }




                  if (totalPellets == cruisePellets) { // cruisePellets = 140, after 140 pellets blinky speeds up
                       GhostController.canCruise = true;
                       //GhostController.cruiseElroy();
                    }

                    if (tile.isLargePellet)
                    {
                        if(GhostController.IsScared)
                        {
                            Debug.Log("Ghost # = " + ghostCounter);
                        } else
                        {
                            ghostCounter = 1;
                        }
                        GhostController.ScaredTimer = 0f;
                        GhostController.IsScared = true;

                    }






                    //game.addTime(BUFFER_PILL_TIME);// WORKS AT SPEED 5 or maybe sorta (.45f*(5/speed))
                    //if (!temp.GetComponent<AudioSource>().isPlaying)
                    //{
                    //    temp.GetComponent<AudioSource>().Play();
                    //}
                }

            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<Animator>().GetBool("frightened"))
        {
            if (ghostCounter < 5)
            {
                Debug.Log("The Ghost is Scared and Eaten");
                gameBoard.points += ghostPoints * ghostCounter;
                ghostCounter += 1;
            } else 
            {
                gameBoard.points += ghostPoints * 8; //score 1600 if more than 4 ghost eaten
            }
            collision.gameObject.GetComponent<GhostController>().Die();
        } else
        {
            gameBoard.LifeCount--;
            GameObject.Find("Game").GetComponent<gameBoard>().Die();
        }
    }

    public Direction getFacing()
    {
        return facing;
    }
}
