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
 * 05.04.2019   aknorre     Created
 * 
 */

public enum Effect
{
    Burn,
    Freeze,
    Special1Invulnerable,
    Special1Speed,
}

public static class EffectExtensions
{
    public static string ToString(this Effect effect)
    {
        switch (effect)
        {
            case Effect.Burn:
                return "Burning";
             case Effect.Freeze:
                return "Freezing";
            case Effect.Special1Invulnerable:
                return "Invulnerable";
            case Effect.Special1Speed:
                return "Increased movement speed";
            default:
                throw new ArgumentOutOfRangeException("effect", effect, null);
        }
    }

    public static EffectBase Instantiate(this Effect effect)
    {
        switch (effect)
        {
            case Effect.Burn:
                return new EffectBurn();
            case Effect.Freeze:
                return new EffectFreeze();
            case Effect.Special1Invulnerable:
                return new EffectInvulnerable();
            case Effect.Special1Speed:
                return new EffectFast();
            default:
                throw new ArgumentOutOfRangeException("effect", effect, null);
        }
    }
}
