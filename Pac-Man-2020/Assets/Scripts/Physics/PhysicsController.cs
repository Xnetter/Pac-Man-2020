using UnityEngine;


public class Controller : MonoBehaviour
{
    //Quaternions won't be used in the final product. Quick and dirty, but needs to be organized.
    //Feel free to do so.
    private int facing = 1; // 0 = left, 1 = right, 2 = down, 3 = up;
    
    const float PM_Collider_Radius = 0.49f;
    

    private GameObject pacMan;
    private GameObject maze;
    private Rigidbody2D pacManRB;
    private CircleCollider2D pacManCollider;
    private Movements pacManRBMovement = new Movements(); 

    
    
    // Start is called before the first frame update
    void Start()
    {
        
        pacMan = GameObject.FindGameObjectWithTag("PacMan");
        maze = GameObject.FindGameObjectWithTag("Wall");
        pacManRB = pacMan.GetComponent<Rigidbody2D>(); // has to be initialized in Start()
        pacManCollider= pacMan.GetComponent<CircleCollider2D>();
        pacManRB.position = new Vector2(-0.497f, 1.504f);
        pacManRBMovement = new Movements(new Vector2(-1f, 0), 0.9f);
        pacManCollider.radius = PM_Collider_Radius;
        pacManRB.gravityScale = 0;
    }

    // Update is called once per frame
    void Update()
    {   
        pacManRBMovement.SetDirection(new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")));
        // CanMove(new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")), pacManRB.position);
            Flip(pacManRBMovement.GetDirection()); 

       
    }
    void OnTriggerEnter2D(Collider2D col)
    {
        
        if(col.name == "point"){
            pacManRBMovement.Turn();
        }else{
            pacManRBMovement.SetInIntersection(true);
            pacManRBMovement.EnterIntersection(col);
        }
            
    }
    void OnTriggerExit2D()
    {
        pacManRBMovement.SetInIntersection(false);
        pacManRBMovement.ExitIntersection();
    }


    void FixedUpdate()
    {
        pacManRB.MovePosition(pacManRB.position + pacManRBMovement.GetVelocity() * Time.deltaTime);
    }



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
