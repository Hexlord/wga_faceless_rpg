using System;
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
 * 
 */

public enum SkillUtility
{
    Attack,
    Shield,
    Dash,
    Shoot
}


public class SkillsUtilityUISystem : MonoBehaviour
{


    public SkillUtility Slot
    {
        set
        {
            if (currentSlot == value) return;

            slotNodes[(int)currentSlot].SetActive(false);
            slotActiveNodes[(int)currentSlot].SetActive(false);

            currentSlot = value;

            slotNodes[(int)currentSlot].SetActive(true);
            slotActiveNodes[(int)currentSlot].SetActive(active);
        }
    }

    public bool Active
    {
        set
        {
            if (active == value) return;
            active = value;
            slotActiveNodes[(int)currentSlot].SetActive(active);

        }
    }

    private GameObject[] slotNodes;
    private GameObject[] slotActiveNodes;

    private SkillUtility currentSlot = SkillUtility.Attack;
    private bool active = false;

    private void Awake()
    {
        var length = Enum.GetValues(typeof(SkillUtility)).Length;
        slotNodes = new GameObject[length];
        slotActiveNodes = new GameObject[length];

        var slots = (SkillUtility[])Enum.GetValues(typeof(SkillUtility));
        var i = 0;
        foreach (var slot in slots)
        {
            var node = transform.FindPrecise(slot.ToString()).gameObject;
            var activeNode = transform.FindPrecise(slot.ToString() + "Active").gameObject;

            slotNodes[i] = node;
            slotActiveNodes[i] = activeNode;

            ++i;
        }
    }
}
