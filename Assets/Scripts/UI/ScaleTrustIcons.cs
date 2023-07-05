using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleTrustIcons : MonoBehaviour
{
    public int optimalHeight =1920;
    public int optimalWidth = 1080;

    public float scaleFac;

    public RectTransform ownTransform;
    // Start is called before the first frame update
    void Awake()
    {
        
        scaleFac = (optimalHeight*1.0f/optimalWidth)  / (Screen.height*1.0f/Screen.width);
        Vector2 sizeDelta = ownTransform.sizeDelta;
        DebugText.LogMessage(" old rect size: "+ sizeDelta.x);
        sizeDelta =new Vector2(sizeDelta.x*scaleFac, sizeDelta.y);
        ownTransform.sizeDelta = sizeDelta;
        DebugText.LogMessage("screen width: "+Screen.width+", rect size: "+ sizeDelta.x+", scale fac: "+scaleFac);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
