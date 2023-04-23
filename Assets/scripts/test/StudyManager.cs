using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using AirtableUnity.PX.Model;
using System.Linq;
using System;
using Newtonsoft.Json;

public class StudyManager : MonoBehaviour
{
    private static StudyManager instance;
    public static StudyManager Instance
    {
        get
        {
            if (instance == null)
            {
                //if there isn't one, try to get an object of the same type
                instance = FindObjectOfType<StudyManager>();
                if (instance == null)
                {
                    //if there is no object of the type loaded, create one 
                    GameObject go = new GameObject("Study Manager");
                    instance = go.AddComponent<StudyManager>();
                }
            }
            return instance;
        }
    }

    [Header("Airtable Environment Configuration")]
    public string apiVersion;
    public string airtableURL;
    private string appKey;
    public string apiKey;

    public string configTable = "Config";
    public string studyDetailsTable = "StudyDetails";
    List<ConfigEntry> studyConfiguration = new List<ConfigEntry>();
    int currentSegment = -1;
    int currentId = 0;

    private string surveyAnswerScriptPath = ".\\Assets/scripts/survey";
    private string surveyJSONPath = ".\\Assets/Study/SurveyJSONs";

    Dictionary<string, object> surveyAnswerObjects = new Dictionary<string, object>();
    [SerializeField] List<StringAndSurveyQuestion> surveyQuestionEntries = new List<StringAndSurveyQuestion>();
    //Dictionary<string, SurveyQuestionEntry> surveyQuestionEntries = new Dictionary<string, SurveyQuestionEntry>();


    private void Start()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
        currentSegment = -1;
        LoadStudy();
    }

    public void SetEnvironment()
    {
        string urlWithoutHTTPS = airtableURL.Replace("https://", "");
        string[] urlportions = urlWithoutHTTPS.Split("/");
        appKey = urlportions[1];
        AirtableIntegrationFunctions.SetEnvironment(apiVersion, appKey, apiKey);
    }

    void LoadStudyConfig(List<BaseRecord<StudyDetailsEntry>> detailsEntries)
    {
        if(detailsEntries.Count > 0)
            currentId = detailsEntries[0].fields.CurrentPariticpantID;

        currentSegment = -1;
        AirtableIntegrationFunctions.ListAllRecords<ConfigEntry>(configTable, PopulateStudyConfig);
    }

    public void LoadStudy()
    {
        SetEnvironment();
        AirtableIntegrationFunctions.ListAllRecords<StudyDetailsEntry>(studyDetailsTable, LoadStudyConfig);
    }

    void PopulateStudyConfig(List<BaseRecord<ConfigEntry>> configEntries)
    {
        studyConfiguration.Clear();
        for(int i = 0; i < configEntries.Count; i++)
        {
            studyConfiguration.Add(configEntries[i].fields);
        }
        studyConfiguration = studyConfiguration.OrderBy(c => c.Order).ToList();

        if (!Application.isPlaying) PrepareSurveys();
        else PrepareSession();
    }

    void PrepareSurveys()
    {
        surveyQuestionEntries.Clear();
        for (int i = 0; i < studyConfiguration.Count; i++)
        {
            ConfigEntry entry = studyConfiguration[i];
            if (entry.Type != "Survey") continue;

            AirtableIntegrationFunctions.ListAllRecords<SurveyQuestionEntry>(entry.SurveyQuestionTable, (questions) =>
            {
                questions = questions.OrderBy(sqe => sqe.fields.Order).ToList();

                string answerEntryType = entry.SurveyAnswerTable + "Entry";
                FileGenerator.WriteAnswerEntryFile(answerEntryType, surveyAnswerScriptPath, questions);

                FileGenerator.CreateSurveyJSON(entry.SegmentName, surveyJSONPath, 
                    entry.SurveyInstructions, entry.SurveyEndMessage, entry.SurveyAcknowledgements, questions);

                for(int i = 0; i < questions.Count; i++)
                {
                    StringAndSurveyQuestion newQuestion = new StringAndSurveyQuestion();
                    newQuestion.key = entry.SegmentName;
                    newQuestion.value = questions[i].fields;
                    surveyQuestionEntries.Add(newQuestion);
                }
            });
        }
    }

    void PrepareSession()
    {
        for (int i = 0; i < studyConfiguration.Count; i++)
        {
            ConfigEntry entry = studyConfiguration[i];
            if (entry.Type != "Survey") continue;

            string answerEntryType = entry.SurveyAnswerTable + "Entry";
            surveyAnswerObjects[entry.SegmentName] = System.Reflection.Assembly.GetExecutingAssembly().CreateInstance(answerEntryType);
            SetAnswerObjectValue(entry.SegmentName, "ID", currentId);
        }

        SetSegment(0);
    }

    public void NextSegment()
    {
        SetSegment(currentSegment + 1);
    }

    public void SetSegment(int segment)
    {
        if (segment < 0 || segment >= studyConfiguration.Count) return;

        currentSegment = segment;

        ConfigEntry entry = studyConfiguration[currentSegment];

        if (entry.Type == "Scene")
        {
            Debug.Log("Scene");
            SceneManager.LoadScene(entry.SceneName);
        }
        else if (entry.Type == "Survey")
        {
            Debug.Log("Survey");
            StartCoroutine(LoadStudy(entry));
            SceneManager.LoadScene("SurveyScene");
        }
    }

    WaitForEndOfFrame eof = new WaitForEndOfFrame();

    private IEnumerator LoadStudy(ConfigEntry entry)
    {
        yield return eof;
        yield return eof;

        VRQuestionnaireToolkit.GenerateQuestionnaire SurveyGenerator = FindObjectOfType<VRQuestionnaireToolkit.GenerateQuestionnaire>();
        if(SurveyGenerator != null)
        {
            SurveyGenerator.ManualLoadNewSurvey(surveyJSONPath + "/" + entry.SegmentName + ".json");
        }
    }

    //surveyname-or-id, questionid, and value
    public void SetAnswerObjectValue<T>(string surveyName, string property, T value)
    {
        if (!surveyAnswerObjects.ContainsKey(surveyName))
        {
            Debug.LogWarning("Surveyname " + surveyName + " not found");
            return;
        }

        if (surveyAnswerObjects[surveyName].GetType().GetField(property) == null) Debug.Log("why? " + property);

        surveyAnswerObjects[surveyName].GetType().GetField(property).SetValue(surveyAnswerObjects[surveyName], value);
    }

    public void SetRadioAnswerObjectValue(string surveyName, string property, int value)
    {
        if (!surveyAnswerObjects.ContainsKey(surveyName) || surveyQuestionEntries.Find(s => s.key == surveyName && s.value.ID == property) == null)
        {
            Debug.LogWarning("Surveyname " + surveyName + " not found");
            return;
        }

        ConfigEntry entry = studyConfiguration.Find(e => e.SegmentName == surveyName);

        SurveyQuestionEntry question = surveyQuestionEntries.Find(s => s.key == surveyName && s.value.ID == property).value;

        if (value == 1) SetAnswerObjectValue(surveyName, property, question.Option1);
        else if (value == 2) SetAnswerObjectValue(surveyName, property, question.Option2);
        else if (value == 3) SetAnswerObjectValue(surveyName, property, question.Option3);
        else if (value == 4) SetAnswerObjectValue(surveyName, property, question.Option4);
        else if (value == 5) SetAnswerObjectValue(surveyName, property, question.Option5);
        else if (value == 6) SetAnswerObjectValue(surveyName, property, question.Option6);
        else if (value == 7) SetAnswerObjectValue(surveyName, property, question.Option7);
    }

    public void PostAnswerObjectToServer(string surveyName)
    {
        if (!surveyAnswerObjects.ContainsKey(surveyName))
        {
            Debug.LogWarning("Surveyname " + surveyName + " not found");
            return;
        }

        var entry = surveyAnswerObjects[surveyName];

        var json = new
        {
            fields = entry
        };

        string tablename = studyConfiguration.Find(e => e.SegmentName == surveyName).SurveyAnswerTable;

        AirtableIntegrationFunctions.CreateRecord<object>(tablename, JsonConvert.SerializeObject(json), null);
    }

    [System.Serializable]
    public class StringAndSurveyQuestion
    {
        public string key;
        public SurveyQuestionEntry value;
    }
}