using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace Tests
{
    public class NPCSkillTests
    {

        private readonly System.Random random = new System.Random();

        [UnityTest]
        public IEnumerator CastRandomly()
        {

            SceneManager.LoadScene("GameSasha");
            yield return new WaitForFixedUpdate();

            var player = GameObject.Find("Player");

            var sheathSystem = player.GetComponent<SheathSystem>();
            sheathSystem.Unsheathe();

            while (sheathSystem.Sheathed || sheathSystem.Busy)
            {
                yield return new WaitForFixedUpdate();
            }

            var aimSystem = player.GetComponent<AimSystem>();
            aimSystem.Aim = player.transform.rotation;

            var skillSystem = player.GetComponent<SkillSystem>();
            var times = 10;
            for (var i = 0; i < times;)
            {
                if (skillSystem.Busy)
                {
                    yield return new WaitForSeconds(0.1f);
                    continue;
                }

                skillSystem.SelectSkill(skillSystem.Skills[random.Next(skillSystem.Skills.Count)]);
                if (!skillSystem.IsSkillSelected)
                {
                    yield return new WaitForSeconds(0.1f);
                    continue;
                }

                skillSystem.Cast();
                ++i;
                yield return new WaitForSeconds(0.1f);
            }

        }

    }
}
