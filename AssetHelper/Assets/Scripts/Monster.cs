using UnityEngine;
using System.Collections;

public class Monster : MonoBehaviour {

	public Rigidbody rigBody;
	public CapsuleCollider capsuleCollider;

	public void DoSerialized()
	{
		rigBody = GetComponent<Rigidbody>();
		if (rigBody == null)
			rigBody = gameObject.AddComponent<Rigidbody>();

		capsuleCollider = GetComponent<CapsuleCollider>();
		if(null == capsuleCollider)
			capsuleCollider = gameObject.AddComponent<CapsuleCollider>();
	}
}
