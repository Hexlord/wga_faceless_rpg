using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 * History:
 * 
 * Date         Author      Description
 * 
 * 03.03.2019   aknorre     Created
 * 
 */

public class TransparentButton : MonoBehaviour
{
    [Tooltip("Alpha indicating that pixel is pressable (default = 0.5)")]
    [Range(0.0f, 1.0f)]
    public float threshold = 0.5f;

    protected void Start()
    {
        GetComponent<Image>().alphaHitTestMinimumThreshold = threshold;
    }

}
