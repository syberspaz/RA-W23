using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(StudyManager))]
public class StudyManagerEditor : Editor
{
    StudyManager obj;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        obj = (StudyManager)target;

        if (GUILayout.Button("Load Study Configuration"))
        {
            obj.LoadStudy();
        }
    }
}
