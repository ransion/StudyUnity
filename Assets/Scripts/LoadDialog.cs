using UnityEngine;
using System.Collections;

public class LoadDialog : MonoBehaviour
{

    void Start()
    {
        GameObject UIRoot = GameObject.FindWithTag("UIRoot");
        if (UIRoot != null)
        {
            Object dialogPrefab = Resources.Load("Prefabs/Dialog") as Object;
            if (dialogPrefab != null)
            {
                GameObject dialog = Instantiate(dialogPrefab) as GameObject;
                dialog.transform.SetParent(UIRoot.transform, false);
                dialog.transform.SetAsLastSibling();
            }
            else
            {
                Debug.Log("Failed to load prefab file");
            }
        }
        else
        {
            Debug.LogError("There is not a GameObject with tag UIRoot");
        }
    }
}