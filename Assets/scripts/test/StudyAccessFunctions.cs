using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StudyAccessFunctions : MonoBehaviour
{
    public void NextSegment()
    {
        StudyManager.Instance.NextSegment();
    }

    public void SetSegment(int segment)
    {
        StudyManager.Instance.SetSegment(segment);
    }

    public void SetAnswerObjectValue<T>(string surveyName, string property, T value)
    {
        StudyManager.Instance.SetAnswerObjectValue(surveyName, property, value);
    }

    public void PostAnswerObjectToServer(string surveyName)
    {
        StudyManager.Instance.PostAnswerObjectToServer(surveyName);
    }
}
