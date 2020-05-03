using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Levels : MonoBehaviour
{
  public int level =1;
  void Start(){

  }


  void Update (){
    if (gameBoard.isDead){
      level = 1;
    }

    else if (gameBoard.nextLev){
      level ++;
     //GameObject.GetComponent<PacManController>().cruisePellets -= 20;

      }


  else if (level == 3){
    level = 1;

    // load menu
    //SceneManager.LoadScene("Winner");
  }

// private void firstLev
// private void nextLev
// private void Winner

}
}
