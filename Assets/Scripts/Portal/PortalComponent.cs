using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalComponent : MonoBehaviour
{
    // Start is called before the first frame update

    [Tooltip("Spawn transform")]
    public Transform spawn;

    [Tooltip("Player")]
    public GameObject player;

    private PortalUIComponent portalUI;

    PlayerCameraController playerCameraController;
    PlayerCharacterController playerCharacterController;
    void Start()
    {
        playerCameraController = player.GetComponent<PlayerCameraController>();
        playerCharacterController = player.GetComponent<PlayerCharacterController>();
        portalUI = player.GetComponent<PortalUIComponent>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && portalUI.IsAllowedToAppear())
        {
            playerCameraController.Freeze = true;
            playerCharacterController.Freeze = true;
            Debug.Log("Trigger");
            portalUI.ShowPortalUI();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
            portalUI.AllowToAppear();
    }
}
