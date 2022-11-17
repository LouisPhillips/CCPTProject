using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement2 : MonoBehaviour
{
    public Rigidbody rigidbody;
    PlayerController controls;
    Vector2 movement;
    bool moving;

    public float forwardAcceleration = 8;
    public float reverseAcceleration = 4;
    public float maxSpeed = 50;
    public float turnStrength = 180;
    public float drag = 2f;

    private float speedInput;
    private float turnInput;

    private bool grounded;

    public LayerMask ground;
    public float groundRayLength = 0.5f;
    public Transform groundRayPoint;

    public float pushSpeed = 1f;
    private bool push;
    private void Awake()
    {
        controls = new PlayerController();

        controls.Player.Turning.performed += context => movement = context.ReadValue<Vector2>();
        controls.Player.Turning.canceled += context => movement = Vector2.zero;

        controls.Player.Push.performed += context => push = true;

    }
    private void OnEnable()
    {
        controls.Player.Enable();
    }
    private void OnDisable()
    {
        controls.Player.Disable();
    }

    void Start()
    {
        rigidbody.transform.parent = null;
    }

    void Update()
    {
        Rotation();
        Movement();
    }

    void Movement()
    {

        if (push)
        {
            rigidbody.AddForce(transform.forward * pushSpeed, ForceMode.Impulse);
            push = false;
        }

        float getSpeed = rigidbody.velocity.magnitude;
        float maxSpeed = 5f;
        Vector3 vel = rigidbody.velocity;
        if (vel.magnitude > maxSpeed)
        {
            rigidbody.velocity = vel.normalized * maxSpeed;
        }

        if (movement.x > 0.1)
        {

        }
        if (grounded)
        {
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0f, movement.x * turnStrength * Time.deltaTime, 0f));
        }

        transform.position = rigidbody.transform.position;
    }

    void Rotation()
    {
        grounded = false;
        RaycastHit hit;

        if (Physics.Raycast(groundRayPoint.position, -transform.up, out hit, groundRayLength, ground))
        {
            grounded = true;

            transform.rotation = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;
        }
        if (grounded)
        {
            rigidbody.drag = drag;
            if (Mathf.Abs(speedInput) > 0)
            {
                rigidbody.AddForce(transform.forward * speedInput);
            }
        }
        else
        {
            rigidbody.drag = 0.1f;
            rigidbody.AddForce(Vector3.up * -5f * 100f);
        }
    }
}
