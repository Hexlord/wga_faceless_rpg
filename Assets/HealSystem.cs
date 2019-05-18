using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealSystem : MonoBehaviour
{
    [Tooltip("Cost per health point")]
    public float concentrationPerHealthPoint = 1.0f;

    [Tooltip("Health point per second regeneration speed")]
    public float regenSpeed = 10.0f;

    private HealthSystem healthSystem;
    private ConcentrationSystem concentrationSystem;

    private void Awake()
    {
        healthSystem = GetComponent<HealthSystem>();
        concentrationSystem = GetComponent<ConcentrationSystem>();

    }

    private void FixedUpdate()
    {
        var delta = Time.fixedDeltaTime;

        if (!InputManager.Down(InputAction.Heal)) return;
            
        var maxRestore = healthSystem.healthMaximum - healthSystem.Health;
        maxRestore = Mathf.Min(maxRestore, regenSpeed * delta);
        var maxCost = concentrationPerHealthPoint * maxRestore;

        if (maxCost > float.Epsilon && concentrationSystem.Concentration > float.Epsilon)
        {
            var cost = Mathf.Min(maxCost, concentrationSystem.Concentration);
            concentrationSystem.Concentration -= cost;
            healthSystem.Heal(gameObject, maxRestore * cost / maxCost);
        }

    }
}
