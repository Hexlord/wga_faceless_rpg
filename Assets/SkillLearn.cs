using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillLearn : MonoBehaviour
{
    public Skill skill;

    public enum State
    {
        Off,
        On,
        Selected
    }

    private GameObject oldState = null;

    public State CurrentState
    {
        set
        {
            if (oldState != null) oldState.SetActive(false);

            switch (value)
            {
                case State.Off:
                    oldState = transform.FindPrecise(skill.ToString() + "_Off").gameObject;
                    break;
                case State.On:
                    oldState = transform.FindPrecise(skill.ToString() + "_On").gameObject;
                    break;
                case State.Selected:
                    oldState = transform.FindPrecise(skill.ToString() + "_OnSelected").gameObject;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("value", value, null);
            }

            if (oldState != null) oldState.SetActive(true);
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
