using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmountTrustDisplayAR : MonoBehaviour
{
    [SerializeField]
    private Camera arCamera;

    // Update is called once per frame
    void Update()
    {
        if(Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            
            if(touch.phase == TouchPhase.Began)
            {
                Ray ray = arCamera.ScreenPointToRay(touch.position);
                RaycastHit hitObject;
                if(Physics.Raycast(ray, out hitObject))
                {
                    if(hitObject.transform.tag == "Candle"){
                        hitObject.transform.gameObject.GetComponent<FireManager>().ShowTrust();
                    }
                }
            }
        }



    }


}
