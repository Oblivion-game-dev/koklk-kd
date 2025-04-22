using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class jumpbaranim : MonoBehaviour
{
    // Start is called before the first frame update
    public bool extending;
    public float width;
    public bool retracting;
    public bool starttimer;
    public float ysize;
    public float start;
    public float end;
    public float speed;
    public float timeinanim;
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (starttimer)
        {
            timeinanim = timeinanim + Time.deltaTime;
        }
        else
        {
            timeinanim = 0;
        }
        if (extending)
        {
            ysize = Mathf.Pow(timeinanim * speed, 3);
            if (ysize >= end)
            {
                extending = false;
                starttimer = false;
                ysize = end;
                //extending = false;
            }
        }
        if (retracting)
        {
            ysize = Mathf.Pow(timeinanim * speed * -1, 3);
            if (ysize <= start)
            {
                retracting = false;
                starttimer = false;
                ysize = start;
            }
        }
        transform.GetComponent<RectTransform>().sizeDelta = new Vector2(width,ysize);
    }

    public void extend()
    {
        extending = true;
        retracting = false;
        ysize = 0;
        timeinanim = 0;
        starttimer = true;

        
    }
    public void retract()
    {
        retracting = true;
        extending = false;
        ysize = 0;
        timeinanim = 0;
        starttimer = true;
    }
}
