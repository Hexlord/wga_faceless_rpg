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
}

public static class EffectExtensions
{
    public static string ToString(this Effect effect)
    {
        switch (effect)
        {
            case Effect.Burn:
                return "Burning";
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
            default:
                throw new ArgumentOutOfRangeException("effect", effect, null);
        }
    }
}
