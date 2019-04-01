using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

/*
 * History:
 * 
 * Date         Author      Description
 * 
 * 03.03.2019   aknorre     Created
 * 
 */

public enum Skill
{
    BlackBall,
    Hook,
    CircleStrike,
    LineStrike,
}

public static class SkillsExtensions
{
    public static string ToString(this Skill skill)
    {
        switch (skill)
        {
            case Skill.BlackBall:
                return "Black Ball";
            case Skill.CircleStrike:
                return "Circle Strike";
            case Skill.LineStrike:
                return "Line Strike";
            case Skill.Hook:
                return "Hook";
            default:
                throw new ArgumentOutOfRangeException("skill", skill, null);
        }

        Debug.Assert(false);
        return "";
    }

    public static SkillBase Instantiate(this Skill skill)
    {
        switch (skill)
        {
            case Skill.BlackBall:
                return new SkillBlackBall();
            case Skill.CircleStrike:
                return new SkillCircleStrike();
            case Skill.LineStrike:
                return new SkillLineStrike();
            case Skill.Hook:
                return new SkillHook();
        }

        Debug.Assert(false);
        return null;
    }
}
