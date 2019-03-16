using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/*
 * History:
 * 
 * Date         Author      Description
 * 
 * 03.03.2019   aknorre     Created
 * 16.03.2019   bkrylov     Allocated to Component Menu
 * 
 */

public class EndScript : MonoBehaviour
{
    [AddComponentMenu("ProjectFaceless/UI")]
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter(Collision collision)
    {
        if(collision.transform.IsChildOf(GameObject.Find("Player").transform))
        {
            SceneManager.LoadScene("End", LoadSceneMode.Single);
        }
    }

}
