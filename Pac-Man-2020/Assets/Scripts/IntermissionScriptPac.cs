using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntermissionScriptPac : MonoBehaviour
{
  protected Vector2 pacTransBegin = new Vector2(22, 10);
  protected Vector2 pacTransEnd = new Vector2(-24, 10);

  public float timeForMove = 6.0f;
  //public float endTime = 0.0f;
  float timer;
  float time = 0.0f;



  void Start()
  {

      transform.position = pacTransBegin;
      timer = timeForMove;

    }





  // Update is called once per frame
  void Update()
  {

    time += Time.deltaTime;




    timer -= Time.deltaTime;
    if (timer > 0) {
         Vector2 distance =  pacTransEnd - pacTransBegin;
                     float degreeOfMovement = (timeForMove - timer) / timeForMove;
                     transform.position = new Vector2 (pacTransBegin.x + (distance.x * degreeOfMovement), pacTransBegin.y );
                   }





    }




}
