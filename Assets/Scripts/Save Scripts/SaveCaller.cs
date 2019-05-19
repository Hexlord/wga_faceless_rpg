using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveCaller : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GameObject ss = GameObject.Find("SaveSystem");
        SaveSystem saveSystem = ss.GetComponent<SaveSystem>();
        if (saveSystem.isLoading == true)
        {
            saveSystem.Load();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
