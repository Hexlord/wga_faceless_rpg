using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectibleSystem : MonoBehaviour
{

    public float xpGain = 0.0f;
    public int maskGain = 0;

    private TouchCondition touchCondition;

    void Awake()
    {
        touchCondition = transform.GetComponent<TouchCondition>();
    }

    void FixedUpdate()
    {
        if (touchCondition.Touch)
        {
            var xpSystem = touchCondition.TouchObject.GetComponent<XpSystem>();
            xpSystem.GrantXp(xpGain);
            xpSystem.GrantMask(maskGain);
            Destroy(gameObject);
        }
    }
}
