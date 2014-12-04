using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor (typeof(RoadManager))]
public class RoadManagerEditor : Editor {

    public string _chunkToAdd;
    public string _phaseToAdd;

    private int m_chunkIndex;
    private int m_phaseIndex;

    public override void OnInspectorGUI()
    {
        string[] assetsName;
        Object[] objects = Resources.LoadAll("Chunks");

        assetsName = new string[objects.Length];
        for (int i = 0; i < objects.Length; i++) { assetsName[i] = objects[i].name; }

        DrawDefaultInspector();
        
        m_chunkIndex = EditorGUILayout.Popup(m_chunkIndex, assetsName);
        m_phaseIndex = EditorGUILayout.Popup(m_phaseIndex, new string[]{"Intro", "Test", "Game"});
        
        var instance = target as RoadManager;
        if(GUILayout.Button("Add chunk"))
        {
            switch (m_phaseIndex)
            {
                case 0 :
                    instance._introChunks.Add(assetsName[m_chunkIndex]);        
                    break;
                case 1 :
                    instance._testChunks.Add(assetsName[m_chunkIndex]);
                    break;
                case 2 :
                    instance._gameChunks.Add(assetsName[m_chunkIndex]); 
                    break;
                    
                default : 
                    break;
            }
            
        }
        
        EditorUtility.SetDirty(target);
    }
}
