using System;
using UnityEngine;
using System.Collections;
class NonCacheBundle : MonoBehaviour
{
	//根据平台，得到相应的路径
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
	private string AssetName = "Cube1";

	IEnumerator Start()
	{
		// 从URL中下载文件，不会存储在缓存中。
		using (WWW www = new WWW(BundleURL))
		{
			yield return www;
			if (www.error != null)
				throw new Exception("WWW download had an error:" + www.error);
			AssetBundle bundle = www.assetBundle;
			GameObject cube = Instantiate(bundle.LoadAsset(AssetName)) as GameObject;
			cube.transform.position = new Vector3(0f, 0f, 0f);
			// 卸载加载完之后的AssetBundle，节省内存。
			bundle.Unload(false);

		}//由于使用using语法，www.Dispose将在加载完成后调用，释放内存
	}
}
