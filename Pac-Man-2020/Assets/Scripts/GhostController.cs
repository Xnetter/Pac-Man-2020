using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostController : ControllerNodes
{

    //Fright Mode Variables
    private static bool isScared = false;
    public static float frightTime= 5f;
    private static float blinkAtTime = 3f;
    private static float scaredTimer = 0f;
    private bool returningHome = false;

    // Scatter Mode Settings
    private int chaseIteration = 0; //Keeps track of current chase iteration.
    private int numberOfChaseIterations = 3; //The number of times ghosts will cycle from chase to scatter before permanent chase
    private float chaseDuration = 20f; // The amount of time each ghost will chase for while iterating. (before perm chase)
    private float scatterDuration = 7f; // The amount of time each ghost will scatter for while iterating. (before perm chase)

    // Bashful settings
    private bool isAtCorner = false; 
    private Vector2 nextNodePos;
    Node[] cornerNodes = new Node[4];
    private bool needNewTarget = true;
    // I am also using isChasing property for Bashful

    private enum Mode
    {
        Scatter,
        Chase,
        Frightened,
        Consumed
    }

    private Mode currentMode;

    // Time before ghosts leave jail;
    private float redStartDelay = 0f;
    private float orangeStartDelay = 5f;
    private float blueStartDelay = 10f;
    private float pinkStartDelay = 15f;

    private float myStartDelay;

    string myHomeBase;

    public float nAhead = 4f; //number of pills to aim ahead when using the nPillsAheadOfPacMan() decision algo

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
    public Node ghostHouse;
    public GhostColor identity = GhostColor.Blue; //Which ghost is this?
    private float releaseTimer = 0f;
    private float behaviorTimer = 0f;
    private bool isChasing = true; //Am I chasing or fleeing (Scatter Mode)
    private bool canLeave = false; //Determines if the ghost can leave.

    public static bool IsScared { get => isScared; set => isScared = value; }
    public static float ScaredTimer { get => scaredTimer; set => scaredTimer = value; }

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
        GameObject[] go = GameObject.FindGameObjectsWithTag("corner");
        
        for(int i = 0; i < cornerNodes.Length; i++){
            Vector2 nodePos = go[i].transform.position;
            cornerNodes[i] = getNodeAtPosition(nodePos);
        }
        //Initialize and Set Home Base by Identity.
        if (identity == GhostColor.Blue)
        {
            myHomeBase = "Home Base Blue";
        }
        else if (identity == GhostColor.Red)
        {
            myHomeBase = "Home Base Red";
        }
        else if (identity == GhostColor.Orange)
        {
            myHomeBase = "Home Base Orange";
        }
        else
            myHomeBase = "Home Base Pink";

        //Set myStartDelay at bootup to save computation during game and make scaling other features easier.
        if (identity == GhostColor.Blue)
        {
            myStartDelay = blueStartDelay;
        }
        else if (identity == GhostColor.Pink)
        {
            myStartDelay = pinkStartDelay;
        }
        else if (identity == GhostColor.Orange)
        {
            myStartDelay = orangeStartDelay;
        }
        else
        {
            myStartDelay = redStartDelay;
        }

        startPosition = startPositions[(int)identity];//Set start position for the ghosts.
        dirNum = Direction.Right;//Reinitialize for refresh;
        this.canReverse = false;//Ghosts cannot move unless they are at an intersection.

        transform.position = startPosition;//Ghost must start at node for now.

        Node current = getNodeAtPosition(transform.position);//Get node at this position.
        if (current != null)
        {
            currentNode = current;
        }

        currentMode = Mode.Chase;
    }

    bool checkIfInGhostHouse()
    {
        return transform.position == ghostHouse.transform.position;
    }

    public override void Update() //Override to change behavior
    {
        releaseTimer += Time.deltaTime; //Increment the Ghost Timer.

        if (releaseTimer - ScaredTimer >= myStartDelay) // Only apply scared mode to ghosts that are outside jail when Pac-Man eats a pellet.
            Scared();

        if (canLeave && !isScared) //Only increment the Behavior, or chase timer, if the ghost has left and isn't scared.
            behaviorTimer += Time.deltaTime;

        chaseOrFlee();//Are we chasing or fleeing? Choose to chase or flee using configuration at top of file. 

        if (!canLeave) //Don't release if we already can leave (efficiency check only).
            releaseGhosts();
        else
        {
            if (isChasing) //Use preprogrammed AI if chasing.
            {
                if (identity == GhostColor.Red)
                    shortestPathTo(objectName: "Pac-Man-Node");
                else if (identity == GhostColor.Pink)
                    nAheadOfPacMan();
                else if (identity == GhostColor.Blue)
                    doubleRedtoPacPlusTwo();
                else
                    BashfulAI();
            }
            else if (!isScared) //Otherwise, "Scatter" or chase home base.
                shortestPathTo(objectName: myHomeBase);
            else
                randomInput();
        }
        if (canLeave) //Don't leave unless your release timer is up.
            Move();

        UpdateOrientation();
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

                if (!returningHome)
                {
                    GameObject tile = GetTileAtPosition(currentPosition.transform.position);//possibly redundant function
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

    //Overloading shortestPathTo to take vector2s as input also

    private void shortestPathTo(Vector2 targetPosition) //Decision making algorithm: ghost finds his neighbor node closest to some postion as a vector and chooses that node as his next direction
    {

        if (getNodeAtPosition(transform.position) != null) //run only if on a node
        {
            float minDistance = 9999; //initialize minDistance to a random big value that's greater than any ghost-pacman distance possible
            Vector2 tempDirection = Vector2.zero; //initialize the direction vector the ghost will take
            Node currentPosition = getNodeAtPosition(transform.position); //get current position to then find my neighbors
            Node[] myNeighbors = currentPosition.neighbors; //get my neighbors, store them in an array of nodes called myNeighbors
            for (int i = 0; i < myNeighbors.Length; i++) //iterate over the neighbors to find the shortest one to pacman
            {
                if (direction * (-1) == currentPosition.validDir[i])
                {
                    continue;
                }
                if (!returningHome)
                {
                    GameObject tile = GetTileAtPosition(currentPosition.transform.position);//possibly redundant function
                    if (tile.GetComponent<Pills>().isJailEntrance && currentPosition.validDir[i] == Vector2.down)
                    {
                        continue;
                    }
                }
                Node neighborNode = myNeighbors[i];

                Vector2 nodePos = neighborNode.transform.position; //get the coordinates of the node

                float tempDistance = (targetPosition - nodePos).sqrMagnitude; //distance from pacman to the node we are currently iterating over
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

    private void doubleRedtoPacPlusTwo() ///Decision making algorithm: that choose the shortest path to the position obtained by doubling the vector from Blinky(red) to PacMan+2units.
    {
        Vector2 target = Vector2.zero;

        PacManController Pac = GameObject.FindGameObjectWithTag("PacMan").GetComponent<PacManController>(); //we find pacman to later access his position
        Direction PacFacing = Pac.getFacing(); //get the direction he is facing

        GameObject red = GameObject.Find("Blinky"); //find red/blinky to use his position

        //Now we need to use the vector coordinates of pac and red to find the vector coordinates of the target node
        Vector2 pacPos = Pac.transform.position; //get the coordinates of pacman
        Vector2 redPos = red.transform.position; //get the coordinates of red

        Vector2 pacPosPlusTwo = Vector2.zero;

        //choose how to add +2 to pac's coordinates and assgin it tt pacPosPlusTwo
        if (PacFacing == Direction.Down)
            pacPosPlusTwo = new Vector2(pacPos.x, pacPos.y - 2);
        else if (PacFacing == Direction.Left)
            pacPosPlusTwo = new Vector2(pacPos.x - 2, pacPos.y);
        else if (PacFacing == Direction.Right)
            pacPosPlusTwo = new Vector2(pacPos.x + 2, pacPos.y);
        else //implies that PacFacing == Direction.Up
            pacPosPlusTwo = new Vector2(pacPos.x, pacPos.y + 2);

        //Now use pacPosPlusTwo and redPos to find and assign target

        target = 2 * (pacPosPlusTwo - redPos); //target is double the vector from red to pac+2 (vector algebra)

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
                if (direction * (-1) == currentPosition.validDir[i]) //Mate must document
                {
                    continue;
                }

                if (!returningHome)
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
private bool b = true;



    private void BashfulAI()
    {
        Vector2 pacPos = GameObject.FindGameObjectWithTag("PacMan").transform.position;
        Vector2 ghostPos = transform.position;
        Node ghostNode = getNodeAtPosition(ghostPos);
    
        if(ghostNode != null){
            for(int i = 0; i < cornerNodes.Length && !isAtCorner && !isChasing; i++){
                if(cornerNodes[i] == ghostNode){
                    print("I am corner");
                    isAtCorner = true;
                }
            }
            if(isAtCorner){
                print("Is at corner");
                isChasing = true;
                isAtCorner = false;
            }
            if((ghostPos - pacPos).sqrMagnitude <= 20 || !isChasing){
                print("Chase turned off");
                isChasing = false;
                if((ghostPos - pacPos).sqrMagnitude <= 20 && needNewTarget){
                    nextNodePos = cornerNodes[(int)Random.Range(0, 4)].transform.position;
                    needNewTarget = false;
                    print("new target: " + nextNodePos);
                }
                print("Taarget: " + nextNodePos);
                shortestPathTo(nextNodePos);
            }else{
                if(isChasing){
                    shortestPathTo(pacPos);
                }
                needNewTarget = true;
            }
        }
        
    }


    private void Scared()//Might need to extract this to the gameboard class so that transitions are instantaneous.
    {
        if (isScared)
        {
            animator.SetBool("frightened", true);
            if(scaredTimer >= blinkAtTime)
            {
                animator.SetBool("blink", true);
            }
        }
        else
        {
            animator.SetBool("frightened", false);
            animator.SetBool("blink", false);
            isScared = false; //This code communicates with the game board script.
        }
    }

    public void Die()
    {
        
        GameObject.Find("Game").GetComponent<gameBoard>().PauseGame(0.5f);
        returningHome = true;

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
    
    GameObject getTileAtPosition(Vector2 pos)
    {
        int tileX = Mathf.RoundToInt(pos.x);
        int tileY = Mathf.RoundToInt(pos.y);

        GameObject tile = GameObject.Find("Game").GetComponent<gameBoard>().board[tileX, tileY];

        if (tile != null)
            return tile;
        return null;
    }
    private void chaseOrFlee()
    {
        if (currentMode == Mode.Chase)
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

    void Respawn()
    {
        canLeave = false;
        Node current = getNodeAtPosition(transform.position);//Get node at this position.
        if (current != null)
        {
            currentNode = current;
        }
        animator.SetBool("frightened", false);
        animator.SetBool("blink", false);
        //returningHome = false;
        behaviorTimer = 0f;
        isScared = false;
    }

}
