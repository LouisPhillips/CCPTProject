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
    private Vector2 manualRotation;
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
    public float breakingForce = 300f;
    public float maxTurnAngle = 12.5f;
    public float maxAcceleration = 225f;

    public float deaccelerationRate = 0.1f;

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
    private float OllieDelayMax = 0.55f;
    public static bool riding = true;
    public static bool ollie = true;
    public float ollieHeight = 4f;
    private bool grounded;
    public LayerMask ground;

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
    public static bool mainCamOn = true;

    private float weight = 0;

    public Vector3 centreOfMass;

    private bool controllerDeactivated = false;

    public float manualTilt = -0.35f;

    private float[] wheelSpeedChanges = {1.075f, 1f, 1.02f};

    public Dropdown dropdown;
    void Awake()
    {
        controls = new PlayerController();

        controls.Player.Break.performed += context => breaking = true;
        controls.Player.Break.canceled += context => breaking = false;

        controls.Player.GoLeft.performed += context => surfaceIndex -= 1;
        controls.Player.GoRight.performed += context => surfaceIndex += 1;

        controls.Player.Pause.performed += context => PauseMenu.pausePressed = true;

        respawnPoint = GameObject.FindGameObjectWithTag("Respawn");

        rb = GetComponent<Rigidbody>();
        ragdoll = GetComponentInChildren<Ragdoll>();

        //playerCam = GetComponentInChildren<CinemachineVirtualCamera>();

        freeCam.gameObject.SetActive(false);

        ui = GameObject.FindGameObjectWithTag("Surface/Changer");
        ui.SetActive(false);

        character = GameObject.FindGameObjectWithTag("Player");
    }

    void FixedUpdate()
    {
        if (mainCamOn)
        {
            Crash();
            FallOver();
            if (!controllerDeactivated)
            {
                Movement();
                Turning();
                SurfaceDetection();
                Breaking();
                Ollie();
                Manual();
            }


            ui.SetActive(false);
        }
    }
    private void Update()
    {
        if (!mainCamOn)
        {
            CameraControls();
        }
        CameraSwitch();
        Debug.DrawRay(new Vector3(transform.position.x, transform.position.y + 0.05f, transform.position.z), transform.forward, Color.cyan, 0.63f);
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

        frontLeft.steerAngle = currentTurnAngle;
        frontRight.steerAngle = currentTurnAngle;
        backLeft.steerAngle = -currentTurnAngle;
        backRight.steerAngle = -currentTurnAngle;

        WheelFrictionCurve frontLeftSidewaysStiffness = frontLeft.sidewaysFriction;
        WheelFrictionCurve frontRightSidewaysStiffness = frontRight.sidewaysFriction;
        WheelFrictionCurve backLeftSidewaysStiffness = backLeft.sidewaysFriction;
        WheelFrictionCurve backRightSidewaysStiffness = backRight.sidewaysFriction;

        frontLeft.sidewaysFriction = frontLeftSidewaysStiffness;
        frontRight.sidewaysFriction = frontRightSidewaysStiffness;
        backLeft.sidewaysFriction = backLeftSidewaysStiffness;
        backRight.sidewaysFriction = backRightSidewaysStiffness;
    }

    void Manual()
    {
        controls.Player.Manual.performed += context => manualRotation = context.ReadValue<Vector2>();

        rb.centerOfMass = centreOfMass;

        centreOfMass = new Vector3(centreOfMass.x, centreOfMass.y, manualRotation.y);

        // lock Y and Z or restrict them to very little in order to allow manual, then when not manualling unlock them

        // turn slip values to none so it goes straight? Only when speed is above moving value, so when idle or very slow you can pivot around centre
        if (getSpeed >= 0.5f)
        {
            if (centreOfMass.z <= -0.2f || centreOfMass.z >= 0.2f)
            {
                rb.constraints = RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
            }
            else if (centreOfMass.z > -0.2f || centreOfMass.z < 0.2f)
            {
                rb.constraints = RigidbodyConstraints.None;
            }
        }

        if (centreOfMass.z < -0.34)
        {
            centreOfMass = new Vector3(centreOfMass.x, centreOfMass.y, -0.34f);
            if (getSpeed < 0.5f)
            {
                transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0f, movement.x * 90 * Time.deltaTime, 0f));
            }
        }
        if (centreOfMass.z > 0.34f)
        {
            centreOfMass = new Vector3(centreOfMass.x, centreOfMass.y, 0.34f);
            if (getSpeed < 0.5f)
            {
                transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0f, movement.x * 90 * Time.deltaTime, 0f));
            }
        }
    }
    void Crash()
    {
        getSpeed = rb.velocity.magnitude;
        if (getSpeed > 1.5 && /*Physics.BoxCast(new Vector3(transform.position.x, transform.position.y + 0.15f, transform.position.z) + transform.forward, transform.localScale / 20, transform.forward, out crash, transform.rotation, 0.8f)*/Physics.Raycast(new Vector3(transform.position.x, transform.position.y + 0.15f, transform.position.z), transform.forward, out crash, 0.7f))
        {
            // player flys off, controls disabled, respawn imminent 
            currentAcceleration = 0f;
            rb.velocity = new Vector3(0, 0, 0);
            controllerDeactivated = true;
            Debug.Log("Crashed");
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
            Debug.Log("respawning  :  " + respawnDelay);
            transform.position = respawnPoint.transform.position;
            transform.rotation = respawnPoint.transform.rotation;

            ragdoll.ToggleRagdoll(false);
            currentAcceleration = 0f;
            character.transform.parent = transform;
            character.transform.position = respawnPoint.transform.position;
            character.transform.rotation = respawnPoint.transform.rotation;

            respawning = false;
            respawnDelay = 0f;

            controllerDeactivated = false;
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
                deaccelerationRate = 5f;
                rb.velocity = rb.velocity / wheelSpeedChanges[0];
                frontLeftForwardStiffness.stiffness = 1f;
                frontRightForwardStiffness.stiffness = 1f;
                backLeftForwardStiffness.stiffness = 1f;
                backRightForwardStiffness.stiffness = 1f;
            }
            else if (surfaceCheck.transform.tag == "Layer/Concrete")
            {
                deaccelerationRate = 0.25f;
                rb.velocity = rb.velocity / wheelSpeedChanges[1];
                frontLeftForwardStiffness.stiffness = 1f;
                frontRightForwardStiffness.stiffness = 1f;
                backLeftForwardStiffness.stiffness = 2f;
                backRightForwardStiffness.stiffness = 2f;
            }
            else if (surfaceCheck.transform.tag == "Layer/Dirt")
            {
                deaccelerationRate = 0.5f;
                rb.velocity = rb.velocity / wheelSpeedChanges[2];
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
            Debug.Log("Fell");
            currentAcceleration = 0f;
            controllerDeactivated = true;
            ragdoll.Die();
        }
    }

    void Ollie()
    {

        if (ollieDelay <= 0f)
        {
            controls.Player.Ollie.performed += context => olliePressed = true;
        }

        if (olliePressed && ollie)
        {
            ollieDelay += Time.deltaTime;
            if (ollieDelay > OllieDelayMax)
            {
                rb.AddForce(transform.up * 5000 * ollieHeight, ForceMode.Impulse);
                olliePressed = false;
                ollieDelay = 0f;
            }

        }

    }

    void CameraSwitch()
    {
        if (!PauseMenu.pausePressed)
        {
            if (controls.Player.Switch.triggered && mainCamOn)
            {
                playerCam.gameObject.SetActive(false);
                freeCam.gameObject.SetActive(true);
                mainCamOn = false;
            }
            else if (controls.Player.Switch.triggered && !mainCamOn)
            {
                playerCam.gameObject.SetActive(true);
                freeCam.gameObject.SetActive(false);
                surfaceBeingChanged = false;
                mainCamOn = true;
            }
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

        if (mainCamOn)
        {
            ui.SetActive(false);
        }

    }

    public void AdjustWeight(float addedWeight)
    {
        rb.drag = addedWeight;
    }

    public void AdjustTrucks(float truckLooseness)
    {
        maxTurnAngle = truckLooseness;
    }

    public void ChangeWheelType()
    {
        if (dropdown.value == 0)
        {
            wheelSpeedChanges[0] = 1.075f;
            wheelSpeedChanges[1] = 1f;
            wheelSpeedChanges[2] = 1.02f;

            ollieHeight = 4f;
        }
        else
        {
            wheelSpeedChanges[0] = 1.035f;
            wheelSpeedChanges[1] = 1f;
            wheelSpeedChanges[2] = 1.01f;

            ollieHeight = 2.5f;
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

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position + transform.rotation * centreOfMass, 0.05f);
        Gizmos.DrawWireCube(new Vector3(transform.position.x, transform.position.y + 0.15f, transform.position.z) + transform.forward * 0.8f, transform.localScale / 20);
    }
}
