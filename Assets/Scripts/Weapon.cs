using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour {

    private Transform wielder;
    private string targetTag;
    private bool isStriking = false;
    private float damage, concentration;


    private void Awake()
    {
        wielder = transform.parent;
    }

    public float Damage
    {
        set
        {
            damage = value;
        }
    }

    public float Concentration
    {
        set
        {
            concentration = value;
        }
    }

    public string TargetTag
    {
        set
        {
            try
            {
                targetTag = value;
            }
            catch
            {
                Debug.LogError("Something wrong with the tag \"" + value + "\"");
            }
        }
    }

    public void TriggerStricking()
    {
            isStriking = true;
    }

    public void Strike(Collider col)
    {
        if (isStriking)
        {
            if (col.tag == targetTag)
            {
                try
                {
                    col.attachedRigidbody.gameObject.GetComponent<HealthSystem>().DealDamage(damage);
                    isStriking = false;
                    if (wielder != null && wielder.tag == "Player")
                    {
                        try
                        {

                            wielder.GetComponent<ConcentrationSystem>().StoreConcentration(concentration);
                        }
                        catch
                        {
                            Debug.LogError("Concentration System is not attached");
                        }
                    }
                }
                catch
                {
                    Debug.LogError("NO HP SYSTEM ATTACHED TO GAMEOBJECT " + col.attachedRigidbody.gameObject.name);
                }
            }
            if (col.tag == "Shield")
            {
                col.attachedRigidbody.transform.parent.GetComponent<DefenseSystem>().Blocked();
                if (wielder != null)
                {
                    Animator anim;
                    if ((anim = wielder.GetComponent<Animator>()) != null)
                    {
                        anim.SetTrigger("Blocked");
                        isStriking = false;
                    }
                }
            }
        }
    }

    private void OnTriggerEnter(Collider col)
    {
        Strike(col);
    }
}
