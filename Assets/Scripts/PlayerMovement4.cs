using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;
using UnityEngine.UI;

public class PlayerMovement4 : MonoBehaviour
{
    private PlayerController controls;
    private Vector2 movement;
    private Vector2 camMovement;
    private Vector2 camLook;

    private GameObject character;

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

    public bool olliePressed;
    private float ollieDelay = 0f;
    private float OllieDelayMax = 0.4f;
    public static bool riding = true;
    public static bool ollie = true;

    public float freeCamTurnSpeed = 15f;

    public static bool surfaceBeingChanged = false;
    RaycastHit objectCheck;
    private GameObject ui;
    public int surfaceIndex = 0;

    [Header("Respawn")]
    private GameObject respawnPoint;
    private float respawnDelay = 0f;
    private float respawnMax = 5f;
    [HideInInspector] public bool respawning = false;
    RaycastHit crash;

    public CinemachineVirtualCamera playerCam;
    public CinemachineVirtualCamera freeCam;
    public static bool MainCamOn = true;

    void Awake()
    {
        controls = new PlayerController();

        controls.Player.Break.performed += context => breaking = true;
        controls.Player.Break.canceled += context => breaking = false;

        controls.Player.GoLeft.performed += context => surfaceIndex -= 1;
        controls.Player.GoRight.performed += context => surfaceIndex += 1;

        respawnPoint = GameObject.FindGameObjectWithTag("Respawn");

        rb = GetComponent<Rigidbody>();
        ragdoll = GetComponentInChildren<Ragdoll>();

        playerCam = GetComponentInChildren<CinemachineVirtualCamera>();

        freeCam.gameObject.SetActive(false);

        ui = GameObject.FindGameObjectWithTag("Surface/Changer");
        ui.SetActive(false);

        character = GameObject.FindGameObjectWithTag("Player");
    }

    void FixedUpdate()
    {
        if (MainCamOn)
        {
            Movement();
            Turning();
            Crash();
            SurfaceDetection();
            Breaking();
            FallOver();
            Ollie();
            ui.SetActive(false);
        }
    }
    private void Update()
    {
        if (!MainCamOn)
        {
            CameraControls();
        }
        CameraSwitch();
        Debug.DrawRay(transform.position, transform.forward, Color.cyan, 0.6f);
    }

    void Movement()
    {
        controls.Player.Push.performed += context => push = true;

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
        controls.Player.Turning.performed += context => movement = context.ReadValue<Vector2>();
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
        if (getSpeed > 1.5 && Physics.Raycast(transform.position, transform.forward, out crash, 0.6f))
        {
            // player flys off, controls disabled, respawn imminent 
            ragdoll.Die();
        }

        if (respawning)
        {
            Respawn();
        }
        // get normalized speed and if crash and average speed is > just a bump allow for a 'crash'
       


    }

    void Respawn()
    {
        respawnDelay += Time.deltaTime;
        if (respawnDelay > respawnMax)
        {
            transform.position = respawnPoint.transform.position;
            transform.rotation = respawnPoint.transform.rotation;

            ragdoll.ToggleRagdoll(false);
            character.transform.parent = transform;
            
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
                deaccelerationRate = 1.25f;
                rb.velocity = rb.velocity / 1.075f;
                frontLeftForwardStiffness.stiffness = 0f;
                frontRightForwardStiffness.stiffness = 0f;
                backLeftForwardStiffness.stiffness = 0f;
                backRightForwardStiffness.stiffness = 0f;

                if (rb.velocity.magnitude > 3f)
                {
                    ragdoll.Die();
                }
            }
            else if (surfaceCheck.transform.tag == "Layer/Concrete")
            {
                deaccelerationRate = 0.25f;
                rb.velocity = rb.velocity / 1f;
                frontLeftForwardStiffness.stiffness = 1f;
                frontRightForwardStiffness.stiffness = 1f;
                backLeftForwardStiffness.stiffness = 2f;
                backRightForwardStiffness.stiffness = 2f;
            }
            else if (surfaceCheck.transform.tag == "Layer/Dirt")
            {
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
        if (breaking)
        {
            rb.velocity = rb.velocity / 1.01f;
            currentAcceleration = 0;
        }
    }

    void FallOver()
    {
        if (transform.rotation.x > 0.30 || transform.rotation.z > 0.30 || transform.rotation.x < -0.30 || transform.rotation.z < -0.30)
        {
            ragdoll.Die();
        }
    }

    void Ollie()
    {
        Debug.Log(riding);
        controls.Player.Ollie.performed += context => olliePressed = true;

        if (olliePressed && ollie)
        {
            ollieDelay += Time.deltaTime;
            if (ollieDelay > OllieDelayMax)
            {
                rb.AddForce(transform.up * 5000, ForceMode.Impulse);
                olliePressed = false;
                ollieDelay = 0f;
            }

        }

    }

    void CameraSwitch()
    {
        if (controls.Player.Switch.triggered && MainCamOn)
        {
            playerCam.gameObject.SetActive(false);
            freeCam.gameObject.SetActive(true);
            MainCamOn = false;
        }
        else if (controls.Player.Switch.triggered && !MainCamOn)
        {
            playerCam.gameObject.SetActive(true);
            freeCam.gameObject.SetActive(false);
            surfaceBeingChanged = false;
            MainCamOn = true;
        }
    }

    void CameraControls()
    {
        controls.Player.Turning.performed += context => camMovement = context.ReadValue<Vector2>();
        controls.Player.Look.performed += context => camLook = context.ReadValue<Vector2>();

        Vector3 cameraDirection = (camMovement.y * transform.forward) + (camMovement.x * transform.right);
        //freeCam.transform.position += cameraDirection * 10 * Time.deltaTime;

        freeCam.transform.position += freeCam.transform.forward * freeCamTurnSpeed * camMovement.y * Time.deltaTime;
        freeCam.transform.position += freeCam.transform.right * freeCamTurnSpeed * camMovement.x * Time.deltaTime;

        freeCam.transform.rotation *= Quaternion.Euler(camLook.y * Time.deltaTime * -100, camLook.x * Time.deltaTime * 100, -freeCam.transform.eulerAngles.z);

        if (Physics.Raycast(freeCam.transform.position, freeCam.transform.forward, out objectCheck, Mathf.Infinity) && !surfaceBeingChanged)
        {
            if (objectCheck.transform.tag == "Layer/Concrete" || objectCheck.transform.tag == "Layer/Grass" || objectCheck.transform.tag == "Layer/Dirt")
            {
                if (controls.Player.Push.triggered)
                {
                    surfaceBeingChanged = true;
                }
            }
        }
        if (surfaceBeingChanged)
        {
            ChangeSurface();
        }
    }

    void ChangeSurface()
    {
        ui.SetActive(true);
        GameObject text = GameObject.FindGameObjectWithTag("Surface/Text");

        if (surfaceIndex == 0)
        {
            text.GetComponent<Text>().text = "Concrete";
            if (controls.Player.Push.triggered)
            {
                objectCheck.transform.tag = "Layer/Concrete";
                objectCheck.transform.GetComponent<SurfaceChange>().state = SurfaceChange.enumState.Concrete;
            }

        }

        if (surfaceIndex == 1)
        {
            text.GetComponent<Text>().text = "Dirt";
            if (controls.Player.Push.triggered)
            {
                objectCheck.transform.tag = "Layer/Dirt";
                objectCheck.transform.GetComponent<SurfaceChange>().state = SurfaceChange.enumState.Dirt;
            }
        }

        if (surfaceIndex == 2)
        {
            text.GetComponent<Text>().text = "Grass";
            if (controls.Player.Push.triggered)
            {
                objectCheck.transform.tag = "Layer/Grass";
                objectCheck.transform.GetComponent<SurfaceChange>().state = SurfaceChange.enumState.Grass;
            }
        }
        if (surfaceIndex > 2)
        {
            surfaceIndex = 0;
        }
        if (surfaceIndex < 0)
        {
            surfaceIndex = 2;
        }

        if (MainCamOn)
        {
            ui.SetActive(false);
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
