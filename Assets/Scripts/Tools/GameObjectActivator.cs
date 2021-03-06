﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * History:
 * 
 * Date         Author      Description
 * 
 * 03.03.2019   aknorre     Created
 * 
 */

public class GameObjectActivator : MonoBehaviour
{
    public GameObject target;

    protected void Awake()
    {
        if(target) target.SetActive(true);
    }

}
