using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugText : MonoBehaviour
{
    public static Text Output;
    public static DebugText Instance;
    public GameObject debugText;


    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        if(Output == null)
            Output = GetComponent<Text>();
        LogMessage("this is Debug");
        LogMessage("Lets go!");
        EnableDebugger(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void EnableDebugger(bool newStatus)
    {
        if (newStatus)
        {
            Output.CrossFadeAlpha(1, 0.1f, true);
        }    
        else
            Output.CrossFadeAlpha(0,0.1f,true);
    }

    public static void EnableDebuggerStatic(bool newStatus)
    {
        if (newStatus)
        {
            Output.CrossFadeAlpha(1, 0.1f, true);
        }    
        else
            Output.CrossFadeAlpha(0,0.1f,true);
    }

    public void EnableText(Text textToEnable, bool newStatus)
    {
        if (newStatus)
        {
            textToEnable.CrossFadeAlpha(1, 0.1f, true);
        }
        else
            textToEnable.CrossFadeAlpha(0, 0.1f, true);
    }

    public static void LogNewMessage(string newMessage)
    {
        Output.text = newMessage;
    }
    public static void LogMessage(string newMessage)
    {
        Output.text = newMessage + "\r\n" + Output.text;
    }
}
