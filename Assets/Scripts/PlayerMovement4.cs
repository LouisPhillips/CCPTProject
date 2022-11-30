using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

public class PlayerMovement4 : MonoBehaviour
{
    private PlayerController controls;
    private Vector2 movement;

    [Header("Wheels")]
    [SerializeField] public WheelCollider frontRight;
    [SerializeField] public WheelCollider frontLeft;
    [SerializeField] public WheelCollider backRight;
    [SerializeField] public WheelCollider backLeft;

    [Header("Speed Attributes")]
    public float pushForce = 75f;
    public float acceleration = 500f;
    public float breakingForce = 300f;
    public float maxTurnAngle = 15f;
    public float maxAcceleration = 225f;

    public float deaccelerationRate = 0.5f;

    public float currentAcceleration = 0f;
    private float currentBreakforce = 0f;
    public float currentTurnAngle = 0f;

    public float getSpeed;
    [HideInInspector] public bool push;

    private float pushDelay = 0f;
    private float pushMax = 0.5f;

    private Rigidbody rb;

    private Ragdoll ragdoll;

    private bool breaking;

    [Header("Respawn")]
    private GameObject respawnPoint;
    private float respawnDelay = 0f;
    private float respawnMax = 5f;
    [HideInInspector] public bool respawning = false;
    RaycastHit crash;

    private CinemachineVirtualCamera camera;

    void Awake()
    {
        controls = new PlayerController();

        controls.Player.Turning.performed += context => movement = context.ReadValue<Vector2>();

        controls.Player.Push.performed += context => push = true;

        controls.Player.Break.performed += context => breaking = true;
        controls.Player.Break.canceled += context => breaking = false;

        respawnPoint = GameObject.FindGameObjectWithTag("Respawn");

        rb = GetComponent<Rigidbody>();
        ragdoll = GetComponentInChildren<Ragdoll>();

        camera = GetComponentInChildren<CinemachineVirtualCamera>();
    }

    void FixedUpdate()
    {
        Movement();
        Turning();
        Crash();
        SurfaceDetection();
        Breaking();
        FallOver();
        //camera.m_Lens.FieldOfView = camera.m_Lens.FieldOfView + (getSpeed * Time.deltaTime);
    }

    void Movement()
    {
        if (currentAcceleration > 0)
        {
            currentAcceleration -= deaccelerationRate;
        }

        if (currentAcceleration > maxAcceleration)
        {
            currentAcceleration = maxAcceleration;
        }

        if (push)
        {

            pushDelay += Time.deltaTime;
            if (pushDelay > pushMax)
            {
                currentAcceleration += pushForce;
                pushDelay = 0f;
                push = false;
            }
        }

        if (controls.Player.Break.triggered)
        {
            currentBreakforce = breakingForce;
        }
        else
        {
            currentBreakforce = 0f;
        }


        frontLeft.motorTorque = currentAcceleration;
        frontRight.motorTorque = currentAcceleration;
        backLeft.motorTorque = currentAcceleration;
        backRight.motorTorque = currentAcceleration;

        frontLeft.steerAngle = currentTurnAngle;
        frontRight.steerAngle = currentTurnAngle;
        backLeft.steerAngle = -currentTurnAngle;
        backRight.steerAngle = -currentTurnAngle;

        frontLeft.brakeTorque = currentBreakforce;
        frontRight.brakeTorque = currentBreakforce;
        backLeft.brakeTorque = currentBreakforce;
        backRight.brakeTorque = currentBreakforce;
    }

    void Turning()
    {
        // \/\/\/\/ this causing steering snap
        currentTurnAngle = maxTurnAngle * movement.x;

        WheelFrictionCurve frontLeftSidewaysStiffness = frontLeft.sidewaysFriction;
        WheelFrictionCurve frontRightSidewaysStiffness = frontRight.sidewaysFriction;
        WheelFrictionCurve backLeftSidewaysStiffness = backLeft.sidewaysFriction;
        WheelFrictionCurve backRightSidewaysStiffness = backRight.sidewaysFriction;

        if (currentAcceleration > 100)
        {
            /*frontLeftSidewaysStiffness.stiffness = 0.5f;
            frontRightSidewaysStiffness.stiffness = 0.5f;
            backLeftSidewaysStiffness.stiffness = 1;
            backRightSidewaysStiffness.stiffness = 1;*/

            /*backLeftSidewaysStiffness.extremumSlip = 1f;
            backRightSidewaysStiffness.extremumSlip = 1f;*/
        }

        else if (currentAcceleration > 0 && currentAcceleration <= 100)
        {
            /*frontLeftSidewaysStiffness.stiffness = 1;
            frontRightSidewaysStiffness.stiffness = 1;
            backLeftSidewaysStiffness.stiffness = 2;
            backRightSidewaysStiffness.stiffness = 2;

            backLeftSidewaysStiffness.extremumSlip = 0.8f;
            backRightSidewaysStiffness.extremumSlip = 0.8f;*/
        }

        frontLeft.sidewaysFriction = frontLeftSidewaysStiffness;
        frontRight.sidewaysFriction = frontRightSidewaysStiffness;
        backLeft.sidewaysFriction = backLeftSidewaysStiffness;
        backRight.sidewaysFriction = backRightSidewaysStiffness;
    }

    void Crash()
    {
        getSpeed = rb.velocity.magnitude;
        if (getSpeed > 1.5 && Physics.Raycast(transform.position, transform.forward, out crash, 1))
        {
            // player flys off, controls disabled, respawn imminent 
            ragdoll.Die();
        }

        if (respawning)
        {
            Respawn();
        }
        // get normalized speed and if crash and average speed is > just a bump allow for a 'crash'
        Debug.DrawRay(transform.position, transform.forward, Color.cyan, 1);

        
    }

    void Respawn()
    {
        respawnDelay += Time.deltaTime;
        if (respawnDelay > respawnMax)
        {
            transform.position = respawnPoint.transform.position;
            transform.rotation = respawnPoint.transform.rotation;

            respawning = false;
            respawnDelay = 0f;
        }
    }

    void SurfaceDetection()
    {
        //if grounded

        WheelFrictionCurve frontLeftForwardStiffness = frontLeft.forwardFriction;
        WheelFrictionCurve frontRightForwardStiffness = frontRight.forwardFriction;
        WheelFrictionCurve backLeftForwardStiffness = backLeft.forwardFriction;
        WheelFrictionCurve backRightForwardStiffness = backRight.forwardFriction;

        RaycastHit surfaceCheck;

        Debug.DrawRay(transform.position, Vector3.down, Color.red, 1f);
        if (Physics.Raycast(transform.position, Vector3.down, out surfaceCheck, 1f))
        {
            if (surfaceCheck.transform.tag == "Layer/Grass")
            {
                Debug.Log("on grass");
                deaccelerationRate = 1.25f;
                rb.velocity = rb.velocity / 1.075f;
                frontLeftForwardStiffness.stiffness = 0f;
                frontRightForwardStiffness.stiffness = 0f;
                backLeftForwardStiffness.stiffness = 0f;
                backRightForwardStiffness.stiffness = 0f;

                if(rb.velocity.magnitude > 3f)
                {
                    ragdoll.Die();
                }
            }
            else if (surfaceCheck.transform.tag == "Layer/Concrete")
            {
                Debug.Log("on concrete");
                deaccelerationRate = 0.25f;
                rb.velocity = rb.velocity / 1f;
                frontLeftForwardStiffness.stiffness = 1f;
                frontRightForwardStiffness.stiffness = 1f;
                backLeftForwardStiffness.stiffness = 2f;
                backRightForwardStiffness.stiffness = 2f;
            }
            else if (surfaceCheck.transform.tag == "Layer/Dirt")
            {
                Debug.Log("on dirt");
                deaccelerationRate = 0.75f;
                rb.velocity = rb.velocity / 1.05f;
                frontLeftForwardStiffness.stiffness = 1f;
                frontRightForwardStiffness.stiffness = 1f;
                backLeftForwardStiffness.stiffness = 2f;
                backRightForwardStiffness.stiffness = 2f;
            }
        }
        frontLeft.forwardFriction = frontLeftForwardStiffness;
        frontRight.forwardFriction = frontRightForwardStiffness;
        backLeft.forwardFriction = backLeftForwardStiffness;
        backRight.forwardFriction = backRightForwardStiffness;
    }

    void Breaking()
    {
        if(breaking)
        {
            rb.velocity = rb.velocity / 1.01f;
            currentAcceleration = 0;
        }
    }

    void FallOver()
    {
        Debug.Log(transform.rotation.x);
        if (transform.rotation.x > 0.30 || transform.rotation.z > 0.30 || transform.rotation.x < -0.30 || transform.rotation.z < -0.30)
        {
            ragdoll.Die();
        }
    }

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }
}
