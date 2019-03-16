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

public class EndScreenScript : MonoBehaviour
{
    [AddComponentMenu("ProjectFaceless/UI")]
    // Start is called before the first frame update
    void Start()
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
        SceneManager.LoadScene("Game", LoadSceneMode.Single);
    }
}
