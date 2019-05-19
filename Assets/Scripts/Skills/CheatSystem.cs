using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheatSystem : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (InputManager.Pressed(InputAction.Cheat))
        {
            GameObject.Find("Player").transform.position =
                GameObject.Find("IslandSpawn").transform.position;
        }
    }
}
