using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeController : MonoBehaviour
{
    public static FadeController FogFade;
    public bool fog;
    public enum ComponentKind{IMAGE, TEXT}

    public ComponentKind cKind;
    private bool fade;
    public bool fadeIn;
    public float transparency;
    public float fadeSpeed;
    
    
    
    // Start is called before the first frame update
    void Start()
    {
        if (fog)
        {
            FogFade = this;
        }

    }

    // Update is called once per frame
    void Update()
    {
        
        if (fade)
        {
            ChangeTrans((fadeIn?fadeSpeed:-fadeSpeed)*Time.deltaTime);
        }
    }


    public void Fade(bool fadeIn)
    {
        fade = true;
        this.fadeIn = fadeIn;


    }
    
    

    private void ChangeTrans(float t)
    {

        
        transparency += t;
        if (transparency <= 0||transparency>=1)
        {
            fade = false;
            transparency = transparency > 0.5 ? 1 : 0;
        }

        
        switch (cKind)
        {
            case ComponentKind.IMAGE:
                Color newCol = GetComponent<Image>().color;
                newCol.a = transparency;
                GetComponent<Image>().color=newCol;
                break;
            case ComponentKind.TEXT:
                Color newCol2 = GetComponent<Text>().color;
                newCol2.a = transparency;
                GetComponent<Text>().color=newCol2;
                break;
        }


        
    }
    
    
    
}
