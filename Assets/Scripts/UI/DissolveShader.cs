using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DissolveShader : MonoBehaviour
{
    private Material mat;
    public  Material ogMat;
    public Vector2 centerPoint;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void StartDissolve(Image i)
    {
        mat = new Material(ogMat);
        mat.SetTexture("_OGSprite",i.mainTexture);
        i.sprite = null;
        mat.SetFloat("_TimeSinceLoad", Time.time);
        i.material = mat;
        
    }
    
    private void StartDissolve(Text t)
    {
        mat = new Material(ogMat);
        mat.SetFloat("_TimeSinceLoad", Time.time);
        t.material = mat;
        
    }

    public void StartDissolve()
    {
        
        StartDissolve(GetComponent<Image>());

        foreach (Transform child in transform)
        {
            if (child.GetComponent<Image>() != null)
            {
                StartDissolve(child.GetComponent<Image>());
            }
            else
            {
                StartDissolve(child.GetComponent<Text>());
            }
        }
        
    }

    public void StartDissolve(Texture t, Vector2 center,float ratio)
    {
        mat = new Material(ogMat);
        mat.SetTexture("_OGSpriteMask",t);
        mat.SetFloat("_TilingRatio",ratio);
        mat.SetFloat("_TimeSinceLoad", Time.time);
        mat.SetFloat("_CenterPointX",center.x);
        mat.SetFloat("_CenterPointY",center.y);
        GetComponent<Image>().material = mat;
    }
    
    
}
