using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 * History:
 * 
 * Date         Author      Description
 * 
 * 10.03.2019   aknorre     Created
 * 16.03.2019   bkrylov     Allocated to Component Menu
 * 
 */
[AddComponentMenu("ProjectFaceless/NPC/NPC")]
public class NPC : MonoBehaviour
{
    

    public enum DialogAction
    {
        None,
        EndDialog,
        Death
    }


    [Serializable]
    public class DialogEntry
    {
        public Button trigger;
        public GameObject resultNode;
        public DialogAction action;
    }


    [Header("Camera Settings")]
    [Tooltip("Select NPC camera")]
    public ConstrainedCamera dialogCamera;

    [Header("Activation Settings")]

    [Tooltip("Activate when game object in that range")]
    public float activationRange = 10.0f;

    [Tooltip("Select dialog root node")]
    public GameObject activationNode;

    [Tooltip("Select dialog talk node")]
    public GameObject talkNode;

    [Header("Basic Settings")]
    [Tooltip("Starting dialog")]
    public GameObject startNode;

    [Tooltip("Dialog list")]
    public DialogEntry[] dialogEntries;

    // Private

    private GameObject player;
    private PlayerCharacterController playerCharacterController;
    private PlayerCameraController playerCameraController;

    private ConstrainedCamera dialogStartCamera;

    private GameObject currentNode;
    private bool inRange = false;
    private bool inDialog = false;

    void Start()
    {
        player = GameObject.Find("Player");
        playerCharacterController = player.GetComponent<PlayerCharacterController>();
        playerCameraController = player.GetComponent<PlayerCameraController>();

        currentNode = startNode;

        foreach (DialogEntry entry in dialogEntries)
        {
            entry.trigger.onClick.AddListener(
                () =>
            {
                currentNode.SetActive(false);
                if (entry.resultNode)
                {
                    currentNode = entry.resultNode;
                    currentNode.SetActive(true);
                }
                OnAction(entry.action);
            });
        }
    }

    void OnAction(DialogAction action)
    {
        switch (action)
        {
            case DialogAction.None:
                break;
            case DialogAction.EndDialog:
                if (inDialog) OnDialogEnd();
                break;
            case DialogAction.Death:
                if (inDialog) OnDialogEnd();
                player.GetComponent<HealthSystem>().Kill(gameObject);
                break;
        }
    }

    protected void OnDialogStart()
    {
        Debug.Assert(!inDialog);
        playerCharacterController.Freeze = true;
        dialogStartCamera = playerCameraController.constrainedCamera;
        if (dialogCamera) playerCameraController.ChangeCamera(dialogCamera);
        activationNode.SetActive(true);
        currentNode.SetActive(true);
        inDialog = true;

    }
    protected void OnDialogEnd()
    {
        Debug.Assert(inDialog);
        playerCharacterController.Freeze = false;
        playerCameraController.ChangeCamera(dialogStartCamera);
        currentNode.SetActive(false);
        activationNode.SetActive(false);
        inDialog = false;
    }

    void Update()
    {
        bool inRangeCurrent = Vector3.Distance(player.transform.position, transform.position) < activationRange;

        if (!inRange && inRangeCurrent)
        {
            inRange = true;
            if (talkNode) talkNode.SetActive(true);
        }

        if (inRange && InputManager.GetInput(InputAction.Use))
        {
            if (!inDialog) OnDialogStart();

        }

        if (!inRangeCurrent)
        {
            inRange = false;
            if (inDialog) OnDialogEnd();

            if (talkNode) talkNode.SetActive(false);
        }
    }
}
