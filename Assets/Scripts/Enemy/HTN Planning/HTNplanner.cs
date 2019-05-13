using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HTNplanner : MonoBehaviour
{
    
    public BaseAgent[] agents;
    public GameObject player;
    
    private NavigationSystem navSystem;
    private Dictionary<uint, BaseAgent> AgentIdDictionary = new Dictionary<uint, BaseAgent>();
    
    
    public enum AgentType
    {
        Base,
        Melee,
        Ranged,
        EliteMelee,
        EliteRanged,
    }
    
    public enum AttackTikets
    {
        None,
        MeleePhysical,
        MeleeMagical,
        MeleeCombined,
        RangedPhysical,
        RangedMagical,
        RangedCombined,
    }
    
    
    // Start is called before the first frame update
    void Awake()
    {
        navSystem = GetComponent<NavigationSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
