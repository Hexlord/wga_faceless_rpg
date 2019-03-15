
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

[CustomEditor(typeof(SkillUser))]
public class SkillUserEditor : Editor
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
        Show(serializedObject.FindProperty("startSkills"));
        serializedObject.ApplyModifiedProperties();
    }

}
#endif
