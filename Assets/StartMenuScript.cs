using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StartMenuScript : MonoBehaviour
{
    // Start is called before the first frame update

    [Header("Main Menu Settings")]

    [Tooltip("Select main menu node")]
    public GameObject mainMenu;

    [Tooltip("Select main menu start button")]
    public GameObject mainMenuStart;
    [Tooltip("Select main menu settings button")]
    public GameObject mainMenuSettings;
    [Tooltip("Select main menu exit button")]
    public GameObject mainMenuExit;


    [Header("Settings Menu Settings")]

    [Tooltip("Select settings menu node")]
    public GameObject settingsMenu;

    [Tooltip("Select settings menu back button")]
    public GameObject settingsMenuBack;


    void Start()
    {
        mainMenuStart.GetComponent<Button>().onClick.AddListener(PressedStart);
        mainMenuSettings.GetComponent<Button>().onClick.AddListener(PressedSettings);
        mainMenuExit.GetComponent<Button>().onClick.AddListener(PressedExit);

        settingsMenuBack.GetComponent<Button>().onClick.AddListener(PressedBack);
    }
    
    void PressedStart()
    {
        SceneManager.LoadScene("Intro", LoadSceneMode.Single);
    }

    void PressedSettings()
    {
        mainMenu.SetActive(false);
        settingsMenu.SetActive(true);
    }
    void PressedBack()
    {
        settingsMenu.SetActive(false);
        mainMenu.SetActive(true);
    }

    void PressedExit()
    {
        Application.Quit();
    }
}
