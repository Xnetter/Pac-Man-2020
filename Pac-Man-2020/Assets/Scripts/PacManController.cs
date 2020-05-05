﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PacManController : ControllerNodes
{
    public bool randomMovement = false;
    private Direction facing = Direction.Left; // 0 = left, 1 = right, 2 = down, 3 = up;
    private static float BUFFER_PILL_TIME = .45f;//Amount of time each pill adds to the pill munching duration length.

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
                if (gameBoard.isPlayerOneUp) {
                    if (!tile.Consumed && (tile.isPellet || tile.isLargePellet || tile.isBonusItem))
                    { //tile has visible pellet and is a pellet of some form
                        o.GetComponent<SpriteRenderer>().enabled = false; //make oil invisible
                        tile.Consumed = true; //update system
                        GameObject temp = GameObject.Find("Game");//get the game object.
                        gameBoard game = temp.GetComponent<gameBoard>();//get the game state
                        game.score();//score
                        game.munch();
                            if (tile.isLargePellet) 
                            {
                                GhostController.ScaredTimer = 0f;
                                GhostController.IsScared = true;
                                gameBoard.playerOneScore += 50;
                            } else {
                                gameBoard.playerOneScore += 10;
                                Pills.playerOnePelletsConsumed++;
                                   }
                             }
                            if (tile.isBonusItem)
                                ConsumedBonusItem(1, tile);

                        //game.addTime(BUFFER_PILL_TIME);// WORKS AT SPEED 5 or maybe sorta (.45f*(5/speed))
                        //if (!temp.GetComponent<AudioSource>().isPlaying)
                        //{
                        //    temp.GetComponent<AudioSource>().Play();
                        //}
                    }
                }
            }
        }



            public void ConsumedBonusItem(int playerNum, Pills bonusItem) 
            {
                if (playerNum == 1) 
                {
                    gameBoard.playerOneScore += bonusItem.pointValue;
                } 
                GameObject.Find("Game").transform.GetComponent<gameBoard>().StartConsumedBonusItem(bonusItem.gameObject, bonusItem.pointValue); 
            } 
             


/*
    void ConsumeBonusItem()
        {                
       //  Pills points = GameObject.Find("bonus_items").GetComponent<Pills>(); //gets pill information
        
       // if (points.currentLifeTime < points.randomLifeExpectancy) {
           //    points.currentLifeTime += Time.deltaTime;


         if (transform.position == GameObject.Find("bonus_items").transform.position)
            {  
               // if (points.currentLifeTime < points.randomLifeExpectancy) {
               // points.currentLifeTime += Time.deltaTime;
                Debug.Log("Entering Consume If Statement");    
                GameObject.Find("bonus_items").GetComponent<SpriteRenderer>().enabled = false;
                gameBoard game = GameObject.Find("bonus_items").GetComponent<gameBoard>();//get the game state
                game.score();//score
                game.munch();
                
                }
            }  
*/




    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<Animator>().GetBool("frightened"))
            collision.gameObject.GetComponent<GhostController>().Die();
        else
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
