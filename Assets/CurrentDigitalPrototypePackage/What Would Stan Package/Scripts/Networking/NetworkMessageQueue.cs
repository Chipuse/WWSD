using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class NetworkMessageQueue
{
    
    public Queue functionsQueue;

    public bool IsEmpty
    {
        get
        {
            if (functionsQueue.Count == 0)
                return true;
            else
                return false;
        }
    }

    public NetworkMessageQueue()
    {
        functionsQueue = new Queue();
    }

    public bool Contains(Action action)
    {
        if (functionsQueue.Contains(action))
            return true;
        else
            return false;
    }

    public Action Pop()
    {
        return functionsQueue.Dequeue() as Action;
        
    }


    //networkMessageQueue.Add(() => Method2(classObject));
    public void Add(Action function)
    {
        functionsQueue.Enqueue(function);
    }
}
