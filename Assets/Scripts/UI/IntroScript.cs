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
 * 16.03.2019   bkrylov     Allocated to Component Menu
 * 
 */
[AddComponentMenu("ProjectFaceless/UI/Intro Script")]
public class IntroScript : MonoBehaviour
{
    
    [Tooltip("Time before switching scene")]
    public float timeBeforeStart = 3.0f;

    private float timer = 0.0f;

    void Update()
    {
        timer += Time.deltaTime;
        if(timer >= timeBeforeStart)
        {
            SceneManager.LoadScene("GameSasha", LoadSceneMode.Single);
        }
    }
}
