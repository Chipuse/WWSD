using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextEnabler : MonoBehaviour
{
    public Text Output;
    
    public void EnableDebugger(bool newStatus)
    {
        if(Output != null)
        {
            if (newStatus)
            {
                Output.CrossFadeAlpha(1, 0.1f, true);
            }
            else
                Output.CrossFadeAlpha(0, 0.1f, true);
        }
        
    }

    
}
