using System.Collections.Generic;
using UnityEngine;

public class SetLayer : MonoBehaviour
{

	void Start()
	{
		GameObject root = Util.CreateHeirarchy();

		float startTime = Time.realtimeSinceStartup;
		Util.SetLayerOnAll(root, LayerMask.NameToLayer("Cube"));
		float totalTimeMs = (Time.realtimeSinceStartup - startTime) * 1000;
		print("Set layer on all time: " + totalTimeMs + "ms");

		startTime = Time.realtimeSinceStartup;
		Util.SetLayerRecusively(root, LayerMask.NameToLayer("Sphere"));
		totalTimeMs = (Time.realtimeSinceStartup - startTime) * 1000;
		print("Set layer on all recursive time: " + totalTimeMs + "ms");

		startTime = Time.realtimeSinceStartup;
		Util.SetLayerNotRecusively(root.transform, LayerMask.NameToLayer("Cube"));
		totalTimeMs = (Time.realtimeSinceStartup - startTime) * 1000;
		print("Set layer on not recursive time: " + totalTimeMs + "ms");
	}

}

public static class Util
{
	public static GameObject CreateHeirarchy()
	{
		GameObject root = new GameObject();

		GameObject[] children = new GameObject[100];
		for (int i = 0; i < 100; i++)
		{
			GameObject child = new GameObject();
			child.transform.parent = root.transform;
			children[i] = child;
		}

		GameObject[] grandchildren = new GameObject[1000];
		for (int i = 0; i < 1000; i++)
		{
			GameObject grandchild = new GameObject();
			grandchild.transform.parent = children[Random.Range(0, 99)].transform;
			grandchildren[i] = grandchild;
		}


		for (int i = 0; i < 10000; i++)
		{
			GameObject greatgrandchild = new GameObject();
			greatgrandchild.transform.parent = grandchildren[Random.Range(0, 999)].transform;
		}

		return root;
	}

	public static void SetLayerOnAll(GameObject obj, int layer)
	{
		if (null == obj)
			return;

		foreach (Transform trans in obj.GetComponentsInChildren<Transform>(true))
		{
			trans.gameObject.layer = layer;
		}

	}

	public static void SetLayerRecusively(GameObject obj, int layer)
	{
		if (null == obj)
			return;

		obj.layer = layer;
		foreach (Transform child in obj.transform)
			SetLayerRecusively(child.gameObject, layer);
	}

	public static void SetLayerNotRecusively(Transform root, int layer)
	{
		Stack<Transform> moveTargets = new Stack<Transform>();
		moveTargets.Push(root);
		Transform currentTarget;
		while (moveTargets.Count != 0)
		{
			currentTarget = moveTargets.Pop();
			currentTarget.gameObject.layer = layer;
			foreach (Transform child in currentTarget)
				moveTargets.Push(child);
		}
	}
}
