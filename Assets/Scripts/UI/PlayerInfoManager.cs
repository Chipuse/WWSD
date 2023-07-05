using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInfoManager : MonoBehaviour
{
    public Color inkBrown;
    public Color inkRed;

    public int numberOfPlayers;
    public int ownID;
    public bool host;

    public string ownName;
    
    public static PlayerInfoManager Instance;
    public Sprite[] icons;
    public Color[] iconColors;
    public Color[] iconColors2;
    public PlayerInfo[] playerInfos;
    public List<NetworkParticipant> testList = new List<NetworkParticipant>();
    public List<int> exoList;


    public GameObject testPaper;

    // Start is called before the first frame update
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }


        /*
        for (int i = 0; i < numberOfPlayers; i++)
        {
            testList.Add(new NetworkParticipant(i, "Player " + i));
        }

        
        
        Votingsystem.Instance.sessionParticipants = testList;
        Votingsystem.Instance.DistributeRoles();
        

        CreatePlayerInfoArray(Votingsystem.Instance.sessionParticipants);
        */
        
        
    }

  

    public void CreatePlayerInfoArray(List<NetworkParticipant> netpart)
    {
        playerInfos = new PlayerInfo[netpart.Count];

        foreach (NetworkParticipant participant in netpart)
        {
            int pID = participant.GetPlayerId();
            int pRole = participant.GetRole();
            string pName = participant.GetName();

            playerInfos[pID] = new PlayerInfo(pID, pRole, pName, pID);
        }
    }

    public void AssignButton(GameObject o, int pNum)
    {
        o.GetComponent<Image>().sprite = icons[playerInfos[pNum].UserIcon];
        o.GetComponent<Image>().color = inkBrown;
    }

    private void ReorderPlayerInfos()
    {
        PlayerInfo[] temp = new PlayerInfo[playerInfos.Length];

        int i = 0;
        int j = 0;
        bool startCount = false;
        while (i < temp.Length)
        {
            if (playerInfos[j].PlayerID == ownID)
            {
                startCount = true;
            }

            if (startCount)
            {
                temp[i] = playerInfos[i];
            }

            j++;
            if (j >= temp.Length)
            {
                j = 0;
            }
        }
    }
}


[Serializable]
public struct PlayerInfo
{
    private int playerID;

    public int PlayerID
    {
        get { return playerID; }
    }

    private int playerRole;

    public int PlayerRole
    {
        get { return playerRole; }
    }

    public string PlayerRoleString
    {
        get
        {
            if (playerRole == 1)
            {
                return "Witch";
            }

            if (playerRole == 2)
            {
                return "Satanist";
            }

            if (playerRole == 3)
            {
                return "Witch";
            }
            else
            {
                return "no teal role";
            }
        }
    }

    private String userName;

    public String UserName
    {
        get { return userName; }
    }

    private int userIcon;

    public int UserIcon
    {
        get { return userIcon; }
    }


    public PlayerInfo(int pID, int pRole, String uName, int uIcon)
    {
        playerID = pID;
        playerRole = pRole;
        userName = uName;
        userIcon = uIcon;
    }
}