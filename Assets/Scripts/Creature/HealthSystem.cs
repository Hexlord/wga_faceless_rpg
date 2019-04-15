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
 * 25.03.2019   bkrylov     Added worldspace healthbar for mobs and targets. Added object destruction is HP == 0. Testing purposes only.
 * 
 */
[AddComponentMenu("ProjectFaceless/Creature/Health System")]
public class HealthSystem : MonoBehaviour
{

    // Public

    public static float deathVerticalThreshold = -20.0f;

    [Header("Basic Settings")]
    [Tooltip("The amount of health the object has")]
    [Range(0.0f, 10000.0f, order = 2)]
    public float healthMaximum = 100.0f;


    [Tooltip("The amount of xp this object grants to killer")]
    [Range(0.0f, 10000.0f, order = 2)]
    public float xpReward = 10.0f;

    [Header("UI Settings")]
    [Tooltip("Health bar anchor offset from transform origin")]
    public Vector3 uiAnchorOffset = new Vector3(0.0f, 3.0f, 0.0f);
    
    [Tooltip("Health bar enabled")]
    public bool uiHealthBarEnabled = true;

    public Image worldSpaceHealthBar;

    public bool Alive
    {
        get { return health > 0; }
    }

    public bool Dead
    {
        get { return !Alive; }
    }

    public float Health
    {
        get
        {
            return health;
        }
        set
        {
            value = Mathf.Clamp(value, 0.0f, healthMaximum);
            health = value;

            if(healthBar) healthBar.fillAmount = health / healthMaximum;
        }
    }

    // Private

    private float health = 0.0f;

    // Cache

    private GameObject healthPrefab;
    private Image healthBar;

    public void Kill(GameObject source)
    {
        Damage(source, health);
    }

    public void Damage(GameObject source, float amount)
    {
        Health -= amount;

        OnDamage(source, amount);
        if (Health <= 0.0f) OnDeath(source);
    }

    public void Heal(GameObject source, float amount)
    {
        Health += amount;

        OnHeal(source, amount);
    }

    protected virtual void OnDamage(GameObject source, float amount)
    {
        if(source)
        {
            ConcentrationSystem concentrationSystem =
                source.GetComponent<ConcentrationSystem>();
            if (concentrationSystem)
            {
                concentrationSystem.Restore(amount * concentrationSystem.concentrationVampirism);
            }
        }
    }

    protected virtual void OnDeath(GameObject source)
    {
        if (source && source != gameObject)
        {
            XpSystem xpSystem =
                source.GetComponent<XpSystem>();
            if (xpSystem)
            {
                xpSystem.GrantXp(xpReward);
            }
        }

        //Added for testing purposes
        if (source && source.tag == "Player" &&
            gameObject != source)
        {
            Destroy(gameObject);
        }
    }

    protected virtual void OnHeal(GameObject source, float amount)
    {
        // Intentionally left empty
    }

    protected virtual void Start()
    {
        if (uiHealthBarEnabled)
        {
            healthPrefab = (GameObject)Resources.Load("Prefabs/UI/Creature/HealthBar", typeof(GameObject));
            GameObject healthObject =
                Instantiate(healthPrefab,
                    Vector3.zero,
                    Quaternion.identity,
                    transform);
            healthBar = healthObject.transform.Find("HealthBar").GetComponent<Image>();
            healthObject.transform.localPosition = uiAnchorOffset;
        }
        else
        {
            healthBar = worldSpaceHealthBar;
        }

        Health = healthMaximum;


    }

    protected void FixedUpdate()
    {
        if (gameObject.transform.position.y < deathVerticalThreshold) Kill(gameObject);
    }

}
