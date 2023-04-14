using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OllieCamera : MonoBehaviour
{
    public Transform target;

    public float xOffset = 0;
    public float yOffset = 2;
    public float zOffset = -2;
    void Update()
    {
        //transform.LookAt(target);
        transform.position = new Vector3(target.transform.position.x, target.transform.position.y, target.transform.position.z)  + new Vector3(xOffset, yOffset, zOffset);
        transform.eulerAngles = new Vector3(target.transform.eulerAngles.x, target.GetComponentInParent<PlayerMovement4>().lastGroundedY, target.transform.eulerAngles.z);
    }
}
