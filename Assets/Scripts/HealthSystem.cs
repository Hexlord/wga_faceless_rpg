using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthSystem : MonoBehaviour {
    [SerializeField]
    private float healthPoints = 100.0f;

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
    }

    public void RestoreHealthPoints(float amount)
    {
        healthPoints += amount;
    }

	// Use this for initialization
	void Start () {
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
