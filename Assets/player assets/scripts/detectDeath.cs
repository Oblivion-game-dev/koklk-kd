using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices.ComTypes;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using System.IO;

public class detectDeath : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject empty;
    public GameObject saveobj;
    public GameObject player;
    public GameObject cam;
    public GameObject canvasText;
    public GameObject timeText;
    public Vector3 savedVelocity;
    public bool paused;
    public bool dead;
    public bool finished;
    public bool pressingMenu;
    public bool pressedMenuThisFrame;
    public bool pressedMenuThisFrame2;
    public bool ctrlrPressingEsc;
    public bool beganPressingEscOnCtrlr;
    private string fileName = "save.txt";
    private string directoryPath = "Assets/Resources";
    private InputAction m_AttackAction;
    public GameObject selectOnPause;
    void Start()
    {
        dead = false;
        GetComponent<Canvas>().enabled = false;
        m_AttackAction = new InputAction("Attack", binding: "<Gamepad>/Start");
        m_AttackAction.Enable();
        if (GameObject.Find("lastscene(Clone)"))
        {
            Destroy(GameObject.Find("lastscene(Clone)"));
        }
        
        //saveobj = Instantiate(empty);
        //saveobj.GetComponent<lastscenemgr>().worldIndex = (int)float.Parse(SceneManager.GetActiveScene().name.Split('-')[0]);
        //DontDestroyOnLoad(saveobj);
    }

    // Update is called once per frame
    void Update()
    {
        
        var attacking = m_AttackAction.ReadValue<float>();
        if (Mathf.Approximately(attacking, 1f))
        {
            if(ctrlrPressingEsc == false)
            {
                beganPressingEscOnCtrlr = true;
            }
            else
            {
                beganPressingEscOnCtrlr = false;
            }
            ctrlrPressingEsc = true;
        }
        else
        {
            ctrlrPressingEsc = false;
        }
        //var inputDevices = new List<UnityEngine.XR.InputDevice>();
        //UnityEngine.XR.InputDevices.GetDevices(inputDevices);
        
        if(player.transform.position.y < -200 && !finished || dead && !finished)
        {
            dead = true;
            if (GameObject.Find("Sphere") != null)
            {
                GameObject.Find("Sphere").SendMessage("pausetoggle");
            }
            GetComponent<Canvas>().enabled = true;
            if(player.transform.position.y < -200)
            {
                player.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePosition;
            }
            else
            {
                player.GetComponent<movement>().enabled = false;
            }
            
            if(player.transform.parent == null)
            {
                player.GetComponent<movement>().cam.GetComponent<cameraMovement>().enabled = false;
            }
            
            Cursor.lockState = CursorLockMode.None;
            canvasText.GetComponent<TMP_Text>().text = "You Died";
        }
        if(Input.GetKeyDown(KeyCode.Escape) && dead == false)
        {
            if(paused)
            {
                GetComponent<Canvas>().enabled = false;
                player.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
                Cursor.lockState = CursorLockMode.Locked;
                canvasText.GetComponent<TMP_Text>().text = "Paused";
                paused = false;
                if (GameObject.Find("Sphere") != null)
                {
                    GameObject.Find("Sphere").SendMessage("pausetoggle");
                }
                player.transform.GetComponent<Rigidbody>().velocity = savedVelocity;
            }
            else
            {
                paused = true;
                if (GameObject.Find("Sphere") != null)
                {
                    GameObject.Find("Sphere").SendMessage("pausetoggle");
                }
                GetComponent<Canvas>().enabled = true;
                savedVelocity = player.transform.GetComponent<Rigidbody>().velocity;
                player.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
                Cursor.lockState = CursorLockMode.None;
                canvasText.GetComponent<TMP_Text>().text = "Paused";
            }
        }
        if (beganPressingEscOnCtrlr && dead == false)
        {
            if (paused)
            {
                GetComponent<Canvas>().enabled = false;
                player.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
                Cursor.lockState = CursorLockMode.Locked;
                canvasText.GetComponent<TMP_Text>().text = "Paused";
                paused = false;
                if (GameObject.Find("Sphere") != null)
                {
                    GameObject.Find("Sphere").SendMessage("pausetoggle");
                }
                player.transform.GetComponent<Rigidbody>().velocity = savedVelocity;
            }
            else
            {

                paused = true;
                if (GameObject.Find("Sphere") != null)
                {
                    GameObject.Find("Sphere").SendMessage("pausetoggle");
                }
                GetComponent<Canvas>().enabled = true;
                savedVelocity = player.transform.GetComponent<Rigidbody>().velocity;
                player.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
                Cursor.lockState = CursorLockMode.None;
                canvasText.GetComponent<TMP_Text>().text = "Paused";
            }
        }
        
    }

    

    public void finish()
    {
        if(!dead)
        {
            //126
            //selectOnPause.GetComponent<ctrlrMenuCtrls>().selected = true;
            directoryPath = Application.persistentDataPath;
            string filePath = Path.Combine(directoryPath, fileName);

            if (File.Exists(filePath))
            {
                string fileText = File.ReadAllText(filePath);
                if (!fileText.Contains("|" + SceneManager.GetActiveScene().name + "|"))
                {
                    Debug.Log("completed lvl and saved");
                    using (StreamWriter sw = new StreamWriter(filePath)) // 'false' means overwrite
                    {
                        sw.WriteLine(fileText + "|" + SceneManager.GetActiveScene().name + "|");
                    }
                }

            }
            if (finished == false)
            {
                finished = true;
                if (GameObject.Find("Sphere") != null)
                {
                    GameObject.Find("Sphere").SendMessage("pausetoggle");
                }
                GetComponent<Canvas>().enabled = true;
                player.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePosition;
                Cursor.lockState = CursorLockMode.None;
                //I definately didn't spend an hour trying to get this to work
                canvasText.transform.parent.localPosition = new Vector3(0, 80, 0);
                canvasText.GetComponent<TMP_Text>().text = "Final Time:";
                timeText.GetComponent<TMP_Text>().text = GameObject.Find("timerCanvas").GetComponent<timerScript>().timerText.GetComponent<TMP_Text>().text;
                Destroy(GameObject.Find("timerCanvas"));
                
                
            }
        }
        
        
    }
}
