using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
class SaveObject
{
    public Vector3[] transform;

    public Dictionary<string, object> fieldsToSave;

    public SaveObject(Transform transform)
    {
        this.transform = new Vector3[3];
        this.transform[0] = transform.position;
        this.transform[1] = transform.rotation.eulerAngles;
        this.transform[2] = transform.localScale;
        fieldsToSave = new Dictionary<string, object>();
    }
}