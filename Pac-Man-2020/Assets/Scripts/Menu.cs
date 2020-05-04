using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    SpriteRenderer playButton;
   
    void Start()
    {
        playButton = GetComponent<SpriteRenderer>();
    }
    
    void Update()
    {
        if(Input.GetKey(KeyCode.Return))
        {            
            // Change the 'color' property of the 'Sprite Renderer'
            playButton.color = new Color (1, 0, 0, 1);
            SceneManager.LoadScene("MazeBricks");
        } else {
            playButton.color = new Color (1, 1, 1, 1);
        }
    }
}
