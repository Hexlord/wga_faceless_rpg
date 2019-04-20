
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
        addButtonContent = new GUIContent("+", "Add stage"),
        addAnswerContent = new GUIContent("+", "Add answer"),
        deleteButtonContent = new GUIContent("-", "Remove stage"),
        deleteAnswerContent = new GUIContent("-", "Remove answer");

    private static GUILayoutOption miniButtonWidth = GUILayout.Width(40f);
    private static GUILayoutOption normButtonWidth = GUILayout.Width(100f);

    private static bool ShowAnswers(SerializedProperty list)
    {
        for (var i = 0; i < list.arraySize; i++)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(list.GetArrayElementAtIndex(i), new GUIContent("Answer " + (i + 1)), true);
            if (GUILayout.Button(moveButtonContent, EditorStyles.miniButtonLeft, miniButtonWidth))
            {
                list.MoveArrayElement(i, i + 1);
            }
            if (GUILayout.Button(deleteAnswerContent, EditorStyles.miniButtonRight, miniButtonWidth))
            {
                int oldSize = list.arraySize;
                list.DeleteArrayElementAtIndex(i);
                if (list.arraySize == oldSize)
                {
                    list.DeleteArrayElementAtIndex(i);
                }

                return false;
            }
            EditorGUILayout.EndHorizontal();


        }

        GUILayout.BeginHorizontal();
        GUILayout.Space(EditorGUI.indentLevel * 12);
        if (GUILayout.Button(addAnswerContent, normButtonWidth))
        {
            list.arraySize += 1;
        }
        GUILayout.EndHorizontal();

        return true;
    }

    private static bool ShowStages(SerializedProperty list)
    {
        for (var i = 0; i < list.arraySize; i++)
        {
            var elem = list.GetArrayElementAtIndex(i);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(elem, new GUIContent("Stage " + (i+1)));
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

                return false;
            }
            EditorGUILayout.EndHorizontal();

            EditorGUI.indentLevel += 4;
            EditorGUILayout.PropertyField(elem.FindPropertyRelative("text"));
            if (!ShowAnswers(elem.FindPropertyRelative("answers"))) return false;
            EditorGUI.indentLevel -= 4;
        }

        if (GUILayout.Button(addButtonContent, normButtonWidth))
        {
            list.arraySize += 1;
        }

        return true;
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
        EditorGUILayout.PropertyField(serializedObject.FindProperty("autoTalk"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("panel"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("talkNode"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("startStage"));
        if (!ShowStages(serializedObject.FindProperty("dialogStages"))) Repaint();
        serializedObject.ApplyModifiedProperties();
    }

}
#endif
