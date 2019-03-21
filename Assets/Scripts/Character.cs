﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : BaseCharacter {

    public Weapon playerSword;
    public GameObject playerProjectile;
    public Transform ShootingPoint;
    
    public enum CharacterState {SheathedSword, SwordStance, MagicStance};
    public GameObject SwordParticles, MaskParticles;
    public float projectileSpeed = 12.0f;
    public float projectileDamage = 15.0f;
    public float swordDamage = 25.0f, swordConcentration = 12.5f;
    public string target;

    GameObject magicProjectile;
    Vector3 startingPos;
    Quaternion startingRot;
    Animator anim;
    private CharacterState currentState = CharacterState.SheathedSword;
    bool isSprinting;

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
            return currentState;
        }
    }

	// Use this for initialization
	void Start ()
    {
        anim = GetComponent<Animator>();
        startingPos = transform.position;
        startingRot = transform.rotation;
        currentState = CharacterState.SheathedSword;
        SwordParticles.SetActive(false);
        MaskParticles.SetActive(false);
        playerSword.TargetTag = "Faceless";
        playerSword.Damage = swordDamage;
        playerSword.Concentration = swordConcentration;

        GameObject.Find("Crosshair").GetComponent<CanvasRenderer>().SetAlpha(0);
    }


    private bool isNotified = false;

    public void SwapPlayerStatus()
    {
       switch (currentState)
       {
            case CharacterState.SheathedSword:
                {
                    DrawSword();
                    //currentState = CharacterState.MagicStance;
                    GetComponent<SmartController>().SwitchState(SmartController.CameraState.Action);
                    GameObject.Find("Crosshair").GetComponent<CanvasRenderer>().SetAlpha(0);
                    //gameObject.layer = LayerMask.NameToLayer("Physical");
                    break;
                }
            case CharacterState.MagicStance:
                {
                    MaskParticles.SetActive(false);
                    SwordParticles.SetActive(true);
                    currentState = CharacterState.SwordStance;
                    GetComponent<SmartController>().SwitchState(SmartController.CameraState.Action);
                    GameObject.Find("Crosshair").GetComponent<CanvasRenderer>().SetAlpha(0);
                    SwitchPhysicalLayer("Physical");
                    gameObject.layer = LayerMask.NameToLayer("Physical");
                    break;
                }
            case CharacterState.SwordStance:
                {
                    SwordParticles.SetActive(false);
                    MaskParticles.SetActive(true);
                    currentState = CharacterState.MagicStance;
                    GetComponent<SmartController>().SwitchState(SmartController.CameraState.Shoot);
                    GameObject.Find("Crosshair").GetComponent<CanvasRenderer>().SetAlpha(1);
                    SwitchPhysicalLayer("Magical");
                    gameObject.layer = LayerMask.NameToLayer("Magical");
                    break;
                }
        }


    }

    //public override void Die()
    //{
    //    Destroy(gameObject);
    //}

    public void DrawSword()
    {
        if (currentState == CharacterState.SheathedSword)
        {
            SwordParticles.SetActive(true);
            MaskParticles.SetActive(false);
            anim.SetTrigger("UnsheatheSword");
            SwitchPhysicalLayer("Physical");
            gameObject.layer = LayerMask.NameToLayer("Physical");
            currentState = CharacterState.SwordStance;
        }
        else
        {
            if (currentState == CharacterState.SwordStance || currentState == CharacterState.MagicStance)
            {
                SwordParticles.SetActive(false);
                MaskParticles.SetActive(false);
                anim.SetTrigger("SheatheSword");
                currentState = CharacterState.SheathedSword;
                SwitchPhysicalLayer("Physical");
                gameObject.layer = LayerMask.NameToLayer("Physical");
                GetComponent<SmartController>().SwitchState(SmartController.CameraState.Action);
            }
        }
    }

    public void SwingSword()
    {
        anim.SetTrigger("Swing");
        playerSword.TriggerStricking();
    }

    public void ShootProjectile()
    {
        GameObject projectile = Instantiate(playerProjectile, ShootingPoint.position, ShootingPoint.rotation);
        Weapon projectileDamageComponent = projectile.GetComponent<Weapon>();
        projectileDamageComponent.TriggerStricking();
        projectileDamageComponent.TargetTag = target;
        projectileDamageComponent.Damage = projectileDamage;
        projectile.GetComponent<Rigidbody>().AddForce(Camera.main.gameObject.transform.forward * projectileSpeed);
    }

    void SwitchPhysicalLayer(string layer)
    {
        foreach (Transform tf in transform.GetComponentInChildren<Transform>())
        {
            tf.gameObject.layer = LayerMask.NameToLayer(layer);
        }
    }

}