using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour {

    Vector3 startingPos;
    Quaternion startingRot;
    Animator anim;
    public bool isBlocking;
    bool isSprinting;
    public enum CharacterState {SheathedSword, SwordStance, MagicStance};
    public GameObject SwordParticles, MaskParticles;
    private CharacterState currentState = CharacterState.SheathedSword;


    public bool IsSprinting
    {
        get
        {
            return isSprinting;
        }
        set 
        {
            isSprinting = value;
            anim.SetBool("isSprinting", value);
        }
    }

    public CharacterState Status
    {
        get
        {
            return currentState;
        }
    }

	// Use this for initialization
	void Start ()
    {
        anim = GetComponent<Animator>();
        startingPos = transform.position;
        startingRot = transform.rotation;
        currentState = CharacterState.SheathedSword;
        SwordParticles.SetActive(false);
        MaskParticles.SetActive(false);
    }
	
	// Update is called once per frame
	void Update ()
    {
        
	}

    public void SwapPlayerStatus()
    {
       switch (currentState)
       {
            case CharacterState.SheathedSword:
                {
                    DrawSword();
                    //currentState = CharacterState.MagicStance;
                    break;
                }
            case CharacterState.MagicStance:
                {
                    MaskParticles.SetActive(false);
                    SwordParticles.SetActive(true);
                    currentState = CharacterState.SwordStance;
                    break;
                }
            case CharacterState.SwordStance:
                {
                    SwordParticles.SetActive(false);
                    MaskParticles.SetActive(true);
                    currentState = CharacterState.MagicStance;
                    break;
                }
        }


    }

    public void DrawSword()
    {
        if (currentState == CharacterState.SheathedSword)
        {
            SwordParticles.SetActive(true);
            MaskParticles.SetActive(false);
            anim.SetTrigger("UnsheatheSword");
            currentState = CharacterState.SwordStance;
        }
        else
        {
            if (currentState == CharacterState.SwordStance || currentState == CharacterState.MagicStance)
            {
                SwordParticles.SetActive(false);
                MaskParticles.SetActive(false);
                anim.SetTrigger("SheatheSword");
                currentState = CharacterState.SheathedSword;
            }
        }
    }

    public void SwingSword()
    {
        anim.SetTrigger("Swing");
    }

}
