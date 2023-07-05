using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceTowardsPhone : MonoBehaviour
{
    Camera c;
    void Awake(){
        c = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        // transform.LookAt(transform.position + c.transform.rotation * Vector3.back, c.transform.rotation * Vector3.up);
            transform.LookAt(c.transform.position );
    }
}   
