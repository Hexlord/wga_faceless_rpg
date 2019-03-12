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
 * 
 */

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

    [Header("Activation Settings")]

    [Tooltip("Activate when game object in that range")]
    public float activationRange = 10.0f;

    [Tooltip("Select dialog root node")]
    public GameObject activationNode;

    [Header("Basic Settings")]
    [Tooltip("Starting dialog")]
    public GameObject startNode;

    [Tooltip("Dialog list")]
    public DialogEntry[] dialogEntries;

    private GameObject player;
    private GameObject currentNode;
    private bool inRange = false;
    
    void Start()
    {
        player = GameObject.Find("Player");
        currentNode = startNode;

        foreach (DialogEntry entry in dialogEntries)
        {
            entry.trigger.onClick.AddListener(
                () =>
            {
                currentNode.SetActive(false);
                if(entry.resultNode)
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
                currentNode.SetActive(false);
                activationNode.SetActive(false);
                // currentNode = startNode;
                break;
            case DialogAction.Death:
                player.GetComponent<HealthSystemWithConcentration>().Kill();
                break;
        }
    }

    void Update()
    {
        bool inRangeCurrent = Vector3.Distance(player.transform.position, transform.position) < activationRange;

        if(!inRange && inRangeCurrent)
        {
            inRange = true;
            activationNode.SetActive(true);
            currentNode.SetActive(true);
        }

        if(!inRangeCurrent)
        {
            inRange = false;
            currentNode.SetActive(false);
            activationNode.SetActive(false);
            // currentNode = startNode;
        }
    }
}
