using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/*
 * History:
 * 
 * Date         Author      Description
 * 
 * 19.05.2019   aknorre     Created
 */
[AddComponentMenu("ProjectFaceless/Player")]
public class PlayerSkillBook : MonoBehaviour
{
    
    // Public
    [Header("Basic Settings")]
    public int physicalSlotCount = 2;
    public int magicalSlotCount = 2;

    // Cache
    private SkillSystem skillSystem;

    // Private
    private Skill?[] physicalSlots;
    private Skill?[] magicalSlots;

    private readonly IList<Skill> learnedSkills = new List<Skill>();

    public Skill? GetSkill(BodyStateSystem.BodyState state, int slot)
    {
        var list = state == BodyStateSystem.BodyState.Physical
            ? physicalSlots
            : magicalSlots;

        return list[slot];
    }

    public bool HasSkill(Skill skill)
    {
        return learnedSkills.Contains(skill);
    }

    public void Learn(Skill skill)
    {
        Debug.Assert(!HasSkill(skill));
        learnedSkills.Add(skill);
    }

    public void Bind(BodyStateSystem.BodyState state, int slot, Skill? skill)
    {
        if(skill.HasValue) Debug.Assert(HasSkill(skill.Value));

        var list = state == BodyStateSystem.BodyState.Physical
            ? physicalSlots
            : magicalSlots;

        Debug.Assert(slot >= 0 && list.Length > slot);
        list[slot] = skill;
    }

    public void Select(BodyStateSystem.BodyState state, int slot)
    {
        var list = state == BodyStateSystem.BodyState.Physical
            ? physicalSlots
            : magicalSlots;

        Debug.Assert(slot >= 0 && list.Length > slot);

        var value = list[slot];
        if (value != null)
        {
            if (skillSystem.Skills[skillSystem.SelectedSkillNumber].Type == value.Value) skillSystem.UnselectSkill();
            else skillSystem.SelectSkill(value.Value);
        }
    }
    
    private void Awake()
    {
        physicalSlots = new Skill?[physicalSlotCount];
        magicalSlots = new Skill?[magicalSlotCount];

        skillSystem = GetComponent<SkillSystem>();
    }



}