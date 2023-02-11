using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Thesis.RecTrack;
using Thesis.Recording;


namespace Thesis.Utility
{
    public class Utility_MenuItems
    {
        //creates an automatic recording manager
        [MenuItem("GameObject/Echo_Plus/Create/Automatic Recording Manager", false, 10)]
        static void CreateAutomaticRecordingManager(MenuCommand menuCommand)
        {
            //craete gameobject
            GameObject go = new GameObject("Automatic Recording Manager");
            //set parent if it was a context click (does nothing otherwise)
            GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);

            //add components
            go.AddComponent<Recording_Manager>();
            go.AddComponent<Automatic_Recording_Manager>();

            //register in the undo system and set as selected object
            Undo.RegisterCompleteObjectUndo(go, "Create " + go.name);
            Selection.activeObject = go;
        }

        //makes the object a static recording object
        [MenuItem("GameObject/Echo_Plus/AddComponent/Static Recording Object", false)]
        static void MakeStaticRecordingObject()
        {
            //get gameobject
            GameObject go = Selection.activeObject as GameObject;

            //add components
            Recording_Object ro = go.AddComponent<Recording_Object>();
            ro.SetupDefaultStatic();

            //register in the undo system
            Undo.RegisterCompleteObjectUndo(go, "Add Recording Object to " + go.name);
        }

        //validates the above function
        [MenuItem("GameObject/Echo_Plus/AddComponent/Static Recording Object", true)]
        static bool ValidateMakeStaticRecordingObject()
        {
            return Selection.activeObject is GameObject;
        }

        //makes the object a dynamic recording object
        [MenuItem("GameObject/Echo_Plus/AddComponent/Dyanmic Recording Object", false)]
        static void MakeDynamicRecordingObject()
        {
            //get gameobject
            GameObject go = Selection.activeObject as GameObject;

            //add components
            Recording_Object ro = go.AddComponent<Recording_Object>();
            ro.SetupDefaultDynamic();

            //register in the undo system
            Undo.RegisterCompleteObjectUndo(go, "Add Recording Object to " + go.name);
        }

        //validates the above function
        [MenuItem("GameObject/Echo_Plus/AddComponent/Dyanmic Recording Object", true)]
        static bool ValidateMakeDynamicRecordingObject()
        {
            return Selection.activeObject is GameObject;
        }

        //makes the object a recording object
        [MenuItem("GameObject/Echo_Plus/AddComponent/Recording Camera", false)]
        static void MakeRecordingObjectCamera()
        {
            //get gameobject
            GameObject go = Selection.activeObject as GameObject;

            //add components
            Recording_Object ro = go.AddComponent<Recording_Object>();
            RecTrack_Position rtp = go.AddComponent<RecTrack_Position>();
            RecTrack_Rotation rtr = go.AddComponent<RecTrack_Rotation>();
            RecTrack_Scale rts = go.AddComponent<RecTrack_Scale>();
            RecTrack_Lifetime rtl = go.AddComponent<RecTrack_Lifetime>();
            RecTrack_Camera rtc = go.AddComponent<RecTrack_Camera>();

            //set up components
            ro.m_trackComponents = new List<MonoBehaviour>();
            rtp.m_target = go.transform;
            ro.m_trackComponents.Add(rtp);
            rtr.m_target = go.transform;
            ro.m_trackComponents.Add(rtr);
            rts.m_target = go.transform;
            ro.m_trackComponents.Add(rts);
            ro.m_trackComponents.Add(rtl);
            rtc.m_targetCam = go.GetComponent<Camera>();
            ro.m_trackComponents.Add(rtc);

            //register in the undo system
            Undo.RegisterCompleteObjectUndo(go, "Add Recording Object Camera to " + go.name);
        }

        //validates the above function
        [MenuItem("GameObject/Echo_Plus/AddComponent/Recording Camera", true)]
        static bool ValidateMakeRecordingObjectCamera()
        {
            if (!(Selection.activeObject is GameObject)) return false;
            GameObject go = Selection.activeObject as GameObject;
            return go.GetComponent<Camera>() != null;
        }
    }
}

