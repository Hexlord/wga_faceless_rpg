using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnOnLoadSystem : MonoBehaviour, ISaveable
{
    [SerializeField]
    public GameObject spawnPoint;
    public void OnLoad()
    {
        gameObject.transform.position = spawnPoint.transform.position;
        gameObject.transform.rotation = spawnPoint.transform.rotation;
    }

    public void OnSave()
    {
        //throw new System.NotImplementedException();
    }

    
    void Start()
    {
        if (!spawnPoint)
        {
            spawnPoint = GameObject.FindGameObjectWithTag("SpawnPoint");
        }
    }

    
    void Update()
    {
        
    }
}
