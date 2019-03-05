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
 * 
 */

public class StartMenuScript : MonoBehaviour
{
    // Start is called before the first frame update

    [Header("Main Menu Settings")]

    [Tooltip("Select main menu node")]
    public GameObject mainMenu;

    [Tooltip("Select main menu background")]
    public GameObject mainMenuBackground;

    [Tooltip("Select main menu start button")]
    public GameObject mainMenuStart;
    [Tooltip("Select main menu settings button")]
    public GameObject mainMenuSettings;
    [Tooltip("Select main menu exit button")]
    public GameObject mainMenuExit;


    [Header("Settings Menu Settings")]

    [Tooltip("Select settings menu node")]
    public GameObject settingsMenu;

    [Tooltip("Select settings menu background")]
    public GameObject settingsMenuBackground;

    [Tooltip("Select settings menu back button")]
    public GameObject settingsMenuBack;


    [Tooltip("Check to hide main menu when in settings menu")]
    public bool settingsMenuHideMainMenu = false;

    void Start()
    {
        mainMenuStart.GetComponent<Button>().onClick.AddListener(PressedStart);
        mainMenuSettings.GetComponent<Button>().onClick.AddListener(PressedSettings);
        mainMenuExit.GetComponent<Button>().onClick.AddListener(PressedExit);

        settingsMenuBack.GetComponent<Button>().onClick.AddListener(PressedBack);

        mainMenuBackground.SetActive(true);
        settingsMenuBackground.SetActive(true);
        settingsMenuBackground.SetActive(false);
    }
    
    void PressedStart()
    {
        SceneManager.LoadScene("Intro", LoadSceneMode.Single);
    }

    void PressedSettings()
    {
        if(settingsMenuHideMainMenu) mainMenu.SetActive(false);
        settingsMenu.SetActive(true);
        settingsMenuBackground.SetActive(true);
    }
    void PressedBack()
    {
        settingsMenu.SetActive(false);
        if(settingsMenuHideMainMenu) mainMenu.SetActive(true);
        settingsMenuBackground.SetActive(false);
    }

    void PressedExit()
    {
        Application.Quit();
    }
}
