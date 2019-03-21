using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour
{
    [Saveable("Position")]
    public Transform currentTransform;

    void Start()
    {
        currentTransform = gameObject.transform;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
