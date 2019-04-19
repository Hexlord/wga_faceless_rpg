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
[AddComponentMenu("ProjectFaceless/Player/PlayerSkillsUISystem")]
public class PlayerSkillsUISystem : MonoBehaviour
{

    // Public

    [Header("UI Settings")]


    [Tooltip("SkillsUI object")]
    public GameObject skillsUI;

    // Private

    // Cache

    private GameObject player;
    private HealthSystem playerHealthSystem;
    private PlayerCharacterController playerCharacterController;
    private PlayerCameraController playerCameraController;

    private bool open = false;

    protected void Start()
    {
        if (!skillsUI)
        {
            skillsUI = GameObject.Find("UI").FindPrecise("Canvas").transform.Find("SkillsUI").gameObject;
        }

        player = GameObject.Find("Player");
        playerHealthSystem = player.GetComponent<HealthSystem>();
        playerCharacterController = player.GetComponent<PlayerCharacterController>();
        playerCameraController = player.GetComponent<PlayerCameraController>();

        skillsUI.SetActive(false);

    }

    protected void OnOpen()
    {
        Debug.Assert(!open);

        skillsUI.SetActive(true);
        playerCharacterController.Freeze = true;
        playerCameraController.Freeze = true;

        open = true;
    }

    protected void OnClose()
    {
        Debug.Assert(open);

        skillsUI.SetActive(false);
        playerCharacterController.Freeze = false ;
        playerCameraController.Freeze = false;

        open = false;
    }

    protected void Update()
    {
        if (InputManager.Released(InputAction.SkillMenu))
        {
            if (open) OnClose();
            else OnOpen();
        }

    }

}
