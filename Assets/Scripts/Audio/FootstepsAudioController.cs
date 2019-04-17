using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootstepsAudioController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if((transform.parent.parent.GetComponent<MovementSystem>().Moving)&&(!GetComponent<AudioSource>().enabled))
        {
            GetComponent<AudioSource>().enabled = true;
        }
        if((!transform.parent.parent.GetComponent<MovementSystem>().Moving)&& (GetComponent<AudioSource>().enabled))
        {
            GetComponent<AudioSource>().enabled = false;
        }
    }
}
