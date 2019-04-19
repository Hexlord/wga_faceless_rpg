using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 * History:
 * 
 * Date         Author      Description
 * 
 * 15.03.2019   aknorre     Created
 * 
 */
[AddComponentMenu("ProjectFaceless/Creature/XP System")]
public class XpSystem : MonoBehaviour
{

    // Public

    [Tooltip("Level xp list")]
    public float[] levelRequirements;

    public float LevelCompletion
    {
        get { return xp / levelRequirements[level]; }
    }

    public int SwordPoints
    {
        get { return swordPoints; }
        set { swordPoints = value; }
    }
    public int MaskPoints
    {
        get { return maskPoints; }
        set { maskPoints = value; }
    }

    // Private

    private int level = 0;
    private float xp = 0.0f;
    private int swordPoints = 0;
    private int maskPoints = 0;

    // Cache

    public void GrantXp(float amount)
    {
        xp += amount;

        while (levelRequirements.Length > level + 1 &&
               levelRequirements[level] <= xp)
        {
            xp -= levelRequirements[level];
            ++level;
            ++swordPoints;
            // TODO: create sword effect with sound
        }

        if (xp > levelRequirements[level])
        {
            xp = System.Math.Min(levelRequirements[level], xp);
        }
    }

    public void GrantMask(int amount)
    {
        while (amount > 0)
        {
            --amount;
            ++maskPoints;
            // TODO: create mask effect with sound
        }
    }

}
