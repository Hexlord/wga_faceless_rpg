using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/*
 * History:
 * 
 * Date         Author      Description
 * 
 * 03.03.2019   aknorre     Created
 * 
 */

/*
 * Skill casting is:
 * 1. Begin animation
 * 2. Channel or instant animation (perform)
 * 3. Return animation
 * 
 * caster is not cached because it could be a component, 
 * which could be added to scene after this class creation
 * 
 */
public class SkillBase
{

    private readonly GameObject textPrefab;

    public SkillBase(string name, bool channeling, float cooldawn)
    {
        this.name = name;
        this.channeling = channeling;
        this.cooldawn = cooldawn;
        this.cooldawnTimer = 0.0f;
        
        textPrefab = (GameObject)Resources.Load("Prefabs/FloatingText", typeof(GameObject));
    }

    public void Update(float delta)
    {
        cooldawnTimer -= delta;
    }
    
    /*
     * Called before casting starts
     */
    public virtual void PrepareEvent(GameObject caster)
    {
    }

    /*
     * time is how much time passed since cast start
     * length is begin animation length
     * 
     */
    public virtual void StartUpdate(GameObject caster, float delta, float time, float length)
    {

    }

    /*
     * 
     */
    public virtual void CastEvent(GameObject caster)
    {
        GameObject text = UnityEngine.Object.Instantiate(textPrefab,
            caster.transform.position + new Vector3(0, 3, 0),
            Quaternion.identity);
        text.GetComponent<TextMeshPro>().text = name;
    }

    /*
     * time is how much time passed since channel start
     * length is begin animation length
     * 
     */
    public virtual void ChannelUpdate(GameObject caster, float delta, float time, float length)
    {
    }

    /*
     * time is how much time passed since cast end
     * length is return animation length
     */
    public virtual void EndUpdate(GameObject caster, float delta, float time, float length)
    {

    }

    public virtual void InterruptEvent(GameObject caster)
    {

    }

    public string Name { get { return name; } }

    public bool Channeling { get { return channeling; } }

    public bool OnCooldawn {  get { return cooldawnTimer > 0; } }

    protected void PutOnCooldawn()
    {
        cooldawnTimer = cooldawn;
    }

    private readonly string name;
    private readonly bool channeling;
    private readonly float cooldawn;
    private float cooldawnTimer;




}

