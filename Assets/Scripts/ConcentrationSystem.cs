using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConcentrationSystem : HealthSystem
{
    [SerializeField]
    private float maxConcentration = 100.0f;
    [SerializeField]
    private Image ConcentrationBar;
    [SerializeField]
    private float exchangeRate = 1.0f;
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

    // Use this for initialization
    void Start()
    {
        originalAmountOfHP = healthPoints;
        currentAmountOfConcentration = 0;
        Healthbar.fillAmount = 1;
        ConcentrationBar.fillAmount = 0;
    }
}
