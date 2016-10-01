using System;
using UnityEngine;
using System.Collections;

public class CacheBundle : MonoBehaviour
{
	public static readonly string BundleURL =
#if UNITY_ANDROID
		"jar:file://" + Application.dataPath + "!/assets/MyAssetBundles/shape/cube";
#elif UNITY_IPHONE
		Application.dataPath + "/Raw/MyAssetBundles/shape/cube";
#elif UNITY_STANDALONE_WIN || UNITY_EDITOR
		//我们将加载以前打包好的cube
 "file://" + Application.dataPath + "/MyAssetBundles/shape/cube";//由于是编辑器下，我们使用这个路径。
#else
        string.Empty;
#endif

	//还记得吗？在cube这个AssetBundle中有两个资源，Cube1和Cube2
	private string AssetName = "Cube2";

	//版本号
	public int version;

	void Start()
	{
		StartCoroutine(DownloadAndCache());
	}

	IEnumerator DownloadAndCache()
	{
		// 需要等待缓存准备好
		while (!Caching.ready)
			yield return null;

		// 有相同版本号的AssetBundle就从缓存中获取，否则下载进缓存。
		using (WWW www = WWW.LoadFromCacheOrDownload(BundleURL, version))
		{
			yield return www;
			if (www.error != null)
				throw new Exception("WWW download had an error:" + www.error);
			AssetBundle bundle = www.assetBundle;
			GameObject cube = Instantiate(bundle.LoadAsset(AssetName)) as GameObject;
			cube.transform.position = new Vector3(1.5f, 0f, 0f);
			// 卸载加载完之后的AssetBundle，节省内存。
			bundle.Unload(false);

		} //由于使用using语法，www.Dispose将在加载完成后调用，释放内存
	}
}
