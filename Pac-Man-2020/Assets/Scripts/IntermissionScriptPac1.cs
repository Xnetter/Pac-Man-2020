using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntermissionScript : MonoBehaviour

{
    // Start is called before the first frame update

    protected Vector2 pacTransBegin = new Vector2(23, 10);
    protected Vector2 pacTransEnd = new Vector2(-10, 10);
    public float timeForMove = 6.0f;
    float timer;


    void Start()
    {
        transform.position = pacTransBegin;
        timer = timeForMove;


    }

    // Update is called once per frame
    void Update()
    {
      timer -= Time.deltaTime;
      if (timer > 0) {
           Vector2 distance =  pacTransEnd - pacTransBegin;
                       float degreeOfMovement = (timeForMove - timer) / timeForMove;
                       transform.position = new Vector2 (pacTransBegin.x + (distance.x * degreeOfMovement), pacTransBegin.y + (distance.y * degreeOfMovement));
               }


    }
}
