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

public class ConcentrationSystem : MonoBehaviour
{
    [AddComponentMenu("ProjectFaceless/Creature")]
    // Public

    [Header("Basic Settings")]
    [Tooltip("The amount of concentration the object has")]
    [Range(0.0f, 10000.0f, order = 2)]
    public float concentrationMaximum = 100.0f;
    
    [Tooltip("The amount of concentration received relative to damage done")]
    [Range(0.0f, 1.0f)]
    public float concentrationVampirism = 0.2f;

    public float Concentration
    {
        get
        {
            return concentration;
        }
        set
        {
            value = Mathf.Clamp(value, 0.0f, concentrationMaximum);
            concentration = value;
        }
    }

    // Private

    private float concentration = 0.0f;

    public bool Has(float amount)
    {
        return Concentration >= amount;
    }

    public void Use(float amount)
    {
        Concentration -= amount;
    }
    public void Restore(float amount)
    {
        Concentration += amount;
    }
    
    protected virtual void Start()
    {
        Concentration = concentrationMaximum;
    }

}
