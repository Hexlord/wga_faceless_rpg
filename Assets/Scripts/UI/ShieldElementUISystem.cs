using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldElementUISystem : MonoBehaviour
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

    private void Awake()
    {
        ready = transform.FindPrecise("ShieldFullState").gameObject;
        used = transform.FindPrecise("ShieldBrokenState").gameObject;
    }
}
