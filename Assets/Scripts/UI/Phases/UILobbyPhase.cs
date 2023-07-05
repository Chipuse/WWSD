using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UILobbyPhase : UIPhaseManagerParent
{

    public string globalNetworkName;

    public Animator anim;

    public Vector3 ownCandlePos;

    public GameObject lobby,logo;


    void Start()
    {
        
        DeleventSystem.ChangeGameState += IfClientEnd;

        DeleventSystem.OnQRCodeScanned += ScannedPentagram;
        DeleventSystem.SetLocalCandlePosition += SetCandlePos;
    }

    public void DisableLogo()
    {
        logo.SetActive(false);
    }
    public override void StartPhase()
    {
        anim.SetTrigger("phaseStart");
        base.StartPhase();
    }

    public void StartGame()
    {

        DissolveManager.Instance.SetUpDissolve(lobby);
        anim.SetTrigger("submit");
        GameLoopTriggers.Instance.LobbyPhaseStartGame();
   
    }
    
    
    

    public void HostJoinButton(bool host)
    {
        PlayerInfoManager.Instance.host = host;
        if (host)
        {
            GameLoopTriggers.Instance.LobbyPhaseStartServer(globalNetworkName,PlayerInfoManager.Instance.ownName);
        }
        else
        {
            GameLoopTriggers.Instance.LobbyPhaseStartClient(globalNetworkName,PlayerInfoManager.Instance.ownName);

        }
        anim.SetTrigger("joinHost");
        FadeController.FogFade.Fade(false);

    }


    public void ScannedPentagram()
    {

        GemGlowManager.Instance.TurnGlow(true);
        anim.SetTrigger("scanned");
    }

    public void PlacedCancle()
    {
        GemGlowManager.Instance.TurnGlow(true);

        anim.SetFloat("IsOKSpeed",-1);
        DeleventSystem.OnCandlePlacementCanceled();


    }
    
    public void PlacedCandle()
    {
        anim.SetFloat("IsOKSpeed",1);
        GemGlowManager.Instance.TurnGlow(false);

        anim.SetTrigger("placed");
        DeleventSystem.OnCandlePlacePreview();

        

    }
    
    public void PlacedCandleSubmit()
    {
        anim.SetTrigger("placeSubmit");
        DeleventSystem.OnCandlePlacementSubmit();


    }

    public void SetIsOKSpeedToZero()
    {
        anim.SetFloat("IsOKSpeed",0);

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            ScannedPentagram();
        }
    }


    public void CheckIfHost()
    {
        if (PlayerInfoManager.Instance.host)
        {
            anim.SetTrigger("showList");
            LobbyManager.Instance.RefreshLobby(Votingsystem.Instance.sessionParticipants);
        }
    }
    public override void EndPhase()
    {
        
        PlayerInfoManager.Instance.CreatePlayerInfoArray(Votingsystem.Instance.sessionParticipants);
        PlayerInfoManager.Instance.numberOfPlayers = PlayerInfoManager.Instance.playerInfos.Length;
        PlayerInfoManager.Instance.ownID = Votingsystem.Instance.identity.GetPlayerId();
        GemGlowManager.Instance.SetCol(PlayerInfoManager.Instance.iconColors[PlayerInfoManager.Instance.ownID]);
        GameLoopTriggers.Instance.RolePhasePlaceCandle(ownCandlePos);
        base.EndPhase();
    }

    public void IfClientEnd(int phase)
    {
        if (phase == 1)
        {
            EndPhase();
        }
    }



    public void SetCandlePos(Vector3 pos)
    {
        ownCandlePos = pos;
    }




}
