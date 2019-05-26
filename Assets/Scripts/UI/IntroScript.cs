using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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

    [Tooltip("Start button")]
    public Button buttonStart;

    private void Awake()
    {
        buttonStart.onClick.AddListener(() => { SceneManager.LoadScene("GameSasha", LoadSceneMode.Single); });
    }
}
