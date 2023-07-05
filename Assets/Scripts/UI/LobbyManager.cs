using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyManager: MonoBehaviour
{
    public DisplayPlayerInfo[] participants;
    public static LobbyManager Instance;
    
    
    public int minPlayers;
    private bool startEnabled;

    public Animator anim;
    // Start is called before the first frame update
    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
        startEnabled = false;
        //RefreshLobby(PlayerInfoManager.Instance.testList);
        DeleventSystem.RefreshLobby += RefreshLobby;        DeleventSystem.OnClientReadyLobby += RefreshLobby;

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    


    public void RefreshLobby(List<NetworkParticipant> list)
    {
        


        PlayerInfoManager.Instance.CreatePlayerInfoArray(list);

        int length = PlayerInfoManager.Instance.playerInfos.Length;
        for(int i = 0; i<length;i++)
        {            if(i == 0)
            {
                participants[i].PrintPlayerName(i);

                participants[i].gameObject.GetComponent<FadeController>().Fade(true);
            }
            else if (list[i].GetfinishedPhase())
            {
                participants[i].PrintPlayerName(i);

                participants[i].gameObject.GetComponent<FadeController>().Fade(true);
            }




        }
        EnableStart(length);

        
    }

    public void EnableStart(int currentPlayerCount)
    {

        if (currentPlayerCount >= minPlayers&&!startEnabled)
        {
            anim.SetTrigger("startEnable");
            startEnabled = true;
        }
        
    }
    
    
}
