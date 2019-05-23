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


            var target = (current == 0)
                ? GameObject.Find("IslandSpawn")
                : GameObject.Find("HubSpawn");

            current = (current + 1) % 2;


            player.transform.position =
                target.transform.position;

            if (first)
            {
                player.GetComponent<XpSystem>().MaskPoints += 3;
                player.GetComponent<XpSystem>().SwordPoints += 3;
                player.GetComponent<ConcentrationSystem>().Concentration = 90.0f;
                player.GetComponent<HealthSystem>().Heal(player, 99999.0f);
                player.GetComponent<PlayerSkillBook>().Learn(Skill.SkillSpecial1);

                first = false;
            }
        }
    }
}
