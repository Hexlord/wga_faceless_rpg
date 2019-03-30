
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
 * 10.03.2019   aknorre     Created
 * 
 */

[CustomEditor(typeof(NPC))]
public class NPCEdtior : Editor
{
    private static GUIContent
        moveButtonContent = new GUIContent("\u21b4", "Move down"),
        addButtonContent = new GUIContent("+", "Add dialog button"),
        deleteButtonContent = new GUIContent("-", "Remove dialog button");

    private static GUILayoutOption miniButtonWidth = GUILayout.Width(40f);
    private static GUILayoutOption normButtonWidth = GUILayout.Width(100f);

    private static void Show(SerializedProperty list)
    {
        for (int i = 0; i < list.arraySize; i++)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(list.GetArrayElementAtIndex(i), true);
            if (GUILayout.Button(moveButtonContent, EditorStyles.miniButtonLeft, miniButtonWidth))
            {
                list.MoveArrayElement(i, i + 1);
            }
            if (GUILayout.Button(deleteButtonContent, EditorStyles.miniButtonRight, miniButtonWidth))
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
    private void OnSceneGUI()
    {
        // get the chosen game object
        NPC t = target as NPC;

        if (t == null)
            return;

        // grab the center of the parent
        Vector3 center = t.transform.position;

        EditorGUI.BeginChangeCheck();
        float newRange = Handles.RadiusHandle(Quaternion.identity, t.transform.position, t.activationRange);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(target, "Changed Activation Range");
            t.activationRange = newRange;
        }
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("dialogCamera"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("activationRange"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("activationNode"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("startNode"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("talkNode"));
        Show(serializedObject.FindProperty("dialogEntries"));
        serializedObject.ApplyModifiedProperties();
    }

}
#endif
