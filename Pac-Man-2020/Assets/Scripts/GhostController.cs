using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostController : ControllerNodes
{

    // Start is called before the first frame update
    // Time before ghosts leave jail;
    private float blueStartDelay = 0f;
    private float orangeStartDelay = 5f;
    private float redStartDelay = 10f;
    private float pinkStartDelay = 15f;

    public float nAhead = 4f; //number of pills to aim ahead when using the nPillsAheadOfPacMan() decision algo

    public Animator animator;//This will need to be edited when each ghost is given a different start position. Appears in the GameBoard script.
    private Direction dirNum = Direction.Right;
    public enum GhostColor
    {
        Blue, //leaves first
        Orange, //leaves second
        Red, //leaves third
        Pink //leaves fourth
    }
    private Vector2[] startPositions = { new Vector2(10, 12), new Vector2(11, 10), new Vector2(10, 10), new Vector2(9,10)};//Corresponding Start Pos for ghost color.
    public GhostColor identity = GhostColor.Blue; //Which ghost is this?
    private float releaseTimer = 0f;
    private bool canLeave = false; //Determines if the ghost can leave.

    public void resetRelease()
    {
        releaseTimer = 0;
        canLeave = false;
    }

    public override void refresh()
    {
        base.refresh();
        resetRelease();
    }
    public override void Start()
    {
        startPosition = startPositions[(int)identity];//Set start position for the ghosts.
        dirNum = Direction.Right;//Reinitialize for refresh;
        this.canReverse = false;//Ghosts cannot move unless they are at an intersection.

        transform.position = startPosition;//Ghost must start at node for now.


        Node current = getNodeAtPosition(transform.position);//Get node at this position.
        if (current != null)
        {
            currentNode = current;
            Debug.Log(currentNode);
        }

 //     ChangePosition(direction);
    }

    public override void Update() //Override to change behavior
    {
        releaseTimer += Time.deltaTime; //Increment the Ghost Timer.

        if(!canLeave) //Don't release if we already can leave (efficiency check only).
            releaseGhosts();

        //        randomInput();
        if (identity == GhostColor.Pink || identity == GhostColor.Red)
            shortestPathToPacMan();
        else if (identity == GhostColor.Orange)
            nAheadOfPacMan();
        else //so only blue goes random, coz he is stupid...
            randomInput();

        if (canLeave) //Don't leave unless your timer is up.
            Move();

        UpdateOrientation();
    }

    private void releaseGhosts()
    {
        if(identity == GhostColor.Blue && releaseTimer > blueStartDelay)
        {
            canLeave = true;
        }
        else if(identity == GhostColor.Pink && releaseTimer > pinkStartDelay)
        {
            canLeave = true;
        }
        else if(identity == GhostColor.Orange && releaseTimer > orangeStartDelay)
        {
            canLeave = true;
        }
        else if(identity == GhostColor.Red && releaseTimer > redStartDelay)
        {
            canLeave = true;
        }
    }

    private void shortestPathToPacMan() //Decision making algorithm: ghost finds his neighbor node closest to PacMan as a vector and chooses that node as his next direction
    {
        GameObject Pac = GameObject.FindGameObjectWithTag("PacMan"); //we find pacman to later access his position

        if(getNodeAtPosition(transform.position) != null) //run only if on a node
        {
            float minDistance = 9999; //initialize minDistance to a random big value that's greater than any ghost-pacman distance possible
            Vector2 tempDirection = Vector2.zero; //initialize the direction vector the ghost will take
            Node currentPosition = getNodeAtPosition(transform.position); //get current position to then find my neighbors
            Node[] myNeighbors = currentPosition.neighbors; //get my neighbors, store them in an array of nodes called myNeighbors

            for (int i = 0; i < myNeighbors.Length; i++) //iterate over the neighbors to find the shortest one to pacman
            {
                if(direction*(-1) == currentPosition.validDir[i])
                {
                    continue;
                }

                Node neighborNode = myNeighbors[i];

                Vector2 nodePos = neighborNode.transform.position; //get the coordinates of the node
                Vector2 pacPos = Pac.transform.position; //get the coordinates of pacman

                float tempDistance = (pacPos - nodePos).sqrMagnitude; //distance from pacman to the node we are currently iterating over
                
                if (tempDistance < minDistance) //if the vector distance between the neighbor is the min, set Ghost to go towards that Node
                {
                    //Access the valid directions of the node we are currently on.
                    minDistance = tempDistance;
                    tempDirection = currentPosition.validDir[i];
                }
            }
            //ghost chooses to go to the position of tempDirection store after the for-loop
            ChangePosition(tempDirection); //similar to randomInput()
        }
    }

    private void nAheadOfPacMan() //Decision making algorithm: ghost finds his neighbor node closest to n pills ahead of PacMan as a vector and chooses that node as his next direction
    {
        PacManController Pac = GameObject.FindGameObjectWithTag("PacMan").GetComponent<PacManController>(); //we find pacman to later access his position
        Direction PacFacing = Pac.getFacing(); //get the direction he is facing

        if (getNodeAtPosition(transform.position) != null) //run only if on a node
        {
            float minDistance = 9999; //initialize minDistance to a random big value that's greater than any ghost-pacman distance possible
            Vector2 tempDirection = Vector2.zero; //initialize the direction vector the ghost will take
            Node currentPosition = getNodeAtPosition(transform.position); //get current position to then find my neighbors
            Node[] myNeighbors = currentPosition.neighbors; //get my neighbors, store them in an array of nodes called myNeighbors

            Vector2 PacNAheadPosition = Vector2.zero;

            for (int i = 0; i < myNeighbors.Length; i++) //iterate over the neighbors to find the shortest one to pacman
            {
                if (direction * (-1) == currentPosition.validDir[i]) //Mate must document
                {
                    continue;
                }

                Node neighborNode = myNeighbors[i];

                Vector2 nodePos = neighborNode.transform.position; //get the coordinates of the node
                Vector2 pacPos = Pac.transform.position; //get the coordinates of pacman

                //if statement to choose the right n pills ahead position depending on which direction pacman is going
                //can be rewritten using a switch statement
                //this is the most significant difference with shortestPathToPacMan() 
                if (PacFacing == Direction.Down)
                    PacNAheadPosition = new Vector2(pacPos.x , pacPos.y - nAhead);
                else if (PacFacing == Direction.Left)
                    PacNAheadPosition = new Vector2(pacPos.x - nAhead , pacPos.y);
                else if (PacFacing == Direction.Right)
                    PacNAheadPosition = new Vector2(pacPos.x + nAhead, pacPos.y);
                else //implies that PacFacing == Direction.Up
                    PacNAheadPosition = new Vector2(pacPos.x, pacPos.y + nAhead);

                //now that we have the right PacFourAheadPosition we want, we can find its distance to the ghost...

                float tempDistance = (PacNAheadPosition - nodePos).sqrMagnitude; //distance from n pills ahead of pacman to the node we are currently iterating over

                if (tempDistance < minDistance) //if the vector distance between the neighbor is the min, set Ghost to go towards that Node
                {
                    //Access the valid directions of the node we are currently on.
                    minDistance = tempDistance;
                    tempDirection = currentPosition.validDir[i];
                }
            }
            //ghost chooses to go to the position of tempDirection store after the for-loop
            ChangePosition(tempDirection); //similar to randomInput() and shortestPathToPacMan()
        }

    }

    private void UpdateOrientation()
    {
        if (direction == Vector2.left)
        {
            dirNum = Direction.Left;
        }
        else if(direction == Vector2.right)
        {
            dirNum = Direction.Right;
        }
        else if(direction == Vector2.up)
        {
            dirNum = Direction.Up;
        }
        else if(direction == Vector2.down)
        {
            dirNum = Direction.Down;
        }
        animator.SetInteger("orientation", (int)dirNum);

    }

}
