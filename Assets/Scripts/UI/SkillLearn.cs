using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SkillLearn : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    public Skill skill;

    public enum State
    {
        Off,
        On,
        Selected
    }

    private GameObject oldState = null;

    public bool Clicked = false;
    public bool Drag = false;
    public Vector3? DropPosition = null;

    private Vector3 startPosition;

    private State currentState = State.Off;
    public State CurrentState
    {
        get { return currentState; }
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

            currentState = value;
        }
    }

    public bool Hover
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
        startPosition = transform.position;

        black = transform.FindPrecise("SkillsBorderBlack").gameObject;
        white = transform.FindPrecise("SkillsBorderWhite").gameObject;

        Hover = false;
        Drag = false;
    }

    // Update is called once per frame
    private void Update()
    {
        
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Hover = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Hover = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (CurrentState == State.Off)
        {
            return;
        }
        transform.position = Input.mousePosition;
        Drag = true;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!Drag) return;

        DropPosition = transform.position;
        transform.position = startPosition;
        Drag = false;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Clicked = true;
    }
}
