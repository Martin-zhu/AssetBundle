using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NonCachingLoadExample : MonoBehaviour
{
    public string BundleURL;
    public string AssetName;

    //file:///C:/Users/张晋枭/Documents/ABLesson/Assets/AssetBundles/test.assetbundle
    IEnumerator Start()
    {
        // Download the file from the URL. It will not be saved in the Cache
        using (WWW www = new WWW(BundleURL))
        {
            yield return www;
            if (www.error != null)
                throw new Exception("WWW download had an error:" + www.error);

            AssetBundle bundle = www.assetBundle;
            if (AssetName == "")
                Instantiate(bundle.mainAsset);
            else
                Instantiate(bundle.LoadAsset(AssetName));

            // Unload the AssetBundles compressed contents to conserve memory
            bundle.Unload(false);

        } // memory is freed from the web stream (www.Dispose() gets called implicitly)
    }
}
