using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 * History:
 * 
 * Date         Author      Description
 * 
 * 03.03.2019   aknorre     Created
 * 
 */

public class BasicStatusSystem : MonoBehaviour
{
    [Header ("Health Settings")]
    [Tooltip ("The amount of Health Points the object has.")]
    [SerializeField]
    protected float healthPoints = 100.0f;

    [Tooltip("The UI element which shows an amount of HP that object has.")]
    [SerializeField]
    protected Image Healthbar;

    //Private

    Vector3 originalPosition, originalScale;
    Quaternion originalRotation;

    protected float originalAmountOfHP;


    public float HP
    {
        get
        {
            return healthPoints;
        }
    }

    public virtual void DealDamage(float amount)
    {
        healthPoints -= amount;
        Debug.Log("Dealt");
        if (healthPoints <= 0.0f)
        {
            OnDeath();
        }

        Healthbar.fillAmount = healthPoints / originalAmountOfHP;
    }

    public virtual void OnDeath()
    {
        Debug.Log("Die");
    }

    public void RestoreHealthPoints(float amount)
    {
        healthPoints += amount;
        Healthbar.fillAmount = healthPoints / originalAmountOfHP;
    }

    public void Respawn()
    {
        transform.position = originalPosition;
        transform.rotation = originalRotation;
        transform.localScale = originalScale;
        healthPoints = originalAmountOfHP;
        Healthbar.fillAmount = 1;
    }

    // Use this for initialization
    protected void Start()
    {
        originalAmountOfHP = healthPoints;
        originalPosition = transform.position;
        originalRotation = transform.rotation;
        originalScale = transform.localScale;
        Healthbar.fillAmount = 1;
    }
    
}
