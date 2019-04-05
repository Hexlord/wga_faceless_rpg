
#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/*
 * History:
 * 
 * Date         Author      Description
 * 
 * 03.03.2019   aknorre     Created
 * 
 */

[CustomEditor(typeof(SkillSystem))]
public class SkillSystemEditor : Editor
{
    private static GUIContent
        moveButtonContent = new GUIContent("\u21b4", "Move down"),
        addButtonContent = new GUIContent("+", "Add skill"),
        deleteButtonContent = new GUIContent("-", "Remove skill");

    private static GUILayoutOption miniButtonWidth = GUILayout.Width(40f);
    private static GUILayoutOption normButtonWidth = GUILayout.Width(100f);

    private static void Show(SerializedProperty list)
    {
        //EditorGUILayout.PropertyField(list);
        for (int i = 0; i < list.arraySize; i++)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(list.GetArrayElementAtIndex(i));
            if(GUILayout.Button(moveButtonContent, EditorStyles.miniButtonLeft, miniButtonWidth))
            {
                list.MoveArrayElement(i, i + 1);
            }
            if(GUILayout.Button(deleteButtonContent, EditorStyles.miniButtonRight, miniButtonWidth))
            {
                int oldSize = list.arraySize;
                list.DeleteArrayElementAtIndex(i);
                if (list.arraySize == oldSize)
                {
                    list.DeleteArrayElementAtIndex(i);
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        if (GUILayout.Button(addButtonContent, normButtonWidth))
        {
            list.arraySize += 1;
        }

    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("skillAnimationStart"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("skillAnimationEnd"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("channelAnimationStart"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("channelAnimationUpdate"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("channelAnimationEnd"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("idleAnimation"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("skillAnimationStartTrigger"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("channelAnimationStartTrigger"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("interruptTrigger"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("interruptInstantTrigger"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("animationLayer"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("preciseEnding"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("preciseChanneling"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("useAnimationTime"));
        
        EditorGUILayout.PropertyField(serializedObject.FindProperty("state"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("skillAnimationStartLength"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("skillAnimationEndLength"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("channelAnimationStartLength"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("channelAnimationUpdateLength"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("channelAnimationEndLength"));
        Show(serializedObject.FindProperty("startSkills"));
        serializedObject.ApplyModifiedProperties();
    }

}
#endif
