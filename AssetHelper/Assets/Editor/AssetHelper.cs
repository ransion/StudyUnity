/// <summary>
/// 文件名： AssetHelper 用于提供保存资源的自动序列化处理
/// </summary>
using UnityEngine;
using UnityEditor;
using System.IO;

public class AssetHelper : UnityEditor.AssetModificationProcessor{
    private static readonly string MONSTER_FOLDER = "Monster/";

    #region Apply
    [InitializeOnLoadMethod]
    static void StartInitializeOnLoadMethod()
    {
        // 注册Apply时的回调
        PrefabUtility.prefabInstanceUpdated = delegate(GameObject instance)
        {
            if(instance)
            SaveMonsterPrefab(instance);
        };
    }    

    static void SaveMonsterPrefab(GameObject instance)
    {
        string prefabPath = AssetDatabase.GetAssetPath(PrefabUtility.GetPrefabParent(instance));
        if(!IsMonsterPrefab(prefabPath))
            return;

        Debug.LogFormat("SaveMonsterPrefab Path = {0}", prefabPath);
        Monster comp = instance.GetComponent<Monster>();
        if (null == comp)
        {
            string msg = string.Format("{0} 缺少Monster组件", prefabPath);
            EditorUtility.DisplayDialog("Apply a Monster prefab!", msg, "OK");
            return;
        }

        comp.DoSerialized();
    }


    static bool IsMonsterPrefab(string path){
        if(path.Contains(MONSTER_FOLDER) && Path.GetExtension(path) == ".prefab")
            return true;

        return false;
    }
    #endregion
    

    #region Save

    static string[] OnWillSaveAssets(string[] paths){
        SaveMonsterPrefabs(paths);
        return paths;
    }

    static void SaveMonsterPrefabs(string[] paths)
    {
        foreach (string path in paths)
        {
            if(!IsMonsterPrefab(path))
                continue;

            Debug.LogFormat("SaveMonsterPrefabs {0}", path);

            GameObject prefab = (GameObject)AssetDatabase.LoadAssetAtPath(path, typeof(GameObject));
            if(prefab == null)
            {
                Debug.LogWarning(string.Format("Can not load prefab {0}", path));
                continue;
            }

            GameObject go = UnityEngine.Object.Instantiate(prefab) as GameObject;
            Monster comp = go.GetComponent<Monster>();
            if (null == comp)
            {
                Debug.LogWarning(string.Format("{0} 缺少Monster组件", path));
                continue;
            }

            comp.DoSerialized();
            PrefabUtility.ReplacePrefab(go, prefab);
            UnityEngine.Object.DestroyImmediate(go);
        }
    }

    #endregion

}