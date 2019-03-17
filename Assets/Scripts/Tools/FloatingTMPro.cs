using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/*
 * History:
 * 
 * Date         Author      Description
 * 
 * 03.03.2019   aknorre     Created
 * 16.03.2019   bkrylov     Allocated to Component Menu
 * 
 */
[AddComponentMenu("ProjectFaceless/Tools/FloatingTMPro")]
public class FloatingTMPro : FloatingTransform
{
    
    [Header("Float Settings")]

    
    [Tooltip("Lifespan alpha fade out text")]
    public TextMeshPro mesh = null;

    override protected void SetAlpha(float alpha) 
    {
        mesh.alpha = alpha;
    }

}
