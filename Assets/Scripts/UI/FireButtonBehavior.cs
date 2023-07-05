using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireButtonBehavior : MonoBehaviour
{
    public bool fireOn;

    public GameObject FireObject;
    // Start is called before the first frame update
    void Start()
    {
        fireOn = true;
    }

    
    public void TurnFire()
    {
       
            FireObject.SetActive(!fireOn);
            fireOn = !fireOn;

    }
    
}
