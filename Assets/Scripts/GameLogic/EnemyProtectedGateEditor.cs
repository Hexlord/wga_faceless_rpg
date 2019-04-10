
#if UNITY_EDITOR
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

[CustomEditor(typeof(EnemyProtectedGate))]
public class EnemyProtectedGateEditor : Editor
{
    
    private void OnSceneGUI()
    {
        // get the chosen game object
        var t = target as EnemyProtectedGate;

        if (t == null)
            return;

        // grab the center of the parent
        var center = t.transform.position;

        EditorGUI.BeginChangeCheck();
        var newRange = Handles.RadiusHandle(Quaternion.identity, t.transform.position, t.protectionRange);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(target, "Changed Protection Range");
            t.protectionRange = newRange;
        }
    }
}
#endif