using UnityEditor;
using UnityEngine;
using Unity.Tutorials.Core;
using System.Collections.Generic;

public class FixDuplicateSceneObjectGuids
{
    [MenuItem("Tools/Fix Duplicate SceneObjectGuids")]
    public static void FixGuids()
    {
        var all = GameObject.FindObjectsOfType<SceneObjectGuid>(true);
        var usedGuids = new HashSet<string>();

        foreach (var obj in all)
        {
            if (!usedGuids.Add(obj.Id))
            {
                // GUID duplicado → lo borramos y dejamos que OnValidate regenere
                Undo.RecordObject(obj, "Fix GUID");
                SerializedObject so = new SerializedObject(obj);
                var property = so.FindProperty("m_Id");
                property.stringValue = System.Guid.NewGuid().ToString();
                so.ApplyModifiedProperties();
                Debug.Log($"Regenerado GUID en {obj.name}");
            }
        }

        Debug.Log("Completado. Revisar consola para ver objetos corregidos.");
    }
    
    [MenuItem("Tools/Remove All SceneObjectGuids")]
    public static void RemoveAllGuids()
    {
        var all = GameObject.FindObjectsOfType<SceneObjectGuid>(true);
        int count = 0;

        foreach (var obj in all)
        {
            Undo.DestroyObjectImmediate(obj);
            count++;
        }

        Debug.Log($"Eliminados {count} componentes SceneObjectGuid de la escena.");
    }
}