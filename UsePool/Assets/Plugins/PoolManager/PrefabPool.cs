using UnityEngine;
using System.Collections.Generic;

public class PrefabPool
{
	public bool EnableLog { get; set; }

	private GameObject assetPrefab;
	private string assetName;


	private List<GameObject> spawnedList = new List<GameObject>();
	private List<GameObject> despawnedList = new List<GameObject>();

	public PrefabPool(GameObject asset)
	{
		assetPrefab = asset;
		assetName = asset.name;
	}

	public GameObject Spawn()
	{
		if (despawnedList.Count > 0)
		{
			GameObject instance = despawnedList[0];
			if (instance == null)
			{
				despawnedList.RemoveAt(0);
				return SpawnNew();
			}
			despawnedList.Remove(instance);
			spawnedList.Add(instance);
			instance.SetActive(true);
			return instance;
		}
		return SpawnNew();
	}

	public GameObject SpawnNew()
	{
		var instance = GameObject.Instantiate(assetPrefab) as GameObject;
		if (instance == null)
		{
			if (EnableLog)
				Debug.Log(string.Format("Can't instantiate instance {0}", assetName));
			return null;
		}
		spawnedList.Add(instance);
		return instance;
	}

	public void Despawn(GameObject instance)
	{
		if (!spawnedList.Contains(instance))
			return;

		instance.SetActive(false);
		spawnedList.Remove(instance);
		despawnedList.Add(instance);
	}
}
