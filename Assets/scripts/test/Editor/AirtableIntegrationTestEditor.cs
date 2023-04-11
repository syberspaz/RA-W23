using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(AirtableIntegrationTest))]
public class AirtableIntegrationTestEditor : Editor
{
    AirtableIntegrationTest obj;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        obj = (AirtableIntegrationTest)target;

        if(GUILayout.Button("Set Environment"))
        {
            obj.SetEnvironment();
        }

        if (GUILayout.Button("Append Record"))
        {
            obj.AppendCurrentRecord();
        }

        if (GUILayout.Button("Read Records"))
        {
            obj.ReadAllRecords();
        }

        if(GUILayout.Button("Rewrite File Test"))
        {
            obj.RewriteFile();
        }
    }
}
