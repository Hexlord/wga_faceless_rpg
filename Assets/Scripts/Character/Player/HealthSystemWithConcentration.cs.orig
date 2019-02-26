using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
            if (respawnAfterDeath)
            {
                healthPoints = 100.0f;
                gameObject.GetComponent<BaseCharacter>().ResetPosition();
                currentAmountOfConcentration = 0.0f;
                ConcentrationBar.fillAmount = 0.0f;
            }
        }

        Healthbar.fillAmount = healthPoints / originalAmountOfHP;
    }

    // Use this for initialization
    void Start()
    {
        originalAmountOfHP = healthPoints;
        currentAmountOfConcentration = 0;

        Healthbar.fillAmount = 1;
        ConcentrationBar.fillAmount = 0;
    }
}
