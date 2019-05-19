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
    BlackBall = 0,
    Hook,
    CircleStrike,
    LineStrike,
    FireBreath,
    Heal,
}

public enum SkillAnimation
{
    Hook = 1,
    LineStrike = 2,
    CircleStrike = 3,
    ShiftLeft = 4,
    ShiftRight = 5,
    BlackHole = 6,
    FireBreath = 7,
    Meteor=8,
    IceWall=9,
    Special=10,
    Heal=11,
    Dash=12
}

public static class SkillExtensions
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
            case Skill.FireBreath:
                return "Fire Breath";
            case Skill.Heal:
                return "Heal";
            default:
                throw new ArgumentOutOfRangeException("skill", skill, null);
        }
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
            case Skill.FireBreath:
                return new SkillFireBreath();
            case Skill.Heal:
                return new SkillHeal();
            default:
                throw new ArgumentOutOfRangeException("skill", skill, null);
        }
    }
}
