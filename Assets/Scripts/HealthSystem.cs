using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthSystem : MonoBehaviour {

    //Public

    [Header ("Health Settings")]
    
    [Tooltip ("The amount of Health Points the object has.")]
    [SerializeField]
    protected float healthPoints = 100.0f;

    [Tooltip("The object respawn after death.")]
    [SerializeField]
    protected bool respawnAfterDeath = true;

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

        if(healthPoints <= 0.0f)
        {
            
            if(respawnAfterDeath)
            {
                healthPoints = 100.0f;
                gameObject.GetComponent<BaseCharacter>().ResetPosition();
            }
        }

        Healthbar.fillAmount = healthPoints / originalAmountOfHP;
    }

    public void RestoreHealthPoints(float amount)
    {
        healthPoints += amount;
        Healthbar.fillAmount = healthPoints / originalAmountOfHP;
    }

    void Respawn()
    {
        transform.position = originalPosition;
        transform.rotation = originalRotation;
        transform.localScale = originalScale;
        healthPoints = originalAmountOfHP;
        Healthbar.fillAmount = 1;
    }

	// Use this for initialization
	void Start ()
    {
        originalAmountOfHP = healthPoints;
        originalPosition = transform.position;
        originalRotation = transform.rotation;
        originalScale = transform.localScale;
        Healthbar.fillAmount = 1;
    }
	
	// Update is called once per frame
	void Update ()
    {
	    if (healthPoints <= 0)
        {
            if (respawnAfterDeath)
            {
                Respawn();
            }
            else
            {
                Destroy(gameObject);
            }
        }
	}
}
