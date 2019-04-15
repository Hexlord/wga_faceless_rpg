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

    void Awake()
    {
        var i = 1;
        while (true)
        {
            var obj = transform.Find(System.Convert.ToString(i));
            if (!obj) break;
            charges.Add(obj.GetComponent<DashElementUISystem>());
            ++i;
        }
    }
}
