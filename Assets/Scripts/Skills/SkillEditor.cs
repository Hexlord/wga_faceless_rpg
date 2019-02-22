using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/*
 *  
 * 
 * 
 */
 [CustomEditor(typeof(SkillUser))]
public class SkillEditor : Editor
{
    SkillUser skillUser;

    // Public

    void OnEnable()
    {
        skillUser = (SkillUser)target;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
    }

    void OnSceneGUI()
    {
        var style = GUI.skin.GetStyle("Label");
        style.normal.textColor = Color.white;

        Handles.BeginGUI();
        Handles.color = Color.black;
        Handles.Label(skillUser.gameObject.transform.position + new Vector3(0, 3, 0), "Skill user", style);
        Handles.EndGUI();
    }

}
