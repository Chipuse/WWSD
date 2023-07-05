using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QueueTesting : MonoBehaviour
{
    NetworkMessageQueue Queue;
    ByteEncoder Coder;
    public float counter = 0;
    string test;
    // Start is called before the first frame update
    void Start()
    {
        Queue = new NetworkMessageQueue();
        Coder = new ByteEncoder();
        test = "Hello World";

        Debug.Log(Coder.SimpleStringDecode(Coder.SimpleString(test)));
    }

    // Update is called once per frame
    void Update()
    {
        if(counter >= 2 && !Queue.IsEmpty)
        {
            Queue.Pop()();
            counter = 0;
        }
           

        counter = counter + 1 * Time.deltaTime;
    }

    void TestFunction1()
    {
        Debug.Log("Function 1");
    }
    void TestFunction2()
    {
        Debug.Log("Function 2");
    }

    void TestFunction3(int input)
    {
        Debug.Log("TestFunction3" + input);
    }
    void TestFunction4(int input1, int input2)
    {
        Debug.Log("TestFunction3" + input1 + input2);
    }
}
