using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BonusItem : MonoBehaviour

{

    public Sprite[] BonusItems;
    public GameObject[] indices;
    float randomLifeExpectancy;
    float currentLifeTime;


    // Start is called before the first frame update
    void Start()
    {
        BonusItems = Resources.LoadAll<Sprite>("bonus/"); //load in sprites to numbers
    }

    // Update is called once per frame
    void Update()
    {
        
        
    }
}
