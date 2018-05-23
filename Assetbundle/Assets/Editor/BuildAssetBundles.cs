using UnityEditor;

public class BuildAssetBundles
{

    [MenuItem("Assets/Build AssetBundles")]
    static void BuildAllAssetBundles()
    {    
        BuildPipeline.BuildAssetBundles("Assets/MyAssetBundles", BuildAssetBundleOptions.UncompressedAssetBundle, EditorUserBuildSettings.activeBuildTarget);
    }
}
