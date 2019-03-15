using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 * History:
 * 
 * Date         Author      Description
 * 
 * 15.03.2019   aknorre     Created
 * 
 */

public class PlayerStatusSystem : MonoBehaviour
{

    // Public

    [Header("UI Settings")]

    [Tooltip("Health bar")]
    public Image healthBar;

    [Tooltip("Concentration bar")]
    public Image concentrationBar;

    // Private

    private HealthSystem health;
    private ConcentrationSystem concentration;

    protected virtual void Start()
    {
        health = GetComponent<HealthSystem>();
        concentration = GetComponent<ConcentrationSystem>();
    }
    protected virtual void Update()
    {
        healthBar.fillAmount = health.Health / health.healthMaximum;
        concentrationBar.fillAmount = concentration.Concentration / concentration.concentrationMaximum;
    }

}
