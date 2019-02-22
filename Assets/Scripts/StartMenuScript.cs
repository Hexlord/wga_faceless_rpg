using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StartMenuScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GameObject.Find("Start").GetComponent<Button>().onClick.AddListener(PressedStart);
        GameObject.Find("Exit").GetComponent<Button>().onClick.AddListener(PressedExit);
    }
    
    void PressedStart()
    {
        SceneManager.LoadScene("Intro", LoadSceneMode.Single);
    }
    void PressedExit()
    {
        Application.Quit();
    }
}
