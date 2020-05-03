using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreObject : MonoBehaviour
{
    public int score;
    public int oneUpScore;
    public Sprite[] numbers;
    public GameObject[] places;
    public int extraLife = 10000; // score to get new life
    public int maxscore = 2147483647; //max number that can be handled
    public bool recalc;

    char[] arrayOfNum;
    char[] arrayOfOneUp; //number array for levelUp score

    void Start()
    {
        numbers = Resources.LoadAll<Sprite>("Score/"); //load in sprites to numbers
        UpdateScore(score, arrayOfNum);
        UpdateScore(oneUpScore, arrayOfOneUp);
    }

    // Update is called once per frame
    void Update()
    {
        if(recalc){//this is probably where points will be added so score stays updated
            score = gameBoard.points; //gets points variable from gameBoard
            //score += 10015;
            oneUpScore = score % extraLife;
            UpdateScore(oneUpScore, arrayOfNum); //should be score but variables are switched with cooresponding sprites
            UpdateScore(score, arrayOfOneUp);
            //recalc = false;
        }
    }

    void UpdateScore(int score, char[] targetArray){
        targetArray = score.ToString().ToCharArray();

        if(score > maxscore){ //not score over max allowed
            score = maxscore;
            UpdateScore(score, targetArray);
        } else if(score < 0){ //no negative score allowed
            score = 0;
            UpdateScore(score, targetArray);

        } else {
            for(int i = 0; i < targetArray.Length; i++){
                int y = int.Parse(targetArray[i].ToString()); //break apart and update score with loop
                //Debug.Log("THIS IS Y " + y);
                places[i].SetActive(true); //turn on number place
                places[i].GetComponent<SpriteRenderer>().sprite = numbers[y]; //turns on correct sprite number
            }

            //Debug.Log("places.Length" + places.Length);
            //Debug.Log("oneUpPlaces.Length" + oneUpPlaces.Length);

            if(targetArray.Length < places.Length){//removes any nonused zeros
                for(int i = targetArray.Length; i < places.Length; i++){
                    places[i].SetActive(false); //turn off 0's
                }
            }
        }
    }
}
