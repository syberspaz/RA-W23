using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class UseITableExample : MonoBehaviour
{
    // Create an IAirtable object
    IAirtable myAirtable;

    bool isPrinted = false;

    private void Start()
    {
        // You must instantiate an IAirtable object before using it
        myAirtable = this.AddComponent<IAirtable>();

        // Initializing IAirtable with APIs would also create a local copy of that table (if TRUE)
        myAirtable.Initialize("", "", "", "", true);
    }

    private void Update()
    {
        // Check if the local copy of the table is available (if it is downloaded)
        if(myAirtable.isLocalTableAvailable && !isPrinted)
        {
            // use GetField to get the value of a field from a record
            Debug.Log(myAirtable.LocalTable.GetField("recGROdyRUKClmxSP", "File Owner(s)"));

            // Or use row name and column name to get the value of a field from a record
            Debug.Log(myAirtable.LocalTable.itable["recZnGghs1rwHD7Jz"]["File ID"]);

            // Use local table to get certain asset from the internet
            myAirtable.GetAsset(myAirtable.LocalTable.itable.Keys.FirstOrDefault(), OnResponseGetUAB);

            // Use record id to get certain asset from the internet
            myAirtable.GetAsset("recZnGghs1rwHD7Jz", OnResponseGetUAB);

            isPrinted = true;
        }
    }

    // All "Get" functions are asynchronous, so you need to use callback functions to get the result
    private void OnResponseGetUAB(GameObject gameObject, string fileId)
    {
        if (gameObject != null)
        {
            GameObject instObj = Instantiate(gameObject);
            instObj.transform.position = new Vector3(Random.Range(-3, 3), Random.Range(-3, 3), Random.Range(-3, 3));
            instObj.name = fileId;
        }
    }
}
