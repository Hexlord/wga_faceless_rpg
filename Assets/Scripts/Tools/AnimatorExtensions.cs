using System.Collections;
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

public static class AnimatorExtensions
{
    public static float GetAnimationLength(this Animator animator, string name)
    {
        RuntimeAnimatorController ac = animator.runtimeAnimatorController;
        for (int i = 0; i < ac.animationClips.Length; i++)
        {
            if (ac.animationClips[i].name == name)
            {
                return ac.animationClips[i].length;
            }
        }

        Debug.LogWarning("Animation " + name + " not found!");
        // Debug.Assert(false);

        return 0.0f;
    }
}
