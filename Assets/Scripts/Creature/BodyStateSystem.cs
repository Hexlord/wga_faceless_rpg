using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * History:
 * 
 * Date         Author      Description
 * 
 * 15.03.2019   aknorre     Created
 * 16.03.2019   bkrylov     Allocated to Component Menu
 * 
 */

public class BodyStateSystem : MonoBehaviour
{
    [AddComponentMenu("ProjectFaceless/Creature")]
    // Public

    public enum BodyState
    {
        Physical,
        Magical
    }

    public GameObject physicalAppearance;
    public GameObject physicalAppearanceSecond;
    public GameObject magicalAppearance;


    public BodyState State
    {
        get { return state; }
    }

    // Private

    private BodyState state = BodyState.Physical;

    protected void Start()
    {
        if (physicalAppearance) physicalAppearance.SetActive(true);
        if (physicalAppearanceSecond) physicalAppearanceSecond.SetActive(true);
        if (magicalAppearance) magicalAppearance.SetActive(false);
    }

    public void ChangeState(BodyState newState)
    {
        if (state == newState) return;

        state = newState;

        if (state == BodyState.Physical)
        {
            if (physicalAppearance) physicalAppearance.SetActive(true);
            if (physicalAppearanceSecond) physicalAppearanceSecond.SetActive(true);
            if (magicalAppearance) magicalAppearance.SetActive(false);
        }
        else
        {
            if (physicalAppearance) physicalAppearance.SetActive(false);
            if (physicalAppearanceSecond) physicalAppearanceSecond.SetActive(false);
            if (magicalAppearance) magicalAppearance.SetActive(true);
        }
    }


}
