using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour {

    [SerializeField]
    private Transform wielder;

    [SerializeField]
    private string targetTag;

    [SerializeField]
    private bool isStriking = false;

    [SerializeField]
    private float damage, concentration;


    private void Awake()
    {
        wielder = transform.parent;

        if (wielder != null) IgnoreCollisionsWithWielder();
    }

    public void SetWielder(Transform value)
    {
        wielder = value;

        IgnoreCollisionsWithWielder();
    }

    public bool CompareToWielderTag(string value)
    {
        return value == wielder.tag;
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
                    col.attachedRigidbody.gameObject.GetComponent<BasicStatusSystem>().DealDamage(damage);
                    isStriking = false;
                    if (wielder != null && wielder.tag == "Player")
                    {
                        try
                        {
                            wielder.GetComponent<PlayerStatusSystem>().StoreConcentration(concentration);
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
        if (col.tag != wielder.tag) Strike(col);
    }

    private void IgnoreCollisionsWithWielder()
    {
        GameObject[] wielderObjects = GameObject.FindGameObjectsWithTag(wielder.tag);
        Collider col1 = gameObject.GetComponent<Collider>();
        if (col1 != null)
        {
            foreach (GameObject obj in wielderObjects)
            {
                Collider col2 = obj.GetComponent<Collider>();
                if (col2 != null) Physics.IgnoreCollision(col1, col2);

            }
        }
    }
}
