using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalComponent : MonoBehaviour
{
    // Start is called before the first frame update

    [Tooltip("Spawn transform")]
    public Transform spawn;

    private PortalUIComponent portalUI;
    void Start()
    {
        portalUI = GameObject.Find("UI").GetComponent<PortalUIComponent>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger");
        portalUI.ShowPortalUI();
    }
}
