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
 * 19.05.2019   mbukhalov   Added active portals saving
 * 
 */


public class PortalUIComponent : MonoBehaviour, ISaveable
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

    [Tooltip("Active portals")]
    [Saveable]
    public List<bool> isActive;

    [Tooltip("Exit key")]
    public KeyCode exitKey;

    [Header("Debug")]

    [Tooltip("Portal activation keys")]
    public KeyCode[] activation;

    [Tooltip("Portal inactivation keys")]
    public KeyCode[] inactivation;

    private SaveSystem saveSystem;

    PlayerCameraController playerCameraController;
    PlayerCharacterController playerCharacterController;
    bool canAppear = true;
    int portalCount;

    private PortalSystem portalSystem;

    public bool IsAllowedToAppear()
    {
        return canAppear;
    }

    private void Awake()
    {
        portalSystem = GameObject.Find("PortalSystem").GetComponent<PortalSystem>();
        portalCount = isActive.Count;
    }

    void Start()
    {
        playerCameraController = gameObject.GetComponent<PlayerCameraController>();
        playerCharacterController = gameObject.GetComponent<PlayerCharacterController>();

        activePortals[0].GetComponent<Button>().onClick.AddListener(() => teleport(0));
        activePortals[1].GetComponent<Button>().onClick.AddListener(() => teleport(1));
        //activePortals[2].GetComponent<Button>().onClick.AddListener(() => teleport(2));
        //activePortals[0].GetComponent<Button>().onClick.AddListener(() => teleport(3));
        //activePortals[1].GetComponent<Button>().onClick.AddListener(() => teleport(4));
        //activePortals[2].GetComponent<Button>().onClick.AddListener(() => teleport(5));
        for (int i = 0; i < portalCount; i++)
        {
            if (isActive[i] == true)
            {
                SetActive(i);
            }
            else
            {
                SetInactive(i);
            }
        }
    }

    void Update()
    {
        if (portalMenu.activeSelf && Input.GetKeyDown(exitKey))
        {
            portalMenu.SetActive(false);
            playerCharacterController.Freeze = false;
            playerCameraController.Freeze = false;
        }
        //for(int i = 0; i < portalCount; i++)
        //{
        //    if (Input.GetKeyDown(activation[i]) == true)
        //    {
        //        SetActive(i);
        //    }
        //    if(Input.GetKeyDown(inactivation[i]) == true)
        //    {
        //        SetInactive(i);
        //    }
        //}
    }

    public void ShowPortalUI()
    {
        if (canAppear)
        {
            canAppear = false;
            portalMenu.SetActive(true);
        }
    }

    public void AllowToAppear()
    {
        canAppear = true;
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
        playerCharacterController.Freeze = false;
        playerCameraController.Freeze = false;
        portalSystem.teleport(portal);
        portalMenu.SetActive(false);
    }

    public void OnSave()
    {
       // throw new System.NotImplementedException();
    }

    public void OnLoad()
    {
      //  throw new System.NotImplementedException();
    }
}

