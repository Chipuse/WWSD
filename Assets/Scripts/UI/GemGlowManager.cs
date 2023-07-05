using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GemGlowManager : MonoBehaviour
{

    public static GemGlowManager Instance;
    public  Image gem;

    public Material ogMat,ownMat;

    public bool glowing;

    public Color glowCol;
    public float pulse, tint;
    
    
    // Start is called before the first frame update
    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }

        InitGlow();
    }

    public void SetCol(Color aCol)
    {
        glowCol = aCol;

        ownMat.SetColor("_GlowCol", glowCol);

    }

    public void InitGlow()
    {
        ownMat= new Material(ogMat);
        ownMat.SetFloat("_FrontGlowRange",0);
        ownMat.SetFloat("_TintVal",0);
        ownMat.SetColor("_GlowCol", glowCol);

        gem.material = ownMat;
        TurnGlow(glowing);
    }

    public void TurnGlow(bool on)
    {
        ownMat.SetFloat("_FrontGlowRange",on?pulse:0);
        ownMat.SetFloat("_TintVal",on?tint:0);
    }
}
