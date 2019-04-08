using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * History:
 * 
 * Date         Author      Description
 * 
 * 08.04.2019   aknorre     Created
 * 
 */
[AddComponentMenu("ProjectFaceless/Tools/Child Collection")]
public class ChildCollection : MonoBehaviour
{
    private GameObject[] childs = new GameObject[0];

    public GameObject[] Childs
    {
        get { return childs; }
    }

    private void FixedUpdate()
    {
        childs = gameObject.Children();
    }
}
