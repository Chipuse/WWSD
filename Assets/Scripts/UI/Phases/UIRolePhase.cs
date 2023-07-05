using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIRolePhase : UIPhaseManagerParent
{
    public Animator anim;


    public GameObject[] witchInfo;
    public GameObject[] satanistInfo;
    private bool isSatanist;
    public int[] otherSatanists;
    public int possessed;
    private PlayerInfo[] _playerInfos;
    public DisplayPlayerInfo[] satanistNames;
    public DisplayPlayerInfo possessedName;
    public GameObject moreThanTwoSatanists;
    public bool firstTime;


    // Start is called before the first frame update
    void Start()
    {
        if (firstTime)
        {
            SetEverything();
            firstTime = false;
        }
    }

 


    public override void StartPhase()
    {
        FadeController.FogFade.Fade(true);
        DeleventSystem.ChangeGameState += ReallyEnd;
        SetEverything();
    }


    private void SetEverything()
    {
        _playerInfos = PlayerInfoManager.Instance.playerInfos;
        moreThanTwoSatanists.gameObject.SetActive(false);

        CheckIfSatanist();
        if (isSatanist)
        {
            CheckOtherRoles();
            CastNames();
        }

        ActivateObjects();
        DisableObjects();
        firstTime = false;
    }

    private void CheckIfSatanist()
    {
        isSatanist = _playerInfos[PlayerInfoManager.Instance.ownID].PlayerRole == 2;

        objectsToActivate = isSatanist ? satanistInfo : witchInfo;
        objectsToDisable = isSatanist ? witchInfo : satanistInfo;
    }


    private void CheckOtherRoles()
    {
        if (_playerInfos.Length >= 5)
            otherSatanists = new int[(_playerInfos.Length - 1) / 2 - 1];
        int i = 0;
        foreach (PlayerInfo p in _playerInfos)
        {
            if (p.PlayerRole == 2 && p.PlayerID != PlayerInfoManager.Instance.ownID)
            {
                otherSatanists[i] = p.PlayerID;
                i++;
            }

            else if (p.PlayerRole == 3)
            {
                possessed = p.PlayerID;
            }
        }
    }

    private void CastNames()
    {
        if (otherSatanists.Length > 1)
        {
            moreThanTwoSatanists.SetActive(true);
            int i = 0;
            foreach (int sID in otherSatanists)
            {
                satanistNames[i].PrintPlayerName(sID);
                i++;
            }
        }
        else if (otherSatanists.Length > 0)
        {
            satanistNames[0].PrintPlayerName(otherSatanists[0]);
        }

        possessedName.PrintPlayerName(possessed);
    }


    public void SubmitRole()
    {
        //TODO network activate
        GameLoopTriggers.Instance.RolePhaseSubmit(true);
        anim.SetTrigger("submit");
    }


    //TODO need to implement

    public void SubmitCandlePlacement(Vector3 p)
    {
        GameLoopTriggers.Instance.RolePhasePlaceCandle(p);
    }

    private void ReallyEnd(int nextPhase)
    {
        if (nextPhase == 2)
        {
            EndPhase();
        }
    }
}