using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController3 : MonoBehaviour
{
    private PlayerController controls;
    public Vector2 movement;
    public GameObject skateboard;

    void Awake()
    {
        controls = new PlayerController();

        controls.Player.Turning.performed += context => movement = context.ReadValue<Vector2>();
    }

    // Update is called once per frame
    void Update()
    {
        //transform.position = new Vector3(transform.position.x + movement.x / 10000, transform.position.y, transform.position.z);
        transform.RotateAround(skateboard.transform.position, Vector3.up, 0.001f * movement.x);
        skateboard.transform.LookAt(transform.position);


        if(movement.x == 0)
        {
            //transform.rotation = new Quaternion(transform.rotation.x, 0f, transform.rotation.z, 0);
            transform.position = new Vector3(0, transform.position.y, transform.position.z);
        }
    }

    void OnEnable()
    {
        controls.Enable();
    }
    void OnDisable()
    {
        controls.Disable();
    }
}
