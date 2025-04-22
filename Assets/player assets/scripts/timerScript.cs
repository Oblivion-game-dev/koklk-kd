using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Globalization;

public class timerScript : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject timerText;
    public TMP_Text text;
    public float timer;
    public GameObject canvas;
    public GameObject leftbar;
    public GameObject rightbar;
    public GameObject speed;
    void Start()
    {
        text = timerText.GetComponent<TMP_Text>();
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        canvas = GameObject.Find("Canvas");
        if(GameObject.Find("VR BODY(Clone)") != null )
        {
            if(transform.parent == null)
            {
                Destroy(gameObject);
            }
        }
        canvas = GameObject.Find("Canvas");
        if(canvas != null)
        {
            if (canvas.GetComponent<detectDeath>().paused == false && canvas.GetComponent<detectDeath>().dead == false)
            {
                timer += Time.deltaTime;
                int minutes = Mathf.FloorToInt(timer / 60F);
                int seconds = Mathf.FloorToInt(timer - minutes * 60);
                int milliseconds = (int)(timer * 100f);

                string niceTime = string.Format("{0:0}:{1:00}", minutes, seconds);



                text.text = niceTime + "." + milliseconds.ToString("00").Substring(milliseconds.ToString("00").Length - 2);
            }
        }
        
    }
}
