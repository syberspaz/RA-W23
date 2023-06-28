using AirtableUnity.PX.Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Unity.VisualScripting;

public class UseIAirtableExample : MonoBehaviour
{
    IAirtable myAirTable;

    private void Awake()
    {
        myAirTable = this.AddComponent<IAirtable>();
        myAirTable.Initialize("", "", "", "", true);

    }


    [ContextMenu("Get Text-based field")]
    public void GetTestRecord()
    {
        // Example of getting a field from a record, callback will return a dictionary of <field name, field value>
        myAirTable.GetRecord("recwEhZeV50V9RJc1", OnResponseGetField);
    }


    [ContextMenu("Instantiate GameObject")]
    public void InstTestObj()
    {
        // Example of getting an Asset Bundle prefab from a record, table must be a synced table with Google Drive.
        // Callback will return a GameObject and a string of the file id to be used to identify the object (if you want)
        myAirTable.GetAsset("recZnGghs1rwHD7Jz", OnResponseGetUAB);
    }

    [ContextMenu("Get All Records")]
    public void ListRecords()
    {
        // Example of getting all records id from a table, callback will return a list of record ids
        myAirTable.GetAllRecords(OnResponseGetRecords);
    }

    // Callback functions

    private void OnResponseGetField(Dictionary<string, string> record)
    {
        Debug.Log(record["QuestionString"]);
    }

    private void OnResponseGetUAB(GameObject gameObject, string fileId)
    {
        if(gameObject != null)
        {
            GameObject instObj = Instantiate(gameObject);
            instObj.transform.position = Vector3.zero;
            instObj.name = fileId;
        }
    }

    private void OnResponseGetRecords(List<string> records)
    {
        foreach(var record in records)
        {
            Debug.Log(record);
        }
    }

}
