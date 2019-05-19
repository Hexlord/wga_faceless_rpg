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

public class SkillsUISystem : MonoBehaviour
{
    

    public Skill? Slot
    {
        set
        {
            if (currentSlot == value) return;

            if (currentSlot != null)
            {
                slotNodes[(int) currentSlot].SetActive(false);
                slotActiveNodes[(int) currentSlot].SetActive(false);
            }

            currentSlot = value;

            if (currentSlot != null)
            {
                slotNodes[(int) currentSlot].SetActive(true);
                slotActiveNodes[(int) currentSlot].SetActive(active);
            }
        }
    }

    public bool Active
    {
        set
        {
            if (active == value) return;
            active = value;
            if (currentSlot != null)
            {
                slotActiveNodes[(int)currentSlot].SetActive(active);
            }
        }
    }

    public bool Selected
    {
        set
        {
            border.SetActive(value);
        }
    }

    public float CooldownNormalized
    {
        set { fade.fillAmount = value; }
    }
    
    private GameObject[] slotNodes;
    private GameObject[] slotActiveNodes;
    private GameObject border;
    private Image fade;

    private Skill? currentSlot = null;
    private bool active = false;
    
    private List<DashElementUISystem> charges = new List<DashElementUISystem>();

    private void Awake()
    {
        var length = Enum.GetValues(typeof(Skill)).Length;
        slotNodes = new GameObject[length];
        slotActiveNodes = new GameObject[length];

        var slots = (Skill[]) Enum.GetValues(typeof(Skill));

        var i = 0;
        foreach (var slot in slots)
        {
            var node = transform.FindPrecise(slot.ToString());
            if (node)
            {
                var go = node.gameObject;
                var activeNode = transform.FindPrecise(slot.ToString() + "Active").gameObject;

                slotNodes[i] = go;
                slotActiveNodes[i] = activeNode;
            }

            ++i;
        }

        border = transform.FindPrecise("SkillBorderWhite").gameObject;
        fade = transform.FindPrecise("SkillFade").GetComponent<Image>();
    }
}
