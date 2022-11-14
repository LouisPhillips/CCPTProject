using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private PlayerController controller;
    private bool push = false;

    private Rigidbody rb;

    public float pushSpeed;
    public float turnSpeed;

    Vector2 movement;
    void Awake()
    {
        controller = new PlayerController();
        rb = GetComponent<Rigidbody>();

        controller.Player.Turning.performed += context => movement = context.ReadValue<Vector2>();

        controller.Player.Push.performed += context => push = true;
        //controller.Movement.Push.performed += context => 
    }

    void OnEnable()
    {
        controller.Enable();
    }
    void OnDisable()
    {
        controller.Disable();
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        Debug.Log(push);

        
       

        Movement(movement);

        // Vector3 direction = rotation * tns.forward * speed
        // rigidbody . addforce (direction)

       
        
    }

    void Movement(Vector2 direction)
    {
        if (push)
        {
            rb.AddForce(transform.forward * pushSpeed, ForceMode.Impulse);
            push = false;
        }
        //rb.AddForce(transform.forward * 1);
        //GetComponent<Rigidbody>().AddForce(new Vector3 (direction.x, 0, direction.y) * 5f);

        //transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0f, direction.x * turnSpeed * Time.deltaTime, 0f));
        float getSpeed = rb.velocity.magnitude;
        float maxSpeed = 5f;
        Vector3 vel = rb.velocity;
        if(vel.magnitude > maxSpeed)
        {
            rb.velocity = vel.normalized * maxSpeed;
        }
        if (getSpeed > 0.1)
        {
            transform.RotateAroundLocal(Vector3.up, direction.x * 2f * Time.deltaTime);
        }
        

        
    }
}
