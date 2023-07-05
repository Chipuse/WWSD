using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class MobileDeviceOrientations : MonoBehaviour
{
    public static MobileDeviceOrientations Instance;
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
        }
    }


    private bool faceUp;
    private bool resting;
    public Image check;

    private float restingCounter = 0;
    private float restingSeconds = 1;
    private bool lyingDown;

    private static bool lyingDownGlobal;
    public static bool GetLyingDownGlobal()
    {
        return lyingDownGlobal;
    }
    void Start()
    {
        Input.gyro.enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.deviceOrientation == DeviceOrientation.FaceUp)
        {
            faceUp = true;
        }
        else
            faceUp = false;
       
        //if (Mathf.Abs(Input.gyro.userAcceleration.x) + Mathf.Abs(Input.gyro.userAcceleration.y) + Mathf.Abs(Input.gyro.userAcceleration.z) <= 0.08f)
        if (Mathf.Abs(Input.gyro.rotationRate.x) <= 0.05f && Mathf.Abs(Input.gyro.rotationRate.y) <= 0.05f /*&& Mathf.Abs(Input.gyro.rotationRate.z) <= 0.05f*/)
        {
            
            restingCounter += 1 * Time.deltaTime;
            if(faceUp)
                lyingDownGlobal = true;
            else
                lyingDownGlobal = false;
        }
        else
        {
            restingCounter = 0;
            lyingDownGlobal = false;
        }
        if (restingCounter >= restingSeconds)
            resting = true;
        else
            resting = false;

        if (resting && faceUp)
            lyingDown = true;
        else
            lyingDown = false;

        if(check != false)
        {
            if (lyingDown)
                check.color = Color.green;
            else
                check.color = Color.red;
        }
        
    }
    private void LateUpdate()
    {
        //DeleventSystem.DisplayMessageEvent(Mathf.Abs(Input.gyro.userAcceleration.x) + Mathf.Abs(Input.gyro.userAcceleration.y) + Mathf.Abs(Input.gyro.userAcceleration.z).ToString());
        //DeleventSystem.DisplayMessageEvent("x: " +Mathf.Abs(Input.gyro.userAcceleration.x).ToString() + "\r\ny: " + Mathf.Abs(Input.gyro.userAcceleration.y).ToString() + "\r\nz: "+ Mathf.Abs(Input.gyro.userAcceleration.z).ToString());
        //DeleventSystem.DisplayMessageEvent("x: " + Mathf.Abs(Input.gyro.rotationRate.x).ToString() + "\r\ny: " + Mathf.Abs(Input.gyro.rotationRate.y).ToString() + "\r\nz: " + Mathf.Abs(Input.gyro.rotationRate.z).ToString());
    }
}
