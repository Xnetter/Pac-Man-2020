
using UnityEngine;


public class Controller : MonoBehaviour
{
    //Quaternions won't be used in the final product. Quick and dirty, but needs to be organized.
    //Feel free to do so.
    private int facing = 1; // 0 = left, 1 = right, 2 = down, 3 = up;
    
    const float PM_Collider_Radius = 0.09f;
    

    private GameObject pacMan;
    private GameObject maze;
    public Rigidbody2D pacManRB;
    private CircleCollider2D pacManCollider;
    private Movements pacManRBMovement; 

    
    
    // Start is called before the first frame update
    void Start()
    {
        
        pacMan = GameObject.FindGameObjectWithTag("PacMan");
        maze = GameObject.FindGameObjectWithTag("Maze");
        pacManRB = pacMan.GetComponent<Rigidbody2D>(); // has to be initialized in Start()
        Debug.Log("hy: " + pacManRB.ToString());
        pacManCollider= pacMan.GetComponent<CircleCollider2D>();
        pacManRB.position = new Vector2(-0.497f, 1.504f);
        pacManRBMovement = new Movements(new Vector2(-1f, 0), 0.1f, maze);
        pacManCollider.radius = PM_Collider_Radius;
        pacManRB.gravityScale = 0;
    }

    // Update is called once per frame
    void Update()
    {   
        pacManRBMovement.SetDirection(new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")), pacManRB.position, pacManCollider.radius);
        // CanMove(new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")), pacManRB.position);
            Flip(pacManRBMovement.GetDirection()); 


        void OnTriggerEnter2D(Collider2D col){
            // if(the collided object == pacMan){
            //     pacManRBMovement.SetValidDirections(col.validDirections);
            // }
        }

        void OnCollisionExit(Collider2D col) {

        }
    }

    void FixedUpdate()
    {
        pacManRB.MovePosition(pacManRB.position + pacManRBMovement.GetVelocity() * Time.deltaTime);
    }

// private bool CanMove(Vector2 direction, Vector2 position)    // Raycast to detect collisions between pac-man and environment.
//     {
//         RaycastHit2D probe = Physics2D.Linecast(position + direction, position);
//         Debug.Log("hey: " + probe.collider.ToString());
//         return probe.collider == GetComponent<Collider2D>();
        
//     }


    //Rotations upon velocity change, using 0-3 as Pac Man's directions.
    void Flip(Vector2 direction) // We are using Quaternions as a very temporary solution -- later, we will use animation frames instead of actually modifying the transform.
    {
        Quaternion rotater = pacManRB.transform.localRotation;
        switch (direction.normalized.x) // Using the unit vector so I can switch on exact cases.
        {     
            case -1: // velocity is to the left
                if (facing != 0) {
                    rotater.eulerAngles = new Vector3(0,0,180);
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
        pacManRB.transform.localRotation = rotater;
    }
}
