using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 控制 一个场景里面所有的资源包
/// </summary>
public class OneSceneAssetBundles
{
    /// <summary>
    /// 包名 和 对应的包 的映射
    /// </summary>
    private Dictionary<string, AssetBundleRelation> nameBundleDict;

    /// <summary>
    /// 包名 和 对应包的缓存 的映射
    /// </summary>
    private Dictionary<string, AssetCaching> nameCacheDict;

    /// <summary>
    /// 当前场景的名字
    /// </summary>
    private string sceneName;

    /// <summary>
    /// 构造函数
    /// </summary>
    public OneSceneAssetBundles(string sceneName)
    {
        this.sceneName = sceneName;

        nameBundleDict = new Dictionary<string, AssetBundleRelation>();
        nameCacheDict = new Dictionary<string, AssetCaching>();
    }

    /// <summary>
    /// 是否加载了这个包
    /// </summary>
    /// <param name="bundleName">包名</param>
    /// <returns></returns>
    public bool IsLoading(string bundleName)
    {
        return nameBundleDict.ContainsKey(bundleName);
    }

    /// <summary>
    /// 是否加载完成这个包
    /// </summary>
    /// <param name="bundleName">包名</param>
    /// <returns></returns>
    public bool IsFinsh(string bundleName)
    {
        if (IsLoading(bundleName))
        {
            return nameBundleDict[bundleName].Finish;
        }

        return false;
    }

    #region 加载包

    /// <summary>
    /// 加载资源包
    /// </summary>
    /// <param name="bundleName"></param>
    /// <param name="lp"></param>
    /// <param name="labcb"></param>
    public void LoadAssetBundle(string bundleName, LoadProgress lp, LoadAssetBundleCallback labcb)
    {
        //如果这个包已经被加载了 就提示
        if (nameBundleDict.ContainsKey(bundleName))
        {
            Debug.LogWarning("此包已经加载了 : " + bundleName);
            return;
        }
        else
        {
            //没有被加载
            AssetBundleRelation assetBundleRelation = new AssetBundleRelation(bundleName, lp);
            //保存到字典里面
            nameBundleDict.Add(bundleName, assetBundleRelation);
            //StartCoroutine("Load", bundleName);


            //开始加载
            labcb(sceneName, bundleName);
        }
    }

    /// <summary>
    /// 加载包
    /// </summary>
    /// <param name="bundleName">包名</param>
    /// <returns></returns>
    public IEnumerator Load(string bundleName)
    {
        while (!AssetBundleManifestLoader.Instance.Finish)
            yield return null;

        AssetBundleRelation assetBundleRelation = nameBundleDict[bundleName];
        //先获取这个包的所有依赖关系
        string[] dependenceBundles = AssetBundleManifestLoader.Instance.GetDependencies(bundleName);
        //添加他的依赖关系
        foreach (var dependencebundleName in dependenceBundles)
        {
            assetBundleRelation.AddDependence(dependencebundleName);
            //加载这个包的所有依赖关系
            yield return LoadDependence(dependencebundleName, bundleName, assetBundleRelation.LoadProgess);
        }

        //开始加载这个包
        yield return assetBundleRelation.Load();
    }

    /// <summary>
    /// 加载依赖的包
    /// </summary>
    /// <param name="bundleName">包名</param>
    /// <param name="referenceBundleName">被依赖的包名</param>
    /// <param name="lp">进度回调</param>
    /// <returns></returns>
    private IEnumerator LoadDependence(string bundleName, string referenceBundleName, LoadProgress lp)
    {
        if (nameBundleDict.ContainsKey(bundleName))
        {
            //已经加载过 就直接添加他的被依赖关系
            AssetBundleRelation assetBundleRelation = nameBundleDict[bundleName];
            //添加这个包的被依赖关系
            assetBundleRelation.AddReference(referenceBundleName);
        }
        else
        {
            //没有加载过 就创建一个新的
            AssetBundleRelation assetBundleRelation = new AssetBundleRelation(bundleName, lp);
            //添加这个包的被依赖关系
            assetBundleRelation.AddReference(referenceBundleName);
            //保存到字典里面
            nameBundleDict.Add(bundleName, assetBundleRelation);

            //开始加载这个依赖的包
            yield return Load(bundleName);
        }
    }

    #endregion

    #region 加载资源

    //如果 第一次获取资源 怎么获取呢？ 就通过 AssetBundle.LoadAsset(assetName)
    //第二次呢？
    //还是通过这个方法 获取 就浪费资源
    //就直接把它保存起来  缓存起来 方便下次使用


    /// <summary>
    /// 获取单个资源
    /// </summary>
    /// <param name="assetName">资源名字</param>
    /// <returns>Obj类型的资源</returns>
    public Object LoadAsset(string bundleName, string assetName)
    {
        //先判断缓存没缓存
        if (nameCacheDict.ContainsKey(bundleName))
        {
            Object[] assets = nameCacheDict[bundleName].GetAsset(assetName);
            //安全校验
            if (assets != null)
                return assets[0];
        }

        //当前包有没有被加载
        if (!nameBundleDict.ContainsKey(bundleName))
        {
            Debug.LogError("当前 " + bundleName + " 包没有加载，无法获取资源");
            return null;
        }

        //当前的包已经被加载了 
        Object asset = nameBundleDict[bundleName].LoadAsset(assetName);
        TempObject tmpAsset = new TempObject(asset);

        //有这个缓存层 里面也有资源 但是 这次获取的资源名字 是以前没缓存过的
        if (nameCacheDict.ContainsKey(bundleName))
        {
            //直接加进去
            nameCacheDict[bundleName].AddAsset(assetName, tmpAsset);
        }
        else
        {
            // 但是 第一次获取这个包里面的资源

            //创建一个新的缓存层
            AssetCaching caching = new AssetCaching();
            caching.AddAsset(assetName, tmpAsset);

            //保存到字典里面 方便下次使用
            nameCacheDict.Add(bundleName, caching);
        }

        return asset;
    }

    /// <summary>
    /// 获取带有子物体的资源
    /// </summary>
    /// <param name="assetName">资源名称</param>
    /// <returns>所有资源</returns>
    public Object[] LoadAssetWithSubAssets(string bundleName, string assetName)
    {
        //if (!nameBundleDict.ContainsKey(bundleName))
        //{
        //    Debug.LogError("当前 " + bundleName + " 包没有加载，无法获取资源");
        //    return null;
        //}

        //return nameBundleDict[bundleName].LoadAssetWithSubAssets(assetName);

        //-------------------------------------------------------------------

        //先判断缓存没缓存
        if (nameCacheDict.ContainsKey(bundleName))
        {
            Object[] assets = nameCacheDict[bundleName].GetAsset(assetName);
            //安全校验
            if (assets != null)
                return assets;
        }

        //当前包有没有被加载
        if (!nameBundleDict.ContainsKey(bundleName))
        {
            Debug.LogError("当前 " + bundleName + " 包没有加载，无法获取资源");
            return null;
        }

        //当前的包已经被加载了 
        Object[] asset = nameBundleDict[bundleName].LoadAssetWithSubAssets(assetName);
        TempObject tmpAsset = new TempObject(asset);

        //有这个缓存层 里面也有资源 但是 这次获取的资源名字 是以前没缓存过的
        if (nameCacheDict.ContainsKey(bundleName))
        {
            //直接加进去
            nameCacheDict[bundleName].AddAsset(assetName, tmpAsset);
        }
        else
        {
            // 但是 第一次获取这个包里面的资源

            //创建一个新的缓存层
            AssetCaching caching = new AssetCaching();
            caching.AddAsset(assetName, tmpAsset);

            //保存到字典里面 方便下次使用
            nameCacheDict.Add(bundleName, caching);
        }

        return asset;
    }

    /// <summary>
    /// 获取包里所有资源
    /// </summary>
    /// <returns></returns>
    public Object[] LoadAllAssets(string bundleName)
    {
        if (!nameBundleDict.ContainsKey(bundleName))
        {
            Debug.LogError("当前 " + bundleName + " 包没有加载，无法获取资源");
            return null;
        }
        else
            return nameBundleDict[bundleName].LoadAllAssets();
    }


    #endregion

    #region 卸载

    /// <summary>
    /// 卸载资源
    /// </summary>
    /// <param name="asset">资源</param>
    public void UnLoadAsset(string bundleName, string assetName)
    {
        if (!nameCacheDict.ContainsKey(bundleName))
        {
            Debug.LogError("当前 " + bundleName + " 包没有缓存资源，无法卸载资源");
        }
        else
        {
            //已经缓存资源了 可以卸载
            nameCacheDict[bundleName].UnLoadAsset(assetName);

            Resources.UnloadUnusedAssets();
        }
    }

    /// <summary>
    /// 卸载一个包里面的所有资源
    /// </summary>
    /// <param name="bundleName"></param>
    public void UnLoadAllAsset(string bundleName)
    {
        if (!nameCacheDict.ContainsKey(bundleName))
        {
            Debug.LogError("当前 " + bundleName + " 包没有缓存资源，无法卸载资源");
        }
        else
        {
            //已经缓存资源了 可以卸载
            nameCacheDict[bundleName].UnLoadAllAssets();
            nameCacheDict.Remove(bundleName);

            Resources.UnloadUnusedAssets();
        }
    }

    /// <summary>
    /// 卸载所有的资源
    /// </summary>
    public void UnLoadAll()
    {
        foreach (var item in nameCacheDict.Keys)
            UnLoadAllAsset(item);

        nameCacheDict.Clear();
    }

    /// <summary>
    /// 卸载包
    /// </summary>
    public void Dispose(string bundleName)
    {
        if (!nameBundleDict.ContainsKey(bundleName))
        {
            Debug.LogError("当前 " + bundleName + " 包没有加载，无法获取资源");
            return;
        }

        //先获取到当前的包
        AssetBundleRelation assetBundleRelation = nameBundleDict[bundleName];

        //获取当前包的所有依赖关系
        string[] allDependences = assetBundleRelation.GetAllDependences();
        foreach (string dependenceBundleName in allDependences)
        {
            AssetBundleRelation tmpAssetBundle = nameBundleDict[dependenceBundleName];
            //首先 移除 依赖的包里面的被依赖关系
            if (tmpAssetBundle.RemoveReference(bundleName))
            {
                //递归
                Dispose(tmpAssetBundle.BundleName);
            }
        }

        //才开始卸载当前包
        if (assetBundleRelation.GetAllReferences().Length <= 0)
        {
            nameBundleDict[bundleName].Dispose();
            nameBundleDict.Remove(bundleName);
        }

    }

    /// <summary>
    /// 卸载所有包
    /// </summary>
    public void DisposeAll()
    {
        foreach (var item in nameBundleDict.Keys)
            Dispose(item);

        nameBundleDict.Clear();
    }

    /// <summary>
    ///  卸载所有包和资源
    /// </summary>
    public void DisposeAndUnLoadAll()
    {
        UnLoadAll();

        DisposeAll();
    }

    #endregion

    /// <summary>
    /// 调试专用
    /// </summary>
    public void GetAllAssetNames(string bundleName)
    {
        nameBundleDict[bundleName].GetAllAssetNames();
    }
}
