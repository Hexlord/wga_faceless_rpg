using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseCharacter : MonoBehaviour
{

    private Vector3 spawnPosition;
    
    public void Spawn()
    {
        string name = gameObject.name;
        Instantiate(gameObject, spawnPosition, gameObject.transform.rotation).name = name;
    }

    public void Die()
    {
        Debug.Log("Died");
        Spawn();
        Destroy(gameObject);
    }

    void Start()
    {
        spawnPosition.Set(5.8f, 3.0f, 0.0f);
    }

    void FixedUpdate()
    {

    }
}


