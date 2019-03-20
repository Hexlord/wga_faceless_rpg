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
        }

        Debug.Assert(false);
        return null;
    }
}
