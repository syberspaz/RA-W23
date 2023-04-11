using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AirtableUnity.PX.Model;

public class AirtableIntegrationTest : MonoBehaviour
{
    [System.Serializable]
    public class testType
    {
        public string User;
        public bool Check;
        public string Choice;
        public string blep;
    }

    public TestType type;

    [Header("Airtable Environment Configuration")]
    public string apiVersion;
    public string appKey;
    public string apiKey;

    [Header("Table Information")]
    public string tableName;

    [Header("Append Data")]
    public testType appendRecord;

    [Header("Get Data")]
    [SerializeField] List<BaseRecord<testType>> allrecords = new List<BaseRecord<testType>>();

    public void SetEnvironment()
    {
        AirtableIntegrationFunctions.SetEnvironment(apiVersion, appKey, apiKey);
    }

    public void AppendCurrentRecord()
    {
        BaseRecord<testType> field = new BaseRecord<testType>();
        field.fields = appendRecord;
        AirtableIntegrationFunctions.CreateRecord(tableName, field, null);
    }

    public void ReadAllRecords()
    {
        AirtableIntegrationFunctions.ListAllRecords<testType>(tableName, PopulateRecords);
    }

    private void PopulateRecords(List<BaseRecord<testType>> records)
    {
        allrecords.Clear();
        for(int i = 0; i < records.Count; i++)
        {
            allrecords.Add(records[i]);
        }
    }

    public void RewriteFile()
    {
        System.IO.StreamWriter file = new System.IO.StreamWriter(".\\Assets/scripts/test/TestType.cs");

        file.WriteLine("public class TestType");
        file.WriteLine("{");

        file.WriteLine("public float example = 0.0f;");

        file.WriteLine("}");
        file.Close();
    }
}
