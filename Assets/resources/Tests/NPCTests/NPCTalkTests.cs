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

        private readonly System.Random random = new System.Random();

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
                    TeleportTo(player, npcList[current]);
                    yield return new WaitForFixedUpdate();
                }
            }

        }

        [UnityTest]
        public IEnumerator AnswerRandomly()
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

            const int times = 10;
            for (var i = 0; i < times; ++i)
            {
                if (current >= npcList.Count)
                {
                    current = 0;
                    npcList = npcList.OrderBy(x => Random.value).ToList();
                }

                var npc = npcList[current];
                var npcComponent = npc.GetComponent<NPC>();
                TeleportTo(player, npc);

                Debug.Log("Speaking with " + npc.name);
                var talk = true;
                var limit = 100;
                while (talk)
                {
                    yield return new WaitForSeconds(0.5f);

                    var length = npcComponent.CurrentStage.answers.Length;
                    var answerOffset = random.Next(length);
                    Debug.Assert(length > 0, "No answers available");
                    var action = npcComponent.CurrentStage.answers[answerOffset].action;
                    if (length == 1 && action == NPC.DialogAction.EndDialog)
                    {
                        Debug.Log("Only end dialog answer left, skipping npc after answer");
                        talk = false;
                    }

                    if (action == NPC.DialogAction.Death)
                    {
                        Debug.Log("Answer leading to death, stopping tests");
                        talk = false;
                        i = times;
                        continue;
                    }
                    npcComponent.OnAnswer(answerOffset);

                    --limit;
                    Debug.Assert(limit > 0, "Dialog infinite loop found");
                }

                ++current;
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
