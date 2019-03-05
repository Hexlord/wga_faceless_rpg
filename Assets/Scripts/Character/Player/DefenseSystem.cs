using System.Collections;
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

public class DefenseSystem : MonoBehaviour
{
    /// <summary>
    /// Handles defence 
    /// </summary>

    public int physicalDefenseCharges = 3, magicalDefenseCharges = 3;
    public float dashCooldown = 3.0f, shieldCooldown = 3.0f;
    public bool[] physicalDefenseChargesAvailable, magicalDefenseChargesAvailable;
    public bool isDashing;
    public float dashRange = 10.0f;
    public float dashTime = 0.2f;


    public Vector3 dashDirection;

    private bool isBlocking;
    private float dashStart, dashSpeed;
    private Animator anim;
    private UIController UIcontroller;
    private float physicalDefenseChargesTimeLastUsed, magicalDefenseChargesTimeLastUsed;
    private int physicalAvailableChargeIndex, magicalAvailableChargeIndex;

    public bool IsBlocking
    {
        get
        {
            return isBlocking;
        }
        set
        {
            if (physicalAvailableChargeIndex < physicalDefenseCharges || !value)
            {
                isBlocking = value;
                anim.SetBool("isBlocking", value);
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        dashSpeed = dashRange / dashTime;
        anim = gameObject.GetComponent<Animator>();
        physicalDefenseChargesAvailable = new bool[physicalDefenseCharges];
        for (int i = 0; i < physicalDefenseChargesAvailable.Length; i++)
        {
            physicalDefenseChargesAvailable[i] = true;
        }

        magicalDefenseChargesAvailable = new bool[magicalDefenseCharges];
        for (int i = 0; i < magicalDefenseChargesAvailable.Length; i++)
        {
            magicalDefenseChargesAvailable[i] = true;
        }
        UIcontroller = FindObjectOfType<UIController>();
        //UIcontroller.UpdateUI();
    }

    // Update is called once per frame
    void Update()
    {
        isDashing = Time.time < dashStart + dashTime;

        if ((physicalDefenseChargesTimeLastUsed + shieldCooldown) < Time.time)
        {
            ShieldReloaded();
        }

        if ((physicalAvailableChargeIndex > 0) && (isBlocking))
        {
            physicalDefenseChargesTimeLastUsed = Time.time;
        }

        if ((magicalDefenseChargesTimeLastUsed + dashCooldown) < Time.time)
        {
            DashReloaded();
        }
    }

    public void InitiateDash(Vector3 direction)
    {
        if (magicalAvailableChargeIndex < magicalDefenseCharges)
        {
            dashStart = Time.time;
            magicalDefenseChargesTimeLastUsed = dashStart;
            magicalDefenseChargesAvailable[magicalAvailableChargeIndex] = false;
            magicalAvailableChargeIndex++;
            isDashing = true;

            dashDirection = direction.normalized * dashSpeed;
            UIcontroller.UpdateUI();
        }
    }

    public void Blocked()
    {
        if (physicalAvailableChargeIndex < physicalDefenseCharges)
        {
            physicalDefenseChargesTimeLastUsed = Time.time;
            physicalDefenseChargesAvailable[physicalAvailableChargeIndex] = false;
            physicalAvailableChargeIndex++;
            UIcontroller.UpdateUI();
            if (physicalDefenseCharges == physicalAvailableChargeIndex)
            {
                anim.SetBool("isBlocking", false);
            }
        }
    }

    private void ShieldReloaded()
    {
        if (physicalAvailableChargeIndex > 0)
        {
            physicalAvailableChargeIndex--;
            physicalDefenseChargesAvailable[physicalAvailableChargeIndex] = true;
            UIcontroller.UpdateUI();
            physicalDefenseChargesTimeLastUsed = Time.time;
        }
    }

    private void DashReloaded()
    {
        if (magicalAvailableChargeIndex > 0)
        {
            magicalAvailableChargeIndex--;
            magicalDefenseChargesAvailable[magicalAvailableChargeIndex] = true;
            UIcontroller.UpdateUI();
            magicalDefenseChargesTimeLastUsed = Time.time;
        }
    }
}
