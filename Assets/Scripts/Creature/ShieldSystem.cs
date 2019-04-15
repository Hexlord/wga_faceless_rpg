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
    [Tooltip("How many HP the shield has in one charge")]
    public float shieldHPPerCharge = 100.0f;

    [Header("Animation Settings")]
    [Tooltip("Animation played when the creature raised his shield")]
    public string defenseStanceAnimation = "Defend";
    [Tooltip("Boolean that plays the animation when the creature raised his shield")]
    public string defenseStanceAnimationBool = "DefendBool";

    //private
    private float shieldHP = 400.0f;
    private bool isRaised = false;
    //cache
    private Animator animator;
    // Start is called before the first frame update

    void Awake()
    {
        shieldHP = maxShieldHP;
        shield.SetActive(isRaised);
        animator = GetComponent<Animator>();
    }
    
    //properties
    public int GetFullShieldCharges()
    {
        return Mathf.RoundToInt(shieldHP -  (shieldHP % shieldHPPerCharge) * shieldHPPerCharge);
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

    public void RegenerateShieldHP(float deltaTime)
    {
        if (shieldHP < 0) shieldHP = 0;

        if (shieldHP < maxShieldHP) shieldHP += shieldHPRegen * deltaTime;

        if (shieldHP > maxShieldHP) shieldHP = maxShieldHP;
    }
}
