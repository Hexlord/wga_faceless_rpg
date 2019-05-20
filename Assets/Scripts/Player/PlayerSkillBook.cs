using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Policy;
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
    public int specialSlotCount = 1;

    // Cache
    private SkillSystem skillSystem;

    // Private
    private Skill?[] physicalSlots;
    private Skill?[] magicalSlots;
    private Skill?[] specialSlots;

    private readonly IList<Skill> learnedSkills = new List<Skill>();

    public Skill? GetSkill(SkillType type, int slot)
    {
        var list = GetList(type);

        Debug.Assert(slot >= 0 && list.Length > slot);
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

    private Skill?[] GetList(SkillType type)
    {
        switch (type)
        {
            case SkillType.Other:
                throw new ArgumentOutOfRangeException("type", type, null);
            case SkillType.Physical:
                return physicalSlots;
            case SkillType.Magical:
                return magicalSlots;
            case SkillType.Special:
                return specialSlots;
            default:
                throw new ArgumentOutOfRangeException("type", type, null);
        }
    }

    public bool IsBound(SkillType type, int slot)
    {
        var list = GetList(type);

        Debug.Assert(slot >= 0 && list.Length > slot);
        return list[slot] != null;
    }

    public void Bind(SkillType type, int slot, Skill? skill)
    {
        if(skill.HasValue) Debug.Assert(HasSkill(skill.Value));

        var list = GetList(type);
        
        Debug.Assert(slot >= 0 && list.Length > slot);
        list[slot] = skill;
    }

    public void Select(SkillType type, int slot)
    {
        var list = GetList(type);

        Debug.Assert(slot >= 0 && list.Length > slot);

        var value = list[slot];
        if (value != null)
        {
            if (skillSystem.SelectedSkillNumber != -1 && skillSystem.Skills[skillSystem.SelectedSkillNumber].Type == value.Value) skillSystem.UnselectSkill();
            else skillSystem.SelectSkill(value.Value);
        }
    }
    
    private void Awake()
    {
        physicalSlots = new Skill?[physicalSlotCount];
        magicalSlots = new Skill?[magicalSlotCount];
        specialSlots = new Skill?[specialSlotCount];

        skillSystem = GetComponent<SkillSystem>();
    }



}