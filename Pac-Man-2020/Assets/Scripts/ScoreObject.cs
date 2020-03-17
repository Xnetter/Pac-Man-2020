using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreObject : MonoBehaviour
{
    public int score;
    public Sprite[] numbers;
    public GameObject[] places;
    public int maxscore = 2147483647; //max number that can be handled
    public bool recalc;

    char[] arrayOfNum;
    void Start()
    {
        numbers = Resources.LoadAll<Sprite>("Score/"); //load in sprites to numbers 
        UpdateScore(score);
    }

    // Update is called once per frame
    void Update()
    {
        if(recalc){//this is probably where points will be added so score stays updated
            UpdateScore(score);
            recalc = false;
        }
    }

    void UpdateScore(int score){
        arrayOfNum = score.ToString().ToCharArray();

        if(score > maxscore){ //not score over max allowed
            score = maxscore;
            UpdateScore(score);
        } else if(score < 0){ //no negative score allowed
            score = 0;
            UpdateScore(score);
        } else {
            Debug.Log("Hitting Score WIth " + score);
            for(int i = 0; i < arrayOfNum.Length; i++){
                int y = int.Parse(arrayOfNum[i].ToString()); //break apart and update score with loop
                places[i].SetActive(true); //turn on number place
                places[i].GetComponent<SpriteRenderer>().sprite = numbers[y]; //turns on correct sprite number 
            }

            //print("places.Length" + places.Length);

            if(arrayOfNum.Length < places.Length){//removes any nonused zeros
                for(int i = arrayOfNum.Length; i < places.Length; i++){
                    places[i].SetActive(false); //turn off 0's
                }
            }
        }
    }
}
