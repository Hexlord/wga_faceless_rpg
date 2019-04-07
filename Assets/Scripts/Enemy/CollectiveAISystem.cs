using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectiveAISystem : MonoBehaviour
{
    public NavigationSystem navSystem;
    public BaseAgent[] agents;
    private Dictionary<uint, BaseAgent> agentsDictionary = new Dictionary<uint, BaseAgent>();

    private void Awake()
    {
        foreach(BaseAgent agent in agents)
        {
            agent.SetControllingSystems(this, navSystem);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
