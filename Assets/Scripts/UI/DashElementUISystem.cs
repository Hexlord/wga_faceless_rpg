using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashElementUISystem : MonoBehaviour
{
    public bool Ready
    {
        set
        {
            ready.SetActive(value);
            used.SetActive(!value);
        }
    }


    private GameObject ready;
    private GameObject used;

    void Awake()
    {
        ready = transform.FindPrecise("DashFull").gameObject;
        used = transform.FindPrecise("DashUsed").gameObject;
    }
}
