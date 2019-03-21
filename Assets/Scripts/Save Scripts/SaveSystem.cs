using System.Collections;
using System.Collections.Generic;
using System.Reflection.Emit;
using CI.QuickSave;
using CI.QuickSave.Core.Serialisers;
using CI.QuickSave.Core.Helpers;
using 
using UnityEngine;
using System;
using System.Linq;
using System.Reflection;

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
            string path = folderPath + "//" + /*folderName + "/" +*/ saveFileName1;
            Debug.Log("Save: " + Save(path));
        }

        if (Input.GetKeyDown(loadKeyCode))
        {
            string path = folderPath + "//" + /*folderName + "/" +*/ saveFileName1;
            Debug.Log("Load: " + Load(path));
        }
    }

    bool Save(string path)
    {
        Debug.Log("Save: " + path);

        GameObject[] gameObjects = FindObjectsOfType<GameObject>();

        Dictionary<string, object> sceneDictionary = new Dictionary<string, object>();

        foreach (GameObject gameObject in FindObjectsOfType<GameObject>())
        {
            Dictionary<string, Dictionary<string, object>> serializedObject;
            bool success = TrySerializeGameObject(gameObject, out serializedObject);
            if(success == true && serializedObject != null)
            {
                if (QuickSaveWriter.TrySave(path, gameObject.name, serializedObject) == false)
                {
                    return false;
                }
            }
        }

        return true;
    }

    bool TrySerializeGameObject(GameObject gameObject, out Dictionary<string, Dictionary<string, object>> serializedObject)
    {
        try
        {
            serializedObject = new Dictionary<string, Dictionary<string, object>>();
            Component[] components = gameObject.GetComponents<Component>();
            foreach (Component component in components)
            {
                serializedObject[component.name] = new Dictionary<string, object>();

                FieldInfo[] fields = component.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                
                foreach(FieldInfo fieldInfo in fields)
                {
                    Saveable[] attribute = fieldInfo.GetCustomAttributes(typeof(Saveable), true) as Saveable[];
                    if (attribute.Length != 0)
                    {
                        object currentField = TypeHelper.ReplaceIfUnityType(fieldInfo.FieldType, fieldInfo.GetValue(component));
                        string JSON = JsonUtility.ToJson(currentField, true);
                        serializedObject[component.name][attribute[0].Name] = JsonUtility.FromJson<Dictionary<string, object>>(JSON);
                    }
                }
            }
            if (serializedObject.Count == 0)
                serializedObject = null;
        }
        catch(Exception e)
        {
            Debug.Log("Failed in TrySerializeGameObject. Reason : " + e.Message);
            serializedObject = null;
            return false;
        }

        return true;
    }

    bool Load(string path)
    {
        Debug.Log("Load: " + path);

        Dictionary<string, object> sceneDictionary = new Dictionary<string, object>();

        GameObject[] gameObjects = FindObjectsOfType<GameObject>();

        for (int i = 0; i < gameObjects.Length; i++)
        {
            Dictionary<string, object> goDictionary;
            if (QuickSaveReader.TryLoad(path, gameObjects[i].name, out goDictionary) == false)
                return false;
            ComposeGameObject(goDictionary, gameObjects[i]);
        }
        return true;
    }

    void ComposeGameObject(Dictionary<string, object> items, GameObject gameObject)
    {
        foreach (var item in items)
        {
            Component component = gameObject.GetComponent(item.Key);
            foreach (KeyValuePair<string, object> componentItem in (IEnumerable)item.Value)
            {
                try
                {
                    var a = AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes()).Where(t => t.Name == item.Key);
                    Type ф = a.First();
                    if (ф.GetMethod("ToUnityType") != null)
                        ф = ф.GetMethod("ToUnityType").ReturnType;
                    var й = ф.GetField(componentItem.Key);
                    й.SetValue(component, componentItem.Value);
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
            { "Transform", TypeHelper.ReplaceIfUnityType(gameObject.transform) }
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
