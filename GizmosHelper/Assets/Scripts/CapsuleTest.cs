using Assets.Scripts.Utils.GizmosHelper;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

public class CapsuleTest : MonoBehaviour
{

    public Vector3 Center;
    public float Radius;
    public float Height;
    public CapsuleDirection Direction;

    // Update is called once per frame
    void Update()
    {
        GizmosHelper.Instance.DrawCapsule("myCapsule", transform.position, Center, transform.localScale, Radius, Height, Direction, Color.red);
    }

}
