using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class bigPacTrans : MonoBehaviour
{
  protected Vector2 bigPacBegin = new Vector2(-22, 10);
  protected Vector2 bigPacEnd = new Vector2(25, 10);
  //protected Vector3 removeSprite = new Vector3(-14,10,0);
  public float timeForMove = 6.0f;
  //public float endTime = 0.0f;
  float timer;
  public float time = 0.0f;
  public int level = 1;


  void Start()
  {

      level ++;
      transform.position = bigPacBegin;
      timer = timeForMove;

    }

    public void NextLevel(){
      if (time >= 10.2f){
        SceneManager.LoadScene("MazeBricks");


      }
    }

  // Update is called once per frame
  void Update()
  {
    time += Time.deltaTime;


    if (time >= 5f){


    timer -= Time.deltaTime;

    if (timer > 0) {
         Vector2 distance =  bigPacBegin - bigPacEnd;
                     float degreeOfMovement = (timeForMove - timer) / timeForMove;
                     transform.position = new Vector2 (bigPacBegin.x - (distance.x * degreeOfMovement), bigPacBegin.y );
                   }
                   NextLevel();

                 }
}

}
