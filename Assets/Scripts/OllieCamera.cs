using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OllieCamera : MonoBehaviour
{
    public Transform target;

    public float yOffset = 2.64f;
    
    void Update()
    {

        /*transform.position += new Vector3(target.GetComponentInParent<PlayerMovement4>().rb.velocity.x * Time.deltaTime,
            -1.75f * Time.deltaTime, 
            target.GetComponentInParent<PlayerMovement4>().rb.velocity.z  * Time.deltaTime);*/
        //transform.eulerAngles = new Vector3(target.transform.eulerAngles.x, target.GetComponentInParent<PlayerMovement4>().lastGroundedY, target.transform.eulerAngles.z);

        transform.position = new Vector3(transform.position.x + target.GetComponentInParent<PlayerMovement4>().rb.velocity.x * Time.deltaTime, target.transform.position.y + yOffset,
            transform.position.z + target.GetComponentInParent<PlayerMovement4>().rb.velocity.z * Time.deltaTime);
    }
}
