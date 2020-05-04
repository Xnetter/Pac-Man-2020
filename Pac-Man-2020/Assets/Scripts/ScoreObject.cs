using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreObject : MonoBehaviour
{
    public int totalScore;
    public int oneUpScore;
    public Sprite[] numbers;
    public GameObject[] places;
    public GameObject[] oneUpPlaces;
    public int extraLife = 10000; // score to get new life
    private int almostLife = 5000;
    private int lifeUpLowRange = 0;
    private int lifeUpHighRange = 3000;
    public int maxscore = 2147483647; //max number that can be handled
    public bool recalc;
    public bool isScore;
    public bool isOneUp;
    public bool LifeUp;

    char[] arrayOfNum;
    char[] arrayOfOneUp; //number array for levelUp score

    void Start()
    {
        numbers = Resources.LoadAll<Sprite>("Score/"); //load in sprites to numbers
        UpdateScore(totalScore);
        UpdateOneUpScore(oneUpScore); //should be score but variables are switched with cooresponding sprites
    }

    // Update is called once per frame
    void Update()
    {
        if(recalc){//this is probably where points will be added so score stays updated
            totalScore = gameBoard.points; //gets points variable from gameBoard
            if(LifeUp && isOneUp)
            {
                oneUpScore = totalScore % extraLife;
                if((oneUpScore >= lifeUpLowRange && oneUpScore <= lifeUpHighRange) && gameBoard.LifeCount < gameBoard.maxLife) //score range between 0-3000 gives lifeUp
                {
                    gameBoard.LifeCount += 1;
                    LifeUp = false;
                    Debug.Log("LIFEUP (OFF)");
                }
            }
            oneUpScore = totalScore % extraLife;
            if(isScore)
            {
                UpdateScore(totalScore); //should be score but variables are switched with cooresponding sprites
            }
            if(isOneUp)
            {
                if(oneUpScore >= almostLife) //score needs to be over 5000 for lifeUp to be an option
                {
                    LifeUp = true;
                    Debug.Log("LIFEUP ON");
                }
                UpdateOneUpScore(oneUpScore);
            }
            //recalc = false;
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
            for(int i = 0; i < arrayOfNum.Length; i++){
                int y = int.Parse(arrayOfNum[i].ToString()); //break apart and update score with loop
                //Debug.Log("THIS IS Y " + y);
                places[i].SetActive(true); //turn on number place
                places[i].GetComponent<SpriteRenderer>().sprite = numbers[y]; //turns on correct sprite number
            }

            //Debug.Log("places.Length" + places.Length);
            //Debug.Log("oneUpPlaces.Length" + oneUpPlaces.Length);

            if(arrayOfNum.Length < places.Length){//removes any nonused zeros
                for(int i = arrayOfNum.Length; i < places.Length; i++){
                    places[i].SetActive(false); //turn off 0's
                }
            }
        }
    }

    void UpdateOneUpScore(int score){
        arrayOfOneUp = score.ToString().ToCharArray();

        if(score > maxscore){ //not score over max allowed
            score = maxscore;
            UpdateOneUpScore(score);
        } else if(score < 0){ //no negative score allowed
            score = 0;
            UpdateOneUpScore(score);    

        } else {
            for(int i = 0; i < arrayOfOneUp.Length; i++){
                int y = int.Parse(arrayOfOneUp[i].ToString()); //break apart and update score with loop
                //Debug.Log("THIS IS Y " + y);
                oneUpPlaces[i].SetActive(true); //turn on number place
                oneUpPlaces[i].GetComponent<SpriteRenderer>().sprite = numbers[y]; //turns on correct sprite number 
            }

            //Debug.Log("places.Length" + places.Length);
            //Debug.Log("oneUpPlaces.Length" + oneUpPlaces.Length);

            if(arrayOfOneUp.Length < oneUpPlaces.Length){//removes any nonused zeros
                for(int i = arrayOfOneUp.Length; i < oneUpPlaces.Length; i++){
                    oneUpPlaces[i].SetActive(false); //turn off 0's
                }
            }
        }
    }
}
