using UnityEngine;
using UnityEditor;
using System.Collections;
using System;


public class KeyBinderEditor : EditorWindow {

    int key;

    [MenuItem("KeyBinder/Settings")]
    static void Init()
    {
        KeyBinderEditor window = (KeyBinderEditor)EditorWindow.GetWindow(typeof(KeyBinderEditor));
    }

    void OnGUI()
    {
        GUILayout.Label ("Base Settings", EditorStyles.boldLabel);

        string[] keys = new string[Enum.GetValues(typeof(KeyCode)).Length];

        for(int i = 0; i < Enum.GetValues(typeof(KeyCode)).Length; i++)
        {
            KeyCode k = (KeyCode)(i);
            keys[i] = k.ToString();
        }

        var jsonBindsRefs = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(PlayerPrefs.GetString("InputsPrefs"));

        try
        {
            foreach (var kb in jsonBindsRefs)
                keyBindsRefsSave.Add(KeyConfig.FromString(kb.Key), kb.Value);
        }
        catch
        {
            PlayerPrefs.SetString("InputsPrefs", "");
            RestoreInputsPrefs();
        }


        //key = EditorGUILayout.Popup((int)key, keys);


    }
}
