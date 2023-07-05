using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SmartMessageQueue
{
    public List<SmartMessage> smartMessageList = new List<SmartMessage>();
    
    public void AddMessage(string _sender, Byte[] _message)
    {
        smartMessageList.Add(new SmartMessage { sender = _sender, message = _message });
        
        Checklist();
    }
    public Byte[] DequeueMessage()
    {
        
        if (smartMessageList.Count >= 1)
        {
            Byte[] dequeuedMessage = smartMessageList[0].message;
            smartMessageList.RemoveAt(0);
            return dequeuedMessage;
        }
        return null;
    }
    public void Checklist()
    {
        for(int tries = 0; tries < 4; tries++)
        {
            if (CheckListIterate())
                break;
        }
        
    }
    bool CheckListIterate()
    {
        //List<SmartMessage> deleteList = new List<SmartMessage>();
        SmartMessage One;
        SmartMessage Two;
        for(int i = 0; i < smartMessageList.Count; i++)
        {
            for (int j = i; j < smartMessageList.Count; j++)
            {
                if(i != j)
                {
                    if(CheckForTrustMessage(smartMessageList[i], smartMessageList[j]))
                    {
                        One = smartMessageList[i];
                        Two = smartMessageList[j];
                        smartMessageList.Remove(One);
                        smartMessageList.Remove(Two);
                        DebugText.LogMessage("destroyed 2 messages");
                        return false;
                    }
                }
            }
        }
        
        return true;
    }

    bool CheckForTrustMessage(SmartMessage One, SmartMessage Two)
    {
        if(BitConverter.ToInt16(One.message, 0) == 1 && BitConverter.ToInt16(Two.message, 0) == 1)
        {
            //check if the target is the same
            if (BitConverter.ToInt32(One.message, 2) ==BitConverter.ToInt32(Two.message, 2))
            {
                //check if the sender is the same
                if (BitConverter.ToInt32(One.message, 6) == BitConverter.ToInt32(Two.message, 6))
                {
                    //check if the values contradict each other
                    if (BitConverter.ToInt32(One.message, 10) != BitConverter.ToInt32(Two.message, 10))
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }
}

public struct SmartMessage
{
    public string sender;
    public Byte[] message;
}
