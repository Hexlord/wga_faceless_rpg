using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[AddComponentMenu("ProjectFaceless/Tests/Navigation System Test")]
public class NavigationTest : CollectiveAISystem
{
    private float frequency = 0.5f;
    private float t = 0.0f;
    public Transform aim;
    NavigationSystem navSystem;
    // Start is called before the first frame update

    // Update is called once per frame
    void Update()
    {
        t += Time.deltaTime;
        if (t > frequency)
        {
            t = 0;
            foreach (BaseAgent a in agents)
            {
                navSystem.PlacePathRequest(a, aim.position);
            }
        }
    }
}
