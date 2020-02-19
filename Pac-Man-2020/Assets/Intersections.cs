// using System.Collections.Generic;
// using System.Collections;
// using UnityEngine;


// public class Intersections : MonoBehaviour
// {
    
//     private Vector2 direction;
//     UnityEngine.Random rand = new UnityEngine.Random();

//     void OnCollisionEnter(Collision collisionInfo){
//         if(collisionInfo.collider.tag == "intersection"){
//             int newDirection = (int)Random.Range(0, 4); // returns a number between 0 and 4 inclusive
//             switch(newDirection){
//                 case 0:
//                     direction = new Vector2(-1,0);
//                     break;
//                 case 1:
//                     direction = new Vector2(1, 0);
//                     break;
//                 case 2:
//                     direction = new Vector2(0, -1);
//                     break;
//                 case 3:
//                     direction = new Vector2(0, 1);
//                     break;
//                 default:
//                     direction = new Vector2(0, 1); // in case newDirection is 4 
//                     break;
//             }
//         }
//         collisionInfo.collider.gameObject
//     }

//     Vector2 getNewDirection(){
//         return direction;
//     }
// }
//     //Quaternions won't be used in the final product. Quick and dirty, but needs to be organized.
//     //Feel free to do so.
//     public Rigidbody2D ghostCharacter;

//     public bool freeMovement;
//     public float speed;
//     private Vector2 moveVelocity;
//     private Vector2 moveInput;
//     private int facing = 1; // 0 = left, 1 = right, 2 = down, 3 = up;
    
//     // Start is called before the first frame update
//     void Start()
//     {
//         ghostCharacter = GetComponent<Rigidbody2D>();
//         ghostCharacter.gravityScale = 0;
//         ghostCharacter.transform.position = new Vector2(-0.46f, 1.6f);
//     }

//     // Update is called once per frame
//     void Update()
//     {
        
//     }

//     private void FixedUpdate()
//     {
//          ghostCharacter.MovePosition(ghostCharacter.position + direction * speed * Time.deltaTime);
//     }
    
    