using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * History:
 * 
 * Date         Author      Description
 * 
 * 03.03.2019   bkrylov     Created
 * 03.03.2019   aknorre     Handling skill casting
 * 
 */
public class Character : BaseCharacter
{

    /// <summary>
    /// Handles player character behaviour
    /// </summary>

    //Public
    [Tooltip("Tag of the enemies")]
    [SerializeField]
    private string target;

    [Tooltip("How much sprinting is faster than standart running")]
    [SerializeField]
    private float sprintModifier = 1.25f;

    [Header("Projectile Settings")]

    [Tooltip("Projectile prefab")]
    [SerializeField]
    private GameObject playerProjectile;

    [Tooltip("Transform projectiles are fired from")]
    [SerializeField]
    private Transform ShootingPoint;

    [Tooltip("Speed of projectiles")]
    [SerializeField]
    private float projectileSpeed = 12.0f;

    [Tooltip("Damage the projectiles deal")]
    [SerializeField]
    private float projectileDamage = 15.0f;

    [Tooltip("Concentration the player recieves upon attack")]
    [SerializeField]
    private float projectileConcentration = 2.5f;

    [Header("Sword Settings")]

    [Tooltip("Gameobject that represent the sword of the player")]
    [SerializeField]
    private Weapon playerSword;

    [Tooltip("Damage the sword deal")]
    [SerializeField]
    private float swordDamage = 25.0f;

    [Tooltip("Concentration the player recieves upon attack")]
    [SerializeField]
    private float swordConcentration = 12.5f;


    [Header("SFX")]

    [Tooltip("Particles that are active in Sword Stance")]
    [SerializeField]
    private GameObject SwordParticles;

    [Tooltip("Particles that are active in Magical Stance")]
    [SerializeField]
    private GameObject MaskParticles;

    //Public enumerations that represent character states. Not using booleans for better code readability.
    public enum CharacterState { SwordStance, MagicStance };
    public enum SwordState { SheathedSword, UnsheathedSword };

    //Private

    private Vector3 startingPos;
    private Quaternion startingRot;
    private DefenseSystem defenseSystem;

    [Header("Debug")]
    [SerializeField]
    private CharacterState currentCharacterState = CharacterState.SwordStance;

    [SerializeField]
    private SwordState currentSwordState = SwordState.SheathedSword;

    private GameObject magicProjectile;
    private bool isSprinting;

    public bool IsSprinting
    {
        get
        {
            return isSprinting;
        }
        set
        {
            isSprinting = value;
            anim.SetBool("isSprinting", value);
        }
    }

    public float SprintModifier
    {
        get
        {
            return sprintModifier;
        }
    }

    public CharacterState Status
    {
        get
        {
            return currentCharacterState;
        }
    }

    public SwordState SwordStatus
    {
        get
        {
            return currentSwordState;
        }
    }

    // Use this for initialization
    protected override void Start()
    {
        base.Start();

        defenseSystem = GetComponent<DefenseSystem>();
        startingPos = transform.position;
        startingRot = transform.rotation;
        currentCharacterState = CharacterState.SwordStance;

        SwordParticles.SetActive(false);
        MaskParticles.SetActive(false);

        playerSword.TargetTag = target;
        playerSword.Damage = swordDamage;
        playerSword.Concentration = swordConcentration;

        GameObject.Find("Crosshair").GetComponent<CanvasRenderer>().SetAlpha(0);
    }

    //Status control

    public void SwapPlayerStatus()
    {
        switch (currentCharacterState)
        {
            case CharacterState.MagicStance:
                {
                    currentCharacterState = CharacterState.SwordStance;
                    if (currentSwordState == SwordState.UnsheathedSword)
                    {
                        MaskParticles.SetActive(false);
                        SwordParticles.SetActive(true);
                    }

                    GetComponent<SmartController>().SwitchState(SmartController.CameraState.Action);
                    GameObject.Find("Crosshair").GetComponent<CanvasRenderer>().SetAlpha(0);
                    SwitchPhysicalLayer("Physical");
                    //gameObject.layer = LayerMask.NameToLayer("Physical");
                    break;
                }
            case CharacterState.SwordStance:
                {
                    if (currentSwordState == SwordState.UnsheathedSword)
                    {
                        MaskParticles.SetActive(true);
                        SwordParticles.SetActive(false);
                    }
                    currentCharacterState = CharacterState.MagicStance;
                    // GetComponent<SmartController>().SwitchState(SmartController.CameraState.Shoot);
                    GameObject.Find("Crosshair").GetComponent<CanvasRenderer>().SetAlpha(1);
                    SwitchPhysicalLayer("Magical");
                    //gameObject.layer = LayerMask.NameToLayer("Magical");
                    break;
                }
        }


    }

    public void DrawSword()
    {
        if (currentSwordState == SwordState.SheathedSword)
        {
            currentSwordState = SwordState.UnsheathedSword;
            if (currentCharacterState == CharacterState.SwordStance)
            {
                SwordParticles.SetActive(true);
                MaskParticles.SetActive(false);
            }
            else
            {
                SwordParticles.SetActive(false);
                MaskParticles.SetActive(true);
            }
            anim.SetTrigger("UnsheatheSword");
            GetComponent<SmartController>().SwitchState(SmartController.CameraState.Action);
        }
        else
        {
            SwordParticles.SetActive(false);
            MaskParticles.SetActive(false);
            anim.SetTrigger("SheatheSword");
            currentSwordState = SwordState.SheathedSword;
            GetComponent<SmartController>().SwitchState(SmartController.CameraState.Action);
        }
    }

    //Attacks

    public void SwingSword()
    {
        GetComponent<SmartController>().TriggerPlayerAutoRotation();
        anim.SetTrigger("Swing");
    }

    public void ShootProjectile()
    {
        Ray rayFromCenterOfTheScreen = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;
        Vector3 shootingDirection;
        int mask = LayerMask.GetMask("Enemy", "Environment");
        float dist = 1000.0f;

        if (Physics.Raycast(rayFromCenterOfTheScreen, out hit, dist, mask, QueryTriggerInteraction.Ignore))
        {
            shootingDirection = (hit.point - ShootingPoint.position).normalized;
        }
        else
        {
            shootingDirection = Camera.main.transform.forward;
        }

        magicProjectile = Instantiate(playerProjectile, ShootingPoint.position, ShootingPoint.rotation);
        Weapon projectileWeaponComponent = magicProjectile.GetComponent<Weapon>();
        projectileWeaponComponent.SetWielder(this);
        projectileWeaponComponent.TargetTag = target;
        projectileWeaponComponent.Damage = projectileDamage;
        projectileWeaponComponent.Concentration = projectileConcentration;
        magicProjectile.GetComponent<Rigidbody>().AddForce(shootingDirection * projectileSpeed);
    }

    //Movement

    public void Move(bool isDashing)
    {

        if (!isDashing)
        {
            if (direction.magnitude > 1) direction.Normalize();
            direction *= speed * ((isSprinting) ? sprintModifier : 1.0f);
            if (charController != null)
            {
                direction += (!charController.isGrounded) ? Physics.gravity : Vector3.zero;
            }
            if (charController != null) charController.Move(direction * Time.deltaTime);
            CurrentDirection = Vector3.zero;
        }
        else
        {
            charController.Move(defenseSystem.dashDirection * Time.deltaTime);
        }
    }

    protected override void FixedUpdate()
    {
        if (direction != Vector3.zero) Move(defenseSystem.isDashing);
    }

    void SwitchPhysicalLayer(string layer)
    {
        Transform[] transformChildren = transform.GetComponentsInChildren<Transform>();
        foreach (Transform tf in transform.GetComponentsInChildren<Transform>())
        {
            tf.gameObject.layer = LayerMask.NameToLayer(layer);
        }
    }

}
