using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 对Object的封装
/// </summary>
public class TempObject
{
    /// <summary>
    /// 资源列表
    /// </summary>
    private List<Object> assetList;

    public TempObject(params Object[] assets)
    {
        this.assetList = new List<Object>(assets);
    }

    /// <summary>
    /// 资源
    /// </summary>
    public List<Object> Asset
    {
        get { return assetList; }
    }

    ///// <summary>
    ///// 添加资源
    ///// </summary>
    ///// <param name="assets"></param>
    //public void AddAsset(params Object[] assets)
    //{
    //}

    /// <summary>
    /// 卸载资源
    /// </summary>
    public void UnloadAsset()
    {
        for (int i = assetList.Count - 1; i >= 0; i--)
        {
            Resources.UnloadAsset(assetList[i]);
        }
    }


}
