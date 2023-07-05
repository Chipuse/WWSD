using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayPlayerInfo : MonoBehaviour
{

    public string printBefore;
    public string printAfter;
    
    
    // Start is called before the first frame update
    void Start()
    {
        
        
        
    }

    public void PrintPlayerName(int playerID)
    {


        GetComponent<Text>().text =
            printBefore + PlayerInfoManager.Instance.playerInfos[playerID].UserName + printAfter;


    }
    
    public void PrintPlayerRole(int playerID)
    {


        GetComponent<Text>().text =
            printBefore + PlayerInfoManager.Instance.playerInfos[playerID].PlayerRoleString + printAfter;


    }


    public void PrintWrongRole(int playerID)
    {
        int pRole = PlayerInfoManager.Instance.playerInfos[playerID].PlayerRole;

        if (pRole == 2)
        {
            GetComponent<Text>().text =
                printBefore + "Witch" + printAfter;
        }
        else
        {
            GetComponent<Text>().text =
                printBefore + "Satanist" + printAfter;
        }

    }
    
    
}
