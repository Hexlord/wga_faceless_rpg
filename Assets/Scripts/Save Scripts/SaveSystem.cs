using System.Collections;
using System.Collections.Generic;
using System.Reflection.Emit;
using CI.QuickSave.Core.Serialisers;
using CI.QuickSave.Core.Helpers;

using UnityEngine;
using System;

public class SaveSystem : MonoBehaviour
{
    public string folderPath;
    public string folderName = "Save Files";
    public string saveFileName1 = "Save1";
    public string saveFileName2 = "Save2";
    public string saveFileName3 = "Save3";

    public KeyCode saveKeyCode = KeyCode.C;

    public KeyCode loadKeyCode = KeyCode.V;


    void Awake()
    {
        folderPath = Application.persistentDataPath;
    }


    void Update()
    {
        if (Input.GetKeyDown(saveKeyCode))
        {
            string path = folderPath + "\\" + folderName + "\\" + saveFileName1;
            Save(path);
        }

        if (Input.GetKeyDown(loadKeyCode))
        {
            string path = folderPath + "\\" + folderName + "\\" + saveFileName1;
            Load(path);
        }
    }

    bool Save(string path)
    {

        return false;
    }

    bool Load(string path)
    {
        return false;
    }

    void ComposeGameObject(Dictionary<string, object> items, ref GameObject gameObject)
    {
        foreach (var item in items)
        {
            Component component = gameObject.GetComponent(item.Key);
            foreach (KeyValuePair<string, object> componentItem in (IEnumerable)item.Value)
            {
                try
                {
                    Type.GetType(item.Key).GetField(componentItem.Key).SetValue(component, componentItem.Value);
                }
                catch(Exception e)
                {
                    Debug.Log("Composing gameObject failed. Reason: " + e.Message);
                }
               
            }
        }
    }

    Dictionary<string, object> DecomposeGameObject(GameObject gameObject)
    {
        Dictionary<string, object> items = new Dictionary<string, object>
        {
            { "transform", TypeHelper.ReplaceIfUnityType(gameObject.transform) }
        };

        foreach (Component component in gameObject.GetComponents<Component>())
        {
            try
            {
                items.Add(component.name, component);
            }
            catch(Exception e)
            {
                Debug.Log("Failed to Save. Reason: " + e.Message);
            }
        }

        return items;
    }

    string DictionaryToJSON(Dictionary<string, object> items)
    {
        return JsonSerialiser.Serialise(items);
    }

    Dictionary<string, object> JSONtoDictionary(string json)
    {
        return JsonSerialiser.Deserialise<Dictionary<string, object>>(json);
    }


}
