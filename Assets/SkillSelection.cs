using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillSelection : MonoBehaviour
{
    public enum Type
    {
        Physical,
        Magical,
        Special
    }

    public Type type = Type.Physical;

    private Skill? selectedSkill = null;
    private GameObject lastSelected = null;

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
    }

    // Update is called once per frame
    private void Update()
    {
        
    }
}
