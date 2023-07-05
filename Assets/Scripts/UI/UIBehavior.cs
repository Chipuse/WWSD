using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIBehavior : MonoBehaviour
{
    public GameObject MoveObject;

    public float moveLength;

    public bool up;
    public Vector3 lastMousePos;
    public bool move;

    // Start is called before the first frame update
    void Start()
    {
        move = false;

    }

    // Update is called once per frame
    void Update()
    {
        if (move)
        {
            UpdateButtonPos();
        }

        
    }


     void MoveUP()
    {
        print("move up");

        MoveObject.transform.localPosition += new Vector3(0, moveLength);
    }
    

     void MoveDown()
    {
        
        print("move down");
        MoveObject.transform.localPosition+=new Vector3(0,-moveLength);
    }

    public void Move()
    {
        if (up)
        {
            MoveDown();
        }
        else
        {
            MoveUP();
        }

        up = !up;
    }

    public void Startmoving()
    {
        move = true;
    }


    public void UpdateButtonPos()
    {
        if (!Input.GetMouseButton(0))
        {
            print("false");

            move = false;
        }
        else
        {
            print("true");

            Vector3 diff = lastMousePos - Input.mousePosition;
            lastMousePos = Input.mousePosition;
            MoveObject.transform.localPosition += new Vector3(0, -diff.y);
        }

    }

}
