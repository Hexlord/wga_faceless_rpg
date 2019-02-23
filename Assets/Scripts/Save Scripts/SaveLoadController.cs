using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveLoadController : MonoBehaviour
{
    public string fileExtension = ".save";
    public string dataPath;

    void Start()
    {
        dataPath = Path.Combine(Application.persistentDataPath, gameObject.scene.name + fileExtension);
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.O) == true)
        {
            SaveProgress(dataPath);
        }
    }

    void SaveProgress(string path)
    {
        SaveObject globalSaveObject = new SaveObject(gameObject.transform);
        foreach (GameObject gameObject in FindObjectsOfType<GameObject>())
        {
            foreach(ISaveable saveable in gameObject.GetComponentsInChildren<ISaveable>())
            {
                SaveObject saveObject = saveable.Save();
                globalSaveObject.fieldsToSave[gameObject.name] = saveObject;
            }
        }
        SerializeSaveObject(globalSaveObject, path);
    }

    void LoadProgress(string path)
    {
        SaveObject globalSaveObject = DeserializeSaveObject(path);
        foreach (GameObject gameObject in FindObjectsOfType<GameObject>())
        {
            foreach (ISaveable saveable in gameObject.GetComponentsInChildren<ISaveable>())
            {
                SaveObject saveObject = (SaveObject)globalSaveObject.fieldsToSave[gameObject.name];
                saveable.Load(saveObject);
            }
        }
    }

    static void SerializeSaveObject(SaveObject saveObject, string path)
    {
        string jsonString = JsonUtility.ToJson(saveObject);

        using (StreamWriter streamWriter = File.CreateText(path))
        {
            streamWriter.Write(jsonString);
        }
    }

    static SaveObject DeserializeSaveObject(string path)
    {
        using (StreamReader streamReader = File.OpenText(path))
        {
            string jsonString = streamReader.ReadToEnd();
            return JsonUtility.FromJson<SaveObject>(jsonString);
        }
    }
}
