using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    //Quaternions won't be used in the final product. Quick and dirty, but needs to be organized.
    //Feel free to do so.
    public Rigidbody2D character;
    public bool freeMovement;
    public float speed;
    private Vector2 moveVelocity;
    private Vector2 moveInput;
    private int facing = 1; // 0 = left, 1 = right, 2 = down, 3 = up;
    
    // Start is called before the first frame update
    void Start()
    {
        character = GetComponent<Rigidbody2D>();
        character.gravityScale = 0;
        character.transform.position = new Vector2(-0.46f, 1.6f);
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        bool validMove = CanMove(moveInput);
        if (!moveInput.Equals(new Vector2(0,0)) && !freeMovement && validMove) // Constantly Apply Velocity Vector
        {
            moveVelocity = moveInput.normalized * speed;
            Flip(moveVelocity); //Rotate.
        }
        else if (freeMovement) //Only apply while key is pressed.
        {
            moveVelocity = moveInput.normalized * speed;
        }
    }

    private void FixedUpdate()
    {
         character.MovePosition(character.position + moveVelocity * Time.deltaTime);
    }
    
    private bool CanMove(Vector2 direction)// Raycast to detect collisions between pac-man and environment.
    {
        Vector2 position = character.transform.position;
        RaycastHit2D probe = Physics2D.Linecast(position + direction, position);
        Debug.Log(probe.collider == GetComponent<Collider2D>());
        return probe.collider == GetComponent<Collider2D>();
    }


    //Rotations upon velocity change, using 0-3 as Pac Man's directions.
    void Flip(Vector2 direction) // We are using Quaternions as a very temporary solution -- later, we will use animation frames instead of actually modifying the transform.
    {
        Quaternion rotater = character.transform.localRotation;
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
        character.transform.localRotation = rotater;
    }
}
