using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace Tests
{
    public class NPCTalkTests
    {
        [UnityTest]
        public IEnumerator TalkRandomly()
        {
            SceneManager.LoadScene("GameSasha");
            yield return new WaitForFixedUpdate();

            var player = GameObject.Find("Player");
            var npcRoot = GameObject.Find("NPC");

            var npcList = new List<GameObject>();
            for (var i = 0; i < npcRoot.transform.childCount; ++i)
            {
                var go = npcRoot.transform.GetChild(i).gameObject;
                go.GetComponent<NPC>().autoTalk = true;
                npcList.Add(go);
            }

            // Shuffle
            npcList = npcList.OrderBy(x => Random.value).ToList();

            var current = 0;
            if (current >= npcList.Count) yield break;
            TeleportTo(player, npcList[current]);

            for (var i = 0; i < 1000; ++i)
            {
                yield return new WaitForFixedUpdate();

                if ((i + 1) % 100 == 0)
                {
                    ++current;
                    if (current >= npcList.Count) yield break;
                    TeleportTo(player, Vector3.zero);
                    yield return new WaitForFixedUpdate();
                    yield return new WaitForFixedUpdate();
                    TeleportTo(player, npcList[current]);
                    yield return new WaitForFixedUpdate();
                }
            }

        }

        private static void TeleportTo(GameObject player, GameObject target)
        {
            var body = player.GetComponent<Rigidbody>();
            var path = target.transform.position - player.transform.position;
            var direction = path.normalized;

            const float approachRange = 2.0f;
            var pathShortened = path - direction * approachRange;

            body.MovePosition(player.transform.position + pathShortened);
        }

        private static void TeleportTo(GameObject player, Vector3 position)
        {
            var body = player.GetComponent<Rigidbody>();
            body.MovePosition(position);
        }

    }
}
