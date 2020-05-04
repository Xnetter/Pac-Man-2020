﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostController : ControllerNodes
{

    //Fright Mode Variables
    public int frightSpeed;
    public int cruiseSpeed;
    private static bool isScared = false;
    private bool currentlyScared = false;
    public static float frightTime= 5f;
    private static float blinkForSeconds = 1.5f;//How long the ghost should blink for at the end of its fright duration. Cannot be greater than or equal to frighttime.
    private static float scaredTimer = 0f;
    private bool isConsumed = false;
    public Animation defaultState;
    public float defaultSpeed;
    public float eyeSpeed;
    public static bool IsScared { get => isScared; set => isScared = value; }
    public static float ScaredTimer { get => scaredTimer; set => scaredTimer = value; }
    public static bool canCruise = false;//
    //public static bool canCruise { get => canCruise; set => canCruise = value; }


    public Animation defualtAnimation;
    public Sprite eyesLeft;
    public Sprite eyesRight;
    public Sprite eyesUp;
    public Sprite eyesDown;

    public Node myGhostHouse;
    public Node otherGhostHouse;

    // Scatter Mode Settings

    private int chaseIteration = 0; //Keeps track of current chase iteration.
    private int numberOfChaseIterations = 3; //The number of times ghosts will cycle from chase to scatter before permanent chase
    private float chaseDuration = 20f; // The amount of time each ghost will chase for while iterating. (before perm chase)
    private float scatterDuration = 7f; // The amount of time each ghost will scatter for while iterating. (before perm chase)

    // Dijkstra
    List<Node> nodes = new List<Node>();
    
    // Time before ghosts leave jail;
    private float redStartDelay = 0f;
    private float orangeStartDelay = 5f;
    private float blueStartDelay = 10f;
    private float pinkStartDelay = 15f;

    //Decisin Algorithm Settings
    private float PacPlusN = 2f; //number of pills ahead of pacman used to obtain the vector pacPosPlusN and target in the function doubleRedToPacPlusN()
    public float nAhead = 4f; //number of pills to aim ahead when using the nPillsAheadOfPacMan() decision algo
    // Bashful settings
    private float approachableRadius = 5f; //the radius of the circle the ghost can approach pacman used in the function BashfulAI()
    private bool isAtCorner = false;
    private Node nextNode;
    private Vector2 nextNodePos;
    List<Node> cornerNodes = new List<Node>();
    private bool needNewTarget = true;
    // also using isChasing property for Bashful


    private float myStartDelay;

    string myHomeBase;

    public Animator animator;//This will need to be edited when each ghost is given a different start position. Appears in the GameBoard script.
    private Direction dirNum = Direction.Right;
    public enum GhostColor
    {
        Red, //leaves first
        Orange, //leaves second
        Blue, //leaves third
        Pink //leaves fourth
    }

    private Vector2[] startPositions = { new Vector2(10, 12), new Vector2(11, 10), new Vector2(10, 10), new Vector2(9,10)};//Corresponding Start Pos for ghost color.
    public GhostColor identity = GhostColor.Blue; //Which ghost is this?
    private float releaseTimer = 0f;
    private float behaviorTimer = 0f;
    private bool isChasing = true; //Am I chasing or fleeing (Scatter Mode)
    private bool canLeave = false; //Determines if the ghost can leave.



    public void resetRelease()
    {
        releaseTimer = 0;
        canLeave = false;
    }

    public override void refresh()
    {
        GetComponent<CircleCollider2D>().enabled = true;
        isConsumed = false;
        base.refresh();
        resetRelease();
    }

    public override void Awake(){
        // get all nodes for Dijkstra and assign it to "nodes"
        foreach(GameObject o in GameObject.FindGameObjectsWithTag("node")){
            nodes.Add(o.GetComponent<Node>());
        }
        foreach(GameObject o in GameObject.FindGameObjectsWithTag("corner")){
            nodes.Add(o.GetComponent<Node>()); // add corners to nodes
            cornerNodes.Add(o.GetComponent<Node>()); // add corners to cornerNode
        }
    }

    //Currently, Ghosts will continue behavior they were currently in upon Pac-Death (Scatter or Chase Mode)
    public override void Start()
    {
        //Initialize, Set Home Base, and myStartDelay by Identity.
        switch(identity){
            case GhostColor.Red: 
                myHomeBase = "Home Base Red";
                myStartDelay = redStartDelay;
                break;
            case GhostColor.Orange: 
                myHomeBase = "Home Base Orange";
                myStartDelay = orangeStartDelay;
                break;
            case GhostColor.Blue: 
                myHomeBase = "Home Base Blue";
                myStartDelay = blueStartDelay;
                break;
            case GhostColor.Pink:
                myHomeBase = "Home Base Pink";
                myStartDelay = pinkStartDelay;
                break;
            default:
                myHomeBase = "Home Base Red";
                myStartDelay = redStartDelay;
                break;
        }

        speed = defaultSpeed;
        startPosition = startPositions[(int)identity];//Set start position for the ghosts.
        dirNum = Direction.Right;//Reinitialize for refresh;
        this.canReverse = false;//Ghosts cannot move unless they are at an intersection.
        animator = GetComponent<Animator>();
        transform.position = startPosition;//Ghost must start at node for now.

        Node current = getNodeAtPosition(transform.position);//Get node at this position.
        if (current != null)
        {
            currentNode = current;
        }


    }

    public override void Update() //Override to change behavior
    {
        if (isConsumed)
        {
            returnToJail();
        }
        else
        {
            releaseTimer += Time.deltaTime; //Increment the Ghost Timer.

            if (releaseTimer - ScaredTimer >= myStartDelay)//If we are outside jail when Pac-Man eats a big pellet, then
            {
                currentlyScared = true;
                checkIfScared();


            }

           if (canCruise){
              if (!currentlyScared){
                cruiseElroy();
              }

            }

            if (canLeave && !isScared) //Only increment the Behavior, or chase timer, if the ghost has left and isn't scared.
                behaviorTimer += Time.deltaTime;

            chaseOrFlee();//Are we chasing or fleeing? Choose to chase or flee using configuration at top of file.

            if (!canLeave) //Don't release if we already can leave (efficiency check only).
                releaseGhosts();
            else
                chooseAI(); //Determine which AI we will use if we are not scared and we can leave jail.

        }

        if (canLeave) //Don't leave unless your release timer is up.
            Move();




        UpdateOrientation();
    }

    private void chooseAI()
    {
        if (currentlyScared)
            randomInput();
        else if (isChasing) //Use preprogrammed AI if chasing.
        {
            switch(identity){
                case GhostColor.Red:
                    shortestPathTo(objectName: "Pac-Man-Node");
                    // Dijkstra(); // goes into the jail
                    break;   
                case GhostColor.Pink:
                    nAheadOfPacMan();
                    break;
                case GhostColor.Blue:
                    doubleRedtoPacPlusN();
                    break;
                case GhostColor.Orange:  
                    randomInput();
                    break;
                default:
                    BashfulAI();
                    break;
            }
        }
        else //Otherwise, "Scatter" or chase home base.
            shortestPathTo(objectName: myHomeBase);
    }

    private void returnToJail()
    {
        shortestPathTo(myGhostHouse.transform.position);
        if (transform.position == myGhostHouse.transform.position || transform.position == otherGhostHouse.transform.position)
        {
            respawn();
        }
    }

    private void cruiseElroy(){
        speed = cruiseSpeed;

      }




    private void releaseGhosts()
    {
        if(releaseTimer >= myStartDelay)
        {
            canLeave = true;
        }
    }

    private void shortestPathTo(string objectName) //Decision making algorithm: ghost finds his neighbor node closest to some object as a vector and chooses that node as his next direction
    {
        GameObject target = GameObject.Find(objectName); //we find pacman to later access his position

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
                if (!isConsumed)//If we are not consumed, then we shouldn't enter jail.
                {
                    GameObject tile = GetTileAtPosition(currentPosition.transform.position);
                    if (tile.GetComponent<Pills>().isJailEntrance && currentPosition.validDir[i] == Vector2.down)
                    {
                        continue;
                    }
                }
                Node neighborNode = myNeighbors[i];

                Vector2 nodePos = neighborNode.transform.position; //get the coordinates of the node
                Vector2 targetPos = target.transform.position; //get the coordinates of pacman

                float tempDistance = (targetPos - nodePos).sqrMagnitude; //distance from pacman to the node we are currently iterating over

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

    private void shortestPathTo(Vector2 targetPosition)
    {

        if (getNodeAtPosition(transform.position) != null)
        {
            float minDistance = 9999;
            Vector2 tempDirection = Vector2.zero;
            Node currentPosition = getNodeAtPosition(transform.position);
            Node[] myNeighbors = currentPosition.neighbors;
            for (int i = 0; i < myNeighbors.Length; i++)
            {
                if (direction * (-1) == currentPosition.validDir[i])
                    continue;
                if (!isConsumed)
                {
                    GameObject tile = GetTileAtPosition(currentPosition.transform.position);
                    if (tile.GetComponent<Pills>().isJailEntrance && currentPosition.validDir[i] == Vector2.down)
                        continue;
                }
    
                Node neighborNode = myNeighbors[i];

                Vector2 nodePos = neighborNode.transform.position;

                float tempDistance = (targetPosition - nodePos).sqrMagnitude;
                if (tempDistance < minDistance)
                {
                    minDistance = tempDistance;
                    tempDirection = currentPosition.validDir[i];
                }
            }
            ChangePosition(tempDirection);
        }
    }

    private void doubleRedtoPacPlusN() //Decision making algorithm: that choose the shortest path to the position obtained by doubling the vector from Blinky(red) to PacMan + PacPlusN units.
    {
        Vector2 target = Vector2.zero;

        PacManController Pac = GameObject.FindGameObjectWithTag("PacMan").GetComponent<PacManController>(); //we find pacman to later access his position
        Direction PacFacing = Pac.getFacing(); //get the direction he is facing

        GameObject red = GameObject.Find("Blinky"); //find red/blinky to use his position

        //Now we need to use the vector coordinates of pac and red to find the vector coordinates of the target node
        Vector2 pacPos = Pac.transform.position; //get the coordinates of pacman
        Vector2 redPos = red.transform.position; //get the coordinates of red

        Vector2 pacPosPlusN = Vector2.zero;

        //choose how to add +PacPlusN to pac's coordinates and assgin it to pacPosPlusN
        if (PacFacing == Direction.Down)
            pacPosPlusN = new Vector2(pacPos.x, pacPos.y - PacPlusN);
        else if (PacFacing == Direction.Left)
            pacPosPlusN = new Vector2(pacPos.x - PacPlusN, pacPos.y);
        else if (PacFacing == Direction.Right)
            pacPosPlusN = new Vector2(pacPos.x + PacPlusN, pacPos.y);
        else //implies that PacFacing == Direction.Up
            pacPosPlusN = new Vector2(pacPos.x, pacPos.y + PacPlusN);

        //Now use pacPosPlusN and redPos to find and assign target

        target = 2 * (pacPosPlusN - redPos); //target is double the vector from red to pac+2 (vector algebra)

        shortestPathTo(targetPosition: target); //now that inky has his target position, just take the shortest path to it.
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
                if (direction * (-1) == currentPosition.validDir[i]) // don't allow reverse direction
                {
                    continue;
                }
                if (!isConsumed)
                {
                    GameObject tile = GetTileAtPosition(currentPosition.transform.position);//possibly redundant function
                    if (tile.GetComponent<Pills>().isJailEntrance && currentPosition.validDir[i] == Vector2.down)
                    {
                        continue;
                    }
                }
                Node neighborNode = myNeighbors[i];

                Vector2 nodePos = neighborNode.transform.position; //get the coordinates of the node
                Vector2 pacPos = Pac.transform.position; //get the coordinates of pacman

                //if statement to choose the right n pills ahead position depending on which direction pacman is going
                //can be rewritten using a switch statement
                //this is the most significant difference with shortestPathToPacMan()
                if (PacFacing == Direction.Down)
                    PacNAheadPosition = new Vector2(pacPos.x, pacPos.y - nAhead);
                else if (PacFacing == Direction.Left)
                    PacNAheadPosition = new Vector2(pacPos.x - nAhead, pacPos.y);
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

    private void BashfulAI()
    {
        Vector2 pacPos = GameObject.FindGameObjectWithTag("PacMan").transform.position;
        Node ghostNode = getNodeAtPosition(transform.position);
        if(ghostNode != null){
            Vector2 ghostPos = ghostNode.transform.position;
            foreach(Node node in cornerNodes){
                if(node == ghostNode)
                    isAtCorner = true;
            }

            if(isAtCorner){ // reset chasing when ghost reached the corner
                isChasing = true;
                isAtCorner = false;
            }

            if((ghostPos - pacPos).magnitude <= approachableRadius || !isChasing){
                isChasing = false;
                if((ghostPos - pacPos).magnitude <= approachableRadius && needNewTarget){ // don't want to find a new target every update but rather 1 target per chase flee cycle
                    nextNodePos = cornerNodes[(int)UnityEngine.Random.Range(0, 4)].transform.transform.position;
                    needNewTarget = false;
                }
                shortestPathTo(nextNodePos);
            } else {

                if(isChasing){
                    shortestPathTo(pacPos);
                }
                needNewTarget = true;
            }
        }
    }

    private void Dijkstra() // this algorithm shows the shortest path to pacMan
    { 
        Node ghostNode = getNodeAtPosition(transform.position);
        if(ghostNode != null){
            foreach(Node node in nodes){    // initialize node distance to INFINITY and predecessor to be null
                node.distance = 9999f;
                node.predecessor = null;
            }
            
            PacManController pac = GameObject.FindGameObjectWithTag("PacMan").GetComponent<PacManController>();
            HashSet<Node> visited = new HashSet<Node>();
            List<Node> priorityList = new List<Node>(); // have to sort after each insertion
            ghostNode.distance = 0f;
            priorityList.Add(ghostNode);
            while(priorityList.Count != 0){

                Node v = priorityList[0]; // retrieve Node with smallest distance property
                priorityList.RemoveAt(0); // remove previous Node
                visited.Add(v); // add node to the "visited" set
                for(int i = 0; i < v.neighbors.Length; i++){
                    Node u = v.neighbors[i];
                    if(u.tag == "jail"){ // don't add jail nodes to the options
                        continue;
                    }
                    if(!visited.Contains(u)){
                        float thisPathLength = v.distance + v.neighborDistance[i]; // distance to parent node from origin + distance to this neighbor node
                        if(u.distance > thisPathLength){
                            u.distance = thisPathLength; // distance to parent node from origin + distance to this neighbor node
                            u.predecessor = v;
                        } 
                        priorityList.Add(u);  
                        priorityList.Sort((x, y) => x.distance.CompareTo(y.distance)); // sort list based on Node distance from ghost; Node with smallest distance first
                    }
                }  
            }
                    ChangePosition(findNextDirection(ghostNode, pac.GetTargetNode()));
        }
    }

    private void DijkstraTo(Node targetNode, Node ghostNode) // this algorithm shows the shortest path to a specific node
    { 
        if(ghostNode != null){
            foreach(Node node in nodes){    // initialize node distance to INFINITY
                node.distance = 9999f;
                node.predecessor = null;
            }
            HashSet<Node> visited = new HashSet<Node>();
            List<Node> priorityList = new List<Node>(); // have to sort after each insertion
            ghostNode.distance = 0f;
            priorityList.Add(ghostNode);
            while(priorityList.Count != 0){
                
                Node v = priorityList[0]; // retrieve Node with smallest distance property
                priorityList.RemoveAt(0); // remove previous Node
                visited.Add(v); // add node to the "visited" set
                for(int i = 0; i < v.neighbors.Length; i++){
                    Node u = v.neighbors[i];
                    if(u.tag != "jail"){ // don't add jail nodes to the options
                        continue;
                    }
                    if(!visited.Contains(u)){
                        float thisPathLength = v.distance + v.neighborDistance[i]; // distance to parent node from origin + distance to this neighbor node
                        if(u.distance > thisPathLength){
                            u.distance = thisPathLength; // distance to parent node from origin + distance to this neighbor node
                            u.predecessor = v;
                        }
                        priorityList.Add(u);
                        priorityList.Sort((x, y) => x.distance.CompareTo(y.distance)); // sort list based on Node distance from ghost; Node with smallest distance first
                    }
                }
            }
            ChangePosition(findNextDirection(ghostNode, targetNode));
        }
    }

    private Vector2 findNextDirection(Node ghostNode, Node pacManNode){
        if(ghostNode != pacManNode.predecessor){
            return findNextDirection(ghostNode, pacManNode.predecessor);
        }else{
            return findDirectionHelper(ghostNode, pacManNode);
        }
    }

    private Vector2 findDirectionHelper(Node myNode, Node targetNode){
        Vector2 dir = new Vector2();
        for(int i = 0; i < myNode.neighbors.Length; i++){
                if(myNode.neighbors[i] == targetNode){
                     dir = myNode.validDir[i];
                     break;
                }
            }
        return dir;
    }

    private void checkIfScared()//Might need to extract this to the gameboard class so that transitions are instantaneous.
    {
        if(blinkForSeconds >= frightTime)
            throw new System.ArgumentException("blinkForSeconds cannot be greater than or eqaul to the specified frightTime", "blinkForSeconds");

        if (scaredTimer > 0 && scaredTimer <= frightTime)//Need to add transition from blink to fright for timer reset.
        {
            speed = frightSpeed;
          //  speed = cruiseSpeed;
            animator.SetBool("frightened", true);
            if(scaredTimer >= (frightTime-blinkForSeconds))
            {
                animator.SetBool("blink", true);
            }
        }
        else
        {
            speed = defaultSpeed;
            animator.SetBool("frightened", false);
            animator.SetBool("blink", false);
            currentlyScared = false;
        }
    }

    public void Die()
    {

        GameObject.Find("Game").GetComponent<gameBoard>().PauseGame(0.5f);
        resetAnimator();
        isConsumed = true;
        GetComponent<Animator>().enabled = false;
        GetComponent<CircleCollider2D>().enabled = false;
        speed = eyeSpeed;


    }



    void resetAnimator()
    {

        GetComponent<Animator>().SetTrigger("reset");
        GetComponent<Animator>().SetBool("frightened", false);
        GetComponent<Animator>().SetBool("blink", false);

    }

    void respawn()
    {
        releaseTimer = myStartDelay;//We were just released. Helps out animator.
        isConsumed = false;
        GetComponent<Animator>().enabled = true;
        GetComponent<CircleCollider2D>().enabled = true;
        speed = defaultSpeed;
    }

    private void UpdateOrientation()
    {
        if (!isConsumed)
        {
            if (direction == Vector2.left)
            {
                dirNum = Direction.Left;
            }
            else if (direction == Vector2.right)
            {
                dirNum = Direction.Right;
            }
            else if (direction == Vector2.up)
            {
                dirNum = Direction.Up;
            }
            else if (direction == Vector2.down)
            {
                dirNum = Direction.Down;
            }
            animator.SetInteger("orientation", (int)dirNum);
        }
        else
        {
            GetComponent<Animator>().enabled = false;
            Debug.Log("Disabling!");
            if (direction == Vector2.left)
            {
                GetComponent<SpriteRenderer>().sprite = eyesLeft;
            }
            else if (direction == Vector2.right)
            {
                GetComponent<SpriteRenderer>().sprite = eyesRight;
            }
            else if (direction == Vector2.up)
            {
                GetComponent<SpriteRenderer>().sprite = eyesUp;
            }
            else if (direction == Vector2.down)
            {
                GetComponent<SpriteRenderer>().sprite = eyesDown;
            }
        }

    }

    private void chaseOrFlee()
    {
        if (!currentlyScared)//If we are currently scared, we should not be chasing.
        {
            if (chaseIteration >= numberOfChaseIterations)
                isChasing = true;
            else if (isChasing && behaviorTimer > 20f)
            {
                isChasing = false;
                behaviorTimer = 0f;
            }
            else if (!isChasing && behaviorTimer > 7f)
            {
                isChasing = true;
                behaviorTimer = 0f;
                chaseIteration++;
            }
        }
        else
        {
            isChasing = false;
        }
    }

}
