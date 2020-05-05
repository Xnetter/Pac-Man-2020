using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class BonusItem : MonoBehaviour
{
    float randomLifeExpectancy;
	float currentLifeTime;

	// Use this for initialization
	void Start () 
		{
		randomLifeExpectancy = Random.Range(9, 10);
		this.name = "bonusItem";
		GameObject.Find("Game").GetComponent<gameBoard>().board[10, 8] = this.gameObject;
		}
	
	// Update is called once per frame
	void Update () 
		{
		if (currentLifeTime < randomLifeExpectancy) 
			{
			currentLifeTime += Time.deltaTime;
			} else {
			Destroy (this.gameObject);
			}
		}


   /* public Sprite[] BonusItems;
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
*/

}
