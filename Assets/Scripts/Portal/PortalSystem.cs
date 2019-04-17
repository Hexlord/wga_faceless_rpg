using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalSystem : MonoBehaviour
{
    [Tooltip("Portals")]
    public List<PortalComponent> portals;
    [Tooltip("Player")]
    public GameObject player;

    int portalCount;
    void Start()
    {
        portalCount = portals.Count;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void teleport(int portal)
    {
        player.transform.position = portals[portal].spawn.position;
        player.transform.rotation = portals[portal].spawn.rotation;
    }
}
