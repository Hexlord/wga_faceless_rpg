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

public enum SkillType
{
    Other,
    Physical,
    Magical,
    Special
}


public enum Skill
{
    BlackBall = 0,
    Hook,
    CircleStrike,
    LineStrike,
    FireBreath,
    Meteor,
    IceWall,
    SkillSpecial1,
    SkillSpecial2,
    ShiftStrike,
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
                return "BlackBall";
            case Skill.CircleStrike:
                return "CircleStrike";
            case Skill.LineStrike:
                return "LineStrike";
            case Skill.Hook:
                return "Hook";
            case Skill.FireBreath:
                return "FireBreath";
            case Skill.Heal:
                return "Heal";
            case Skill.Meteor:
                return "Meteor";
            case Skill.IceWall:
                return "IceWall";
            case Skill.SkillSpecial1:
                return "SkillSpecial1";
            case Skill.SkillSpecial2:
                return "SkillSpecial2";
            case Skill.ShiftStrike:
                return "ShiftStrike";
            default:
                throw new ArgumentOutOfRangeException("skill", skill, null);
        }
    }

    public static SkillType SkillType(this Skill skill)
    {
        switch (skill)
        {
            case Skill.BlackBall:
                return global::SkillType.Magical;
            case Skill.CircleStrike:
                return global::SkillType.Physical;
            case Skill.LineStrike:
                return global::SkillType.Physical;
            case Skill.Hook:
                return global::SkillType.Physical;
            case Skill.FireBreath:
                return global::SkillType.Magical;
            case Skill.Heal:
                return global::SkillType.Other;
            case Skill.Meteor:
                return global::SkillType.Magical;
            case Skill.IceWall:
                return global::SkillType.Magical;
            case Skill.SkillSpecial1:
                return global::SkillType.Special;
            case Skill.SkillSpecial2:
                return global::SkillType.Special;
            case Skill.ShiftStrike:
                return global::SkillType.Physical;
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
            case Skill.Meteor:
                return new SkillMeteor();
            case Skill.IceWall:
                return new SkillIceWall();
            case Skill.SkillSpecial1:
                return new SkillSpecial1();
            case Skill.SkillSpecial2:
                return new SkillSpecial2();
            case Skill.ShiftStrike:
                return new SkillShiftStrike();
            default:
                throw new ArgumentOutOfRangeException("skill", skill, null);
        }
    }
}
