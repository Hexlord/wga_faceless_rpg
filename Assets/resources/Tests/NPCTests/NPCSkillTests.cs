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

            player.GetComponent<PlayerCameraController>().Freeze = true;
            player.GetComponent<PlayerCharacterController>().Freeze = true;

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

                skillSystem.SelectSkill(random.Next(skillSystem.Skills.Count));
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


        [UnityTest]
        public IEnumerator CastToKill()
        {

            SceneManager.LoadScene("FightTest");
            yield return new WaitForFixedUpdate();

            var player = GameObject.Find("Player");

            player.GetComponent<PlayerCameraController>().Freeze = true;
            player.GetComponent<PlayerCharacterController>().Freeze = true;

            var sheathSystem = player.GetComponent<SheathSystem>();
            sheathSystem.Unsheathe();

            var bodyStateSystem = player.GetComponent<BodyStateSystem>();

            while (sheathSystem.Sheathed || sheathSystem.Busy)
            {
                yield return new WaitForFixedUpdate();
            }

            var aimSystem = player.GetComponent<AimSystem>();
            aimSystem.Aim = player.transform.rotation;

            var skillSystem = player.GetComponent<SkillSystem>();
            const int times = 10;
            for (var i = 0; i < times;)
            {
                if (skillSystem.Busy)
                {
                    skillSystem.Interrupt();
                    yield return new WaitForSeconds(0.5f);
                    continue;
                }

                var skill = i % skillSystem.Skills.Count;

                bodyStateSystem.ChangeState(skill >= 2
                    ? BodyStateSystem.BodyState.Magical
                    : BodyStateSystem.BodyState.Physical);

                skillSystem.SelectSkill(skill);
                if (!skillSystem.IsSkillSelected)
                {
                    yield return new WaitForSeconds(0.1f);
                    continue;
                }

                skillSystem.Cast();
                ++i;
                yield return new WaitForSeconds(0.1f);
            }

            var enemies = GameObject.Find("Enemies").Children("Faceless");
            foreach (var enemy in enemies)
            {
                Debug.Assert(enemy.GetComponent<HealthSystem>().Dead, "Enemy is not dead");
            }
        }

    }
}
