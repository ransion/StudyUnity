using UnityEngine;
using Assets.Scripts.Utils.GizmosHelper;

public class SphereTest : MonoBehaviour
{
    public Vector3 Center;
    public float Radius;


    void Update()
    {
        GizmosHelper.Instance.DrawSphere("mySphere", transform.position, Center, transform.localScale, Radius, Color.red);
    }
}
