using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour
{
    // Start is called before the first frame update

    void Start()
    {
        // GameObject.Find("Capsule");
        // Vector3 dir =  GameObject.Find("Capsule").transform.position - this.transform.position;

        // this.transform.rotation = Quaternion.LookRotation(dir);
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 dir =  GameObject.Find("Capsule").transform.position - this.transform.position;

        this.transform.rotation = Quaternion.LookRotation(dir);
    }
}
