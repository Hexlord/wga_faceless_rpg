
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
 * 05.04.2019   aknorre     Created
 * 
 */

[CustomEditor(typeof(CollisionDamageProjectile))]
public class CollisionDamageProjectileEditor : Editor
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
        for (var i = 0; i < list.arraySize; i++)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(list.GetArrayElementAtIndex(i), new GUIContent("Effect " + i), true);
            if(GUILayout.Button(moveButtonContent, EditorStyles.miniButtonLeft, miniButtonWidth))
            {
                list.MoveArrayElement(i, i + 1);
            }
            if(GUILayout.Button(deleteButtonContent, EditorStyles.miniButtonRight, miniButtonWidth))
            {
                var oldSize = list.arraySize;
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
        EditorGUILayout.PropertyField(serializedObject.FindProperty("canDamage"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("source"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("negativeFilterTarget"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("negativeFilterTargetTag"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("damage"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("shieldDamage"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("uniqueDamage"));
        Show(serializedObject.FindProperty("effectsOnDamage"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("pierceCount"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("projectile"));
        serializedObject.ApplyModifiedProperties();
    }

}
#endif
