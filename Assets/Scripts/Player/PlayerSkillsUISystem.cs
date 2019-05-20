using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 * History:
 * 
 * Date         Author      Description
 * 
 * 15.03.2019   aknorre     Created
 * 16.03.2019   bkrylov     Allocated to Component Menu
 */
[AddComponentMenu("ProjectFaceless/Player/PlayerSkillsUISystem")]
public class PlayerSkillsUISystem : MonoBehaviour
{

    // Public

    [Header("UI Settings")]

    public float dragDistanceUpperBound = 60.0f;
    public float unbindDragDistanceLowerBound = 60.0f;

    [Tooltip("SkillsUI object")]
    public GameObject skillsUI;

    public Text physicalPointCount;
    public Text magicalPointCount;

    public GameObject acceptOn;
    public GameObject acceptOff;
    public GameObject cancelOn;
    public GameObject cancelOff;

    public Button quit;

    public GameObject skillsNode;
    public GameObject selectionsNode;

    // Private

    // Cache

    private HealthSystem healthSystem;
    private PlayerCharacterController characterController;
    private PlayerCameraController cameraController;

    private PlayerSkillBook skillBook;
    private SkillSystem skillSystem;
    private XpSystem xpSystem;

    private List<KeyValuePair<Skill, SkillLearn>> slots = new List<KeyValuePair<Skill, SkillLearn>>();
    private List<SkillSelection> selections = new List<SkillSelection>();
    
    private SkillType? dragSkillType = null;
    private bool open = false;

    protected void Start()
    {
        healthSystem = GetComponent<HealthSystem>();
        characterController = GetComponent<PlayerCharacterController>();
        cameraController = GetComponent<PlayerCameraController>();
        skillBook = GetComponent<PlayerSkillBook>();
        skillSystem = GetComponent<SkillSystem>();
        xpSystem = GetComponent<XpSystem>();

        skillsUI.SetActive(false);

        foreach (var child in skillsNode.Children())
        {
            var component = child.GetComponent<SkillLearn>();
            slots.Add(new KeyValuePair<Skill, SkillLearn>(component.skill, component));
        }
        foreach (var child in selectionsNode.Children())
        {
            var component = child.GetComponent<SkillSelection>();
            selections.Add(component);
        }

        quit.onClick.AddListener(OnClose);
    }

    protected void OnOpen()
    {
        Debug.Assert(!open);

        skillsUI.SetActive(true);
        characterController.Freeze = true;
        cameraController.Freeze = true;

        open = true;
    }

    protected void OnClose()
    {
        Debug.Assert(open);

        skillsUI.SetActive(false);
        characterController.Freeze = false;
        cameraController.Freeze = false;

        open = false;
    }

    protected void Update()
    {
        if (InputManager.Released(InputAction.SkillMenu))
        {
            if (open) OnClose();
            else OnOpen();
        }

        if (open && InputManager.Released(InputAction.Escape))
        {
            OnClose();
        }

        if (open)
        {
            physicalPointCount.text = xpSystem.SwordPoints.ToString();
            magicalPointCount.text = xpSystem.MaskPoints.ToString();

            foreach (var slot in slots)
            {
                if (slot.Value.Drag)
                {
                    dragSkillType = slot.Key.SkillType();
                }
            }

            foreach (var selection in selections)
            {
                if (selection.Drag)
                {
                    dragSkillType = selection.SelectedSkill.Value.SkillType();
                }
            }

            foreach (var slot in slots)
            {
                if (slot.Value.Clicked)
                {
                    slot.Value.Clicked = false;

                    if (!skillBook.HasSkill(slot.Key))
                    {
                        if (slot.Key.SkillType() == SkillType.Physical)
                        {
                            if (xpSystem.SwordPoints > 0)
                            {
                                --xpSystem.SwordPoints;
                                skillBook.Learn(slot.Key);
                            }
                        }
                        else if (slot.Key.SkillType() == SkillType.Magical)
                        {
                            if (xpSystem.MaskPoints > 0)
                            {
                                --xpSystem.MaskPoints;
                                skillBook.Learn(slot.Key);
                            }
                        }
                    }
                }
                
                if (!skillBook.HasSkill(slot.Key))
                {
                    slot.Value.CurrentState = SkillLearn.State.Off;
                }
                else
                {
                    slot.Value.CurrentState = SkillLearn.State.On;

                    foreach (var selection in selections)
                    {
                        selection.Glow = selection.type == dragSkillType;
                    }

                    if (slot.Value.DropPosition.HasValue)
                    {
                        var dropPosition = slot.Value.DropPosition.Value;

                        var minDistance = float.MaxValue;
                        SkillSelection closest = null;
                        foreach (var selection in selections)
                        {
                            if (selection.type == dragSkillType)
                            {
                                var distance = Vector3.Distance(selection.transform.position, dropPosition);
                                if (distance < dragDistanceUpperBound && distance < minDistance)
                                {
                                    minDistance = distance;
                                    closest = selection;
                                }
                            }
                        }

                        slot.Value.DropPosition = null;
                        dragSkillType = null;

                        if (closest)
                        {
                            Debug.Log("Drag distance = " + minDistance.ToString());

                            closest.SelectedSkill = slot.Key;
                            skillBook.Bind(closest.type, closest.slotNumber, slot.Key);
                        }
                    }
                }
            }

            foreach (var selection in selections)
            {
                if (selection.DropPosition.HasValue)
                {
                    var dropPosition = selection.DropPosition.Value;
                    if (Vector3.Distance(dropPosition, selection.StartPosition) >= unbindDragDistanceLowerBound)
                    {
                        var minDistance = float.MaxValue;
                        SkillSelection closest = null;
                        foreach (var selectionIt in selections)
                        {
                            if (selectionIt.type == dragSkillType)
                            {
                                var distance = Vector3.Distance(selectionIt.transform.position, dropPosition);
                                if (distance < dragDistanceUpperBound && distance < minDistance)
                                {
                                    minDistance = distance;
                                    closest = selectionIt;
                                }
                            }
                        }

                        if (closest)
                        {
                            var temp = selection.SelectedSkill;
                            selection.SelectedSkill = closest.SelectedSkill;
                            closest.SelectedSkill = temp;

                            skillBook.Bind(selection.type, selection.slotNumber, selection.SelectedSkill);
                            skillBook.Bind(closest.type, closest.slotNumber, closest.SelectedSkill);
                        }
                        else
                        {
                            selection.SelectedSkill = null;
                            skillBook.Bind(selection.type, selection.slotNumber, null);
                        }

                    }

                    selection.DropPosition = null;
                    dragSkillType = null;
                }
            }

        }

    }

}
