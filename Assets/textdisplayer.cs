using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class textdisplayer : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject toptxt;
    public GameObject bottomtxt;
    public float toptimer;
    public bool toptimerenabled;
    public float bottomtimer;
    public bool bottomtimerenabled;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(toptimerenabled)
        {
        if(toptimer > 0)
        {
            toptimer -= Time.deltaTime;

        }
        else
        {
            toptimerenabled = false;
            toptxt.transform.GetComponent<TMP_Text>().text = "";
        }
        }
        if(bottomtimerenabled)
        {
        if(bottomtimer > 0)
        {
            bottomtimer -= Time.deltaTime;

        }
        else
        {
            bottomtimerenabled = false;
            bottomtxt.GetComponent<TMP_Text>().text = "";
        }
        }
    }
    public void settoptext(string text, float time)
    {
        toptxt.transform.GetComponent<TMP_Text>().text = text;
        if(time > 0)
        {
            toptimerenabled = true;
            toptimer = time;
        }
        else
        {
            toptimerenabled = false;
        }
    }
    public void setbottomtext(string text, float time)
    {
        bottomtxt.transform.GetComponent<TMP_Text>().text = text;
        if(time > 0)
        {
            bottomtimerenabled = true;
            bottomtimer = time;
        }
        else
        {
            bottomtimerenabled = false;
        }
    }
}
