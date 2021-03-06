﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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
[AddComponentMenu("ProjectFaceless/UI/DeathUI System")]
public class DeathUISystem : MonoBehaviour
{
    
    // Start is called before the first frame update
    void Awake()
    {
        GameObject.Find("ToMainMenuButton").GetComponent<Button>().onClick.AddListener(ToMainMenu);
        GameObject.Find("RestartButton").GetComponent<Button>().onClick.AddListener(Restart);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void ToMainMenu()
    {
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
    }
    void Restart()
    {
        SceneManager.LoadScene("GameSasha", LoadSceneMode.Single);
    }
}
