using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Config {

    //根据平台，得到相应的路径
    public static readonly string BundleURL =
#if UNITY_ANDROID
		"jar:file://" + Application.dataPath + "!/assets/MyAssetBundles/shape/";
#elif UNITY_IPHONE
		Application.dataPath + "/Raw/MyAssetBundles/shape/";
#elif UNITY_STANDALONE_WIN || UNITY_EDITOR
 "file://" + Application.dataPath + "/MyAssetBundles/shape/";
#else
        string.Empty;
#endif
}
