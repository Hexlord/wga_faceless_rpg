﻿using System.Collections;
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

public enum InputAction
{
    ChangeBodyState,
    Sheathe,
    Attack,
    Aim,
    Heal,
    Skill_1,
    Skill_2,
    Skill_3,
    Skill_4,
    Skill_5,
    Skill_6,
    Skill_7,
    Skill_8,
    Escape,
    Enter,
}

public static class InputManager
{
    
    private static string ActionToName(InputAction action)
    {
        switch (action)
        {
            case InputAction.ChangeBodyState:
                return "Space";
            case InputAction.Sheathe:
                return "F";
            case InputAction.Aim:
                return "RMB";
            case InputAction.Heal:
                return "X";
            case InputAction.Skill_1:
                return "1";
            case InputAction.Skill_2:
                return "2";
            case InputAction.Skill_3:
                return "3";
            case InputAction.Skill_4:
                return "4";
            case InputAction.Skill_5:
                return "5";
            case InputAction.Skill_6:
                return "6";
            case InputAction.Skill_7:
                return "7";
            case InputAction.Skill_8:
                return "8";
            case InputAction.Escape:
                return "Escape";
            case InputAction.Enter:
                return "Enter";
            case InputAction.Attack:
                return "LMB";
        }
        return "none";
    }

    public static bool Get(InputAction action)
    {
        return Input.GetButtonDown(ActionToName(action));
    }
}
