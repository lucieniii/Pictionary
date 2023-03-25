using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
public class LoadJson{
    public static T LoadJsonFromFile<T>()where T:class
    {
        if (!File.Exists(Application.dataPath + "/Scripts/wordlist.json"))
        {   
            // Debug.LogError(Application.dataPath);
            Debug.LogError("Don't Find");
            return null;
        }
 
        StreamReader sr = new StreamReader(Application.dataPath + "/Scripts/wordlist.json");
        if (sr == null)
        {
            Debug.LogError("null;");
            return null;
        }
        string json = sr.ReadToEnd();
 
        if (json.Length > 0)
        {
            //Debug.LogError(json);
            return JsonUtility.FromJson<T>(json);
        }
        return null;
    }
}