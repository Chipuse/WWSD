using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiManager : MonoBehaviour
{
    public Votingsystem vSystem;

    public Text roleText;

    public GameObject screenLobbyPhase;
    public GameObject screenRolePhase;
    public GameObject screenVotingPhase;
    public GameObject screenVesselPhase;
    public GameObject screenEndPhase;
    public GameObject screenWitchesHaveWon;
    public GameObject screenSatanistsHaveWon;



    public Button trustColorScheme;
    public Button distrustColorScheme;

    private GameObject[] screensForPhases;

    //public GameObject[] UiElementsLobbyPhase;
    //public GameObject[] UiElementsRolePhase;
    //public GameObject[] UiElementsVotingPhase;
    //public GameObject[] UiElementsVesselPhase;
    //public GameObject[] UiElementsEndPhase;
    // Start is called before the first frame update

    //Elements LobbyPhase
    public GameObject serverLobbyPhase;
    public GameObject clientLobbyPhase;
    public GameObject startServerLobbyPhase;
    public GameObject startClientLobbyPhase;
    public GameObject startGameLobbyPhase;
    public void LobbyInit()
    {
        startGameLobbyPhase.SetActive(false);
    }

    void ChangeButtonsInLobbyServer(bool success)
    {
        if (success)
        {
            startServerLobbyPhase.SetActive(false);
            startClientLobbyPhase.SetActive(false);
            startGameLobbyPhase.SetActive(true);
        }
    }
    void ChangeButtonsInLobbyClient(bool success)
    {
        if (success)
        {
            startServerLobbyPhase.SetActive(false);
            startClientLobbyPhase.SetActive(false);
            startGameLobbyPhase.SetActive(false);
        }
    }
    
    //Elements RolePhase
    public GameObject submitRolePhase;
    public void RoleInit()
    {
        //non ar route
        //submitRolePhase.SetActive(true);
        submitRolePhase.SetActive(false);
        string rolePhaseMessage = "";
        if (vSystem.identity.GetRole() == 2)
        {
            rolePhaseMessage += "You are a Satanist";
            foreach (var item in vSystem.sessionParticipants)
            {
                if(item.GetRole() == 2 && item != vSystem.identity)
                {
                    
                    rolePhaseMessage += "\r\n" + item.GetName() + " is also a satanist";
                }
                else if (item.GetRole() == 3 && item != vSystem.identity)
                {
                   
                    rolePhaseMessage += "\r\n" + item.GetName() + " is possessed";
                }
            }
        }
        
        else
        {            
            rolePhaseMessage += "You are a Witch";
        }
        if (roleText != null)
            roleText.text = rolePhaseMessage;
        DeleventSystem.DisplayMessageEvent(rolePhaseMessage);

    }

    public void ChangeButtonInRolePhase()
    {
        submitRolePhase.SetActive(false);
    }

    public void PlaceCandle( Vector3 position)
    {
        if(vSystem.identity != null)
            vSystem.AddMessageToBuffer(vSystem.encoder.CandleHolderTransform(position, vSystem.identity.GetPlayerId()));
        submitRolePhase.SetActive(true);
    }

    //Elements VotingPhase
    public GameObject trustButtonPlayer1;
    public GameObject trustButtonPlayer2;
    public GameObject trustButtonPlayer3;
    public GameObject trustButtonPlayer4;
    public GameObject trustButtonPlayer5;
    public GameObject submitVotingPhase;

    private void ResetTrustValues()
    {
        foreach (var participant in vSystem.sessionParticipants)
        {
            for (int i = 0; i < participant.trustGiven.Length; i++)
            {
                if(participant.GetPlayerId() != i)
                    participant.trustGiven[i] = 1;
            }
            for (int j = 0; j < participant.trust.Length; j++)
            {
                if (participant.GetPlayerId() != j)
                    participant.trust[j] = 1;
            }
        }
    }
    public void VotingInit()
    {
        trustButtonPlayer1.SetActive(true);
        trustButtonPlayer2.SetActive(true);
        trustButtonPlayer3.SetActive(true);
        trustButtonPlayer4.SetActive(true);
        trustButtonPlayer5.SetActive(true);
        switch (vSystem.identity.GetPlayerId())
        {
            case 0:
                trustButtonPlayer1.SetActive(false);
                break;
            case 1:
                trustButtonPlayer2.SetActive(false);
                break;
            case 2:
                trustButtonPlayer3.SetActive(false);
                break;
            case 3:
                trustButtonPlayer4.SetActive(false);
                break;
            case 4:
                trustButtonPlayer5.SetActive(false);
                break;

            default:
                break;
        }
        submitRolePhase.SetActive(true);
        UpdateVotingButtons();
    }
    private bool crRunning = false;
    public void OnButtonSubmitVotingPhase()
    {
        if (vSystem.identity.GetfinishedPhase())
        {
            VotingInit();
            
            vSystem.SubmitForPhase(false);
            //StopCoroutine("DoOrientationCheck");
        }
        else
        {
            if (crRunning)
            {
                VotingInit();
                StopCoroutine("DoOrientationCheck");
                crRunning = false;
            }
            else
            {
                trustButtonPlayer1.SetActive(false);
                trustButtonPlayer2.SetActive(false);
                trustButtonPlayer3.SetActive(false);
                trustButtonPlayer4.SetActive(false);
                trustButtonPlayer5.SetActive(false);
                //vSystem.SubmitForPhase(true);
                StartCoroutine("DoOrientationCheck");
            }
            
        }
    }
    IEnumerator DoOrientationCheck()
    {
        float restingCounter = 0;
        float restingSeconds = 1;
        float xthOfASecond = 0.1f;
        crRunning = true;
        while (restingCounter < restingSeconds )
        {
            //ProximityCheck();
            if (MobileDeviceOrientations.GetLyingDownGlobal())
                restingCounter += xthOfASecond;
            else
                restingCounter = 0;
            yield return new WaitForSeconds(xthOfASecond);
        }
        crRunning = false;
        vSystem.SubmitForPhase(true);
    }


    public void UpdateVotingButtonsOnSubmit(int playerIdSender, int phase, bool voteFinished)
    {
        if (vSystem.gameState == vSystem.votingPhase)
        {
            UpdateVotingButtons();
        }
    }
    public void UpdateVotingButtons(int targetPlayer, int senderPlayer, int trustvalue) { UpdateVotingButtons(); }
    public void UpdateVotingButtons()
    {
        //Button1
        if (vSystem.sessionParticipants.Count >= 1)
        {
            NetworkParticipant pOne = vSystem.GetParticipantById(0);
            int pOneTrust = 0;
            foreach (var item in pOne.trust)
                pOneTrust = pOneTrust + item;            
            if (pOne.GetfinishedPhase() != false )            
                trustButtonPlayer1.GetComponentInChildren<Text>().text = pOne.GetName() + "\r\n --";            
            else            
                trustButtonPlayer1.GetComponentInChildren<Text>().text = pOne.GetName() + "\r\n"+ pOneTrust.ToString();
            trustButtonPlayer1.GetComponentInChildren<Text>().text = pOne.GetName();

            if (vSystem.identity.trustGiven[0] == 0)            
                trustButtonPlayer1.GetComponent<Button>().colors = distrustColorScheme.colors;
            else
                trustButtonPlayer1.GetComponent<Button>().colors = trustColorScheme.colors;

            DeleventSystem.LitPlayersCandle(0, pOneTrust);

            //Button2
            if (vSystem.sessionParticipants.Count >= 2)
            {
                pOne = vSystem.GetParticipantById(1);
                pOneTrust = 0;
                foreach (var item in pOne.trust)
                    pOneTrust = pOneTrust + item;
                if (pOne.GetfinishedPhase() != false)
                    trustButtonPlayer2.GetComponentInChildren<Text>().text = pOne.GetName() + "\r\n --";
                else
                    trustButtonPlayer2.GetComponentInChildren<Text>().text = pOne.GetName() + "\r\n" + pOneTrust.ToString();
                //trustButtonPlayer2.GetComponentInChildren<Text>().text = pOne.GetName();

                if (vSystem.identity.trustGiven[1] == 0)
                    trustButtonPlayer2.GetComponent<Button>().colors = distrustColorScheme.colors;
                else
                    trustButtonPlayer2.GetComponent<Button>().colors = trustColorScheme.colors;

                DeleventSystem.LitPlayersCandle(1, pOneTrust);

                //Button3
                if (vSystem.sessionParticipants.Count >= 3)
                {
                    pOne = vSystem.GetParticipantById(2);
                    pOneTrust = 0;
                    foreach (var item in pOne.trust)
                        pOneTrust = pOneTrust + item;
                    if (pOne.GetfinishedPhase() != false)
                        trustButtonPlayer3.GetComponentInChildren<Text>().text = pOne.GetName() + "\r\n --";
                    else
                        trustButtonPlayer3.GetComponentInChildren<Text>().text = pOne.GetName() + "\r\n" + pOneTrust.ToString();
                    //trustButtonPlayer3.GetComponentInChildren<Text>().text = pOne.GetName();

                    if (vSystem.identity.trustGiven[2] == 0)
                        trustButtonPlayer3.GetComponent<Button>().colors = distrustColorScheme.colors;
                    else
                        trustButtonPlayer3.GetComponent<Button>().colors = trustColorScheme.colors;

                    DeleventSystem.LitPlayersCandle(2, pOneTrust);

                    //Button4
                    if (vSystem.sessionParticipants.Count >= 4)
                    {
                        pOne = vSystem.GetParticipantById(3);
                        pOneTrust = 0;
                        foreach (var item in pOne.trust)
                            pOneTrust = pOneTrust + item;
                        if (pOne.GetfinishedPhase() != false)
                            trustButtonPlayer4.GetComponentInChildren<Text>().text = pOne.GetName() + "\r\n --";
                        else
                            trustButtonPlayer4.GetComponentInChildren<Text>().text = pOne.GetName() + "\r\n" + pOneTrust.ToString();
                        //trustButtonPlayer4.GetComponentInChildren<Text>().text = pOne.GetName();

                        if (vSystem.identity.trustGiven[3] == 0)
                            trustButtonPlayer4.GetComponent<Button>().colors = distrustColorScheme.colors;
                        else
                            trustButtonPlayer4.GetComponent<Button>().colors = trustColorScheme.colors;

                        DeleventSystem.LitPlayersCandle(3, pOneTrust);

                        //Button5
                        if (vSystem.sessionParticipants.Count >= 5)
                        {
                            pOne = vSystem.GetParticipantById(4);
                            pOneTrust = 0;
                            foreach (var item in pOne.trust)
                                pOneTrust = pOneTrust + item;
                            if (pOne.GetfinishedPhase() != false)
                                trustButtonPlayer5.GetComponentInChildren<Text>().text = pOne.GetName() + "\r\n --";
                            else
                                trustButtonPlayer5.GetComponentInChildren<Text>().text = pOne.GetName() + "\r\n" + pOneTrust.ToString();
                            //trustButtonPlayer5.GetComponentInChildren<Text>().text = pOne.GetName();

                            if (vSystem.identity.trustGiven[4] == 0)
                                trustButtonPlayer5.GetComponent<Button>().colors = distrustColorScheme.colors;
                            else
                                trustButtonPlayer5.GetComponent<Button>().colors = trustColorScheme.colors;

                            DeleventSystem.LitPlayersCandle(4, pOneTrust);
                        }
                    }
                }
            }
        }
    }

    public void OnButtonTrustChange(int trustReceiver)
    {
        if(vSystem.identity.trustGiven[trustReceiver] == 0)
        {
            vSystem.identity.trustGiven[trustReceiver] = 1;
            vSystem.GetParticipantById(trustReceiver).trust[vSystem.identity.GetPlayerId()] = 1;
            vSystem.AddMessageToBuffer(vSystem.encoder.SendTrust(trustReceiver, vSystem.identity.GetPlayerId(), 1));
            
        }
        else
        {
            vSystem.identity.trustGiven[trustReceiver] = 0;
            vSystem.GetParticipantById(trustReceiver).trust[vSystem.identity.GetPlayerId()] = 0;
            vSystem.AddMessageToBuffer(vSystem.encoder.SendTrust(trustReceiver, vSystem.identity.GetPlayerId(), 0));
        }
        UpdateVotingButtons();
    }

    //Elements VesselPhase
    public GameObject checkRoleVesselPhase;
    public GameObject exorcismVesselPhase;
    public GameObject checkButtonPlayer1;
    public GameObject checkButtonPlayer2;
    public GameObject checkButtonPlayer3;
    public GameObject checkButtonPlayer4;
    public GameObject checkButtonPlayer5;
    public GameObject submitVesselPhase;
    public GameObject swapActionVesselPhase;
    public GameObject submitDecisionVesselPhase;

    //exorcism = 1
    //chechinkg = 2
    private int exorcismOrChecking = -1;

    private int playerToPerformActionOn = -1;

    public void VesselInit()
    {
        exorcismOrChecking = -1;
        playerToPerformActionOn = -1;
        TargetPlayerButtonsColorsVessel();

        if (vSystem.identity.GetVessel() == true)
        {
            checkRoleVesselPhase.SetActive(true);
            exorcismVesselPhase.SetActive(true);
            checkButtonPlayer1.SetActive(false);
            checkButtonPlayer2.SetActive(false);
            checkButtonPlayer3.SetActive(false);
            checkButtonPlayer4.SetActive(false);
            checkButtonPlayer5.SetActive(false);
            submitVesselPhase.SetActive(false);
            submitDecisionVesselPhase.SetActive(false);
            swapActionVesselPhase.SetActive(false);
        }
        else
        {
            checkRoleVesselPhase.SetActive(false);
            exorcismVesselPhase.SetActive(false);
            checkButtonPlayer1.SetActive(false);
            checkButtonPlayer2.SetActive(false);
            checkButtonPlayer3.SetActive(false);
            checkButtonPlayer4.SetActive(false);
            checkButtonPlayer5.SetActive(false);
            submitVesselPhase.SetActive(false);
            submitDecisionVesselPhase.SetActive(false);
            swapActionVesselPhase.SetActive(false);
            //players who are not chosen as vessel are directly ready for the next phase
            string nameOfVessel = "";
            foreach (var item in vSystem.sessionParticipants)
            {
                if (item.GetVessel())
                    nameOfVessel = item.GetName();
            }
            DeleventSystem.DisplayMessageEvent(nameOfVessel + " is the current supreme witch");
            vSystem.SubmitForPhase(true);
        }
    }

    public void OnButtonExorcismOrChecking(int decision)
    {
        exorcismOrChecking = decision;
        checkRoleVesselPhase.SetActive(false);
        exorcismVesselPhase.SetActive(false);
        if (false)
        {

        }
        /*
         * Depricated GameLoopFunctionality
        if(exorcismOrChecking == 1 && vSystem.exorcismUsed == true)
        {
            //vessel sees that exorcism was already used
            DeleventSystem.DisplayMessageEvent("Exorcism was already used this MoonCycle");
            vSystem.AddMessageToBuffer(vSystem.encoder.VesselAction(exorcismOrChecking, -1));
            submitVesselPhase.SetActive(true);
        }
        */
        else
        {
            checkButtonPlayer1.SetActive(true);

            checkButtonPlayer2.SetActive(true);
            checkButtonPlayer3.SetActive(true);
            checkButtonPlayer4.SetActive(true);
            checkButtonPlayer5.SetActive(true);
            submitDecisionVesselPhase.SetActive(true);
            swapActionVesselPhase.SetActive(true);
            TargetPlayerButtonsColorsVessel();

            int playercount = vSystem.sessionParticipants.Count;
            if(playercount >= 1)
            {
                checkButtonPlayer1.GetComponentInChildren<Text>().text = vSystem.GetParticipantById(0).GetName();
            }
            if (playercount >= 2)
            {
                checkButtonPlayer2.GetComponentInChildren<Text>().text = vSystem.GetParticipantById(1).GetName();
            }
            if (playercount >= 3)
            {
                checkButtonPlayer3.GetComponentInChildren<Text>().text = vSystem.GetParticipantById(2).GetName();
            }
            if (playercount >= 4)
            {
                checkButtonPlayer4.GetComponentInChildren<Text>().text = vSystem.GetParticipantById(3).GetName();
            }
            if (playercount >= 5)
            {
                checkButtonPlayer5.GetComponentInChildren<Text>().text = vSystem.GetParticipantById(4).GetName();
            }
            
            switch (vSystem.identity.GetPlayerId())
            {
                case 0:
                    checkButtonPlayer1.SetActive(false);
                    break;
                case 1:
                    checkButtonPlayer2.SetActive(false);
                    break;
                case 2:
                    checkButtonPlayer3.SetActive(false);
                    break;
                case 3:
                    checkButtonPlayer4.SetActive(false);
                    break;
                case 4:
                    checkButtonPlayer5.SetActive(false);
                    break;

                default:
                    break;
            }
            if(exorcismOrChecking == 1)
            {
                DeleventSystem.DisplayMessageEvent("EXORCISM\r\nChoose a Player");
            }
            else if(exorcismOrChecking == 2)
            {
                DeleventSystem.DisplayMessageEvent("ROLE CHECK\r\nChoose a Player");
            }
        }
    }

    public void OnButtonVesselActionTarget(int targetPlayerId)
    {
        playerToPerformActionOn = targetPlayerId;
        TargetPlayerButtonsColorsVessel();        
    }

    public void OnButtonSwapActionVessel()
    {
        if(exorcismOrChecking == 1)
        {
            exorcismOrChecking = 2;
            DeleventSystem.DisplayMessageEvent("ROLE CHECK\r\nChoose a Player");
            TargetPlayerButtonsColorsVessel();
        }
        else if (exorcismOrChecking == 2)
        {
            exorcismOrChecking = 1;
            DeleventSystem.DisplayMessageEvent("EXORCISM\r\nChoose a Player");
            TargetPlayerButtonsColorsVessel();
        }
    }

    public void OnButtonSubmitDecisionVessel()
    {
        checkButtonPlayer1.SetActive(false);
        checkButtonPlayer2.SetActive(false);
        checkButtonPlayer3.SetActive(false);
        checkButtonPlayer4.SetActive(false);
        checkButtonPlayer5.SetActive(false);
        submitDecisionVesselPhase.SetActive(false);
        swapActionVesselPhase.SetActive(false);
        submitVesselPhase.SetActive(true);
        if (vSystem.exorcismUsed == true)
        {
            //vessel sees that exorcism was already used
            vSystem.AddMessageToBuffer(vSystem.encoder.VesselAction(exorcismOrChecking, -1));
            //vSystem.AddMessageToBuffer(vSystem.encoder.VesselAction(exorcismOrChecking, -1));
            //submitVesselPhase.SetActive(true);
        }
        else
        {
            //might get relevant for clients aswell as soon as the client does not get his own message back again
            if (vSystem.isServer)
            {
                vSystem.EvaluatVesselAction(exorcismOrChecking, playerToPerformActionOn);
            }

            vSystem.AddMessageToBuffer(vSystem.encoder.VesselAction(exorcismOrChecking, playerToPerformActionOn));
            if (vSystem.sessionParticipants.Count > playerToPerformActionOn)
            {
                NetworkParticipant target = vSystem.GetParticipantById(playerToPerformActionOn);
                if (exorcismOrChecking == 1)
                {
                    if (target.GetRole() == 3)
                    {
                        DeleventSystem.DisplayMessageEvent("Exorcism");
                        //DeleventSystem.DisplayMessageEvent("This Players was posessed by Satan\r\n");
                    }
                    else
                    {
                        DeleventSystem.DisplayMessageEvent("Exorcism");
                        //DeleventSystem.DisplayMessageEvent(target.GetName() + " is NOT posessed by Satan\r\n");
                    }
                }
                else
                {
                    if (vSystem.identity.GetRole() == 3)
                    {
                        //False Information for Satan
                        if (target.GetRole() == 1)
                        {
                            DeleventSystem.DisplayMessageEvent(target.GetName() + " is a\r\nSATANIST");
                        }
                        else
                        {
                            DeleventSystem.DisplayMessageEvent(target.GetName() + " is a\r\nWITCH");
                        }
                    }
                    else
                    {
                        if (target.GetRole() == 2)
                        {
                            DeleventSystem.DisplayMessageEvent(target.GetName() + " is a\r\nSATANIST");
                        }
                        else
                        {
                            DeleventSystem.DisplayMessageEvent(target.GetName() + " is a\r\nWITCH");
                        }
                    }
                }

            }
            else
            {
                DeleventSystem.DisplayMessageEvent("Sorry\r\nThis player is in another realm of reality");
            }
        }
            
    }

    public void OnButtonSubmitVessel()
    {
        vSystem.SubmitForPhase(true);
    }

    private void TargetPlayerButtonsColorsVessel()
    {
        if (playerToPerformActionOn == -1)
        {
            submitDecisionVesselPhase.SetActive(false);
        }
        else
        {
            submitDecisionVesselPhase.SetActive(true);
        }
        
        checkButtonPlayer1.GetComponent<Button>().colors = submitDecisionVesselPhase.GetComponent<Button>().colors;
        checkButtonPlayer2.GetComponent<Button>().colors = submitDecisionVesselPhase.GetComponent<Button>().colors;
        checkButtonPlayer3.GetComponent<Button>().colors = submitDecisionVesselPhase.GetComponent<Button>().colors;
        checkButtonPlayer4.GetComponent<Button>().colors = submitDecisionVesselPhase.GetComponent<Button>().colors;
        checkButtonPlayer5.GetComponent<Button>().colors = submitDecisionVesselPhase.GetComponent<Button>().colors;
        
        if (playerToPerformActionOn == 0)
        {
            checkButtonPlayer1.GetComponent<Button>().colors = trustColorScheme.colors;
        }
        else if (playerToPerformActionOn == 1)
        {
            checkButtonPlayer2.GetComponent<Button>().colors = trustColorScheme.colors;
        }
        else if(playerToPerformActionOn == 2)
        {
            checkButtonPlayer3.GetComponent<Button>().colors = trustColorScheme.colors;
        }
        else if(playerToPerformActionOn == 3)
        {
            checkButtonPlayer4.GetComponent<Button>().colors = trustColorScheme.colors;
        }
        else if(playerToPerformActionOn == 4)
        {
            checkButtonPlayer5.GetComponent<Button>().colors = trustColorScheme.colors;
        }
        
    }

    //Elements EndPhase


    void Start()
    {
        screensForPhases = new GameObject[]
        {
            screenLobbyPhase,
            screenRolePhase,
            screenVotingPhase,
            screenVesselPhase,
            screenEndPhase,
            screenWitchesHaveWon,
            screenSatanistsHaveWon
        };

        
        DeleventSystem.ChangeGameState += LoadNewPage;
        DeleventSystem.StartingClient += ChangeButtonsInLobbyClient;
        DeleventSystem.StartingServer += ChangeButtonsInLobbyServer;
        DeleventSystem.CandlePlacedLocalPlayer += PlaceCandle;
        DeleventSystem.ChangePlayerTrust += UpdateVotingButtons;
        DeleventSystem.SubmitForPhase += UpdateVotingButtonsOnSubmit;

        //StartCoroutine("DoOrientationCheck");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void LoadNewPage(int newPage)
    {
        //DeleventSystem.DisplayMessageEvent("Round " + (vSystem.roundCounter + 1).ToString() + "\r\n"+ "Moon " + ((vSystem.roundCounter+1) / 3 + 1));
        vSystem.gameState = newPage;
        foreach (var item in screensForPhases)
        {
            item.SetActive(false);
        }
        switch (newPage)
        {
            case 0:
                screensForPhases[newPage].SetActive(true);
                LobbyInit();
                break;
            case 1:
                screensForPhases[newPage].SetActive(true);
                RoleInit();
                break;
            case 2:
                screensForPhases[newPage].SetActive(true);
                ResetTrustValues();
                DeleventSystem.DisplayMessageEvent("Round " + (vSystem.roundCounter + 1).ToString() +" out of 9" + "\r\n" + "Moon " + ((vSystem.roundCounter ) / 3 + 1)+"\r\n"+ vSystem.exorcisedEndOfMoonList);
                VotingInit();
                break;
            case 3:
                screensForPhases[newPage].SetActive(true);
                DeleventSystem.DisplayMessageEvent("Round " + (vSystem.roundCounter + 1).ToString() + " out of 9" + "\r\n" + "Moon " + ((vSystem.roundCounter ) / 3 + 1) + "\r\n" + vSystem.exorcisedEndOfMoonList);
                VesselInit();
                break;
            case 4:
                screensForPhases[newPage].SetActive(true);
                DeleventSystem.DisplayMessageEvent("Round " + (vSystem.roundCounter + 1).ToString() + " out of 9" + "\r\n" + "Moon " + ((vSystem.roundCounter ) / 3 + 1) + "\r\n" + vSystem.exorcisedEndOfMoonList);
                break;
            case 5:
                screensForPhases[newPage].SetActive(true);
                break;
            case 6:
                screensForPhases[newPage].SetActive(true);
                break;
            case 7:
                screensForPhases[newPage].SetActive(true);
                break;
            default:
                break;
        }
    }

    

}
