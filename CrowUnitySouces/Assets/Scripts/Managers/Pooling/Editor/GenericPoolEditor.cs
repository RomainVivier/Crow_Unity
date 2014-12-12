using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomEditor (typeof(GenericPool))]

public class GenericPoolEditor : Editor
{
    public string _chunkToAdd;
    public Object _phaseToAdd;

    private int m_chunkIndex;
    private int m_phaseIndex;

    public override void OnInspectorGUI()
    {
        GenericPool instance = target as GenericPool;
        
        DrawDefaultInspector();

        foreach(PoolKey pk in instance._pool.Dictionary.Keys)
        {
            EditorGUILayout.BeginHorizontal();
            pk.Object = EditorGUILayout.ObjectField(pk.Object, typeof(Object), false);
            pk.Number = EditorGUILayout.IntField(pk.Number);
            if (GUILayout.Button("Delete"))
            {
                instance._pool.Dictionary.Remove(pk);
            }
            EditorGUILayout.EndHorizontal();
        }

        if (GUILayout.Button("Add PoolKey"))
        {
            instance._pool.Dictionary.Add(new PoolKey(), new GenericPool.ListObject());
        }

        EditorUtility.SetDirty(target);
    }
}
