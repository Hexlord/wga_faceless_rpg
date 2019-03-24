using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldSystem : MonoBehaviour
{
    public GameObject Shield;

    public float shieldHP = 400.0f;

    public bool isRaised = false;


    // Start is called before the first frame update
    void Start()
    {
        Shield.SetActive(isRaised);
    }

    // Update is called once per frame
    void Update()
    {
        if (isRaised)
        {

        }
    }

    public void ShieldBreak()
    {

    }
}
