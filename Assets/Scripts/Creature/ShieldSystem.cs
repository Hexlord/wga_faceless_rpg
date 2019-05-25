using UnityEngine;

[AddComponentMenu("ProjectFaceless/Creature/Shield System")]
public class ShieldSystem : MonoBehaviour
{
    public GameObject shield;
    [Header("Basic Settings")]

    [Tooltip("Maximum HP the shield can have")]
    public float maxShieldHP = 400.0f;

    [Tooltip("How many HP the shield can regenerate per second")]
    public float shieldHPRegen = 50.0f;

    [Tooltip("Charge count")]
    public int chargeCount = 5;
    
    [Header("Animation Settings")]
    [Tooltip("Animation played when the creature raised his shield")]
    public string defenseStanceAnimation = "Defend";
    [Tooltip("Boolean that plays the animation when the creature raised his shield")]
    public string defenseStanceAnimationBool = "DefendBool";

    //private
    private float shieldHP = 400.0f;
    private bool isRaised = false;
    private float shieldHPPerCharge;
    //cache
    private Animator animator;
    // Start is called before the first frame update

    void Awake()
    {
        shieldHPPerCharge = maxShieldHP / chargeCount;
        shieldHP = maxShieldHP;
        if(shield) shield.SetActive(isRaised);
        animator = GetComponent<Animator>();
    }
    
    //properties
    public int GetFullShieldCharges()
    {
        return Mathf.Max(0, Mathf.CeilToInt(shieldHP / shieldHPPerCharge - 0.001f));
    }

    public float GetRemainingHPInCharge()
    {
        if (shieldHP == maxShieldHP) return shieldHPPerCharge;
        if (shieldHP <= 0) return 0.0f;
        return shieldHP % shieldHPPerCharge;
    }

    public void RecieveDamage(float amount)
    {
        shieldHP -= amount;
        Debug.Log(shieldHP + " SHP");
        if (shieldHP <= 0) ShieldBreak();
    }

    public bool CanShield
    {
        get { return (shieldHP > 0) && !isRaised; }
    }

    public bool IsRaised
    {
        get { return isRaised; }
    }

    public void RaiseShield()
    {

        shield.SetActive(true);
        isRaised = true;
        //TO DO: Make animations
    }

    public void LowerShield()
    {
        shield.SetActive(false);
        isRaised = false;
        //TO DO: Make animations
    }

    //TO DO: Shield breaks if all Shield HP are spent.
    public void ShieldBreak()
    {
        LowerShield();
        //TO DO: Make animations
    }

    protected void Update()
    {
        if (!isRaised && shieldHP < maxShieldHP)
        {
            RegenerateShieldHP(Time.deltaTime);
        }
    }

    public void RestoreShieldHP(float amount)
    {
        Debug.Assert(amount >= 0.0f);
        if (shieldHP < 0) shieldHP = 0;

        if (shieldHP < maxShieldHP) shieldHP += amount;
        if (shieldHP > maxShieldHP) shieldHP = maxShieldHP;
    }

    public void RegenerateShieldHP(float deltaTime)
    {
        if (shieldHP < 0) shieldHP = 0;

        if (shieldHP < maxShieldHP) shieldHP += shieldHPRegen * deltaTime;

        if (shieldHP > maxShieldHP) shieldHP = maxShieldHP;
    }
}
