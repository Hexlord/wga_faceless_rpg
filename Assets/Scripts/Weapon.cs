﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * History:
 * 
 * Date         Author      Description
 * 
 * 03.03.2019   bkrylov     Created
 * 
 */

public class Weapon : MonoBehaviour {

    /// <summary>
    /// Implements damage dealing logic
    /// 1. Must be attached to a gameobject with Trigger collider that acts as a weapon or to a projectile
    /// 2. Must be set up via code during runtime
    /// </summary>
    
    //Public

    public enum WeaponMode {Melee, Projectile, Raycast}

    [SerializeField]
    private WeaponMode Mode;

    // Private

    //gameobject that attacks the target
    private BaseCharacter wielder;

    //tag of an object that will recieve the damage (Faceless or Player)
    private string targetTag;

    //Determines whether the damage should be dealt or not
    private bool isStriking = false;

    //Damage the weapon deals when triggered
    private float damage;

    //Concentration that the wielder recieves upon dealing damage
    private float concentration;

    private void Awake()
    {
        if ((transform.parent != null) && ((wielder = transform.parent.GetComponent<BaseCharacter>()) != null)) IgnoreCollisionsWithWielder();
    }

    public void SetWielder(BaseCharacter value)
    {
        wielder = value;

        IgnoreCollisionsWithWielder();
    }

    public bool CompareToWielderTag(string value)
    {
        return value == wielder.tag;
    }

    public float Damage
    {
        set
        {
            damage = value;
        }
    }

    public float Concentration
    {
        set
        {
            concentration = value;
        }
    }

    public string TargetTag
    {
        set
        {
            try
            {
                targetTag = value;
            }
            catch
            {
                Debug.LogError("Something wrong with the tag \"" + value + "\"");
            }
        }
    }

    private void Update()
    {
        switch (Mode)
        {
            case WeaponMode.Melee:
                isStriking = wielder.IsStriking();
                break;
            case WeaponMode.Projectile:
                if (!isStriking) isStriking = true;
                break;
            case WeaponMode.Raycast:
                break;
        }
    }

    public void Strike(Collider col)
    {
        if (isStriking)
        {
            if (col.tag == targetTag)
            {
                col.transform.root.gameObject.GetComponent<HealthSystem>().DealDamage(damage);
                Debug.Log(wielder.name + " dealt " + damage + " damage to " + col.name);
                isStriking = false;
                if (wielder != null && wielder.tag == "Player")
                    {
                        try
                        {
                            wielder.GetComponent<HealthSystemWithConcentration>().StoreConcentration(concentration);
                        }
                        catch
                        {
                            Debug.LogError("Concentration System is not attached");
                        }
                    }
            }
            if (col.tag == "Shield")
            {
                col.attachedRigidbody.transform.parent.GetComponent<DefenseSystem>().Blocked();
                if (wielder != null)
                {
                    Animator anim;
                    if ((anim = wielder.GetComponent<Animator>()) != null)
                    {
                        anim.SetTrigger("Blocked");
                    }
                }
            }
        }
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.tag != wielder.tag) Strike(col);
    }

    private void IgnoreCollisionsWithWielder()
    {
        GameObject[] wielderObjects = GameObject.FindGameObjectsWithTag(wielder.tag);
        Collider col1 = gameObject.GetComponent<Collider>();
        if (col1 != null)
        {
            foreach (GameObject obj in wielderObjects)
            {
                Collider col2 = obj.GetComponent<Collider>();
                if (col2 != null) Physics.IgnoreCollision(col1, col2);
            }
        }
    }
}
