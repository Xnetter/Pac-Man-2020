using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class frightTrans : MonoBehaviour
{
  protected Vector2 frightBegin = new Vector2(-17, 10);
  protected Vector2 frightEnd = new Vector2(25, 10);
  //protected Vector3 removeSprite = new Vector3(-14,10,0);
  public float timeForMove = 6.0f;
  //public float endTime = 0.0f;
  float timer;

  public float time = 0.0f;

  //public Vector3 StopPos =  GameObject.Find("New Pikel-26").transform.position;




  void Start()
  {

      transform.position = frightBegin;
      timer = timeForMove;

    }





  // Update is called once per frame
  void Update()
  {
    time += Time.deltaTime;
    if (time >= 4.0f){


    timer -= Time.deltaTime;
    if (timer > 0) {
         Vector2 distance =  frightBegin - frightEnd;
                     float degreeOfMovement = (timeForMove - timer) / timeForMove;
                     transform.position = new Vector2 (frightBegin.x - (distance.x * degreeOfMovement), frightBegin.y );
                   }
                 }



    }




}
