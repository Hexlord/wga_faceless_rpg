using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : BaseCharacter {

    //Handles player character behaviour
    //Public
    public string target;

    [Header("Projectile Settings")]
    public GameObject playerProjectile;
    public Transform ShootingPoint;
    public float projectileSpeed = 12.0f;
    public float projectileDamage = 15.0f;

    [Header ("Sword Settings")]
    public Weapon playerSword;
    public float swordDamage = 25.0f;
    public float swordConcentration = 12.5f;


    [Header ("SFX")]
    public GameObject SwordParticles, MaskParticles;

    //Public enumerations that represent character states. Not using booleans for better code readability.
    public enum CharacterState {SwordStance, MagicStance};
    public enum SwordState {SheathedSword, UnsheathedSword};

    //Private

    private Vector3 startingPos;
    private Quaternion startingRot;

    private Animator anim;

    [Header ("Debug")]
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
    void Start ()
    {
        anim = GetComponent<Animator>();

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

    private bool isNotified = false;

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

    public void SwingSword()
    {
        GetComponent<SmartController>().TriggerPlayerAutoRotation();
        anim.SetTrigger("Swing");
        playerSword.TriggerStricking();
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
        //Debug.Log(hit.collider.name);
        projectileWeaponComponent.SetWielder(transform);
        projectileWeaponComponent.TriggerStricking();
        projectileWeaponComponent.TargetTag = target;
        projectileWeaponComponent.Damage = projectileDamage;
        magicProjectile.GetComponent<Rigidbody>().AddForce(shootingDirection * projectileSpeed);
    }

    void SwitchPhysicalLayer(string layer)
    {
        foreach (Transform tf in transform.GetComponentInChildren<Transform>())
        {
            tf.gameObject.layer = LayerMask.NameToLayer(layer);
        }
    }

}
