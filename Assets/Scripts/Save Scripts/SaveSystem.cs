

using System.Collections;
using System.Collections.Generic;
using CI.QuickSave;
using CI.QuickSave.Core.Helpers;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;
using System;
using System.Linq;
using System.Reflection;

public enum SaveType
{
    Quick,
    Auto
}

public class SaveSystem : MonoBehaviour
{
    private string folderPath;
    public string folderName = "SaveFiles";
    public string autoFilename = "AutoSave";
    public string quickFilename = "QuickSave";
    public string saveFileName1 = "Save1";
    public string saveFileName2 = "Save2";
    public string saveFileName3 = "Save3";

    public string loadingSceneName;
    public string gameSceneName;

    public KeyCode saveKeyCode = KeyCode.C;

    public KeyCode loadKeyCode = KeyCode.V;


    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        folderPath = System.IO.Path.Combine(Application.persistentDataPath, folderName);
        System.IO.Directory.CreateDirectory(folderPath);
    }


    void Update()
    {
       /* if (Input.GetKeyDown(saveKeyCode))
        {
            QuickSaveWriter saver = QuickSaveWriter.Create(System.IO.Path.Combine(folderPath, saveFileName1));
            Debug.Log(Save(saver));
        }

        if (Input.GetKeyDown(loadKeyCode))
        {
            QuickSaveReader loader = QuickSaveReader.Create(System.IO.Path.Combine(folderPath, saveFileName1));
            Debug.Log(Load(loader));
        }*/
    }

    public void Save()
    {

    }

    public void Load(SaveType saveType)
    {
        SceneManager.LoadScene(loadingSceneName, LoadSceneMode.Single);
        StartCoroutine("LoadGameScene", saveType);
    }

    IEnumerator LoadGameScene(SaveType saveType)
    {
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(gameSceneName);
        asyncOperation.allowSceneActivation = false;

        Debug.Log("Pro :" + asyncOperation.progress);

        while (!asyncOperation.isDone)
        {
            yield return null;
        }
        Scene gameScene = SceneManager.GetSceneByName(gameSceneName);
        switch (saveType)
        {
            case SaveType.Auto:
                _Load(gameScene.GetRootGameObjects(), QuickSaveReader.Create(System.IO.Path.Combine(folderPath, autoFilename)));
                break;
            case SaveType.Quick:
                _Load(gameScene.GetRootGameObjects(), QuickSaveReader.Create(System.IO.Path.Combine(folderPath, quickFilename)));
                break;
        }
        asyncOperation.allowSceneActivation = true;
        yield return null;
    }

    GameObject[] GetAllGameObjects(GameObject[] rootGameObjects)
    {
        List<GameObject> result = new List<GameObject>();
        foreach (GameObject gameObject in rootGameObjects)
        {
            foreach(Transform child in gameObject.transform)
            {
                result.Add(child.gameObject);
            }
        }
        return result.ToArray();
    }


    bool _Save(QuickSaveWriter saver)
    {
        Debug.Log("Save");

        GameObject[] gameObjects = GetSaveableGameObjects(FindObjectsOfType<GameObject>());

        foreach (GameObject gameObject in gameObjects)
        {
            if (TrySerializeGameObject(gameObject, saver) == false)
            {
                return false;
            }
        }
        if (saver.TryCommit() == true)
        {
            foreach (GameObject gameObject in gameObjects)
            {
                foreach (ISaveable saveable in gameObject.GetComponents<ISaveable>())
                {
                    saveable.OnSave();
                }
            }
            return true;
        }
        else
        {
            return false;
        }
    }

    bool TrySerializeGameObject(GameObject gameObject, QuickSaveWriter saver)
    {
        foreach (Component component in gameObject.GetComponents<Component>())
        {
            if (IsComponentSaveble(component) == true)
            {
                if (TrySerializeComponent(component, saver) == false)
                {
                    return false;
                }
            }
        }
        return true;
    }

    bool TrySerializeComponent(Component component, QuickSaveWriter saver)
    {
        FieldInfo[] fields = component.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
        foreach (FieldInfo fieldInfo in fields)
        {
            Saveable[] attribute = fieldInfo.GetCustomAttributes(typeof(Saveable), true) as Saveable[];
            string fullFieldName = GetFullFieldName(component, fieldInfo.Name);
            saver.Write(fullFieldName, TypeHelper.ReplaceIfUnityType(fieldInfo.FieldType, fieldInfo.GetValue(component)));

        }
        return true;
    }


    bool _Load(GameObject [] rootGameObjects, QuickSaveReader loader)
    {
        Debug.Log("Load");

        GameObject[] gameObjects = GetAllGameObjects(rootGameObjects);

        foreach (GameObject gameObject in gameObjects)
        {
            if (TryDeserializeGameObject(gameObject, loader) == false)
            {
                return false;
            }
        }

        foreach (GameObject gameObject in gameObjects)
        {
            foreach (ISaveable saveable in gameObject.GetComponents<ISaveable>())
            {
                saveable.OnLoad();
            }
        }
        return true;
    }

    bool TryDeserializeGameObject(GameObject gameObject, QuickSaveReader loader)
    {
        Component[] components = gameObject.GetComponents<Component>();
        for (int i = 0; i < components.Length; i++)
        {
            if(IsComponentSaveble(components[i]) == true)
            {
                if (TryDeserializeComponent(components[i], loader) == false)
                {
                    return false;
                }
            }
        }
        return true;
    }

    bool TryDeserializeComponent(Component component, QuickSaveReader loader)
    {
        FieldInfo[] fields = component.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
        foreach (FieldInfo fieldInfo in fields)
        {
            Saveable[] attribute = fieldInfo.GetCustomAttributes(typeof(Saveable), true) as Saveable[];
            if (attribute.Length != 0)
            {
                string fullFieldName = GetFullFieldName(component, fieldInfo.Name);
                object result;
                if (loader.TryRead(fullFieldName, out result, fieldInfo.FieldType) == true)
                {
                    fieldInfo.SetValue(component, result);
                }
                else
                {
                    return false;
                }
            }
        }

        try
        {
            ISaveable saveableComponent = component as ISaveable;
            saveableComponent.OnLoad();
        }
        catch(Exception e)
        {
            Debug.Log("failed call onLoad, probably " + GetFieldPrefix(component) + " does not implement ISaveable. Exact Reason: " + e.Message);
            return false;
        }


        return true;
    }

    string GetFieldPrefix(Component component)
    {
        return component.name + "." + component.GetType().Name;
    }

    string GetFullFieldName(Component component, string fieldName)
    {
         return GetFieldPrefix(component) + "." + fieldName;
    }

    GameObject[] GetSaveableGameObjects(GameObject[] gameObjects)
    {
        List<GameObject> result = new List<GameObject>();

        foreach (GameObject gameObject in gameObjects)
        {
            if (IsGOSaveable(gameObject) == true)
                result.Add(gameObject);
        }
        return result.ToArray();
    }

    bool IsGOSaveable(GameObject gameObject)
    {
        Component[] components = gameObject.GetComponents<Component>();
        foreach (Component component in components)
        {
            if(IsComponentSaveble(component) == true)
            {
                return true;
            }
        }
        return false;
    }

    bool IsComponentSaveble(Component component)
    {
        FieldInfo[] fields = component.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance); string componentKey = component.name + "." + component.GetType().FullName;
        foreach (FieldInfo fieldInfo in fields)
        {
            Saveable[] attribute = fieldInfo.GetCustomAttributes(typeof(Saveable), true) as Saveable[];
            if (attribute.Length != 0)
            {
                return true;
            }
        }
        return false;
    }
}