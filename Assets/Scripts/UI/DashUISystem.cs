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
public class DashUISystem : MonoBehaviour
{
    public int DashCharges
    {
        set
        {
            for (var i = 0; i < charges.Count; ++i)
            {
                charges[i].Ready = i < value;
            }
        }
    }
    
    private List<DashElementUISystem> charges = new List<DashElementUISystem>();

    private void Awake()
    {
        for(var i = 0; i < transform.childCount; ++i)
        {
            var obj = transform.GetChild(i);
            if (!obj) break;
            charges.Add(obj.GetComponent<DashElementUISystem>());
        }
    }
}
