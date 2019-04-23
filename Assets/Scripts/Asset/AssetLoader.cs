using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// 加载 包里面的资源
/// </summary>
public class AssetLoader : System.IDisposable
{
    /// <summary>
    /// 当前资源包
    /// </summary>
    private AssetBundle assetBundle;

    public AssetBundle AssetBundle
    {
        set { this.assetBundle = value; }
    }

    /// <summary>
    /// 获取单个资源
    /// </summary>
    /// <param name="assetName">资源名字</param>
    /// <returns>Obj类型的资源</returns>
    public Object LoadAsset(string assetName)
    {
        if (assetBundle == null)
        {
            Debug.LogError("当前资源包为空，无法获取该 " + assetName + " 资源");
            return null;
        }
        else if (!assetBundle.Contains(assetName))
        {
            Debug.LogError("当前资源包里不包括 " + assetName + " 资源");
            return null;
        }
        else
            return assetBundle.LoadAsset(assetName);
    }

    ///// <summary>
    ///// 获取单个资源
    ///// </summary>
    ///// <param name="assetName">资源名字</param>
    ///// <returns>Obj类型的资源</returns>
    //public T LoadAsset<T>(string assetName)
    //    where T : class
    //{
    //    if (assetBundle == null)
    //    {
    //        Debug.LogError("当前资源包为空，无法获取该 " + assetName + " 资源");
    //        return null;
    //    }
    //    else if (!assetBundle.Contains(assetName))
    //    {
    //        Debug.LogError("当前资源包里不包括 " + assetName + " 资源");
    //        return null;
    //    }
    //    else
    //        return assetBundle.LoadAsset(assetName) as T;
    //}

    /// <summary>
    /// 获取包里所有资源
    /// </summary>
    /// <returns></returns>
    public Object[] LoadAllAssets()
    {
        if (assetBundle == null)
        {
            Debug.LogError("当前资源包为空，无法获取资源");
            return null;
        }
        else
            return assetBundle.LoadAllAssets();
    }

    /// <summary>
    /// 获取带有子物体的资源
    /// </summary>
    /// <param name="assetName">资源名称</param>
    /// <returns>所有资源</returns>
    public Object[] LoadAssetWithSubAssets(string assetName)
    {
        if (assetBundle == null)
        {
            Debug.LogError("当前资源包为空，无法获取该 " + assetName + " 资源");
            return null;
        }
        else if (!assetBundle.Contains(assetName))
        {
            Debug.LogError("当前资源包里不包括 " + assetName + " 资源");
            return null;
        }
        else
            return assetBundle.LoadAssetWithSubAssets(assetName);
    }

    /// <summary>
    /// 卸载资源
    /// </summary>
    /// <param name="asset">资源</param>
    public void UnLoadAsset(Object asset)
    {
        Resources.UnloadAsset(asset);
    }

    /// <summary>
    /// 释放资源包
    /// </summary>
    public void Dispose()
    {
        if (this.assetBundle == null)
            return;

        //false:只卸载 包
        //true:卸载 包 和 Obj
        assetBundle.Unload(false);
    }

    /// <summary>
    /// 调试专用
    /// </summary>
    public void GetAllAssetNames()
    {
        string[] names = assetBundle.GetAllAssetNames();

        foreach (var item in names)
            Debug.Log(item);
    }
}
