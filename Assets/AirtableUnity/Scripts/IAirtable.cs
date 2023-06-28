using AirtableUnity.PX.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.Networking;
using UnityEditor;

public class IAirtable : MonoBehaviour
{
    private string ApiVersion;
    private string AppKey;
    private string ApiKey;
    private string TableName;

    public void Initialize(string ConApiVersion, string ConAppToken, string ConApiKey, string ConTableName, bool CreateLocalTable)
    {
        ApiVersion = ConApiVersion;
        AppKey = ConAppToken;
        ApiKey = ConApiKey;
        TableName = ConTableName;
        if (CreateLocalTable)
        {
            UpdateLocalTable();
        }
    }

    public void SetProxy(string ConApiVersion, string ConAppToken, string ConApiKey, string ConTableName)
    {
        ApiVersion = ConApiVersion;
        AppKey = ConAppToken;
        ApiKey = ConApiKey;
        TableName = ConTableName;

    }

    private void SelfCheck()
    {
        if (!AirtableUnity.PX.Proxy.CheckEnvironment(ApiVersion, AppKey, ApiKey))
            AirtableUnity.PX.Proxy.SetEnvironment(ApiVersion, AppKey, ApiKey);
    }


    /// <summary>
    /// Get all records id from the table.
    /// Callback will return a list of string with all records id.
    /// </summary>
    /// <param name="callback"></param>
    public void GetAllRecords(Action<List<string>> callback)
    {
        SelfCheck();
        StartCoroutine(GetRecords(TableName, callback));
    }

    private IEnumerator GetRecords(string tableName, Action<List<string>> callback)
    {
        yield return StartCoroutine(AirtableUnity.PX.Proxy.ListAllRecordsCo<BaseField>(tableName,
            (records) =>
            {
                OnGetRecords(records);
                callback?.Invoke(records);
            }
            ));
    }

    private static void OnGetRecords(List<string> records)
    {

    }


    /// <summary>
    /// Creating a record with specific new data
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="newData"></param>
    /// <param name="callback"></param>
    public void CreateRecord<T>(string newData, Action<BaseRecord<T>> callback)
    {
        SelfCheck();
        StartCoroutine(CreateRecordCo(newData, callback));
    }

    private IEnumerator CreateRecordCo<T>(string newData, Action<BaseRecord<T>> callback)
    {

        yield return StartCoroutine(AirtableUnity.PX.Proxy.CreateRecordCo<T>(TableName, newData, (createdRecord) =>
        {
            OnCreateResponseFinish(createdRecord);
            callback?.Invoke(createdRecord);
        }));
    }

    private static void OnCreateResponseFinish<T>(BaseRecord<T> record)
    {
        var msg = "record id: " + record?.id + "\n";
        msg += "created at: " + record?.createdTime;

        Debug.Log("[Airtable Unity] - Create Record: " + "\n" + msg);
    }


    /// <summary>
    /// Deleting a record
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="recordID"></param>
    /// <param name="callback"></param>
    public void DeleteRecord<T>(string recordID, Action<BaseRecord<T>> callback)
    {
        SelfCheck();
        StartCoroutine(DeleteRecordCo(recordID, callback));
    }

    private IEnumerator DeleteRecordCo<T>(string recordId, Action<BaseRecord<T>> callback)
    {
        yield return StartCoroutine(AirtableUnity.PX.Proxy.DeleteRecordCo<T>(TableName, recordId, (recordDeleted) =>
        {
            OnDeleteResponseFinish(recordDeleted);
            callback?.Invoke(recordDeleted);
        }));
    }

    private static void OnDeleteResponseFinish<T>(BaseRecord<T> record)
    {
        var msg = "record id: " + record?.id + "\n";

        Debug.Log("[Airtable Unity] - Delete Record: " + "\n" + msg);
    }


    /// <summary>
    /// Update record with new data in json string format.
    /// Callback will return the updated record information.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="recordID"></param>
    /// <param name="newData"></param>
    /// <param name="callback"></param>
    /// <param name="useHardUpdate"></param>
    public void UpdateRecord<T>(string recordID, string newData, Action<BaseRecord<T>> callback, bool useHardUpdate = false)
    {
        SelfCheck();
        StartCoroutine(GetRecordCo(TableName, recordID, newData, callback));
    }

    private IEnumerator GetRecordCo<T>(string tableName, string recordId, string newData, Action<BaseRecord<T>> callback, bool useHardUpdate = false)
    {
        yield return StartCoroutine(AirtableUnity.PX.Proxy.UpdateRecordCo<T>(tableName, recordId, newData,
            (baseRecordUpdated) =>
            {
                OnUpdateResponseFinish(baseRecordUpdated);
                callback?.Invoke(baseRecordUpdated);
            }, useHardUpdate));
    }

    private static void OnUpdateResponseFinish<T>(BaseRecord<T> record)
    {
        var msg = "record id: " + record?.id + "\n";
        msg += "created at: " + record?.createdTime;

        Debug.Log("[Airtable Unity] - Update Record: " + "\n" + msg);
    }

    /// <summary>
    /// Get all data from certain record in Airtable by record id.
    /// Callback will return a dictionary, which the keys are the field names and the values are the field values.
    /// </summary>
    /// <param name="recordID"></param>
    /// <param name="callback"></param>
    public void GetRecord(string recordID, Action<Dictionary<string, string>> callback)
    {
        SelfCheck();
        StartCoroutine(GetRecordContent(recordID, callback));
    }

    private IEnumerator GetRecordContent(string recordID, Action<Dictionary<string, string>> callback)
    {
        yield return StartCoroutine(AirtableUnity.PX.Proxy.GetRecordField<BaseField>(TableName, recordID,
            (recordGot) =>
            {
                OnGetResponseFinish(recordGot);
                callback?.Invoke(recordGot);
            }
            ));
    }

    private void OnGetResponseFinish(Dictionary<string, string> record)
    {

    }

    /// <summary>
    /// Getting asset from Airtable. 
    /// The record id must be from a synced table with Google Drive. 
    /// Asset bundle in Google Drive must be shared with anyone with the link.
    /// Callback will return the asset prefab and the file id in Airtable.
    /// </summary>
    /// <param name="recordID"></param>
    /// <param name="callback"></param>
    public void GetAsset(string recordID, Action<GameObject, string> callback)
    {
        SelfCheck();
        StartCoroutine(GetRecordContent(recordID,
            (recordGot) =>
            {
                OnGetAssetID(recordGot, callback);
            }
            ));
    }

    private void OnGetAssetID(Dictionary<string, string> record, Action<GameObject, string> callback)
    {
        if (record.ContainsKey("File ID"))
        {
            string fileId = record["File ID"];
            string objUrl = $"https://drive.google.com/u/0/uc?id={fileId}&export=download";

            StartCoroutine(GetAssetBundle(objUrl, fileId, callback));
        }
    }

    private IEnumerator GetAssetBundle(string assetBundleURL, string fileId, Action<GameObject, string> callback)
    {
        yield return StartCoroutine(AirtableUnity.PX.Proxy.GetRecordAssetBundle<BaseField>(assetBundleURL,
            (gameObjGot) =>
            {
                OnGetObjFinish(gameObjGot, fileId);
                callback?.Invoke(gameObjGot, fileId);
            }
            ));
    }

    private void OnGetObjFinish(GameObject gameObject, string fileId)
    {
        // Anything you want to do with the game object and file id
    }

    public ITable LocalTable;
    bool isUpdating = false;
    bool isUpdatingStarted = false;
    int targetRowNumber = 0;
    public void UpdateLocalTable()
    {
        isUpdatingStarted = true;
        Destroy(LocalTable);
        LocalTable = new ITable();

        this.GetAllRecords(OnLocalTableGetRecords);
    }

    private void OnLocalTableGetRecords(List<string> records)
    {
        targetRowNumber = records.Count;
        isUpdating = true;
        isUpdatingStarted = false;
        foreach (string recordId in records)
        {
            this.GetRecord(recordId,
                (fields) =>
                {
                    OnLocalTableGetFields(fields, recordId);
                }
                );
        }
    }

    private void OnLocalTableGetFields(Dictionary<string, string> fields, string recordId)
    {
        LocalTable.AddRecord(recordId, fields);
    }

    private void Update()
    {
        if (isUpdating) isUpdating = LocalTable.GetRecordCount() < targetRowNumber;
    }

    public bool isLocalTableAvailable { get { return !isUpdating && !isUpdatingStarted; } }

}
