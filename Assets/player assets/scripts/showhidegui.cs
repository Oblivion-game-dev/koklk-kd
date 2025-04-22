using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class showhidegui : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject empty;
    void Start()
    {
        if(GameObject.Find("HideGUI"))
        {
            transform.localScale = Vector3.zero;
        }
        else
        {
            transform.localScale = Vector3.one;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.F1))
        {
            if (transform.localScale == Vector3.one)
            {
                
                if(GameObject.Find("HideGUI") == null)
                {
                    GameObject temp = Instantiate(empty);
                    temp.transform.name = "HideGUI";
                    DontDestroyOnLoad(temp);
                    transform.localScale = Vector3.zero;
                }
            }
            else
            {
                

                if (GameObject.Find("HideGUI") != null)
                {
                    Destroy(GameObject.Find("HideGUI"));
                    transform.localScale = Vector3.one;
                }
            }
        }
    }
}
