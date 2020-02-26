using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ValidDirections : MonoBehaviour
{
    private GameObject[] pills;
    private static Dictionary<int, bool[]> intersections = new Dictionary<int, bool[]>();
    

    


    // Has to run before ghosts and pac-man are added
    void Start()
    {
        //Dictionary<int, bool[]> intersections;
        pills = GameObject.FindGameObjectsWithTag("Intersection"); // find all pills at intersections
        bool[] directions = new bool[4];

        for (int i = 0; i < pills.Length; i++)
        {
            Vector2 position = (Vector2)pills[i].transform.position; // position is originally Vector3
            
            // determine valid directions
            directions[0] = Physics2D.Linecast(position, position + new Vector2(0.6f, 0f)).collider == null ? true : 
            Physics2D.Linecast(position, position + new Vector2(0.6f, 0f)).collider.tag != "Wall"; // the next cell starts at pos. + 0.5f so I use a bit longer line
            directions[1] = Physics2D.Linecast(position, position + new Vector2(-0.6f, 0f)).collider == null ? true : 
            Physics2D.Linecast(position, position + new Vector2(-0.6f, 0f)).collider.tag != "Wall";
            directions[2] = Physics2D.Linecast(position, position + new Vector2(0f, 0.6f)).collider == null ? true : 
            Physics2D.Linecast(position, position + new Vector2(0f, 0.6f)).collider.tag != "Wall";
            directions[3] = Physics2D.Linecast(position, position + new Vector2(0f, -0.6f)).collider == null ? true : 
            Physics2D.Linecast(position, position + new Vector2(0f, -0.6f)).collider.tag != "Wall";

            intersections.Add(pills[i].GetInstanceID(), directions); // add pillID and valid directions to dictionary
        }
    }

    public Dictionary<int, bool[]> getIntersections(){
        return intersections;
    }

    
}
