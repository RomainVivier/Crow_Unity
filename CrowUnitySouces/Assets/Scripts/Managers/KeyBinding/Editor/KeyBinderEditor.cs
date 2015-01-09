using UnityEngine;
using UnityEditor;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;


public class KeyBinderEditor : EditorWindow {

    int[] keysIndex;

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
            {
                
            }
        }
        catch
        {
            Debug.LogError("Editor error while editing bind.");
        }

        //TODO
        // Mettre en place un système d'ajout de keybinds


        //key = EditorGUILayout.Popup((int)key, keys);


    }
}
