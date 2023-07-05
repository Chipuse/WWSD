using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DissolveManager : MonoBehaviour
{
    public static DissolveManager Instance;

    public GameObject parentCanvas, dissolveImagePrefab, dissolveImage, dissolvingObject,  copiedObject;
    public float canvasSize;
    public float timer;
    public float endTime;
    private bool count;


    // Start is called before the first frame update
    void Start()
    {
        timer = 0;
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (count)
        {

            timer += Time.deltaTime;
            if (timer >= endTime)
            {
                EndDissolve();
            }
        }
    }


    private void EndDissolve()
    {

        timer = 0;
        count = false;
        Destroy(dissolveImage);
        Destroy(copiedObject);
    }

    public void SetUpDissolve(GameObject obj)
    {
        dissolvingObject = obj;
        copiedObject = Instantiate(obj,parentCanvas.transform);
        copiedObject.GetComponent<RectTransform>().localPosition=new Vector3(0,0);
        float scaleFactor = obj.GetComponent<RectTransform>().localScale.x;
        float copySizeX = canvasSize/obj.GetComponent<RectTransform>().rect.width;
        float copySizeY = canvasSize/obj.GetComponent<RectTransform>().rect.height;
        float ratio = copySizeX / copySizeY;
        copiedObject.GetComponent<RectTransform>().localScale=new Vector3(copySizeX,copySizeY,1);
        copiedObject.GetComponent<RectTransform>().rotation=new Quaternion();
        dissolveImage = Instantiate(dissolveImagePrefab,obj.transform.parent);
        Quaternion objQuat = obj.GetComponent<RectTransform>().rotation;
        dissolveImage.GetComponent<RectTransform>().localPosition = obj.transform.localPosition;
        dissolveImage.GetComponent<RectTransform>().sizeDelta= 
            new Vector2(obj.GetComponent<RectTransform>().rect.width*scaleFactor,
            obj.GetComponent<RectTransform>().rect.height*scaleFactor);
        dissolveImage.GetComponent<RectTransform>().rotation = objQuat;
        
        Vector2 center =new Vector2(0.5f,0.5f);
        if (obj.GetComponent<DissolveShader>() != null)
        {
            center = obj.GetComponent<DissolveShader>().centerPoint;
        }
        dissolveImage.GetComponent<DissolveShader>().StartDissolve(obj.GetComponent<Image>().mainTexture,center,ratio);
        obj.SetActive(false);
        count = true;

    }
}
