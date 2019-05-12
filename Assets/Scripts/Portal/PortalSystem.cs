using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalSystem : MonoBehaviour
{
    [Tooltip("Portals")]
    public List<PortalComponent> portals;
    [Tooltip("Player")]
    public GameObject player;

    SaveSystem saveSystem;

    int portalCount;
    void Start()
    {
        saveSystem = GameObject.Find("SaveSystem").GetComponent<SaveSystem>();
        portalCount = portals.Count;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void teleport(int portal)
    {
        saveSystem.Save();
        player.transform.position = portals[portal].spawn.position;
        player.transform.rotation = portals[portal].spawn.rotation;

    }
}
