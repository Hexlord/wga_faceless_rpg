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
[AddComponentMenu("ProjectFaceless/UI/DeathUI System")]
public class DeathUISystem : MonoBehaviour
{
    SaveSystem saveSystem;
    // Start is called before the first frame update
    void Awake()
    {
        GameObject.Find("ToMainMenuButton").GetComponent<Button>().onClick.AddListener(ToMainMenu);
        GameObject.Find("RestartButton").GetComponent<Button>().onClick.AddListener(Restart);
        GameObject.Find("LoadButton").GetComponent<Button>().onClick.AddListener(Load);
        saveSystem = GameObject.Find("SaveSystem").GetComponent<SaveSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void ToMainMenu()
    {
        saveSystem.isLoading = false;
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
    }
    void Restart()
    {
        saveSystem.isLoading = false;
        SceneManager.LoadScene("GameSasha", LoadSceneMode.Single);
    }

    void Load()
    {
        saveSystem.isLoading = true;
        SceneManager.LoadScene("GameSasha", LoadSceneMode.Single);
    }
}
