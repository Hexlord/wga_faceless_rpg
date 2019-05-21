using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheatSystem : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (InputManager.Pressed(InputAction.Cheat))
        {
            var player = GameObject.Find("Player");

            player.transform.position =
                GameObject.Find("IslandSpawn").transform.position;
            player.GetComponent<XpSystem>().MaskPoints += 3;
            player.GetComponent<XpSystem>().SwordPoints += 3;
            player.GetComponent<ConcentrationSystem>().Concentration = 90.0f;
            player.GetComponent<HealthSystem>().Heal(player, 99999.0f);
            player.GetComponent<PlayerSkillBook>().Learn(Skill.SkillSpecial1);
        }
    }
}
