using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HealthSystemWithConcentration : HealthSystem
{

    // Handles player characted health and concentration logic

    [Header ("Concentration settings")]

    [Tooltip ("The amount of concentration the object can have.")]
    [SerializeField]
    private float maxConcentration = 100.0f;

    [Tooltip("The amount of concentration the object currently has.")]
    [SerializeField]
    private float currentAmountOfConcentration;

    [Tooltip("The UI element that shows how much concentration the object has.")]
    [SerializeField]
    private Image ConcentrationBar;

    [Tooltip("How much HP one concentration point restores.")]
    [SerializeField]
    private float exchangeRate = 10.0f;

    [Tooltip("How fast healing will restore the object's HP.")]
    [SerializeField]
    private float restorationRate = 1.0f;

    public GameObject deathScreen = null;

    public void SpendConcentration(float time)
    {
        if (currentAmountOfConcentration > 0)
        {
            float amount = time * restorationRate;
            currentAmountOfConcentration -= amount;

            RestoreHealthPoints(amount * exchangeRate);

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

    public override void DealDamage(float amount)
    {
        healthPoints -= amount;

        if (healthPoints <= 0.0f)
        {
            OnDeath();
        }

        Healthbar.fillAmount = healthPoints / originalAmountOfHP;
    }

    // Use this for initialization
    new void Start()
    {
        originalAmountOfHP = healthPoints;
        currentAmountOfConcentration = 0;

        Healthbar.fillAmount = 1;
        ConcentrationBar.fillAmount = 0;

        deathScreen.SetActive(true);
        GameObject.Find("ToMainMenuButton").GetComponent<Button>().onClick.AddListener(ToMainMenu);
        GameObject.Find("RestartButton").GetComponent<Button>().onClick.AddListener(Restart);
        deathScreen.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKey("escape"))
        {
            ToMainMenu();
        }
    }
    protected override void OnDeath()
    {
        //base.OnDeath();

        Debug.Log("Player died");
        deathScreen.SetActive(true);

        GetComponent<SmartController>().enabled = false;
        GetComponent<Control>().enabled = false;
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
