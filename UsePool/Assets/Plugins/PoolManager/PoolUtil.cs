using UnityEngine;
using System.Collections;

public static class PoolUtil
{
    public static SpawnPool CreateSpawnPool(string poolName)
    {
        GameObject spawnPool = new GameObject(poolName);
        return spawnPool.AddComponent<SpawnPool>();
    }
}
