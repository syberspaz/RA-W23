using System;
using System.IO;
using UnityEngine;
using UnityEditor;

namespace Thesis.Recording
{
    [RequireComponent(typeof(Recording_Manager))]
    public class Automatic_Recording_Manager : MonoBehaviour
    {
        [Header("Static File Data Saving")]
        [SerializeField] private string staticFileSaveLocation;
        [Tooltip("file name without extension")]
        [SerializeField] private string staticFileName = "StaticData";

        [Space]
        [Header("Static File Data Saving")]
        [SerializeField] private string dynamicFileSaveLocation;
        [Tooltip("file name without extension")]
        [SerializeField] private string dynamicFileName = "DyanmicData";

        [Space]
        [Header("Controls")]
        [SerializeField] private bool record = false;
        [SerializeField] private float startDelay = 0.0f;
        [SerializeField] private bool displayDialogue = false;


        private Recording_Manager m_recManager;
        private bool recording = false;
        private float elapsed = 0.0f;
        
        void Start()
        {
            m_recManager = GetComponent<Recording_Manager>();
            recording = false;
        }

        void Update()
        {
            if(record && !recording)
            {
                if (elapsed >= startDelay) StartRecording();

                elapsed += Time.deltaTime;
            }
        }

        void OnDestroy()
        {
            if (recording) StopRecording();
        }

        void Reset()
        {
            string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Echo_Plus\\Data";
            staticFileSaveLocation = folderPath;
            dynamicFileSaveLocation = folderPath;
        }

        void StartRecording()
        {
            m_recManager.StartRecording();
            recording = true;
        }

        void StopRecording()
        {
            m_recManager.StopRecording();
            recording = false;
            Save();
        }

        void Save()
        {
            //create the necessary directories if they do not already exist
            try
            {
                if (!Directory.Exists(staticFileSaveLocation)) Directory.CreateDirectory(staticFileSaveLocation);
                if (!Directory.Exists(dynamicFileSaveLocation)) Directory.CreateDirectory(dynamicFileSaveLocation);
            }
            catch (Exception e)
            {
                Debug.LogError("Failed to create desired directory, error details: " + e.ToString());
            }

            // Tell the recording manager to save the static file
            string staticPath = staticFileSaveLocation + "\\" + staticFileName + ".log";
            bool staticSaveWorked = m_recManager.SaveStaticData(staticPath);

            // Tell the recording manager to save the dynamic file as well
            string dynamicPath = dynamicFileSaveLocation + "\\" + dynamicFileName + ".log";
            bool dynamicSaveWorked = m_recManager.SaveDynamicData(dynamicPath);

#if UNITY_EDITOR
            // Show a dialog to indicate if the saving worked
            if (staticSaveWorked && dynamicSaveWorked && displayDialogue)
            {
                EditorUtility.DisplayDialog("Save Successful", "Saving the static file and the dynamic file worked correctly!", "Continue");
            }
            else if (displayDialogue)
            {
                EditorUtility.DisplayDialog("Save Failed", "There was an error when saving the static file and the dynamic file!", "Continue");
            }
#endif
        }
    }
}
