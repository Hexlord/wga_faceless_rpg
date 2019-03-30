using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 * History:
 * 
 * Date         Author      Description
 * 
 * 15.03.2019   aknorre     Created
 * 16.03.2019   bkrylov     Allocated to Component Menu
 */
[AddComponentMenu("ProjectFaceless/Player/PlayerDeathUISystem")]
public class PlayerDeathUISystem : MonoBehaviour
{

    // Public

    [Header("UI Settings")]


    [Tooltip("DeathUI object")]
    public GameObject deathUI;

    // Private

    // Cache

    private GameObject player;
    private HealthSystem playerHealthSystem;
    private PlayerCharacterController playerCharacterController;
    private PlayerCameraController playerCameraController;

    protected void Start()
    {
        if (!deathUI)
        {
            deathUI = GameObject.Find("UI").FindPrecise("Canvas").transform.Find("DeathUI").gameObject;
        }

        player = GameObject.Find("Player");
        playerHealthSystem = player.GetComponent<HealthSystem>();
        playerCharacterController = player.GetComponent<PlayerCharacterController>();
        playerCameraController = player.GetComponent<PlayerCameraController>();

        deathUI.SetActive(false);

    }

    protected void Update()
    {
        if (playerHealthSystem.Dead)
        {
            deathUI.SetActive(true);
            playerCharacterController.Freeze = true;
            playerCameraController.Freeze = true;

        }

    }

}
