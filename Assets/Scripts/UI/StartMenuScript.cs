   
using System.Collections;
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
[AddComponentMenu("ProjectFaceless/UI/Start Menu")]
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
    [Tooltip("Select main menu save button")]
    public GameObject mainMenuSave;
    [Tooltip("Select main menu load button")]
    public GameObject mainMenuLoad;
    [Tooltip("Select main menu exit button")]
    public GameObject mainMenuExit;

    [Tooltip("Select other menu background")]
    public GameObject otherMenuBackground;

    [Header("Settings Menu Settings")]

    [Tooltip("Select settings menu node")]
    public GameObject settingsMenu;

    [Tooltip("Select settings menu back button")]
    public GameObject settingsMenuBack;

    [Header("Save Menu Settings")]

    [Tooltip("Select save menu node")]
    public GameObject saveMenu;

    [Tooltip("Select save menu back button")]
    public GameObject saveMenuBack;

    [Header("Load Menu Settings")]

    [Tooltip("Select load menu node")]
    public GameObject loadMenu;

    [Tooltip("Select load menu back button")]
    public GameObject loadMenuBack;



    [Tooltip("Check to hide main menu when in settings menu")]
    public bool settingsMenuHideMainMenu = false;

    void Awake()
    {
        mainMenuStart.GetComponent<Button>().onClick.AddListener(PressedStart);
        mainMenuSettings.GetComponent<Button>().onClick.AddListener(PressedSettings);
        mainMenuSave.GetComponent<Button>().onClick.AddListener(PressedSave);
        mainMenuLoad.GetComponent<Button>().onClick.AddListener(PressedLoad);
        mainMenuExit.GetComponent<Button>().onClick.AddListener(PressedExit);

        settingsMenuBack.GetComponent<Button>().onClick.AddListener(PressedBack);
        saveMenuBack.GetComponent<Button>().onClick.AddListener(PressedBack);
        loadMenuBack.GetComponent<Button>().onClick.AddListener(PressedBack);

        mainMenuBackground.SetActive(true);
        otherMenuBackground.SetActive(false);
    }

    void PressedStart()
    {
        SceneManager.LoadScene("Intro", LoadSceneMode.Single);
    }

    void PressedSettings()
    {
        PressedBack();
        if (settingsMenuHideMainMenu) mainMenu.SetActive(false);
        settingsMenu.SetActive(true);
        otherMenuBackground.SetActive(true);
    }

    void PressedSave()
    {
        PressedBack();
        if (settingsMenuHideMainMenu) mainMenu.SetActive(false);
        saveMenu.SetActive(true);
        otherMenuBackground.SetActive(true);
    }

    void PressedLoad()
    {
        PressedBack();
        if (settingsMenuHideMainMenu) mainMenu.SetActive(false);
        loadMenu.SetActive(true);
        otherMenuBackground.SetActive(true);
    }
    void PressedBack()
    {
        settingsMenu.SetActive(false);
        loadMenu.SetActive(false);
        saveMenu.SetActive(false);
        if (settingsMenuHideMainMenu) mainMenu.SetActive(true);
        otherMenuBackground.SetActive(false);
    }

    void PressedExit()
    {
        Application.Quit();
    }
}