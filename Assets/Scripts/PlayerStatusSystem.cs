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
 * 
 */

public class PlayerStatusSystem : BasicStatusSystem
{
    [SerializeField]
    private float maxConcentration = 100.0f;
    [SerializeField]
    private Image ConcentrationBar;
    [SerializeField]
    private float exchangeRate = 1.0f;

    public GameObject deathScreen = null;

    float currentAmountOfConcentration;

    public void SpendConcentration(float time)
    {
        if (currentAmountOfConcentration > 0)
        { 
            currentAmountOfConcentration -= time;
            RestoreHealthPoints(time * exchangeRate);
            ConcentrationBar.fillAmount = currentAmountOfConcentration / maxConcentration;
        }
    }

    public void StoreConcentration(float amount)
    {
        if (currentAmountOfConcentration < maxConcentration)
        {
            currentAmountOfConcentration += (currentAmountOfConcentration + amount <= maxConcentration) ? amount : (maxConcentration - currentAmountOfConcentration);
            ConcentrationBar.fillAmount = currentAmountOfConcentration / maxConcentration;
        }
    }

    public override void OnDeath()
    {
        base.OnDeath();

        Debug.Log("Player died");
        deathScreen.SetActive(true);

        GetComponent<SmartController>().enabled = false;
        GetComponent<Control>().enabled = false;
    }

    void Update()
    {
        if (Input.GetKey("escape"))
        {
            ToMainMenu();
        }
    }

    // Use this for initialization
    new void Start()
    {
        base.Start();

        deathScreen.SetActive(true);
        GameObject.Find("ToMainMenuButton").GetComponent<Button>().onClick.AddListener(ToMainMenu);
        GameObject.Find("RestartButton").GetComponent<Button>().onClick.AddListener(Restart);
        deathScreen.SetActive(false);

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
