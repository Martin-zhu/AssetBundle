using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 资源缓存层
/// </summary>
public class AssetCaching
{
    /// <summary>
    /// 已经加载过的 资源名字 和 资源 的映射关系
    /// </summary>
    private Dictionary<string, TempObject> nameAssetDict;

    public AssetCaching()
    {
        nameAssetDict = new Dictionary<string, TempObject>();
    }

    /// <summary>
    /// 添加缓存
    /// </summary>
    public void AddAsset(string assetName, TempObject asset)
    {
        if (nameAssetDict.ContainsKey(assetName))
        {
            Debug.LogWarning("此 " + assetName + " 资源已经加载!");
            return;
        }
        //缓存起来
        nameAssetDict.Add(assetName, asset);
    }

    /// <summary>
    /// 获取缓存的资源
    /// </summary>
    /// <param name="assetName"></param>
    /// <returns></returns>
    public Object[] GetAsset(string assetName)
    {
        if (nameAssetDict.ContainsKey(assetName))
        {
            return nameAssetDict[assetName].Asset.ToArray();
        }
        else
        {
            Debug.LogError("此 " + assetName + " 资源尚未被加载!");
            return null;
        }
    }

    /// <summary>
    /// 卸载资源
    /// </summary>
    /// <param name="assetName"></param>
    public void UnLoadAsset(string assetName)
    {
        //如果资源已经被加载 就直接释放
        if (nameAssetDict.ContainsKey(assetName))
        {
            nameAssetDict[assetName].UnloadAsset();
        }
        else
        {
            Debug.LogError("此 " + assetName + " 资源尚未被加载!");
        }
    }

    /// <summary>
    /// 卸载所有的资源
    /// </summary>
    public void UnLoadAllAssets()
    {
        foreach (string assetName in nameAssetDict.Keys)
            UnLoadAsset(assetName);

        nameAssetDict.Clear();
    }

}
