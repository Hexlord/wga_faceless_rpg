using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SkillSelection : MonoBehaviour, IDragHandler, IEndDragHandler
{

    public SkillType type = SkillType.Physical;
    public int slotNumber = 0;

    private Skill? selectedSkill = null;
    private GameObject lastSelected = null;

    public bool Drag = false;
    public Vector3? DropPosition = null;

    private Vector3 startPosition;
    public Vector3 StartPosition
    {
        get { return startPosition; }
    }

    public Skill? SelectedSkill
    {
        get { return selectedSkill;}
        set
        {
            if (lastSelected)
            {
                lastSelected.SetActive(false);
            }

            selectedSkill = value;
            if (selectedSkill.HasValue)
            {
                var type = selectedSkill.Value;
                var node = transform.FindPrecise(type.ToString() + "_OnSelected");
                lastSelected = node ? node.gameObject : null;
                if (lastSelected != null) lastSelected.SetActive(true);
            }
        }
    }

    public bool Glow
    {
        set
        {
            black.SetActive(!value);
            white.SetActive(value);
        }
    }

    private GameObject black;
    private GameObject white;

    // Start is called before the first frame update
    private void Awake()
    {
        black = transform.FindPrecise("SkillsBorderBlack").gameObject;
        white = transform.FindPrecise("SkillsBorderWhite").gameObject;

        Glow = false;

        startPosition = transform.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (SelectedSkill == null)
        {
            return;
        }

        transform.position = Input.mousePosition;
        Drag = true;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        DropPosition = transform.position;

        transform.position = startPosition;
        Drag = false;
    }
}
