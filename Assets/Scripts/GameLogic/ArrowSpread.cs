using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowSpread : MonoBehaviour
{

    public Transform[] arrows;
    public float RingAngle = 5.0f;
    private int firstRingIndex, secondRingIndex, thirdRingIndex;
    private int index;
    private Vector3 storedPosition;
    private Vector3 delta, shooting;
    private int ring;
    
    // Start is called before the first frame update
    void Start()
    {
        ring = (arrows.Length - 1) / 3;
        firstRingIndex = 1;
        secondRingIndex = 1 + ring;
        thirdRingIndex = secondRingIndex + ring;
        delta = arrows[1].position - transform.position;
        for (index = firstRingIndex; index < secondRingIndex; index++)
        {
            
        }
        
        for (index = secondRingIndex; index < thirdRingIndex; index++)
        {
            
        }
        
        for (index = thirdRingIndex; index < arrows.Length; index++)
        {
            
        }
    }

    // Update is called once per frame
    void Update()
    {
        delta = transform.position - storedPosition;
        shooting = transform.up * (Mathf.Sin(RingAngle) * delta.magnitude);
        for (index = firstRingIndex; index < secondRingIndex; index++)
        {

        }
        
        for (index = secondRingIndex; index < thirdRingIndex; index++)
        {
            
        }
        
        for (index = thirdRingIndex; index < arrows.Length; index++)
        {
            
        }
    }
}
