using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthSystem : MonoBehaviour {
    [SerializeField]
    protected float healthPoints = 100.0f;
    [SerializeField]
    protected bool respawnAfterDeath = true;

    [SerializeField]
    protected Image Healthbar; 

    Vector3 originalPosition;
    Quaternion originalRotation;
    Vector3 originalScale;
    protected float originalAmountOfHP;


    public float HP
    {
        get
        {
            return healthPoints;
        }
    }

    public void DealDamage(float amount)
    {
        healthPoints -= amount;
        Debug.Log("Dealt");
        if(healthPoints <= 0.0f)
        {
            Debug.Log("Die");
            healthPoints = 100.0f;
            gameObject.GetComponent<BaseCharacter>().Die();
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
