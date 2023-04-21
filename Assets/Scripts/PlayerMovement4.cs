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
    public Rigidbody rb;
    private GameObject character;
    public GameObject characterModel;
    private Ragdoll ragdoll;
    public GameObject sphere;
    public Transform skatedeck;

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
    private bool pushed;
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
    public Vector3 lastForwardValue;
    public float liftOffRotation;
    public float previousLiftOffRotation;

    private bool canOllie = true;
    private float ollieDelay = 0f;
    private float OllieDelayMax = 0.55f;
    private bool inAir = false;
    private float dampYTimeMax = 1f;
    private float dampYTime = 0f;
    private bool flipped = true;
    private float liftDelay = 0f;
    private float liftTick = 1f;

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

        previousLiftOffRotation = liftOffRotation;
    }

    private void FixedUpdate()
    {

        if (mainCamOn)
        {
            Crash();
            FallOver();
            if (!controllerDeactivated)
            {
                Ollie();
                Movement();
                Turning();
                SurfaceDetection();
                Breaking();
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
        RespawnButtonPressed();
        CameraSwitch();
        Debug.DrawRay(new Vector3(transform.position.x, transform.position.y + 0.05f, transform.position.z), transform.forward, Color.cyan, 0.63f);
    }

    private void Movement()
    {
        // if the player is grounded, they can push
        if (grounded && !inAir && !olliePressed)
        {
            controls.Player.Push.performed += context => push = true;
        }
        else
        {
            controls.Player.Push.performed += context => push = false;
        }

        // deaccelerate the player over time
        if (currentAcceleration > 0)
        {
            currentAcceleration -= deaccelerationRate;
        }

        if (currentAcceleration > maxAcceleration)
        {
            currentAcceleration = maxAcceleration;
        }


        // push delay
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


        // set the breakforce based on if the break is being pressed
        if (controls.Player.Break.triggered)
        {
            currentBreakforce = breakingForce;
        }
        else
        {
            currentBreakforce = 0f;
        }

        // sets wheel and break force to the corresponsing values
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

    private void Turning()
    {
        controls.Player.Turning.performed += context => movement = context.ReadValue<Vector2>();

        rb.AddTorque(-rb.angularVelocity * 2);
        rb.angularDrag = 1 + (getSpeed / 2);

        // lerp x values to the point of the vector2.x
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

        // sets steering back to 0 gradually
        if (movement.x == 0)
        {
            turnValue = Mathf.Lerp(turnValue, 0, 0.00005f);
        }

        // sets the turning radius of the skateboard based on speed
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

        frontLeftSidewaysStiffness.stiffness = Mathf.Lerp(5, 1, getSpeed / 3);
        frontRightSidewaysStiffness.stiffness = Mathf.Lerp(5, 1, getSpeed / 3);

        frontLeft.sidewaysFriction = frontLeftSidewaysStiffness;
        frontRight.sidewaysFriction = frontRightSidewaysStiffness;
        backLeft.sidewaysFriction = backLeftSidewaysStiffness;
        backRight.sidewaysFriction = backRightSidewaysStiffness;

        // rotates skatedeck based on where the user is turning to visualize turning
        skatedeck.localEulerAngles = new Vector3(skatedeck.localEulerAngles.x, skatedeck.localEulerAngles.y, -movement.x * 10);
    }

    private void Manual()
    {
        controls.Player.Manual.performed += context => manualRotation = context.ReadValue<Vector2>();

        rb.centerOfMass = centreOfMass;

        centreOfMass = new Vector3(centreOfMass.x, centreOfMass.y, manualRotation.y);

        // freezing the player while manualing
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

        // if the player manuals backwards
        if (centreOfMass.z < -0.34)
        {
            centreOfMass = new Vector3(centreOfMass.x, centreOfMass.y, -0.34f);
            if (getSpeed < 0.5f)
            {
                transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0f, movement.x * 90 * Time.deltaTime, 0f));
            }
        }
        // if the player manuals forward
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

        // only checks crash if over a certain speed, as going any slower would not be fast enough to trigger a crash
        if (getSpeed > 1.5 && Physics.Raycast(new Vector3(transform.position.x, transform.position.y + 0.15f, transform.position.z), transform.forward, out crash, 0.7f))
        {
            // player flys off, controls disabled, respawn imminent 
            currentAcceleration = 0f;
            rb.velocity = new Vector3(0, 0, 0);
            controllerDeactivated = true;
            ragdoll.Fall();
        }
        // respawn the player
        if (respawning)
        {
            Respawn();
        }
    }

    private void Respawn()
    {
        respawnDelay += Time.deltaTime;
        // sets all the players values back to normal to restart gameplay
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
        // setting up friction values for each surface
        WheelFrictionCurve frontLeftForwardStiffness = frontLeft.forwardFriction;
        WheelFrictionCurve frontRightForwardStiffness = frontRight.forwardFriction;
        WheelFrictionCurve backLeftForwardStiffness = backLeft.forwardFriction;
        WheelFrictionCurve backRightForwardStiffness = backRight.forwardFriction;

        RaycastHit surfaceCheck;

        Debug.DrawRay(transform.position, Vector3.down, Color.red, 1f);
        // check what material the player is on
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
        // if the player is moving and breaking
        if (breaking && getSpeed > 0.25f)
        {
            rb.velocity = rb.velocity / 1.01f;
            currentAcceleration = 0;
        }
        // if the player is practically stopped, breaking will now reverse instead
        else if (breaking && getSpeed < 0.25f)
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
        // if the skateboarder rotates past a tipping point they will fall off
        if (transform.rotation.x > 0.30 || transform.rotation.z > 0.30 || transform.rotation.x < -0.30 || transform.rotation.z < -0.30)
        {
            currentAcceleration = 0f;
            controllerDeactivated = true;
            ragdoll.Fall();
        }
    }

    private void Ollie()
    {
        // if can press ollie
        if (ollieDelay <= 0f)
        {
            controls.Player.Ollie.performed += context => olliePressed = true;
        }

        grounded = Physics.Raycast(transform.position, Vector3.down, 0.2f, ground);

        // ollie
        if (olliePressed && grounded && canOllie && !push)
        {
            liftOffRotation = transform.eulerAngles.y;
            previousLiftOffRotation = liftOffRotation;
            lastForwardValue = Vector3.forward;
            ollieDelay += Time.deltaTime;
            // if the ollie animation has reached where the ollie should happen
            if (ollieDelay > OllieDelayMax)
            {
                // add force upwards to the skateboard
                rb.AddForce(transform.up * 5000 * ollieHeight, ForceMode.Impulse);
                olliePressed = false;
                ollieDelay = 0f;
                inAir = true;
                flipped = false;
                Invoke(nameof(ResetOllie), 0.5f);
            }
        }
        if (grounded)
        {
            // reset follow camera values
            ollieCam.transform.parent = transform;
            ollieCam.transform.localPosition = new Vector3(-0.0218416024f, 0.556589425f, -0.461597949f);
            lastGroundedY = transform.eulerAngles.y;
            ollieCam.gameObject.SetActive(false);
            playerCam.gameObject.SetActive(true);
            playerCam.m_LookAt = characterModel.transform;
            playerCam.m_Follow = characterModel.transform;

            // resetting Y damping so that the camera switch back is smooth and not snappy
            if (playerCam.GetCinemachineComponent<CinemachineTransposer>().m_YDamping != 1.5f)
            {
                dampYTime += Time.deltaTime;
                if (dampYTime > dampYTimeMax)
                {
                    playerCam.GetCinemachineComponent<CinemachineTransposer>().m_YDamping = 1.5f;
                    dampYTime = 0;
                }
            }
            // if the player rotates their board in the air in order to ollie 180
            if (((transform.eulerAngles.y > liftOffRotation + 140 && transform.eulerAngles.y < liftOffRotation + 220) || (transform.eulerAngles.y < liftOffRotation + -140 && transform.eulerAngles.y > liftOffRotation + -220)) && inAir)
            {
                RaycastHit hit = new RaycastHit();
                if (Physics.Raycast(transform.position, Vector3.down, out hit))
                {
                    if (hit.transform.eulerAngles.x == 0 && currentAcceleration > 1)
                    {
                        // flips the player 180 degrees to return to normal direction
                        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, liftOffRotation, transform.rotation.eulerAngles.z);
                        liftOffRotation = 10000;
                        inAir = false;
                        flipped = true;
                    }
                    else
                    {
                        liftOffRotation = 10000;
                        flipped = true;
                        inAir = false;
                    }
                }
            }
            // if the user ollied and didn't rotate to activate a ollie 180
            liftDelay += Time.deltaTime;
            if (liftDelay > liftTick)
            {
                if (inAir && !flipped && liftOffRotation == previousLiftOffRotation)
                {
                    liftOffRotation = 10000;
                    flipped = true;
                    inAir = false;
                }
                liftDelay = 0;
            }
        }
        else
        {

            // stopping the follow camera from tracking the player
            if (inAir)
            {
                playerCam.m_LookAt = null;
                playerCam.m_Follow = null;
                playerCam.transform.position = ollieCam.transform.position;
            }


            RaycastHit hit = new RaycastHit();
            if (Physics.Raycast(transform.position, Vector3.down, out hit))
            {
                // check if the player is a considerable distance from the ground
                var distanceToGround = hit.distance;
                if (distanceToGround < 1 && inAir)
                {
                    // setting up ollie cam to follow the player without rotating the camera
                    ollieCam.transform.parent = null;
                    ollieCam.gameObject.SetActive(true);
                    playerCam.gameObject.SetActive(false);
                    transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0f, movement.x * 300 * Time.deltaTime, 0f));
                }
                
                // if above certain height Y damping on camera will stop
                if (distanceToGround < 1 && hit.transform.eulerAngles.x == 0)
                {
                    playerCam.GetCinemachineComponent<CinemachineTransposer>().m_YDamping = 0;
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
            // switch to the free camera
            if (controls.Player.Switch.triggered && mainCamOn)
            {
                playerCam.gameObject.SetActive(false);
                freeCam.gameObject.SetActive(true);
                surfaceBeingChanged = true;
                mainCamOn = false;
            }
            // switch to the player camera
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
