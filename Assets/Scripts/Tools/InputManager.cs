using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * History:
 * 
 * Date         Author      Description
 * 
 * 15.03.2019   aknorre     Created
 * 25.03.2019   bkrylov     Added method that handles input action button release
 * 
 */

public enum InputAction
{
    ChangeBodyState,
    Sheathe,
    Attack,
    Defend,
    Aim,
    Heal,
    SkillMenu,
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
            case InputAction.Defend:
                return "Shift";
            case InputAction.SkillMenu:
                return "H";
        }
        return "none";
    }

    public static bool GetInput(InputAction action)
    {
        return Input.GetButtonDown(ActionToName(action));
    }

    public static bool GetInputRelease(InputAction action)
    {
        return Input.GetButtonUp(ActionToName(action));
    }

    public static Vector2 GetMovement()
    {
        return new Vector2(
            Input.GetAxis("Horizontal"),
            Input.GetAxis("Vertical"));
    }
}
