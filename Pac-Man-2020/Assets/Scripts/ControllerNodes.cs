using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerNodes : MonoBehaviour
{

    private Vector2 direction = new Vector2(0,0);
    private Vector2 queuedDirection;

    public Sprite idle; //The sprite Pac-Man lands on when he stops moving. 

    public float speed = 3f;
    private int facing = 1; // 0 = left, 1 = right, 2 = down, 3 = up;
    private Node currentNode;
    private Node previousNode;
    private Node targetNode;

    // Start is called before the first frame update
    void Start()
    {
        transform.position = new Vector2(6, 10);//PAC-MAN MUST START ON A NODE FOR NOW.

        Node current = getNodeAtPosition(transform.position);//Get node at this position.
        if (current != null)
        {
            currentNode = current;
            Debug.Log(currentNode);
        }

        direction = Vector2.left;//Auto start.
        ChangePosition(direction);
    }


    void Update()
    {
        CheckInput();//Disallows diagonal or conflicting movements.

        Move();//Move, or act on gathered user  input.

        Flip();//Update orientation using current direction data.

        ConsumePellet();  //Run to see if pill to be consumed. 

        stopChewing();//Check if not moving to stop chewing animation.
    }

    void CheckInput()//Check Input and update current direction.
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

    // Update is called once per frame
    private void FixedUpdate()
    {

    }

    void ConsumePellet(){
        GameObject o = GetTileAtPosition(transform.position);

        if(o != null){
            Pills tile = o.GetComponent<Pills>();
            if(tile != null){
                if(!tile.Consumed && (tile.isPellet || tile.isLargePellet)){
                    o.GetComponent<SpriteRenderer>().enabled = false;
                    tile.Consumed = true;
                }
            }
        }
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
        //disallow diagonal movement here.
        return moveToNode;
    }

    GameObject GetTileAtPosition(Vector2 pos)
    {
        int tileX = Mathf.RoundToInt(pos.x);
        int tileY = Mathf.RoundToInt(pos.y); //finding position of pill

        GameObject tile = GameObject.Find("Game").GetComponent<gameBoard>().board[tileX,tileY];
        if(tile != null){
            return tile;
        }
        return null;
    }

    void ChangePosition(Vector2 d)
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


    void Move()
    {

        if(targetNode != currentNode && targetNode != null)
        {

            if(queuedDirection == direction * -1) 
            {
                direction *= -1;
                Node tempNode = targetNode;
                targetNode = previousNode;
                previousNode = tempNode;
            }

            if (OverShotTarget())
            {
                currentNode = targetNode;
                transform.localPosition = currentNode.transform.position;

                GameObject otherPortal = GetPortal(currentNode.transform.position);

                if(otherPortal != null){ //Is it a portal
                    transform.localPosition = otherPortal.transform.position;
                    currentNode = otherPortal.GetComponent<Node>();
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

    Node getNodeAtPosition(Vector2 pos)//Get the intersection at this position.
    {
        GameObject tile = GameObject.Find("Game").GetComponent<gameBoard>().board[(int)pos.x, (int)pos.y];
        if (tile != null)
        {
            Debug.Log("Not Null");
            return tile.GetComponent<Node>();//Node is a component of the pill objects.
        }
        return null;
    }

    void Flip() // We are using Quaternions as a very temporary solution -- later, we will use animation frames instead of actually modifying the transform.
    {
        Quaternion rotater = transform.localRotation;
        switch (direction.normalized.x) // Using the unit vector so I can switch on exact cases.
        {
            case -1: // velocity is to the left
                if (facing != 0)
                {
                    rotater.eulerAngles = new Vector3(0, 0, 180);
                    facing = 0;
                }
                break;
            case 1: // velocity is to the right
                if (facing != 1)
                {
                    rotater.eulerAngles = new Vector3(0, 0, 0);
                    facing = 1;
                }
                break;
        }
        switch (direction.normalized.y)
        {
            case -1: // velocity is down.
                if (facing != 2)
                {
                    rotater.eulerAngles = new Vector3(0, 0, 270);
                    facing = 2;
                }
                break;
            case 1: // velocity is up.
                if (facing != 3)
                {
                    rotater.eulerAngles = new Vector3(0, 0, 90);
                    facing = 3;
                }
                break;
        }
        transform.localRotation = rotater;
    }

    void stopChewing()
    {
        if(direction == Vector2.zero)
        {
            GetComponent<Animator>().enabled = false;
            GetComponent<SpriteRenderer>().sprite = idle; //Uncomment this, and set the graphic you want Pac-Man to stop on.
        }
        else
        {
            GetComponent<Animator>().enabled = true;
        }
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
            if(tile.GetComponent<Pills>() !=  null){
                if(tile.GetComponent<Pills>().isPortal){
                    GameObject otherPortal = tile.GetComponent<Pills>().portalReceiver;
                    return  otherPortal;
                }
            }
        } 
        return null;
    }
}
