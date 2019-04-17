using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/*
 * History:
 * 
 * Date         Author      Description
 * 
 * 17.04.2019   mbukhalov   Created
 * 
 */


public class PortalUIComponent : MonoBehaviour
{
    [Header("Portal Menu Settings")]

    [Tooltip("Select portal menu node")]
    public GameObject portalMenu;

    [Tooltip("Select portal menu background")]
    public GameObject portalMenuBackground;

    [Tooltip("Active portals")]
    public List<GameObject> activePortals;

    [Tooltip("Inactive portals")]
    public List<GameObject> inactivePortals;

    [Saveable]
    [Tooltip("Active portals")]
    public List<bool> isActive;

    [Tooltip("Exit key")]
    public KeyCode exitKey;

    [Header("Debug")]

    [Tooltip("Portal activation keys")]
    public KeyCode[] activation;

    [Tooltip("Portal inactivation keys")]
    public KeyCode[] inactivation;

    private SaveSystem saveSystem;



    int portalCount;

    private PortalSystem portalSystem;

    private void Awake()
    {
        portalSystem = GameObject.Find("PortalSystem").GetComponent<PortalSystem>();
        portalCount = isActive.Count;
    }

    void Start()
    {
        activePortals[0].GetComponent<Button>().onClick.AddListener(() => teleport(0));
        activePortals[1].GetComponent<Button>().onClick.AddListener(() => teleport(1));
        activePortals[2].GetComponent<Button>().onClick.AddListener(() => teleport(2));
    }

    void Update()
    {
        if (portalMenu.activeSelf && Input.GetKeyDown(exitKey))
        {
            portalMenu.SetActive(false);
        }
        for(int i = 0; i < portalCount; i++)
        {
            if (Input.GetKeyDown(activation[i]) == true)
            {
                SetActive(i);
            }
            if(Input.GetKeyDown(inactivation[i]) == true)
            {
                SetInactive(i);
            }
        }
    }

    public void ShowPortalUI()
    {
        portalMenu.SetActive(true);
    }

    void SetActive(int portal)
    {
        if (portal >= portalCount || portal < 0)
        {
            throw new System.Exception("portal number out of range");
        }
        isActive[portal] = true;
        activePortals[portal].SetActive(true);
        inactivePortals[portal].SetActive(false);
        Debug.Log("Active " + portal.ToString());
    }

    void SetInactive(int portal)
    {
        if (portal >= portalCount || portal < 0)
        {
            throw new System.Exception("portal number out of range");
        }
        isActive[portal] = false;
        activePortals[portal].SetActive(false);
        inactivePortals[portal].SetActive(true);
        Debug.Log("Inactive " + portal.ToString());
    }

    void teleport(int portal)
    {
        if (portal >= portalCount || portal < 0)
        {
            throw new System.Exception("portal number out of range");
        }
        portalSystem.teleport(portal);
        portalMenu.SetActive(false);
    }

}

