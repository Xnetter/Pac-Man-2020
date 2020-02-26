using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public Node[] neighbors;
    public Vector2[] validDir;

    // Start is called before the first frame update
    void Start()
    {
        validDir = new Vector2[neighbors.Length];
        for(int i = 0; i < neighbors.Length; i++)
        {
            Node neighbor = neighbors[i];
            validDir[i] = (neighbor.transform.localPosition - transform.localPosition).normalized;

        }
    }
}
