using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIOraclePhase : UIPhaseManagerParent
{
    public int oracle;

    public GameObject spells,
        messages,
        isOracleMessage,
        isRoleMessage,
        exorciseMessage,
        youAreOracleMessage,
        endPhaseButton;

    public DisplayPlayerInfo nameOfOracle, nameOfChecked, role;

    public int selectedPlayer;

    // Start is called before the first frame update
    void Start()
    {
        DeleventSystem.ChangeGameState += ReallyEnd;
    }

    // Update is called once per frame
    void Update()
    {
    }


    private void GetOracle()
    {
        foreach (NetworkParticipant nP in Votingsystem.Instance.sessionParticipants)
        {
            if (nP.GetVessel())
            {
                oracle = nP.GetPlayerId();
            }
        }
    }


    public override void StartPhase()
    {
        UpdateExorcisedList();
        ActivateObjects();
        DisableObjects();
        GetOracle();
        if (oracle == PlayerInfoManager.Instance.ownID)
        {
            OracleStart();
        }
        else
        {
            NonOracleStart();
        }
    }

    private void OracleStart()
    {
        spells.SetActive(true);
        spells.GetComponent<SpellManager>().StartPhase();
        messages.SetActive(true);
        youAreOracleMessage.SetActive(true);
    }


    private void NonOracleStart()
    {
        messages.SetActive(true);
        isOracleMessage.SetActive(true);
        nameOfOracle.PrintPlayerName(oracle);
        DisplayIsOracle();
    }


    public void DisplayIsOracle()
    {
        isOracleMessage.SetActive(true);
        nameOfOracle.PrintPlayerName(oracle);
    }

    public void DisplayIsRole(int pID, bool isPossessed)
    {
        GameLoopTriggers.Instance.VesselPhaseAction(2, pID);

        messages.SetActive(true);
        // spells.SetActive(false);
        isRoleMessage.SetActive(true);
        nameOfChecked.PrintPlayerName(pID);

        if (isPossessed)
            role.PrintWrongRole(pID);
        else
            role.PrintPlayerRole(pID);
        endPhaseButton.SetActive(true);
    }


    public void DisplayExorcise(int pID)
    {
        GameLoopTriggers.Instance.VesselPhaseAction(1, pID);

        messages.SetActive(true);
        //spells.SetActive(false);
        exorciseMessage.SetActive(true);
        endPhaseButton.SetActive(true);
    }

    private void ReallyEnd(int nextPhase)
    {
        if (nextPhase == 2)
        {
            EndPhase();
        }
    }


    public void OracleReady()
    {
        GameLoopTriggers.Instance.VesselPhaseSubmit(true);
    }


    public void UpdateExorcisedList()
    {
        if (Votingsystem.Instance.roundCounter==3||Votingsystem.Instance.roundCounter==6)
        {
            PlayerInfoManager.Instance.exoList=new List<int>();

            int exoP = 0;
            foreach (int i in Votingsystem.Instance.exorcismTargetsId)
            {
                if (i > -1)
                {
                    exoP++;
                    PlayerInfoManager.Instance.exoList.Add(i);
                }
            }

        }
    }

    public override void EndPhase()
    {
        //TODO network activate
        // GameLoopTriggers.Instance.VesselPhaseSubmit(true);
        base.EndPhase();
    }
}