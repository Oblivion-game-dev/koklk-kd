using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Haptics;
using UnityEngine.Scripting;
using System.IO;
using System;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Runtime.InteropServices.ComTypes;
using Unity.Mathematics;
using System.ComponentModel;

using TMPro;

[Preserve]
public class Gamepad : InputDevice, IDualMotorRumble, IHaptics
{
    public void PauseHaptics()
    {
        throw new System.NotImplementedException();
    }

    public void ResetHaptics()
    {
        throw new System.NotImplementedException();
    }

    public void ResumeHaptics()
    {
        throw new System.NotImplementedException();
    }

    public void SetMotorSpeeds(float lowFrequency, float highFrequency)
    {
        throw new System.NotImplementedException();
    }
}

public class movement : MonoBehaviour
{
    // Initialize war crime
    public bool usenewsys;
    public float moveSpeed;
    public float currentSpeed;
    public float jumpHeight;
    public bool onSlideWallLeft;
    public bool onSlideWallRight;
    public bool onNormalWall;
    public float slowDownAmount;
    public bool jumpedOnWall;
    public bool bouncing;
    public float minVelocity;
    public float maxVelocity;
    public bool canJump;
    public float airFriction;
    public float friction;
    public bool spaceLock;
    public bool facingWall;
    public bool sliding;
    public float groundMoveSpeed;
    public Vector3 grndMove;
    public float onWallGravity;
    public Vector3 lastPos;
    public bool hasGem;
    public int gemType;
    public float fov;
    public float fovMultiplier;
    public float fovInterpolation;
    public float fovCap;
    public float simSpeed;
    public GameObject cam;
    public float VRrotSpeed;
    public InputAction move;
    public bool VRpressingW;
    public bool VRpressingS;
    public bool VRpressingA;
    public bool VRpressingD;
    public bool VRpressingSpace;
    public float joystickSensitivity;
    public AudioClip crystalHit;
    public AudioClip normalHit;
    public float camTiltSin;
    private Vector2 m_PlayerMovement;
    private InputAction m_MoveAction;
    private InputAction m_AttackAction;
    private string directoryPath;
    private float sfxVol;
    public float camTiltTimer = 0;
    public bool increaseCamTiltTimer;
    public float initCamTiltSwingAmount;
    public float otherCamTiltSin;
    public Vector3 relativeVelocity;
    public Rigidbody rigidbodyf;
    public float lastReasonableCamTilt;
    public float lastReasonableOtherCamTilt;
    public bool unreasonableCamTilting;
    private bool connected = false;
    public float sfxvol;
    public bool inCloud;
    public float coyoteTime;
    public float inverseCoyoteTime;
    public float maxCoyoteTime;
    public bool coyoteTimeEnabled;
    public bool coyoteTapSpace;
    public int lastWall;
    public float coyoteTimeCooldown;
    public float lastYrot;
    public float lastrotdifferencevar;
    public float timescale;
    public bool pressingJumpAtBeginning;
    public int howlongnotjumppress;
    public GameObject timercanvas;
    private float prevturn;
    public float turnspeed;
    public float horizontalmovespeed;
    public float allmovespeed;
    public Vector3 oldpos;
    public GameObject extrasfx;
    public float maxTurn;
    public float turndivide;
    
    void Start()
    {

        lastWall = 0;
        rigidbody = GetComponent<Rigidbody>();
        Debug.Log(string.Join("\n", InputSystem.devices));
        m_MoveAction = new InputAction("Move");
        m_MoveAction.AddBinding("<Gamepad>/leftStick");
        m_MoveAction.Enable();
        m_AttackAction = new InputAction("Attack", binding: "<Gamepad>/buttonSouth");
        m_AttackAction.Enable();
        sliding = false;
        VRpressingA = false;
        VRpressingD = false;
        VRpressingS = false;
        VRpressingW = false;
        VRpressingSpace = false;
        GameObject.Find("Canvas").GetComponent<detectDeath>().player = gameObject;
        GameObject.Find("Canvas").GetComponent<detectDeath>().cam = transform.GetChild(0).gameObject;
        //simSpeed = cam.transform.GetComponent<ParticleSystem>().main.simulationSpeed;
        Debug.Log("hi");
        Debug.Log(Application.persistentDataPath);
        string directoryPath = Application.persistentDataPath;
        string filePath = Path.Combine(directoryPath, "sfxVol.txt");
        

        if (File.Exists(filePath))
        {
            Debug.Log($"The file exists at '{filePath}'.");
            string fileText = File.ReadAllText(filePath);
            GetComponent<AudioSource>().volume = float.Parse(fileText);
            sfxvol = float.Parse(fileText);
        }
        else
        {
            Debug.Log($"The file doesn't exist at '{filePath}'.");
        }
        currentSpeed = Mathf.Abs(GetComponent<Rigidbody>().velocity.x) + Mathf.Abs(GetComponent<Rigidbody>().velocity.y) + Mathf.Abs(GetComponent<Rigidbody>().velocity.z) / 3;
        simSpeed = currentSpeed / 10;
        //cam.transform.GetComponent<Camera>().fieldOfView = fov + currentSpeed / 2.5f;

        fovInterpolation = (fov + currentSpeed / fovMultiplier) + 10;
        lastYrot = transform.eulerAngles.y;
        var attacking = m_AttackAction.ReadValue<float>();

        pressingJumpAtBeginning = true;
        howlongnotjumppress = 0;

        if (GameObject.Find("OnlineManagerInit"))
        {
            //if (GameObject.Find("OnlineManagerInit").GetComponent<onlinePlay>().timeSinceSceneChange >= 1)
            //{
            //    GameObject.Find("OnlineManagerInit").SendMessage("sceneWasReloaded");
            //}
        }
        if (GameObject.Find("timerCanvas") != null)
        {
            timercanvas = GameObject.Find("timerCanvas");
        }

    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.R))
        {
            //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            LoadScene(SceneManager.GetActiveScene().name);

        }
        Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.down) * 1.2f, Color.yellow);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // this is why my game doesn't run well on mobile...
        if (Time.timeScale != timescale)
        {
            Time.timeScale = timescale;
        }
        relativeVelocity = transform.InverseTransformDirection(rigidbody.velocity);
        var VRinputDevices = new List<UnityEngine.XR.InputDevice>();
        UnityEngine.XR.InputDevices.GetDevices(VRinputDevices);
        VRpressingA = false;
        VRpressingD = false;
        VRpressingS = false;
        VRpressingW = false;
        VRpressingSpace = false;
        //this is completely unused and only makes the game slower
        foreach (var device in VRinputDevices)
        {

            Vector2 primary2DAxisValue;
            if ((device.characteristics & UnityEngine.XR.InputDeviceCharacteristics.Left) != 0)
            {
                if (device.TryGetFeatureValue(UnityEngine.XR.CommonUsages.primary2DAxis, out primary2DAxisValue))
                {
                    // Check if the y-coordinate of the thumbstick is greater than 0, indicating forward movement
                    if (primary2DAxisValue.y > joystickSensitivity)
                    {
                        Debug.Log("Thumbstick is pushed forward.");
                        VRpressingW = true;
                    }
                    if (primary2DAxisValue.y < joystickSensitivity * -1)
                    {
                        Debug.Log("Thumbstick is pushed backward.");
                        VRpressingS = true;
                    }
                    if (primary2DAxisValue.x < joystickSensitivity * -1)
                    {
                        Debug.Log("Thumbstick is pushed left.");
                        VRpressingA = true;
                    }
                    if (primary2DAxisValue.x > joystickSensitivity)
                    {
                        Debug.Log("Thumbstick is pushed Right. " + primary2DAxisValue.x);
                        VRpressingD = true;
                    }
                }


            }
            bool primaryButtonPressed;
            if (device.TryGetFeatureValue(UnityEngine.XR.CommonUsages.primaryButton, out primaryButtonPressed))
            {
                if ((device.characteristics & UnityEngine.XR.InputDeviceCharacteristics.Right) != 0)
                {
                    if (primaryButtonPressed)
                    {
                        VRpressingSpace = true;
                    }
                }

            }
        }
        if (m_MoveAction.ReadValue<Vector2>() != null)
        {
            m_PlayerMovement = m_MoveAction.ReadValue<Vector2>();
        }
        else
        {
            m_PlayerMovement = Vector2.zero;
        }

        //Debug.Log(Input.GetJoystickNames());

        if (m_PlayerMovement.y > 0.4f)
        {
            VRpressingW = true;
        }
        if (m_PlayerMovement.y < -0.4f)
        {
            VRpressingS = true;
        }
        if (m_PlayerMovement.x > 0.4f)
        {
            VRpressingD = true;
        }
        if (m_PlayerMovement.x < -0.4f)
        {
            VRpressingA = true;
        }

        var attacking = m_AttackAction.ReadValue<float>();
        if (Mathf.Approximately(attacking, 1f) && pressingJumpAtBeginning == false)
        {
            VRpressingSpace = true;
        }
        if (Mathf.Approximately(attacking, 0f) && howlongnotjumppress < 10)
        {
            howlongnotjumppress += 1;
        }
        if (Mathf.Approximately(attacking, 0f) && howlongnotjumppress >= 10)
        {
            pressingJumpAtBeginning = false;
        }


        currentSpeed = Mathf.Abs(GetComponent<Rigidbody>().velocity.x) + Mathf.Abs(GetComponent<Rigidbody>().velocity.y) + Mathf.Abs(GetComponent<Rigidbody>().velocity.z) / 3;
        simSpeed = currentSpeed / 10;
        //cam.transform.GetComponent<Camera>().fieldOfView = fov + currentSpeed / 2.5f;
        if ((fov + currentSpeed / fovMultiplier) > fovInterpolation + 2 && (fov + currentSpeed / fovMultiplier) <= fovCap)
        {
            //fovInterpolation += Time.deltaTime * (0.4f * (fov + currentSpeed / 2.5f));
            fovInterpolation += 0.1f * math.abs(fovInterpolation - (fov + currentSpeed / fovMultiplier));
        }
        if ((fov + currentSpeed / fovMultiplier) < fovInterpolation - 2)
        {
            //fovInterpolation -= Time.deltaTime * (0.4f * (fov + currentSpeed / 2.5f));
            fovInterpolation -= 0.1f * math.abs(fovInterpolation - (fov + currentSpeed / fovMultiplier));
        }
        cam.transform.GetComponent<Camera>().fieldOfView = fovInterpolation;

        RaycastHit hit;
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.left), out hit, 0.6f))
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.left) * hit.distance, Color.blue);
            //Debug.Log("Did Hit");
            if (hit.transform.gameObject.tag == "wall")
            {
                coyoteTimeEnabled = true;
                coyoteTime = 0;
                lastWall = 1;
                onNormalWall = false;
                if (onSlideWallLeft == false)
                {
                    Debug.Log("velocity on hit wall was " + relativeVelocity.x);
                    initCamTilt(relativeVelocity.x);
                    GetComponent<AudioSource>().Stop();
                    //if (hit.transform.GetComponent<hitsound>() != null)
                    //{
                    //
                    //    GetComponent<AudioSource>().clip = hit.transform.GetComponent<hitsound>().hitSFX;
                    //    if (hit.transform.GetComponent<hitsound>().randomPitch)
                    //    {
                    //        GetComponent<AudioSource>().pitch = UnityEngine.Random.Range(hit.transform.GetComponent<hitsound>().minPitch, hit.transform.GetComponent<hitsound>().maxPitch);
                    //    }

                    //}
                    //else
                    //{
                    //    GetComponent<AudioSource>().clip = normalHit;
                    //    GetComponent<AudioSource>().pitch = 1;
                    //}
                    //GetComponent<AudioSource>().volume = math.min(math.abs(relativeVelocity.x), sfxvol * 10) / 10;
                    //GetComponent<AudioSource>().Play();


                }
                onSlideWallLeft = true;
                onSlideWallRight = false;
            }
            else
            {
                onSlideWallLeft = false;
                onSlideWallRight = false;
                onNormalWall = true;
            }
        }
        else
        {
            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.right), out hit, 0.6f))
            {
                Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.right) * hit.distance, Color.blue);
                //Debug.Log("Did Hit");
                if (hit.transform.gameObject.tag == "wall")
                {
                    coyoteTimeEnabled = true;
                    coyoteTime = 0;
                    lastWall = 2;
                    if (onSlideWallRight == false)
                    {
                        Debug.Log("velocity on hit wall was " + relativeVelocity.x);
                        initCamTilt(relativeVelocity.x);
                        GetComponent<AudioSource>().Stop();
                        /**if (hit.transform.GetComponent<hitsound>() != null)
                        {

                            GetComponent<AudioSource>().clip = hit.transform.GetComponent<hitsound>().hitSFX;
                            if (hit.transform.GetComponent<hitsound>().randomPitch)
                            {
                                GetComponent<AudioSource>().pitch = UnityEngine.Random.Range(hit.transform.GetComponent<hitsound>().minPitch, hit.transform.GetComponent<hitsound>().maxPitch);
                            }

                        }
                        else
                        {
                            GetComponent<AudioSource>().clip = normalHit;
                            GetComponent<AudioSource>().pitch = 1;

                        }
                        GetComponent<AudioSource>().volume = math.min(math.abs(relativeVelocity.x), sfxvol * 10) / 10;
                        GetComponent<AudioSource>().Play();


                    }**/
                        onNormalWall = false;
                        onSlideWallRight = true;
                        onSlideWallLeft = false;
                    }
                    else
                    {
                        onNormalWall = true;
                        onSlideWallRight = false;
                        onSlideWallLeft = false;
                    }
                }
                else
                {

                    onSlideWallLeft = false;
                    onSlideWallRight = false;
                    onNormalWall = false;


                }
            }
            RaycastHit hit4;
            // Does the ray intersect any objects excluding the player layer
            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hit4, 1.8f))
            {
                Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.down) * hit4.distance, Color.red);
                //Debug.Log("Did Hit");
                //canJump = true;
                if (hit4.transform.gameObject.tag == "die")
                {
                    GameObject.Find("Canvas").GetComponent<detectDeath>().dead = true;
                }
                /**if (hit4.transform.gameObject.tag == "bounce" && rigidbody.velocity.y < -1)
                {
                    Debug.Log("bouncing");
                    if (hit4.transform.GetComponent<bounceOptions>() != null)
                    {
                        rigidbody.velocity = new Vector3(rigidbody.velocity.x, rigidbody.velocity.y * hit4.transform.GetComponent<bounceOptions>().bounceMult, rigidbody.velocity.z);
                        if(hit4.transform.GetComponent<bounceOptions>().maxbounce)
                        {
                            if (hit4.transform.GetComponent<bounceOptions>().bouncecap < rigidbody.velocity.y)
                            {
                                rigidbody.velocity = new Vector3(rigidbody.velocity.x, hit4.transform.GetComponent<bounceOptions>().bouncecap);
                            }
                        }
                        rigidbody.velocity += transform.forward * Time.deltaTime * moveSpeed * hit4.transform.GetComponent<bounceOptions>().speedMult;
                    }
                    else
                    {
                        rigidbody.velocity = new Vector3(rigidbody.velocity.x, rigidbody.velocity.y * -0.8f, rigidbody.velocity.z);
                        rigidbody.velocity += transform.forward * Time.deltaTime * moveSpeed * 1.5f;
                    }

                    //bouncing = true;
                }**/


            }

            RaycastHit hit2;
            // Does the ray intersect any objects excluding the player layer
            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hit2, 1.2f))
            {


                if (hit2.transform.gameObject.tag == "bounce" && rigidbody.velocity.y < -1)
                {
                    //boing boing
                    /**if (hit2.transform.GetComponent<bounceOptions>() != null)
                    {
                        rigidbody.velocity = new Vector3(rigidbody.velocity.x, rigidbody.velocity.y * hit2.transform.GetComponent<bounceOptions>().bounceMult, rigidbody.velocity.z);
                        rigidbody.velocity += transform.forward * Time.deltaTime * moveSpeed * hit2.transform.GetComponent<bounceOptions>().speedMult;
                    }
                    else
                    {
                        rigidbody.velocity = new Vector3(rigidbody.velocity.x, rigidbody.velocity.y * -0.8f, rigidbody.velocity.z);
                        rigidbody.velocity += transform.forward * Time.deltaTime * moveSpeed * 1.5f;
                    }**/
                }
                else
                {
                    if (hit2.transform.gameObject.tag != "arrow")
                    {
                        if (hit2.transform.GetComponent<Collider>().isTrigger == false)
                        {

                            canJump = true;

                            if (spaceLock)
                            {
                                canJump = false;
                            }

                            if (Mathf.Abs(this.GetComponent<Rigidbody>().velocity.x) > maxVelocity)
                            {
                                this.GetComponent<Rigidbody>().velocity = new Vector3(this.GetComponent<Rigidbody>().velocity.x * friction, this.GetComponent<Rigidbody>().velocity.y, this.GetComponent<Rigidbody>().velocity.z);

                            }
                            if (Mathf.Abs(this.GetComponent<Rigidbody>().velocity.z) > maxVelocity)
                            {
                                this.GetComponent<Rigidbody>().velocity = new Vector3(this.GetComponent<Rigidbody>().velocity.x, this.GetComponent<Rigidbody>().velocity.y, this.GetComponent<Rigidbody>().velocity.z * friction);

                            }
                        }
                        else
                        {
                            //Debug.Log("hit trigger");
                            spaceLock = false;
                            canJump = false;
                            if (Mathf.Abs(this.GetComponent<Rigidbody>().velocity.x) > maxVelocity)
                            {
                                this.GetComponent<Rigidbody>().velocity = new Vector3(this.GetComponent<Rigidbody>().velocity.x * airFriction, this.GetComponent<Rigidbody>().velocity.y, this.GetComponent<Rigidbody>().velocity.z);

                            }
                            if (Mathf.Abs(this.GetComponent<Rigidbody>().velocity.z) > maxVelocity)
                            {
                                this.GetComponent<Rigidbody>().velocity = new Vector3(this.GetComponent<Rigidbody>().velocity.x, this.GetComponent<Rigidbody>().velocity.y, this.GetComponent<Rigidbody>().velocity.z * airFriction);

                            }
                        }

                    }
                    else
                    {

                        spaceLock = false;
                        canJump = false;
                        if (Mathf.Abs(this.GetComponent<Rigidbody>().velocity.x) > maxVelocity)
                        {
                            this.GetComponent<Rigidbody>().velocity = new Vector3(this.GetComponent<Rigidbody>().velocity.x * airFriction, this.GetComponent<Rigidbody>().velocity.y, this.GetComponent<Rigidbody>().velocity.z);

                        }
                        if (Mathf.Abs(this.GetComponent<Rigidbody>().velocity.z) > maxVelocity)
                        {
                            this.GetComponent<Rigidbody>().velocity = new Vector3(this.GetComponent<Rigidbody>().velocity.x, this.GetComponent<Rigidbody>().velocity.y, this.GetComponent<Rigidbody>().velocity.z * airFriction);

                        }


                    }
                }

            }
            else
            {
                spaceLock = false;
                canJump = false;
                if (Mathf.Abs(this.GetComponent<Rigidbody>().velocity.x) > maxVelocity)
                {
                    this.GetComponent<Rigidbody>().velocity = new Vector3(this.GetComponent<Rigidbody>().velocity.x * airFriction, this.GetComponent<Rigidbody>().velocity.y, this.GetComponent<Rigidbody>().velocity.z);

                }
                if (Mathf.Abs(this.GetComponent<Rigidbody>().velocity.z) > maxVelocity)
                {
                    this.GetComponent<Rigidbody>().velocity = new Vector3(this.GetComponent<Rigidbody>().velocity.x, this.GetComponent<Rigidbody>().velocity.y, this.GetComponent<Rigidbody>().velocity.z * airFriction);

                }
            }
            RaycastHit hit3;
            // Does the ray intersect any objects excluding the player layer
            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit3, 0.8f))
            {
                Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit3.distance, Color.yellow);
                //Debug.Log("Did Hit");

                if (hit3.transform.GetComponent<Collider>().isTrigger == false)
                {
                    if (hit3.transform.gameObject != null)
                    {
                        facingWall = true;
                        if (Mathf.Abs(GetComponent<Rigidbody>().velocity.x) + Mathf.Abs(GetComponent<Rigidbody>().velocity.z) / 2 >= 10)
                        {
                            Debug.Log("*Bonk*");


                        }
                    }
                }
                else
                {
                    facingWall = false;
                }



            }
            else
            {
                facingWall = false;
            }
            grndMove = new Vector3(0, GetComponent<Rigidbody>().velocity.y, 0);
            if (GetComponent<Rigidbody>().velocity.y < -1 && inCloud)
            {
                GetComponent<Rigidbody>().velocity = new Vector3(GetComponent<Rigidbody>().velocity.x, onWallGravity, GetComponent<Rigidbody>().velocity.z);
                canJump = true;
            }
            if (Input.GetKey(KeyCode.W) || VRpressingW)
            {
                if (!facingWall)
                {
                    if (!canJump || sliding)
                    {
                        if (onSlideWallLeft || onSlideWallRight)
                        {
                            this.GetComponent<Rigidbody>().velocity += transform.forward * Time.deltaTime * moveSpeed * 1.5f;
                        }
                        else
                        {
                            if (lastYrot != transform.eulerAngles.y)
                            {
                                lastrotdifferencevar = transform.eulerAngles.y - lastYrot;
                                lastYrot = transform.eulerAngles.y;
                            }
                            //float spd = (Mathf.Abs(rigidbody.velocity.x) + Mathf.Abs(rigidbody.velocity.z) * 0.5f);
                            //this.GetComponent<Rigidbody>().velocity = Vector3.(Vector3.forward.x * spd * Time.deltaTime * moveSpeed,rigidbody.velocity.y, Vector3.forward.z * spd * Time.deltaTime * moveSpeed);
                            this.GetComponent<Rigidbody>().velocity += transform.forward * Time.deltaTime * moveSpeed;
                            //this.GetComponent<Rigidbody>().velocity += new Vector3(Mathf.Clamp(lastrotdifferencevar / 50,-2,2), 0, 0);
                            //this.GetComponent<Rigidbody>().velocity += new Vector3(0, 0, lastrotdifferencevar);
                            //Debug.Log(rigidbody.velocity);
                        }

                    }
                    else
                    {
                        if (!inCloud)
                        {
                            grndMove += transform.forward * Time.deltaTime * groundMoveSpeed;
                        }
                        else
                        {
                            this.GetComponent<Rigidbody>().velocity += transform.forward * Time.deltaTime * moveSpeed * 1.5f;
                        }
                    }
                }







            }
            if (Input.GetKey(KeyCode.S) || VRpressingS)
            {

                if (!canJump || sliding)
                {
                    this.GetComponent<Rigidbody>().velocity += transform.forward * Time.deltaTime * moveSpeed * -1;
                }
                else
                {
                    if (!inCloud)
                    {
                        grndMove += transform.forward * Time.deltaTime * groundMoveSpeed * -1;
                    }
                    else
                    {
                        this.GetComponent<Rigidbody>().velocity += transform.forward * Time.deltaTime * moveSpeed * -1;
                    }
                }
            }
            if (Input.GetKey(KeyCode.A) || VRpressingA)
            {
                if (!onNormalWall && !onSlideWallLeft)
                {

                    if (!canJump || sliding)
                    {
                        this.GetComponent<Rigidbody>().velocity += transform.right * Time.deltaTime * moveSpeed * -1;
                        if (usenewsys && turnspeed <= -1)
                        {
                            this.GetComponent<Rigidbody>().velocity += transform.forward * Time.deltaTime * moveSpeed / 2 * (horizontalmovespeed + 1) * Mathf.Clamp(Mathf.Abs(turnspeed / 5), 0, turndivide);
                        }
                    }
                    else
                    {

                        if (!inCloud)
                        {
                            grndMove += transform.right * Time.deltaTime * groundMoveSpeed * -1;
                        }
                        else
                        {
                            this.GetComponent<Rigidbody>().velocity += transform.right * Time.deltaTime * moveSpeed * -1;
                        }
                    }
                }
                if (onSlideWallLeft)
                {
                    if (GetComponent<Rigidbody>().velocity.y < -1)
                    {
                        GetComponent<Rigidbody>().velocity = new Vector3(GetComponent<Rigidbody>().velocity.x, onWallGravity, GetComponent<Rigidbody>().velocity.z);
                    }
                }
                if (onNormalWall)
                {
                    if (!inCloud)
                    {
                        grndMove += transform.right * Time.deltaTime * groundMoveSpeed * -1;
                    }
                    else
                    {
                        this.GetComponent<Rigidbody>().velocity += transform.right * Time.deltaTime * moveSpeed * -1;
                    }
                }


            }
            if (Input.GetKey(KeyCode.D) || VRpressingD)
            {

                if (!onNormalWall && !onSlideWallRight)
                {

                    if (!canJump || sliding)
                    {
                        this.GetComponent<Rigidbody>().velocity += transform.right * Time.deltaTime * moveSpeed;
                        if (usenewsys && turnspeed >= 1)
                        {
                            this.GetComponent<Rigidbody>().velocity += transform.forward * Time.deltaTime * moveSpeed / 2 * (horizontalmovespeed + 1) * Mathf.Clamp(Mathf.Abs(turnspeed / 5), 0, turndivide);
                        }
                    }
                    else
                    {
                        if (!inCloud)
                        {
                            grndMove += transform.right * Time.deltaTime * groundMoveSpeed;
                        }
                        else
                        {
                            this.GetComponent<Rigidbody>().velocity += transform.right * Time.deltaTime * moveSpeed;
                        }
                    }
                }
                if (onSlideWallRight)
                {
                    if (GetComponent<Rigidbody>().velocity.y < -1)
                    {
                        GetComponent<Rigidbody>().velocity = new Vector3(GetComponent<Rigidbody>().velocity.x, onWallGravity, GetComponent<Rigidbody>().velocity.z);
                    }
                }
                if (onNormalWall)
                {
                    if (!inCloud)
                    {
                        grndMove += transform.right * Time.deltaTime * groundMoveSpeed;
                    }
                    else
                    {
                        this.GetComponent<Rigidbody>().velocity += transform.right * Time.deltaTime * moveSpeed;
                    }
                }


            }
            if (canJump && !inCloud)
            {
                this.GetComponent<Rigidbody>().velocity = grndMove;
            }

            grndMove = Vector3.zero;
            if (Input.GetKey(KeyCode.Space) || VRpressingSpace)
            {

                // Does the ray intersect any objects excluding the player layer
                if (canJump)
                {
                    spaceLock = true;
                    if (this.GetComponent<Rigidbody>().velocity.y < 0)
                    {
                        this.GetComponent<Rigidbody>().velocity = new Vector3(this.GetComponent<Rigidbody>().velocity.x, 0, this.GetComponent<Rigidbody>().velocity.z);
                    }

                    this.GetComponent<Rigidbody>().velocity += new Vector3(0, jumpHeight, 0);
                }
                else
                {
                    if (!jumpedOnWall)
                    {
                        Debug.Log("jumped");
                    }
                    if (onSlideWallLeft && !jumpedOnWall)
                    {
                        if (this.GetComponent<Rigidbody>().velocity.y < 0)
                        {
                            this.GetComponent<Rigidbody>().velocity = new Vector3(this.GetComponent<Rigidbody>().velocity.x, 0, this.GetComponent<Rigidbody>().velocity.z);
                        }
                        this.GetComponent<Rigidbody>().velocity += new Vector3(0, jumpHeight, 0);
                        this.GetComponent<Rigidbody>().velocity += transform.right * Time.deltaTime * moveSpeed * 23;
                        this.GetComponent<Rigidbody>().velocity += transform.forward * Time.deltaTime * moveSpeed * 23;
                        jumpedOnWall = true;
                    }
                    else
                    {
                        if (onSlideWallRight && !jumpedOnWall)
                        {
                            if (this.GetComponent<Rigidbody>().velocity.y < 0)
                            {
                                this.GetComponent<Rigidbody>().velocity = new Vector3(this.GetComponent<Rigidbody>().velocity.x, 0, this.GetComponent<Rigidbody>().velocity.z);
                            }
                            this.GetComponent<Rigidbody>().velocity += new Vector3(0, jumpHeight, 0);
                            this.GetComponent<Rigidbody>().velocity += transform.right * Time.deltaTime * moveSpeed * -23;
                            this.GetComponent<Rigidbody>().velocity += transform.forward * Time.deltaTime * moveSpeed * 23;
                            jumpedOnWall = true;
                        }
                        else
                        {
                            if (coyoteTime > 0 && onSlideWallLeft == false && onSlideWallRight == false && jumpedOnWall == false)
                            {
                                if (lastWall == 1)
                                {
                                    if (this.GetComponent<Rigidbody>().velocity.y < 0)
                                    {
                                        this.GetComponent<Rigidbody>().velocity = new Vector3(this.GetComponent<Rigidbody>().velocity.x, 0, this.GetComponent<Rigidbody>().velocity.z);
                                    }
                                    this.GetComponent<Rigidbody>().velocity += new Vector3(0, jumpHeight, 0);
                                    this.GetComponent<Rigidbody>().velocity += transform.right * Time.deltaTime * moveSpeed * 23;
                                    this.GetComponent<Rigidbody>().velocity += transform.forward * Time.deltaTime * moveSpeed * 23;
                                    jumpedOnWall = true;
                                    coyoteTapSpace = false;
                                    coyoteTime = 0;
                                    coyoteTimeEnabled = false;
                                    coyoteTimeCooldown = 0.1f;
                                }
                                if (lastWall == 2)
                                {
                                    if (this.GetComponent<Rigidbody>().velocity.y < 0)
                                    {
                                        this.GetComponent<Rigidbody>().velocity = new Vector3(this.GetComponent<Rigidbody>().velocity.x, 0, this.GetComponent<Rigidbody>().velocity.z);
                                    }
                                    this.GetComponent<Rigidbody>().velocity += new Vector3(0, jumpHeight, 0);
                                    this.GetComponent<Rigidbody>().velocity += transform.right * Time.deltaTime * moveSpeed * -23;
                                    this.GetComponent<Rigidbody>().velocity += transform.forward * Time.deltaTime * moveSpeed * 23;
                                    jumpedOnWall = true;
                                    coyoteTapSpace = false;
                                    coyoteTime = 0;
                                    coyoteTimeEnabled = false;
                                    coyoteTimeCooldown = 0.1f;
                                }
                            }
                        }
                    }




                    if (bouncing)
                    {
                        if (this.GetComponent<Rigidbody>().velocity.y < 0)
                        {
                            this.GetComponent<Rigidbody>().velocity = new Vector3(this.GetComponent<Rigidbody>().velocity.x, 0, this.GetComponent<Rigidbody>().velocity.z);
                        }
                        this.GetComponent<Rigidbody>().velocity += new Vector3(0, jumpHeight, 0);
                    }
                }
                if (coyoteTapSpace && jumpedOnWall)
                {
                    coyoteTapSpace = false;
                    coyoteTime = 0;
                    coyoteTimeEnabled = false;
                }


            }
            else
            {
                if (canJump)
                {
                    spaceLock = false;
                }
            }
            if (!Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.A) && canJump)
            {
                this.GetComponent<Rigidbody>().velocity = new Vector3(this.GetComponent<Rigidbody>().velocity.x * friction, this.GetComponent<Rigidbody>().velocity.y, this.GetComponent<Rigidbody>().velocity.z * friction);
            }
            if (increaseCamTiltTimer)
            {
                camTiltTimer += Time.deltaTime * 1.3f;
                //initCamTiltSwingAmount = initCamTiltSwingAmount * 1 - (math.abs(cam.transform.eulerAngles.x) / 180);
                //if it works, don't fix it
                camTiltSin = math.sin(camTiltTimer) * initCamTiltSwingAmount;
                //update: it doesn't work
                initCamTiltSwingAmount = initCamTiltSwingAmount * 0.99f;
            }

            if (otherCamTiltSin != 0)
            {

                otherCamTiltSin = otherCamTiltSin * 0.96f;
                if (math.abs(otherCamTiltSin) < 0.05f)
                {
                    otherCamTiltSin = 0;
                }
            }
            if (camTiltTimer > 1 && math.abs(cam.transform.eulerAngles.x) < 0.05f)
            {
                increaseCamTiltTimer = false;
                camTiltSin = 0;
                otherCamTiltSin = 0;
            }
            if (camTiltTimer > 1 && math.abs(initCamTiltSwingAmount) < 0.05f)
            {
                increaseCamTiltTimer = false;
                camTiltSin = 0;
                otherCamTiltSin = 0;
            }

            //coyote time logic, ig
            if (coyoteTimeEnabled && coyoteTime < maxCoyoteTime && coyoteTimeCooldown <= 0)
            {
                coyoteTime += Time.deltaTime;
                coyoteTapSpace = true;
            }
            else
            {
                if (coyoteTimeEnabled && coyoteTime >= maxCoyoteTime)
                {
                    coyoteTimeEnabled = false;
                    coyoteTime = 0;
                    coyoteTapSpace = false;
                }
                if (coyoteTimeCooldown > 0)
                {
                    coyoteTimeCooldown -= Time.deltaTime;
                    coyoteTimeEnabled = false;
                    coyoteTime = 0;
                    coyoteTapSpace = false;
                }
            }


            //camTiltSin = math.clamp(camTiltSin, -15, 15);
            //idek jump indicators ig
            if (timercanvas != null)
            {
                if (!jumpedOnWall)
                {
                    if (onSlideWallLeft)
                    {
                        if (timercanvas.GetComponent<timerScript>().leftbar.GetComponent<RectTransform>().sizeDelta.y != timercanvas.GetComponent<timerScript>().leftbar.GetComponent<jumpbaranim>().end && timercanvas.GetComponent<timerScript>().leftbar.GetComponent<jumpbaranim>().extending == false)
                        {
                            timercanvas.GetComponent<timerScript>().leftbar.GetComponent<jumpbaranim>().extend();
                            Debug.Log("extend left");
                        }
                    }
                    else
                    {
                        if (timercanvas.GetComponent<timerScript>().leftbar.GetComponent<RectTransform>().sizeDelta.y != timercanvas.GetComponent<timerScript>().leftbar.GetComponent<jumpbaranim>().start && timercanvas.GetComponent<timerScript>().leftbar.GetComponent<jumpbaranim>().retracting == false)
                        {
                            timercanvas.GetComponent<timerScript>().leftbar.GetComponent<jumpbaranim>().retract();
                        }
                    }
                    if (onSlideWallRight)
                    {
                        if (timercanvas.GetComponent<timerScript>().rightbar.GetComponent<RectTransform>().sizeDelta.y != timercanvas.GetComponent<timerScript>().rightbar.GetComponent<jumpbaranim>().end && timercanvas.GetComponent<timerScript>().rightbar.GetComponent<jumpbaranim>().extending == false)
                        {
                            timercanvas.GetComponent<timerScript>().rightbar.GetComponent<jumpbaranim>().extend();
                        }

                    }
                    else
                    {
                        if (timercanvas.GetComponent<timerScript>().rightbar.GetComponent<RectTransform>().sizeDelta.y != timercanvas.GetComponent<timerScript>().rightbar.GetComponent<jumpbaranim>().start && timercanvas.GetComponent<timerScript>().rightbar.GetComponent<jumpbaranim>().retracting == false)
                        {
                            timercanvas.GetComponent<timerScript>().rightbar.GetComponent<jumpbaranim>().retract();
                        }
                    }
                }
                else
                {
                    if (onSlideWallLeft)
                    {
                        if (timercanvas.GetComponent<timerScript>().leftbar.GetComponent<RectTransform>().sizeDelta.y != timercanvas.GetComponent<timerScript>().leftbar.GetComponent<jumpbaranim>().start && timercanvas.GetComponent<timerScript>().leftbar.GetComponent<jumpbaranim>().retracting == false)
                        {
                            timercanvas.GetComponent<timerScript>().leftbar.GetComponent<jumpbaranim>().retract();
                        }
                    }
                    if (onSlideWallRight)
                    {
                        if (timercanvas.GetComponent<timerScript>().rightbar.GetComponent<RectTransform>().sizeDelta.y != timercanvas.GetComponent<timerScript>().rightbar.GetComponent<jumpbaranim>().start && timercanvas.GetComponent<timerScript>().rightbar.GetComponent<jumpbaranim>().retracting == false)
                        {
                            timercanvas.GetComponent<timerScript>().rightbar.GetComponent<jumpbaranim>().retract();
                        }
                    }
                }
            }

            if (usenewsys)
            {
                turnspeed = transform.eulerAngles.y - prevturn;
                if (prevturn != transform.eulerAngles.y)
                {
                    prevturn = transform.eulerAngles.y;
                }
            }
            //detect horiz omvement spd

            if (oldpos.x + oldpos.z != transform.position.x + transform.position.z)
            {
                horizontalmovespeed = Mathf.Abs(oldpos.x + oldpos.z - transform.position.x - transform.position.z);
                allmovespeed = Mathf.Abs(oldpos.x) + Mathf.Abs(oldpos.y) + Mathf.Abs(oldpos.z) - Mathf.Abs(transform.position.x) - Mathf.Abs(transform.position.y) - Mathf.Abs(transform.position.z);
                oldpos = new Vector3(transform.position.x, 0, transform.position.z);
            }

            if (onSlideWallLeft == false && onSlideWallRight == false)
            {
                jumpedOnWall = false;
            }

            //sfx wind

            //extrasfx.GetComponent<AudioSource>().volume = sfxvol * Mathf.Clamp(0, Mathf.Round(Mathf.Abs(GetComponent<Rigidbody>().velocity.x) + Mathf.Abs(GetComponent<Rigidbody>().velocity.y) + Mathf.Abs(GetComponent<Rigidbody>().velocity.z) * 10) / 10, 5);

            //speed ui thing

            if (timercanvas != null)
            {
                var temp = Mathf.Round(Mathf.Abs(GetComponent<Rigidbody>().velocity.x) + Mathf.Abs(GetComponent<Rigidbody>().velocity.y) + Mathf.Abs(GetComponent<Rigidbody>().velocity.z));
                var truncated = temp.ToString().Truncate(3);
                var final = "";
                if (truncated.Length == 1)
                {
                    final = "00" + truncated;
                }
                if (truncated.Length == 2)
                {
                    final = "0" + truncated;
                }
                if (truncated.Length == 3)
                {
                    final = truncated;
                }
                if (truncated.Length > 3)
                {
                    final = "999";
                }
                timercanvas.GetComponent<timerScript>().speed.GetComponent<TMP_Text>().text = final;
            }
            else
            {
                if (GameObject.Find("timerCanvas") != null)
                {
                    timercanvas = GameObject.Find("timerCanvas");
                }
            }
        }

        //end of fixed update
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "bounce")
        {
            bouncing = true;
            Time.timeScale = 0.5f;
        }
        if (other.tag == "gem")
        {
            other.SendMessage("collect");
        }
        if (other.tag == "slide")
        {
            //sliding = true;
            //other.transform.parent.GetComponent<slideIndex>().wall1.GetComponent<Collider>().enabled = true;
            //other.transform.parent.GetComponent<slideIndex>().wall2.GetComponent<Collider>().enabled = true;
        }
        if (other.tag == "Finish")
        {
            if (GameObject.Find("VR BODY(Clone)") != null)
            {
                //GameObject.Find("VR BODY(Clone)").GetComponent<indexOfVrPlayer>().Canvas.SendMessage("finish");
            }
            else
            {
                GameObject.Find("Canvas").SendMessage("finish");
            }

        }
        if (other.tag == "dieTrigger")
        {
            GameObject.Find("Canvas").GetComponent<detectDeath>().dead = true;
            //.GetComponent<Button>().onClick.Invoke();
        }
        if (other.tag == "Button")
        {
            //GameObject.Find("Canvas").GetComponent<detectDeath>().dead = true;
            other.transform.GetComponent<UnityEngine.UI.Button>().onClick.Invoke();
        }
        if (other.tag == "cloud")
        {
            inCloud = true;
        }


    }

    private void OnTriggerStay(Collider other)

    {
        if (other.tag == "arrow")
        {
            if (canJump == false)
            {
                //Destroy(other.gameObject);
                /**if (other.GetComponent<arrowPointer>().resetPlayerSpeed)
                {
                    this.GetComponent<Rigidbody>().velocity = Vector3.zero;
                    this.GetComponent<Rigidbody>().velocity = other.transform.forward * other.GetComponent<arrowPointer>().multiplier;
                }**/
                //else
                //{
                //    this.GetComponent<Rigidbody>().velocity += other.transform.forward * other.GetComponent<arrowPointer>().multiplier;
                //}
            }




        }
        if (other.tag == "water")
        {
            GetComponent<Rigidbody>().velocity = new Vector3(Mathf.Clamp(GetComponent<Rigidbody>().velocity.x, -5, 5), Mathf.Clamp(GetComponent<Rigidbody>().velocity.y, -5, 5), Mathf.Clamp(GetComponent<Rigidbody>().velocity.z, -5, 5));


        }

    }

    public static void LoadScene(string SceneNameToLoad)
    {
        PendingPreviousScene = SceneManager.GetActiveScene().name;
        SceneManager.sceneLoaded += ActivatorAndUnloader;
        SceneManager.LoadScene(SceneNameToLoad, LoadSceneMode.Additive);
    }

    static string PendingPreviousScene;
    static void ActivatorAndUnloader(Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= ActivatorAndUnloader;
        SceneManager.SetActiveScene(scene);
        SceneManager.UnloadSceneAsync(PendingPreviousScene);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "bounce")
        {
            bouncing = false;
            Time.timeScale = 1f;
        }
        if (other.tag == "slide")
        {
            sliding = false;
            //other.transform.parent.GetComponent<slideIndex>().wall1.GetComponent<Collider>().enabled = false;
            //other.transform.parent.GetComponent<slideIndex>().wall2.GetComponent<Collider>().enabled = false;
        }
        if (other.tag == "cloud")
        {
            inCloud = false;
        }
    }

    public void initCamTilt(float swingAmount)
    {
        if (onSlideWallLeft && onSlideWallRight)
        {

        }
        else
        {
            increaseCamTiltTimer = false;
            initCamTiltSwingAmount = swingAmount + camTiltSin;
            camTiltTimer = 0;

            otherCamTiltSin += camTiltSin;
            camTiltSin = 0;

            increaseCamTiltTimer = true;
        }






    }

    public void Rick_Astley()
    {
        //do nothing
    }

}



public static class StringExt
{
    public static string Truncate(this string value, int maxLength)
    {
        if (string.IsNullOrEmpty(value)) return value;
        return value.Length <= maxLength ? value : value.Substring(0, maxLength);
    }
}
