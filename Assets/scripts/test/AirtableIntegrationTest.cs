using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirtableIntegrationTest : MonoBehaviour
{
    [Header("Airtable Environment Configuration")]
    public string apiVersion;
    public string appKey;
    public string apiKey;

    [Header("Table Information")]
    public string tableName;

    void Start()
    {
        AirtableIntegrationFunctions.SetEnvironment(apiVersion, appKey, apiKey);
    }
}
