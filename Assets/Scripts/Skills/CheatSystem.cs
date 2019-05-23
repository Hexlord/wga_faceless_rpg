using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheatSystem : MonoBehaviour
{
    private bool first = true;
    private int current = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (InputManager.Released(InputAction.Cheat))
        {

            var player = GameObject.Find("Player");


            GameObject target = null;
            if (current == 0)
            {
                target = GameObject.Find("IslandSpawn");
            }
            else if (current == 1)
            {

                target = GameObject.Find("HubSpawn");
            }
            else if (current == 2)
            {

                target = GameObject.Find("ShipSpawn");
            }
            else
            {

                target = GameObject.Find("ArenaSpawn");
            }

            current = (current + 1) % 4;


            player.transform.position =
                target.transform.position;

            if (first)
            {
                player.GetComponent<XpSystem>().MaskPoints += 3;
                player.GetComponent<XpSystem>().SwordPoints += 3;
                player.GetComponent<ConcentrationSystem>().Concentration = 90.0f;
                //player.GetComponent<HealthSystem>().Heal(player, 99999.0f);

                first = false;
            }
        }
    }
}
