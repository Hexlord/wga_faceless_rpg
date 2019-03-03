using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/*
 * History:
 * 
 * Date         Author      Description
 * 
 * 03.03.2019   aknorre     Created
 * 
 */

public class IntroScript : MonoBehaviour
{

    [Tooltip("Time before switching scene")]
    public float timeBeforeStart = 5.0f;

    private float timer = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if(timer >= timeBeforeStart)
        {
            SceneManager.LoadScene("Game", LoadSceneMode.Single);
        }
    }
}
