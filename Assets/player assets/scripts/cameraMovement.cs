using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices.ComTypes;
using UnityEngine;
using UnityEngine.InputSystem;

public class cameraMovement : MonoBehaviour
{
    public bool usenewsys;
    public float Sensitivity
    {
        get { return sensitivity; }
        set { sensitivity = value; }
    }
    [Range(0.1f, 9f)][SerializeField] float sensitivity = 2f;
    public float CTRLRsensitivity
    {
        get { return CTRLRsensitivity2; }
        set { CTRLRsensitivity2 = value; }
    }
    [Range(0.1f, 9f)][SerializeField] float CTRLRsensitivity2 = 2f;
    public float VRsensitivity
    {
        get { return VRsensitivity2; }
        set { VRsensitivity2 = value; }
    }
    [Range(0.1f, 9f)][SerializeField] float VRsensitivity2 = 2f;
    [Tooltip("Limits vertical camera rotation. Prevents the flipping that happens when rotation goes above 90.")]
    [Range(0f, 90f)][SerializeField] float yRotationLimit = 88f;

    Vector2 rotation = Vector2.zero;
    const string xAxis = "Mouse X"; //Strings in direct code generate garbage, storing and re-using them creates no garbage
    const string yAxis = "Mouse Y";
    public bool inVR;
    public GameObject body;
    public GameObject yRotOffset;
    public GameObject zRotOffset;
    public float VRrotSpeed;
    public Vector3 OgBodyRot;
    private Vector2 m_PlayerMovement;
    private InputAction m_MoveAction;
    private string directoryPath;
    public string fileName;
    public bool camTilt;
    //And then, she asks me how we got here
    //I told her, "I don't know"
    //And if you keep on asking, I'll just keep saying so
    private void Start()
    {
        directoryPath = Application.persistentDataPath;
        fileName = "sensitivity.txt";
        string filePath = Path.Combine(directoryPath, fileName);

        if (File.Exists(filePath))
        {
            string fileText = File.ReadAllText(filePath);
            sensitivity = float.Parse(fileText);

            
        }
        else
        {
            sensitivity = 2f;
        }
        fileName = "CTRLRsensitivity.txt";
        filePath = Path.Combine(directoryPath, fileName);

        if (File.Exists(filePath))
        {
            string fileText = File.ReadAllText(filePath);
            CTRLRsensitivity2 = float.Parse(fileText);


        }
        else
        {
            CTRLRsensitivity2 = 2f;
        }
        fileName = "VRsensitivity.txt";
        filePath = Path.Combine(directoryPath, fileName);

        if (File.Exists(filePath))
        {
            string fileText = File.ReadAllText(filePath);
            VRsensitivity2 = float.Parse(fileText);


        }
        else
        {
            VRsensitivity2 = 2f;
        }
        fileName = "camTilt.txt";
        filePath = Path.Combine(directoryPath, fileName);
        if (File.Exists(filePath))
        {
            string fileText = File.ReadAllText(filePath);
            if(fileText.Contains("1"))
            {
                camTilt = true;
            }


        }
        else
        {
            camTilt = false;
        }
        m_MoveAction = new InputAction("Move");
        m_MoveAction.AddBinding("<Gamepad>/rightStick");
        m_MoveAction.Enable();
        OgBodyRot = body.transform.eulerAngles;
        rotation.x = OgBodyRot.y;
        inVR = false;
        if(transform.parent.name == "Capsule")
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    void Update()
    {
        
        var inputDevices = new List<UnityEngine.XR.InputDevice>();
        UnityEngine.XR.InputDevices.GetDevices(inputDevices);
        foreach (var device in inputDevices)
        {
            inVR = true;
        }
        
        if (!inVR)
        {
            m_PlayerMovement = m_MoveAction.ReadValue<Vector2>();
            
            rotation.x += Input.GetAxis(xAxis) * sensitivity;
            rotation.y += Input.GetAxis(yAxis) * sensitivity * -1;
            rotation.x += m_PlayerMovement.x * sensitivity;
            rotation.y += m_PlayerMovement.y * sensitivity * -1;
            
            rotation.y = Mathf.Clamp(rotation.y, -yRotationLimit, yRotationLimit);
            //var xQuat = Quaternion.AngleAxis(rotation.x, Vector3.up);
            //var yQuat = Quaternion.AngleAxis(rotation.y, Vector3.left);
            if(camTilt)
            {
                transform.eulerAngles = new Vector3(rotation.y, rotation.x, transform.parent.GetComponent<movement>().camTiltSin + transform.parent.GetComponent<movement>().otherCamTiltSin);
            }
            else
            {
                transform.eulerAngles = new Vector3(rotation.y, rotation.x, 0);
            }
            
            body.transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
            
        }
        
        if(inVR)
        {
            //transform.parent.localPosition = new Vector3(body.transform.position.x, body.transform.position.y + 1, body.transform.position.z);
            yRotOffset.transform.position = new Vector3(body.transform.position.x, body.transform.position.y - 0.5f, body.transform.position.z);
            zRotOffset.transform.localPosition = Vector3.zero;
            body.transform.eulerAngles = new Vector3(yRotOffset.transform.eulerAngles.x,transform.localEulerAngles.y + yRotOffset.transform.eulerAngles.y, yRotOffset.transform.eulerAngles.z);

            Vector2 primary2DAxisValue;

            foreach (var device in inputDevices)
            {
                if ((device.characteristics & UnityEngine.XR.InputDeviceCharacteristics.Right) != 0)
                {
                    if (device.TryGetFeatureValue(UnityEngine.XR.CommonUsages.primary2DAxis, out primary2DAxisValue))
                    {
                        if (primary2DAxisValue.x < -0.05)
                        {
                            Debug.Log("Thumbstick is pushed left.");
                            yRotOffset.transform.eulerAngles = new Vector3(yRotOffset.transform.eulerAngles.x, yRotOffset.transform.eulerAngles.y + (VRrotSpeed * Time.deltaTime * primary2DAxisValue.x), yRotOffset.transform.eulerAngles.z);
                        }
                        if (primary2DAxisValue.x > 0.05)
                        {
                            Debug.Log("Thumbstick is pushed Right. " + primary2DAxisValue.x);
                            yRotOffset.transform.eulerAngles = new Vector3(yRotOffset.transform.eulerAngles.x, yRotOffset.transform.eulerAngles.y + VRrotSpeed * Time.deltaTime * primary2DAxisValue.x, yRotOffset.transform.eulerAngles.z);

                        }
                    }

                }
            }
            if(camTilt)
            {
                zRotOffset.transform.eulerAngles = new Vector3(zRotOffset.transform.eulerAngles.x, zRotOffset.transform.eulerAngles.y, body.GetComponent<movement>().camTiltSin + body.GetComponent<movement>().otherCamTiltSin);
            }
            else
            {
                zRotOffset.transform.eulerAngles = new Vector3(zRotOffset.transform.eulerAngles.x, zRotOffset.transform.eulerAngles.y, 0);
            }

        }

        
        



    }
}
