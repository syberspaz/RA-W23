using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ITable : MonoBehaviour
{
    public Dictionary<string, Dictionary<string, string>> itable { get; }

    public ITable()
    {
        itable = new Dictionary<string, Dictionary<string, string>>();
    }

    public void AddRecord(string recordId, Dictionary<string, string> record)
    {
        itable.Add(recordId, record);
    }

    public string GetField(string recordId, string key)
    {
        return itable[recordId][key];
    }

    public int GetRecordCount()
    {
        return itable.Count;
    }
}
