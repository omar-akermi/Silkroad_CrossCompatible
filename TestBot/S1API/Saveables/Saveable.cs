using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using MelonLoader;
using Newtonsoft.Json;
using UnityEngine;

namespace S1API.Saveables
{
    /// <summary>
    /// Generic wrapper for saveable classes.
    /// Intended for use within the API only.
    /// </summary>
    public abstract class Saveable
    {
        internal virtual void InitializeInternal(GameObject gameObject, string guid = "") { }
        
        internal virtual void StartInternal() => OnStarted();
        
        internal virtual void LoadInternal(string folderPath)
        {
            FieldInfo[] saveableFields = GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            foreach (FieldInfo saveableField in saveableFields)
            {
             SaveableField saveableFieldAttribute = saveableField.GetCustomAttribute<SaveableField>();
             if (saveableFieldAttribute == null)
                 continue;

             MelonLogger.Msg($"Loading field {saveableField.Name}");
             string filename = saveableFieldAttribute.SaveName.EndsWith(".json")
                 ? saveableFieldAttribute.SaveName
                 : $"{saveableFieldAttribute.SaveName}.json";
             
             string saveDataPath = Path.Combine(folderPath, filename);
             if (!File.Exists(saveDataPath))
                 continue;

             MelonLogger.Msg($"reading json for field {saveableField.Name}");
             string json = File.ReadAllText(saveDataPath);
             Type type = saveableField.FieldType;
             object? value = JsonConvert.DeserializeObject(json, type);
             saveableField.SetValue(this, value);
            }

            OnLoaded();
        }

        internal virtual void SaveInternal(string path, ref List<string> extraSaveables)
        {
             FieldInfo[] saveableFields = GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
             foreach (FieldInfo saveableField in saveableFields)
             {
                 SaveableField saveableFieldAttribute = saveableField.GetCustomAttribute<SaveableField>();
                 if (saveableFieldAttribute == null)
                     continue;

                 string saveFileName = saveableFieldAttribute.SaveName.EndsWith(".json")
                     ? saveableFieldAttribute.SaveName
                     : $"{saveableFieldAttribute.SaveName}.json";
                 
                 string saveDataPath = Path.Combine(path, saveFileName);

                 object value = saveableField.GetValue(this);
                 if (value == null)
                     // Remove the save if the field is null
                     File.Delete(saveDataPath);
                 else
                 {
                     // We add this to the extra saveables to prevent the game from deleting it
                     // Otherwise, it'll delete it after it finishes saving and does clean up
                     extraSaveables.Add(saveFileName);
                     
                     // Write our data
                     string data = JsonConvert.SerializeObject(value, Formatting.Indented);
                     File.WriteAllText(saveDataPath, data);
                 }
             }

             OnSaved();
        }
        
        protected virtual void OnStarted() { }
        protected virtual void OnLoaded() { }
        protected virtual void OnSaved() { }
    }
}