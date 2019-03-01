
#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(EnemyProtectedGate))]
public class EnemyProtectedGateEditor : Editor
{
    

    void OnSceneGUI()
    {
        // get the chosen game object
        EnemyProtectedGate t = target as EnemyProtectedGate;

        if (t == null)
            return;

        // grab the center of the parent
        Vector3 center = t.transform.position;

        EditorGUI.BeginChangeCheck();
        float newRange = Handles.RadiusHandle(Quaternion.identity, t.transform.position, t.protectionRange);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(target, "Changed Protection Range");
            t.protectionRange = newRange;
        }
    }
}
#endif