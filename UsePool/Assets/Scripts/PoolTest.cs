using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolTest : MonoBehaviour {

    private SpawnPool testPool;

	// Use this for initialization
	void Start()
	{
        testPool = PoolUtil.CreateSpawnPool("TestPool");
		var asset = Resources.Load("Prefabs/Test1") as GameObject;
        PrefabPool prefabPool = new PrefabPool(asset);
	}
}
