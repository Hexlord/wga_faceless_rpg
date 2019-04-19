using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestChangeTransform : MonoBehaviour
{
    // Start is called before the first frame update

    private Vector3 prevPos;
    private Vector3 prevScale;
    private Quaternion prevRot;
    private bool initial = true;

    void Start()
    {
        prevPos = new Vector3(10, 12, 15);
        prevRot = new Quaternion(111, 222, 333, 444);
        prevScale = new Vector3(9, 8, 7);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.N))
        {
            Vector3 tempPos, tempScale;
            Quaternion tempRot;

            tempPos = gameObject.transform.position;
            gameObject.transform.position = prevPos;
            prevPos = tempPos;

            tempRot = gameObject.transform.rotation;
            gameObject.transform.rotation = prevRot;
            prevRot = tempRot;

            tempScale = gameObject.transform.localScale;
            gameObject.transform.localScale = prevScale;
            prevScale = tempScale;

            initial = !initial;
            Debug.Log("Initial: " + initial.ToString());
        }
    }
}
