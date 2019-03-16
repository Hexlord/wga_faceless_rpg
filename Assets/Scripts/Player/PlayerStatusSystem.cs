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
 * 16.03.2019   bkrylov     Allocated to Component Menu
 * 
 */

public class PlayerStatusSystem : MonoBehaviour
{
    [AddComponentMenu("ProjectFaceless/Player")]
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
        if(healthBar) healthBar.fillAmount = health.Health / health.healthMaximum;
        if(concentrationBar) concentrationBar.fillAmount = concentration.Concentration / concentration.concentrationMaximum;
    }

}
