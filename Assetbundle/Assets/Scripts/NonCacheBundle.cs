using System;
using UnityEngine;
using System.Collections;
class NonCacheBundle : MonoBehaviour
{

    //还记得吗？在cube这个AssetBundle中有两个资源，Cube1和Cube2
    private string AssetName = "Cube1";

	IEnumerator Start()
	{
		// 从URL中下载文件，不会存储在缓存中。
		using (WWW www = new WWW(Config.BundleURL+ "cube"))
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


