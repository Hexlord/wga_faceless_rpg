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
 * 
 */

public class BodyStateSystem : MonoBehaviour
{

    // Public

    public enum BodyState
    {
        Physical,
        Magical
    }

    public GameObject physicalAppearance;
    public GameObject physicalAppearanceSheathed;
    public GameObject magicalAppearance;
    public GameObject magicalAppearanceSheathed;


    public BodyState State
    {
        get { return state; }
    }

    // Private

    private SheathSystem sheathSystem;
    private BodyState state = BodyState.Physical;

    private void Show(bool isSheathed, GameObject unsheathed, GameObject sheathed)
    {
        if (unsheathed && unsheathed == sheathed) unsheathed.SetActive(true);
        else
        {
            if (unsheathed) unsheathed.SetActive(!isSheathed);
            if (sheathed) sheathed.SetActive(isSheathed);
        }
    }

    protected void Start()
    {
        sheathSystem = GetComponent<SheathSystem>();

        bool sheathed = sheathSystem.Sheathed;

        Show(sheathed, physicalAppearance, physicalAppearanceSheathed);
        if (magicalAppearance) magicalAppearance.SetActive(false);
        if (magicalAppearanceSheathed) magicalAppearance.SetActive(false);
    }

    protected void FixedUpdate()
    {
        bool sheathed = sheathSystem.Sheathed;

        if (state == BodyState.Physical)
        {

            Show(sheathed, physicalAppearance, physicalAppearanceSheathed);
            if (magicalAppearance) magicalAppearance.SetActive(false);
            if (magicalAppearanceSheathed) magicalAppearance.SetActive(false);
        }
        else
        {
            Show(sheathed, magicalAppearance, magicalAppearanceSheathed);
            if (physicalAppearance) physicalAppearance.SetActive(false);
            if (physicalAppearanceSheathed) physicalAppearanceSheathed.SetActive(false);
        }
    }

    public void ChangeState(BodyState newState)
    {
        if (state == newState) return;

        state = newState;
        
    }


}
