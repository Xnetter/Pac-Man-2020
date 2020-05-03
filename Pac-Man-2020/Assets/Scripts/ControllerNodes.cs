using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerNodes : MonoBehaviour
{
    protected bool isPacMan = false;//Used for jailhouse logic
    private Vector2[] dirs = { Vector2.left, Vector2.right, Vector2.up, Vector2.down };
    protected bool canReverse = true;
    protected Vector2 direction = new Vector2(0,0);
    protected Vector2 startPosition = new Vector2(10, 4);
    protected Vector2 queuedDirection;
    public Sprite idle; //The sprite Pac-Man lands on when he stops moving. 
    public float speed = 3f;

    protected Node currentNode;
    protected Node previousNode;
    protected Node targetNode;

    public Node getTargetNode(){
        return targetNode;
    }
    public Node getPreviousNode(){
        return previousNode;
    }

    private int pelletsConsumed = 0;

    // Start is called before the first frame update
    public virtual void Start()
    {
        
    }


    public virtual void Update()
    { }

    public virtual void randomInput()
    {
        Vector2 newDir = dirs[RandomNumber()];
        while(direction*(-1) == newDir || (newDir == Vector2.down && GetTileAtPosition(transform.position).GetComponent<Pills>().isJailEntrance)){
            newDir = dirs[RandomNumber()];
        }
        ChangePosition(newDir);
    }

    public virtual void CheckInput()//Check Input and update current direction.
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            ChangePosition (Vector2.left);
            //MoveToNode(direction);
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            ChangePosition(Vector2.down);
            //MoveToNode(direction);
        }
        else if (Input.GetKeyDown(KeyCode.W))
        {
            ChangePosition(Vector2.up);
            //MoveToNode(direction);
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            ChangePosition(Vector2.right);
            //MoveToNode(direction);
        }
        
    }

    public virtual void refresh()
    {
        
        direction = Vector2.zero;
        queuedDirection = Vector2.zero;
        currentNode = null;
        targetNode = null;
        previousNode = null;
        Start();//Implemented Differently for each Character
    }

    // Update is called once per frame
    private void FixedUpdate()
    {

    }

    Node CanMove(Vector2 d)
    {
        Node moveToNode = null;
        Vector2[] validDirs = currentNode.validDir;
        for (int i = 0; i < validDirs.Length; i++)
        {
            if(currentNode.validDir[i] == d)
            {
                
                moveToNode = currentNode.neighbors[i];
                break;
            }
        }
        if (isPacMan && d == Vector2.down && currentNode.GetComponent<Pills>().isJailEntrance)
        {
            return null;
        }
        //disallow diagonal movement here.
        return moveToNode;
    }

    protected GameObject GetTileAtPosition(Vector2 pos)
    {
        int tileX = Mathf.RoundToInt(pos.x);
        int tileY = Mathf.RoundToInt(pos.y); //finding position of pill

        GameObject tile = GameObject.Find("Game").GetComponent<gameBoard>().board[tileX,tileY];
        if(tile != null){ //finds nonempty tiles
            return tile;
        }
        return null;
    }

    protected void ChangePosition(Vector2 d)
    {
        if (d != direction) //If the direction is different from current direction, store it for next intersection.
        {
            queuedDirection = d;
        }
        if(currentNode != null) //If we are at an intersection
        {
            Node moveToNode = CanMove(d);//Check if input direction is a valid move.

            if(moveToNode != null)//If it's a valid move.
            {
                direction = d;//Start moving in the direction of that node.
                targetNode = moveToNode;//Update target node to node that we just checked.
                previousNode = currentNode;//We are leaving current node, so it becomes previous node.
                currentNode = null;//We are no longer on a node.

            }
        }
    }

    protected void Move()
    {

        if(targetNode != currentNode && targetNode != null)
        {

            if(canReverse && queuedDirection == direction * -1) 
            {
                direction *= -1; //if quueued is inverse, invert direction
                Node tempNode = targetNode; //switch targetNode and previousNode
                targetNode = previousNode;
                previousNode = tempNode;
            }

            if (OverShotTarget())
            {
                currentNode = targetNode;
                transform.localPosition = currentNode.transform.position;//Snap pacman back to intersection.

                GameObject otherPortal = GetPortal(currentNode.transform.position);

                if(otherPortal != null){ //Is it a portal
                    transform.localPosition = otherPortal.transform.position; //move pac-man to other portal
                    currentNode = otherPortal.GetComponent<Node>(); // get components so pac-man can continue
                }

                Node moveToNode = CanMove(queuedDirection);

                if(moveToNode != null)//Does the queued direction actually exist at the intersection?
                {
                    direction = queuedDirection;
                }
                if(moveToNode == null)//Does the queued direction not exist?
                {
                    moveToNode = CanMove(direction);//Try to move using existing direction -- don't update.
                }
                if(moveToNode != null)//If we can move using existing direction, update node targets.
                {
                    targetNode = moveToNode;
                    previousNode = currentNode;
                    currentNode = null;
                }
                else//We can't move on existing direction OR queued direction, so we stop.
                {
                    direction = Vector2.zero;
                }
            }
            else
            {
                transform.localPosition += (Vector3)(direction * speed) * Time.deltaTime;//Move.
            }
        }
    }

    void MoveToNode(Vector2 d)
    {
        Node moveToNode = CanMove(d);

        if(moveToNode != null)
        {
            transform.position = moveToNode.transform.position;
            currentNode = moveToNode;
        }
    }

    protected Node getNodeAtPosition(Vector2 pos)//Get the intersection at this position.
    {
        GameObject tile = GameObject.Find("Game").GetComponent<gameBoard>().board[(int)pos.x, (int)pos.y];
        if (tile != null)
        {
            return tile.GetComponent<Node>();//Node is a component of the pill objects.
        }
        return null;
    }

    bool OverShotTarget()
    {
        float previousToTarget = LengthFromNode(targetNode.transform.position);
        float previousToSelf = LengthFromNode(transform.position);
        return previousToSelf > previousToTarget;
    }

    float LengthFromNode(Vector2 target)
    {
        Vector2 vect = target - (Vector2)previousNode.transform.position;
        return vect.sqrMagnitude;
    }

    GameObject GetPortal(Vector2 pos)
    {
        GameObject tile = GameObject.Find("Game").GetComponent<gameBoard>().board[(int)pos.x, (int)pos.y];
        if(tile != null){
            if(tile.GetComponent<Pills>() !=  null){ //retrieves components of tile
                if(tile.GetComponent<Pills>().isPortal){ //if portal
                    GameObject otherPortal = tile.GetComponent<Pills>().portalReceiver;
                    return  otherPortal; //get components of reciever portal 
                }
            }
        } 
        return null;
    }

    int RandomNumber() //Random Number Generator for Ghost to use as move input.
    {
        return Random.Range(0, 4);
    }

    public enum Direction {
        Left,
        Right,
        Down, 
        Up
    }
}
