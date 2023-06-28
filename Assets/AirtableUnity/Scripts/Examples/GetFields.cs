using AirtableUnity.PX.Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GetFields : MonoBehaviour
{
    [Header("Record Id")]
    public string TableName;
    public string RecordId;

    [ContextMenu("Get Record")]
    public void GetRecord()
    {
        StartCoroutine(GetRecordCo());
    }

    private IEnumerator GetRecordCo()
    {
        yield return StartCoroutine(AirtableUnity.PX.Proxy.GetRecordField<BaseField>(TableName, RecordId, OnResponseFinish));
    }

    private void OnResponseFinish(Dictionary<string, string> record)
    {
        foreach(var str in record)
        {
            Debug.Log(str);
        }
    }
}
