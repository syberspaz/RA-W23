using System;
using UnityEditor;
using UnityEngine;

public class CreateAssetBundle
{
    [MenuItem("Assets/IAirtable/Create or Rebuild All Asset Bundles")]
    private static void  GenerateAssetBundle()
    {
        string BuildPath = Application.dataPath + "/GeneratedAssetBundles";

		try
		{
			BuildPipeline.BuildAssetBundles(BuildPath, BuildAssetBundleOptions.None, EditorUserBuildSettings.activeBuildTarget);
		}
		catch (Exception e)
		{
			Debug.LogError(e);
			throw;
		}
    }


}
