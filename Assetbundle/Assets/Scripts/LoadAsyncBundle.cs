using UnityEngine;
using System;
using System.Collections;

public class LoadAsyncBundle : MonoBehaviour
{



	private string AssetName = "Sphere1";

	IEnumerator Start()
	{
		while (!Caching.ready)
			yield return null;
		using (WWW www = new WWW(Config.BundleURL+ "sphere"))
		{
			yield return www;
			if (www.error != null)
				throw new Exception("WWW download had an error:" + www.error);
			AssetBundle bundle = www.assetBundle;

			// 异步加载
			AssetBundleRequest request = bundle.LoadAssetAsync(AssetName, typeof(GameObject));

			// 等待加载完成
			yield return request;

			// 获取加载好的对象引用
			GameObject prefab = request.asset as GameObject;
			GameObject sphere = Instantiate(prefab);
			sphere.transform.position = new Vector3(0f, 1.5f, 0f);
			// 卸载加载完之后的AssetBundle，节省内存。
			bundle.Unload(false);

		}//由于使用using语法，www.Dispose将在加载完成后调用，释放内存
	}
}

