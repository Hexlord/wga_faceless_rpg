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
    
    // Start is called before the first frame update
    void Awake()
    {
        var buttonsDeath = transform.FindPrecise("Background", false).FindPrecise("ButtonsDeath");
        buttonsDeath.FindPrecise("ToMainMenuButton").GetComponent<Button>().onClick.AddListener(ToMainMenu);
    }

    void ToMainMenu()
    {
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
    }
}
