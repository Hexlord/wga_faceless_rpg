using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 * History:
 * 
 * Date         Author      Description
 * 
 * 10.03.2019   aknorre     Created
 * 16.03.2019   bkrylov     Allocated to Component Menu
 * 19.05.2019   mbukhalov   Added stage offset saving
 * 
 */
[AddComponentMenu("ProjectFaceless/NPC/NPC")]
public class NPC : MonoBehaviour
{
    public enum DialogAction
    {
        None,
        EndDialog,
        Death,
        IntroductionPhase1,
        IntroductionPhase2,
        Special1,
        Special2
    }

    [Serializable]
    public class DialogStage
    {
        public string text = "Sample NPC text";
        public DialogAnswer[] answers;
    }

    [Serializable]
    public class DialogAnswer
    {
        public string text = "Sample answer text";
        public int resultStage = 1;
        public DialogAction action;
    }


    [Header("Camera Settings")]
    [Tooltip("Select NPC camera")]
    public ConstrainedCamera dialogCamera;

    [Header("Activation Settings")]

    [Tooltip("Activate when game object in that range")]
    public float activationRange = 10.0f;

    [Tooltip("Starts talking automatically")]
    public bool autoTalk = false;

    [Tooltip("Select dialog panel node")]
    public GameObject panel;

    [Tooltip("Select press key to talk node")]
    public GameObject talkNode;

    [Header("Basic Settings")]
    [Tooltip("Starting stage of dialog")]
    public int startStage = 1;

    [Tooltip("Dialog stage list")]
    public DialogStage[] dialogStages;

    public DialogStage CurrentStage
    {
        get { return dialogStages[currentStageOffset]; }
    }

    // Private

    private Text panelText;
    private List<Button> answerButtons = new List<Button>();
    private List<Text> answerTexts = new List<Text>();

    private GameObject player;
    private PlayerCharacterController playerCharacterController;
    private PlayerCameraController playerCameraController;

    private ConstrainedCamera dialogStartCamera;
    [Saveable]
    private int currentStageOffset;
    private bool inRange = false;
    private bool inDialog = false;

    private static readonly Color goodColor = new Color(33, 238, 37);
    private static readonly Color goodDarkColor = new Color(0, 150, 0);

    private static readonly Color badColor = new Color(241, 50, 47);
    private static readonly Color badDarkColor = new Color(150, 0, 0);

    private void Awake()
    {
        player = GameObject.Find("Player");
        playerCharacterController = player.GetComponent<PlayerCharacterController>();
        playerCameraController = player.GetComponent<PlayerCameraController>();

        Debug.Assert(dialogStages.Length > 0, "NPC must have dialog stages");

        if (!panel)
        {
            panel = GameObject.Find("UI").FindPrecise("Canvas").FindPrecise("Dialog").FindPrecise("Panel");
        }
        Debug.Assert(panel);

        panelText = panel.FindPrecise("Text").GetComponent<Text>();

        var i = 1;
        while (true)
        {
            var answer = panel.FindPrecise("Answer" + i);
            if (!answer) break;

            var button = answer.GetComponent<Button>();
            var offset = i - 1;
            button.onClick.AddListener(() =>
            {
                if (!inDialog) return;
                OnAnswer(offset);
            });

            answerButtons.Add(button);
            answerTexts.Add(answer.FindPrecise("Text").GetComponent<Text>());
            ++i;
        }

        // Designers start counting from 1
        // Arrays offset from 0
        currentStageOffset = startStage - 1;
        panel.SetActive(false);
    }

    private void LoadStage(int offset)
    {
        currentStageOffset = offset;

        var stage = dialogStages[currentStageOffset];
        panelText.text = stage.text;
        Debug.Assert(stage.answers.Length <= answerButtons.Count);

        // Update visuals
        for (var i = 0; i < answerButtons.Count; ++i)
        {
            var button = answerButtons[i];
            var text = answerTexts[i];

            if (stage.answers.Length <= i)
            {
                // Excessive option
                button.gameObject.SetActive(false);
                continue;
            }
            button.gameObject.SetActive(true);

            var answer = stage.answers[i];
            
            var colors = button.colors;
            var bad = IsBad(answer.action);
            colors.highlightedColor = bad
                ? badColor
                : goodColor;
            colors.selectedColor = bad
                ? badColor
                : goodColor;
            colors.pressedColor = bad
                ? badDarkColor
                : goodDarkColor;

            button.colors = colors;

            text.text = answer.text;
        }
    }

    private static bool IsBad(DialogAction action)
    {
        return action == DialogAction.Death;
    }

    public void OnAnswer(int offset)
    {
        var stage = dialogStages[currentStageOffset];
        var answer = stage.answers[offset];
        var newStage = answer.resultStage - 1;
        
        if (newStage != currentStageOffset) LoadStage(newStage);
        
        OnAction(answer.action);
    }

    private void OnAction(DialogAction action)
    {
        switch (action)
        {
            case DialogAction.None:
                break;
            case DialogAction.EndDialog:
                if (inDialog) OnDialogEnd();
                break;
            case DialogAction.Death:
                if (inDialog) OnDialogEnd();
                player.GetComponent<HealthSystem>().Kill(gameObject);
                break;
            case DialogAction.IntroductionPhase1:
                player.GetComponent<SheathSystem>().canUnsheathe = true;
                player.GetComponent<AttackSystem>().canAttack = true;
                if (inDialog) OnDialogEnd();
                break;
            case DialogAction.IntroductionPhase2:
                player.GetComponent<PlayerCharacterController>().CanChangeBodyState = true;
                if (inDialog) OnDialogEnd();
                break;
            case DialogAction.Special1:
                player.GetComponent<PlayerSkillBook>().Learn(Skill.SkillSpecial1);
                if (inDialog) OnDialogEnd();
                break;
            case DialogAction.Special2:
                player.GetComponent<PlayerSkillBook>().Learn(Skill.SkillSpecial2);
                if (inDialog) OnDialogEnd();
                break;
            default:
                throw new ArgumentOutOfRangeException("action", action, null);
        }
    }

    protected void OnDialogStart()
    {
        Debug.Assert(!inDialog);
        playerCharacterController.Freeze = true;
        dialogStartCamera = playerCameraController.constrainedCamera;
        if (dialogCamera) playerCameraController.ChangeCamera(dialogCamera);

        LoadStage(currentStageOffset);
        panel.SetActive(true);
        inDialog = true;

    }
    protected void OnDialogEnd()
    {
        Debug.Assert(inDialog);

        if (playerCameraController.ConstrainedCamera == dialogCamera)
        {
            playerCharacterController.Freeze = false;
            playerCameraController.ChangeCamera(dialogStartCamera);
        }

        panel.SetActive(false);
        inDialog = false;
    }

    private void Update()
    {
        var inRangeCurrent = Vector3.Distance(player.transform.position, transform.position) < activationRange;
        
        if (!inRange && inRangeCurrent)
        {
            inRange = true;
        }

        // Either just entered the range or pressed key while in range
        if (inRange && autoTalk || inRangeCurrent && InputManager.Pressed(InputAction.Use))
        {
            if (!inDialog) OnDialogStart();
        }

        if (!inRangeCurrent)
        {
            inRange = false;
            if (inDialog) OnDialogEnd();
        }

        if (talkNode) talkNode.SetActive(!inDialog && inRangeCurrent);
    }
}
