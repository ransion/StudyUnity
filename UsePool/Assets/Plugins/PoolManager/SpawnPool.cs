﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPool : MonoBehaviour
{
    private Dictionary<string, PrefabPool> assetPools = new Dictionary<string, PrefabPool>();

    public GameObject Spawn(GameObject prefab)
    {
        PrefabPool assetPool;
        if(assetPools.TryGetValue(prefab.name, out assetPool))
        {
            return assetPool.Spawn();
        }
        assetPool = new PrefabPool(prefab);
        assetPools.Add(prefab.name, assetPool);
        return assetPool.Spawn();
    }

    public void Despawn(GameObject instance)
    {
		PrefabPool assetPool;
        var prefabName = instance.name;
        prefabName = prefabName.Substring(0, prefabName.LastIndexOf("(Clone", System.StringComparison.OrdinalIgnoreCase));
        if (assetPools.TryGetValue(prefabName, out assetPool))
		{
            assetPool.Despawn(instance);
            return;
		}
        Debug.LogWarning("The instance you want to despawn is not in pool!");
    }

}
