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

    public float gravity = 9.8f;
    public float gravityMultiplier = 2f;
    private bool resetPush = true;

    public float distToGround;
    Vector2 movement;
    void Awake()
    {
        controller = new PlayerController();
        rb = GetComponent<Rigidbody>();

        controller.Player.Turning.performed += context => movement = context.ReadValue<Vector2>();
        controller.Player.Turning.performed += context => resetPush = true;
        //controller.Player.Turning.performed += context => rb.mass = 0.1f;
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
        Debug.Log(resetPush);






        // Vector3 direction = rotation * tns.forward * speed
        // rigidbody . addforce (direction)


        // ground check

        if (!IsGrounded())
        {
            rb.velocity = (Vector3.down + rb.velocity) * gravityMultiplier;
        }
        else
        {
            Movement(movement);
        }


    }

    void Movement(Vector2 direction)
    {
        if (push)
        {
            rb.AddForce(transform.forward * pushSpeed, ForceMode.Impulse);
            //rb.mass += 0.03f;
        }


        //rb.velocity -= new Vector3(rb.velocity.x, gravity, rb.velocity.z);


        //4/3 p ^ 3;
        //rb.AddForce(transform.forward * pushSpeed, ForceMode.Impulse);
        //rb.AddForce(transform.forward * 1);
        //GetComponent<Rigidbody>().AddForce(new Vector3 (direction.x, 0, direction.y) * 5f);

        //transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0f, direction.x * turnSpeed * Time.deltaTime, 0f));
        float getSpeed = rb.velocity.magnitude;
        float maxSpeed = 5f;
        Vector3 vel = rb.velocity;
        if (vel.magnitude > maxSpeed)
        {
            rb.velocity = vel.normalized * maxSpeed;
        }
        if (getSpeed > 0.1)
        {
            transform.RotateAroundLocal(Vector3.up, direction.x * turnSpeed * Time.deltaTime);

            //ANGULAR VELOCITY
        }





    }

    bool IsGrounded()
    {
        return Physics.Raycast(transform.position, -Vector3.up, (float)(distToGround + 0.1));
    }
}
