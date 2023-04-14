using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class PlayerMovement4 : MonoBehaviour
{
    [Header("Components")]
    private PlayerController controls;
    private Rigidbody rb;
    private GameObject character;
    public GameObject characterModel;
    private Ragdoll ragdoll;
    public GameObject sphere;

    private Vector2 movement;
    private Vector2 manualRotation;
    private Vector2 camMovement;
    private Vector2 camLook;

    [Header("Wheels")]
    public WheelCollider frontRight;
    public WheelCollider frontLeft;
    public WheelCollider backRight;
    public WheelCollider backLeft;

    [Header("Speed Attributes")]
    public float pushForce = 75f;
    public float breakingForce = 300f;
    public float maxAcceleration = 225f;
    public float deaccelerationRate = 0.1f;
    public float getSpeed;

    [HideInInspector] public bool push;
    private float pushDelay = 0f;
    private float pushMax = 0.5f;
    private bool breaking;
    private float currentAcceleration = 0f;
    private float currentBreakforce = 0f;

    [Header("Turn Attributes")]
    [SerializeField] private float turnValue;
    [SerializeField] private float maxTurnAngle = 12.5f;
    [SerializeField] private float currentTurnAngle = 0f;
    [SerializeField] private float steeringWeight = 2f;

    [Header("Ollie")]
    public bool olliePressed;
    public static bool riding = true;
    public static bool ollie = true;
    public float ollieHeight = 4f;
    public LayerMask ground;

    public float liftOffRotation;

    private bool canOllie = true;
    private float ollieDelay = 0f;
    private float OllieDelayMax = 0.55f;
    private bool inAir = false;

    [Header("Camera")]
    public CinemachineVirtualCamera playerCam;
    public CinemachineVirtualCamera freeCam;
    public CinemachineVirtualCamera ollieCam;
    public float freeCamTurnSpeed = 15f;
    public static bool surfaceBeingChanged = false;
    public int surfaceIndex = 0;
    public static bool mainCamOn = true;
    public float lastGroundedY;

    private RaycastHit objectCheck;
    private GameObject ui;

    [Header("Respawn")]
    private GameObject respawnPoint;
    private float respawnDelay = 0f;
    private float respawnMax = 5f;
    [HideInInspector] public bool respawning = false;
    private RaycastHit crash;
    private bool controllerDeactivated = false;

    [Header("Manual")]
    public Vector3 centreOfMass;
    public float manualTilt = -0.35f;

    [Header("Surface Changes")]
    private float[] wheelSpeedChanges = { 1.075f, 1f, 1.02f };
    private float customDeaccelerationWeight = 0f;
    private GameObject slider;
    private EventSystem eventSystem;

    [Header("Settings")]
    public Dropdown dropdown;

    [Header("Gravity")]
    [SerializeField] private float gravity = 9.81f;
    [SerializeField] private float gravityFallCurrent = -100f;
    [SerializeField] private float gravityFallMin = -100f;
    [SerializeField] private float gravityFallMax = -500f;
    [SerializeField] [Range(-5f, -35f)] float gravityIncrement = -20f;
    [SerializeField] private float gravityIncrementTime = 0.05f;
    [SerializeField] private float fallTimer = 0f;

    [SerializeField] private bool grounded = true;
    [Range(0f, 1.8f)] float radius = 0.9f;
    [Range(-0.95f, 1.05f)] float distance = 0.05f;
    private RaycastHit groundHit = new RaycastHit();

    private void Awake()
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
        character = GameObject.FindGameObjectWithTag("Player");

        freeCam.gameObject.SetActive(false);

        slider = GameObject.FindGameObjectWithTag("Surface/Slider");
        slider.SetActive(false);
        ui = GameObject.FindGameObjectWithTag("Surface/Changer");
        ui.SetActive(false);

        eventSystem = GameObject.FindGameObjectWithTag("Events").GetComponent<EventSystem>();
    }

    private void FixedUpdate()
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

                /*grounded = GroundCheck();
                movement.y = Gravity();
                movement = ApplyMass();*/
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
        RespawnButtonPressed();
        CameraSwitch();
        Debug.DrawRay(new Vector3(transform.position.x, transform.position.y + 0.05f, transform.position.z), transform.forward, Color.cyan, 0.63f);
    }

    private void Movement()
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
        /*else
        {
            Debug.Log("STOP");
            if (getSpeed < 0.1f)
            {
                currentAcceleration = 0;
            }
        }*/



        if (controls.Player.Break.triggered)
        {
            currentBreakforce = breakingForce;
        }
        else
        {
            currentBreakforce = 0f;
        }

        if (!breaking)
        {
            frontLeft.motorTorque = currentAcceleration;
            frontRight.motorTorque = currentAcceleration;
            backLeft.motorTorque = currentAcceleration;
            backRight.motorTorque = currentAcceleration;

            frontLeft.brakeTorque = currentBreakforce;
            frontRight.brakeTorque = currentBreakforce;
            backLeft.brakeTorque = currentBreakforce;
            backRight.brakeTorque = currentBreakforce;
        }
    }

    /*private bool GroundCheck()
    {
        return Physics.Raycast(rb.position, Vector3.down, 0.5f, ground);
    }

    private float Gravity()
    {
        if (grounded)
        {
            gravity = 0f;
            gravityFallCurrent = gravityFallMin;
        }
        else
        {
            fallTimer -= Time.fixedDeltaTime;
            if(fallTimer < 0)
            {
                if (gravityFallCurrent >= gravityFallMax)
                {
                    gravityFallCurrent -= gravityIncrement;
                }
                fallTimer = gravityIncrementTime;
                gravity = gravityFallCurrent;
            }
        }
        return gravity;
    }

    private Vector2 ApplyMass()
    {
        Vector2 playerMovement = (new Vector2(movement.x * 30f * rb.mass, movement.y * rb.mass));
        return playerMovement;
    }*/
    private void Turning()
    {
        controls.Player.Turning.performed += context => movement = context.ReadValue<Vector2>();
        rb.AddTorque(-rb.angularVelocity * 2);
        rb.angularDrag = 1 + (getSpeed / 2);

        if (movement.x >= 0)
        {
            turnValue += 0.05f;
            if (turnValue > movement.x)
            {
                turnValue = movement.x;
            }
        }
        else
        {
            turnValue -= 0.05f;
            if (turnValue < movement.x)
            {
                turnValue = movement.x;
            }
        }

        if (movement.x == 0)
        {
            // needs to lerp back to 0
            turnValue = Mathf.Lerp(turnValue, 0, 0.00005f);
        }

        // \/\/\/\/ this causing steering snap
        // good value is 0.005f
        if (!breaking)
        {
            maxTurnAngle = Mathf.Lerp(25f, 15f, currentAcceleration * 0.005f);
            currentTurnAngle = maxTurnAngle * turnValue;
        }


        frontLeft.steerAngle = currentTurnAngle;
        frontRight.steerAngle = currentTurnAngle;
        backLeft.steerAngle = -currentTurnAngle;
        backRight.steerAngle = -currentTurnAngle;

        WheelFrictionCurve frontLeftSidewaysStiffness = frontLeft.sidewaysFriction;
        WheelFrictionCurve frontRightSidewaysStiffness = frontRight.sidewaysFriction;
        WheelFrictionCurve backLeftSidewaysStiffness = backLeft.sidewaysFriction;
        WheelFrictionCurve backRightSidewaysStiffness = backRight.sidewaysFriction;

        //reverse this
        frontLeftSidewaysStiffness.stiffness = Mathf.Lerp(5, 1, getSpeed / 3);
        frontRightSidewaysStiffness.stiffness = Mathf.Lerp(5, 1, getSpeed / 3);

        frontLeft.sidewaysFriction = frontLeftSidewaysStiffness;
        frontRight.sidewaysFriction = frontRightSidewaysStiffness;
        backLeft.sidewaysFriction = backLeftSidewaysStiffness;
        backRight.sidewaysFriction = backRightSidewaysStiffness;


    }

    private void Manual()
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
    private void Crash()
    {
        getSpeed = rb.velocity.magnitude;
        //bool crashed = Physics.BoxCast(new Vector3(transform.position.x, transform.position.y + 0.15f, transform.position.z) + transform.forward * 0.7f, (new Vector3(transform.position.x, transform.position.y + 0.15f, transform.position.z) + transform.forward * 0.7f) / 2, new Vector3(0.3f, 0.2f, 0.05f));
        if (getSpeed > 1.5 && Physics.Raycast(new Vector3(transform.position.x, transform.position.y + 0.15f, transform.position.z), transform.forward, out crash, 0.7f))
        {
            // player flys off, controls disabled, respawn imminent 
            currentAcceleration = 0f;
            rb.velocity = new Vector3(0, 0, 0);
            controllerDeactivated = true;
            ragdoll.Fall();
        }

        if (respawning)
        {
            Respawn();
        }
        // get normalized speed and if crash and average speed is > just a bump allow for a 'crash'
    }

    private void Respawn()
    {
        respawnDelay += Time.deltaTime;
        if (respawnDelay >= respawnMax)
        {
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

    private void SurfaceDetection()
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
            else if (surfaceCheck.transform.tag == "Layer/Custom")
            {
                deaccelerationRate = customDeaccelerationWeight;
                rb.velocity = rb.velocity / (1 + (customDeaccelerationWeight / 100));
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

    private void Breaking()
    {
        if (breaking)
        {
            rb.velocity = rb.velocity / 1.01f;
            currentAcceleration = 0;
        }

        if (breaking && getSpeed < 0.25f)
        {
            currentAcceleration = 100;
            frontLeft.motorTorque = -currentAcceleration;
            frontRight.motorTorque = -currentAcceleration;
            backLeft.motorTorque = -currentAcceleration;
            backRight.motorTorque = -currentAcceleration;
        }
    }

    private void FallOver()
    {
        if (transform.rotation.x > 0.30 || transform.rotation.z > 0.30 || transform.rotation.x < -0.30 || transform.rotation.z < -0.30)
        {
            currentAcceleration = 0f;
            controllerDeactivated = true;
            ragdoll.Fall();
        }
    }

    private void Ollie()
    {

        if (ollieDelay <= 0f)
        {
            controls.Player.Ollie.performed += context => olliePressed = true;
        }

        grounded = Physics.Raycast(transform.position, Vector3.down, 0.2f, ground);

        if (olliePressed && grounded && canOllie)
        {
            liftOffRotation = transform.eulerAngles.y;
            ollieDelay += Time.deltaTime;
            if (ollieDelay > OllieDelayMax)
            {
                rb.AddForce(transform.up * 5000 * ollieHeight, ForceMode.Impulse);
                olliePressed = false;
                ollieDelay = 0f;
                inAir = true;
                Invoke(nameof(ResetOllie), 0.5f);
            }
        }
        if (grounded)
        {
            lastGroundedY = transform.eulerAngles.y;
            ollieCam.gameObject.SetActive(false);
            playerCam.gameObject.SetActive(true);
            //playerCam.m_LookAt = characterModel.transform;
            //playerCam.m_Follow = characterModel.transform;
            //playerCam.GetCinemachineComponent<CinemachineTransposer>().m_BindingMode = CinemachineTransposer.BindingMode.LockToTargetWithWorldUp;
            if ((transform.eulerAngles.y > liftOffRotation + 140 && transform.eulerAngles.y < liftOffRotation + 220 || transform.eulerAngles.y < liftOffRotation + -140 && transform.eulerAngles.y > liftOffRotation + -220) && inAir)
            {
                Debug.Log("Flips direction");
                transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, liftOffRotation * 2, transform.rotation.eulerAngles.z);
                liftOffRotation = 10000;
                inAir = false;
            }
        }
        else
        {

            //playerCam.m_LookAt = null;
            //playerCam.m_Follow = null;
            //playerCam.GetCinemachineComponent<CinemachineTransposer>().m_BindingMode = CinemachineTransposer.BindingMode.SimpleFollowWithWorldUp;

            RaycastHit hit = new RaycastHit();
            if (Physics.Raycast(transform.position, Vector3.down, out hit))
            {
                var distanceToGround = hit.distance;
                if (distanceToGround < 1)
                {
                    ollieCam.gameObject.SetActive(true);
                    playerCam.gameObject.SetActive(false);
                    transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0f, movement.x * 300 * Time.deltaTime, 0f));
                }

            }
        }
    }

    private void ResetOllie()
    {
        canOllie = true;
    }

    private void CameraSwitch()
    {
        if (!PauseMenu.pausePressed && grounded)
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

    private void CameraControls()
    {
        controls.Player.Turning.performed += context => camMovement = context.ReadValue<Vector2>();
        controls.Player.Look.performed += context => camLook = context.ReadValue<Vector2>();


        eventSystem.firstSelectedGameObject = GameObject.FindGameObjectWithTag("Surface/Button");


        Vector3 cameraDirection = (camMovement.y * transform.forward) + (camMovement.x * transform.right);
        //freeCam.transform.position += cameraDirection * 10 * Time.deltaTime;

        freeCam.transform.position += freeCam.transform.forward * freeCamTurnSpeed * camMovement.y * Time.deltaTime;
        freeCam.transform.position += freeCam.transform.right * freeCamTurnSpeed * camMovement.x * Time.deltaTime;

        freeCam.transform.eulerAngles += new Vector3(camLook.y * Time.deltaTime * -100, camLook.x * Time.deltaTime * 100, -freeCam.transform.eulerAngles.z);
        /*if (freeCam.transform.eulerAngles.x < -79)
        {
            freeCam.transform.eulerAngles = new Vector3(-79, freeCam.transform.eulerAngles.y, freeCam.transform.eulerAngles.z);
        }*/
        /*if (freeCam.transform.eulerAngles.x < -89)
        {
            freeCam.transform.eulerAngles = new Vector3(-89, freeCam.transform.eulerAngles.y, freeCam.transform.eulerAngles.z);
        }*/
        //camLook.y = Mathf.Clamp(camLook.y, -70f, 70f);

        if (Physics.Raycast(freeCam.transform.position, freeCam.transform.forward, out objectCheck, Mathf.Infinity) && !surfaceBeingChanged)
        {
            if (objectCheck.transform.tag == "Layer/Concrete" || objectCheck.transform.tag == "Layer/Grass" || objectCheck.transform.tag == "Layer/Dirt" || objectCheck.transform.tag == "Layer/Custom")
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

    private void ChangeSurface()
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

        if (surfaceIndex == 3)
        {
            text.GetComponent<Text>().text = "Custom";
            slider.SetActive(true);
            if (controls.Player.Push.triggered)
            {
                objectCheck.transform.tag = "Layer/Custom";
                objectCheck.transform.GetComponent<SurfaceChange>().state = SurfaceChange.enumState.Custom;
            }
        }
        else
        {
            slider.SetActive(false);
        }
        if (surfaceIndex > 3)
        {
            surfaceIndex = 0;
        }
        if (surfaceIndex < 0)
        {
            surfaceIndex = 3;
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

    public void CustomFriction(float friction)
    {
        customDeaccelerationWeight = friction;
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
        Gizmos.DrawWireCube(new Vector3(transform.position.x, transform.position.y + 0.15f, transform.position.z) + transform.forward * 0.7f, new Vector3(0.3f, 0.2f, 0.05f));
    }

    private void RespawnButtonPressed()
    {
        if (controls.Player.Respawn.triggered)
        {
            respawning = true;
            respawnDelay = respawnMax;
        }
    }
}
