using System.Collections.Generic;

namespace AirtableUnity.PX.Model
{
    [System.Serializable]
    public class BaseRecord<T>
    {
        public string id;
        public T fields;
        public string createdTime;
    }
}