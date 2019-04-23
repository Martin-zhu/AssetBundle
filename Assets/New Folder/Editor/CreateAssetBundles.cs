using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class CreateAssetBundles
{
    [MenuItem("Assets/Build AssetBundles")]
    static void BuildAllAssetBundles()
    {
        //string outPath = "Assets/AssetBundles";
        string outPath = PathUtil.GetAssetBundleOutPath();

        BuildPipeline.BuildAssetBundles(outPath, 0, BuildTarget.StandaloneWindows64);
    }
}
