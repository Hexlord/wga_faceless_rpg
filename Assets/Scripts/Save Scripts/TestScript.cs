using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour, ISaveable
{
    [Saveable("Position")]
    public Transform currentTransform;

    [Saveable("VeryImportantVector")]
    public Vector3 someVector;

    void Start()
    {
        someVector = new Vector3(-1, -2, -3);
        currentTransform = gameObject.transform;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnSave()
    {
        
    }

    public void OnLoad()
    {
        gameObject.transform.position = currentTransform.position;
        gameObject.transform.rotation = currentTransform.rotation;
        gameObject.transform.localScale = currentTransform.localScale;
    }
}
