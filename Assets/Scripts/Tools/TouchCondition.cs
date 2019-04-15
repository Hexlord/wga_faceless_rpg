using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/*
 * History:
 * 
 * Date         Author      Description
 * 
 * 30.03.2019   aknorre     Created
 * 
 */
[AddComponentMenu("ProjectFaceless/Tools")]
public class TouchCondition : MonoBehaviour
{
    public string filterTag;
    public bool searchParent = false;

    public bool Touch
    {
        get { return touch; }
    }
    public GameObject TouchObject
    {
        get { return touchObject; }
    }

    public float DetouchedTime
    {
        get { return detouchedTime; }
    }

    private bool touch = false;
    private GameObject touchObject;
    private bool obsTouch = false;
    private float detouchedTime = 0.0f;

    protected void FixedUpdate()
    {
        if (obsTouch == false) touch = false;

        if (touch == false)
        {
            detouchedTime += Time.fixedDeltaTime;
        }

        obsTouch = false;
    }

    private void OnTriggerStay(Collider other)
    {
        var go = other.gameObject;
        var good = filterTag.Length == 0 || filterTag == other.tag;
        if (!good && searchParent)
        {
            go = other.gameObject.TraverseParent(filterTag);
            if (go) good = true;
        }

        if (good)
        {
            obsTouch = true;
            touch = true;
            touchObject = go;
            detouchedTime = 0.0f;
        }
    }


}
