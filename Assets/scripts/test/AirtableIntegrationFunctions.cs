using UnityEngine;
using AirtableUnity.PX.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;

public class AirtableIntegrationFunctions : MonoBehaviour
{
    //singleton (because it uses the StartCorountine function of monobehaviour it cannot be a static class)
    static AirtableIntegrationFunctions instance = null;
    //singleton accessor
    static AirtableIntegrationFunctions Instance
    {
        get
        {
            if(instance == null)
            {
                GameObject go = new GameObject("Airtable Integration Functions");
                instance = go.AddComponent<AirtableIntegrationFunctions>();
                DontDestroyOnLoad(go);
            }
            return instance;
        }
    }

    //-----------------------------------------------------------------------------------------------------------------
    // PUBLIC FUNCTIONS
    //-----------------------------------------------------------------------------------------------------------------

    //sets up the airtable enviroment
    public static void SetEnvironment(string apiVersion, string appKey, string apiKey)
    {
        AirtableUnity.PX.Proxy.SetEnvironment(apiVersion, appKey, apiKey);
    }

    //retrives all of the records in the given table
    public static void ListAllRecords<T>(string tableName, Action<List<BaseRecord<T>>> callback)
    {
        Instance.ListAllRecordsImpl(tableName, callback);
    }

    //retrives the specificed record from the given table
    public static void GetRecord<T>(string tableName, string recordId, Action<BaseRecord<T>> callback)
    {
        Instance.GetRecordImpl(tableName, recordId, callback);
    }

    //creates a new record from a json string
    public static void CreateRecord<T>(string tableName, string newDataJson, Action<BaseRecord<T>> callback)
    {
        Instance.CreateRecordImpl(tableName, newDataJson, callback);
    }

    //creates a new record from a base record object
    public static void CreateRecord<T>(string tableName, BaseRecord<T> newDataObject, Action<BaseRecord<T>> callback)
    {
        var json = new
        {
            fields = newDataObject.fields
        };

        Instance.CreateRecordImpl(tableName, JsonConvert.SerializeObject(json), callback);
    }

    //Deletes the specificed record from the given table
    public static void DeleteRecord<T>(string tableName, string recordId, Action<BaseRecord<T>> callback)
    {
        Instance.DeleteRecordImpl<T>(tableName, recordId, callback);
    }

    //Updates the specified record with new data from a json string
    public static void UpdateRecord<T>(string tableName, string recordId, string newDataJson, Action<BaseRecord<T>> callback)
    {
        Instance.UpdateRecordImpl(tableName, recordId, newDataJson, callback);
    }

    //Updates the specified record with new data from a base record object
    public static void UpdateRecord<T>(string tableName, string recordId, BaseRecord<T> newDataObject, Action<BaseRecord<T>> callback)
    {
        var json = new
        {
            fields = newDataObject.fields
        };

        Instance.UpdateRecordImpl(tableName, recordId, JsonConvert.SerializeObject(json), callback);
    }

    //-----------------------------------------------------------------------------------------------------------------
    // PRIVATE IMPLEMENTATION
    //-----------------------------------------------------------------------------------------------------------------

    //list all records
    private void ListAllRecordsImpl<T>(string tableName, Action<List<BaseRecord<T>>> callback)
    {
        StartCoroutine(ListAllRecordsCorountine(tableName, callback));
    }

    private IEnumerator ListAllRecordsCorountine<T>(string tablename, Action<List<BaseRecord<T>>> callback)
    {
        yield return StartCoroutine(AirtableUnity.PX.Proxy.ListRecordsCo<T>(tablename, (records) =>
        {
            callback?.Invoke(records);
        }));
    }

    //Get specific record
    private void GetRecordImpl<T>(string tableName, string recordId, Action<BaseRecord<T>> callback)
    {
        StartCoroutine(GetRecordCoroutine(tableName, recordId, callback));
    }

    private IEnumerator GetRecordCoroutine<T>(string tableName, string recordId, Action<BaseRecord<T>> callback)
    {
        yield return StartCoroutine(AirtableUnity.PX.Proxy.GetRecordCo<T>(tableName, recordId, (record) =>
        {
            callback?.Invoke(record);
        }));
    }

    //Create new record
    private void CreateRecordImpl<T>(string tableName, string newData, Action<BaseRecord<T>> callback)
    {
        StartCoroutine(CreateRecordCoroutine(tableName, newData, callback));
    }

    private IEnumerator CreateRecordCoroutine<T>(string tableName, string newData, Action<BaseRecord<T>> callback)
    {
        yield return StartCoroutine(AirtableUnity.PX.Proxy.CreateRecordCo<T>(tableName, newData, (newRecord) =>
        {
            callback?.Invoke(newRecord);
        }));
    }

    //Delete specific record
    private void DeleteRecordImpl<T>(string tableName, string recordId, Action<BaseRecord<T>> callback)
    {
        StartCoroutine(DeleteRecordCorountine(tableName, recordId, callback));
    }

    private IEnumerator DeleteRecordCorountine<T>(string tableName, string recordId, Action<BaseRecord<T>> callback)
    {
        yield return StartCoroutine(AirtableUnity.PX.Proxy.DeleteRecordCo<T>(tableName, recordId, (deletedRecord) =>
        {
            callback?.Invoke(deletedRecord);
        }));
    }

    //Update specific record
    private void UpdateRecordImpl<T>(string tableName, string recordId, string newData, Action<BaseRecord<T>> callback)
    {
        StartCoroutine(UpdateRecordCorountine(tableName, recordId, newData, callback));
    }

    private IEnumerator UpdateRecordCorountine<T>(string tableName, string recordId, string newData, Action<BaseRecord<T>> callback)
    {
        yield return StartCoroutine(AirtableUnity.PX.Proxy.UpdateRecordCo<T>(tableName, recordId, newData, (updatedRecord) =>
        {
            callback?.Invoke(updatedRecord);
        }));
    }
}
