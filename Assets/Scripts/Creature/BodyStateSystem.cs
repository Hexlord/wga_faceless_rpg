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
 * 25.03.2019   bkrylov     Remade Component for better collider filtration. 
 * 
 */
[AddComponentMenu("ProjectFaceless/Creature/Body State System")]
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
    public GameObject physicalHitbox;
    public GameObject magicalHitbox;

    private SkillSystem skillSystem;

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
        skillSystem = GetComponent<SkillSystem>();

        bool sheathed = sheathSystem.Sheathed;

        Show(sheathed, physicalAppearance, physicalAppearanceSheathed);
        if (magicalAppearance) magicalAppearance.SetActive(false);
        if (magicalAppearanceSheathed) magicalAppearance.SetActive(false);

        //Consistency checks
        if (!magicalHitbox) Debug.Log("No magical hitbox attached!");
        else
        {
            if (magicalHitbox.layer != LayerMask.NameToLayer("Magical")) Debug.LogError("Magical hitbox isn't placed on proper layer");
        }

        if (!magicalHitbox) Debug.Log("No physical hitbox attached!");
        else
        {
            if (magicalHitbox.layer != LayerMask.NameToLayer("Physical")) Debug.LogError("Physical hitbox isn't placed on proper layer");
        }
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
        if (skillSystem && skillSystem.IsSkillSelected && !skillSystem.Busy)
        {
            skillSystem.UnselectSkill();
        }
    }

    public static int StateToLayer(BodyState state)
    {
        if (state == BodyState.Magical) return LayerMask.NameToLayer("Magical");
        if (state == BodyState.Physical) return LayerMask.NameToLayer("Physical");
        else return 0;
    }
}