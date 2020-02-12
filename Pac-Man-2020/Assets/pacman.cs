using UnityEngine;

public class PlayerMovement : MonoBehaviour {

    public Rigidbody rb;

    public float forwardForce = 2000f;
    public float sidewaysForce = 500f;

    void FixedUpdate () {

        rb.AddForce(0, 0, forwardForce * Time.deltaTime);


        if (input.GetKey("d")) 
        {
            rb.AddForce(sideways * Time.deltaTime);
        }


        if (input.GetKey("a")) 
        {
            rb.AddForce(sideways * Time.deltaTime);
        }


        if (input.GetKey("s")) 
        {
            rb.AddForce(forwardForce * Time.deltaTime);
        }

        
        if (input.GetKey("w")) 
        {
            rb.AddForce(forwardForce * Time.deltaTime);
        }
    }

} 