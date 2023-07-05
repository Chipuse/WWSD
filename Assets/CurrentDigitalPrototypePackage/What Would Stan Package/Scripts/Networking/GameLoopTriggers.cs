using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLoopTriggers : MonoBehaviour
{

    public static GameLoopTriggers Instance;
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            //DeleventSystem.ChangeGameState += ShowMeWhatPhaseItIs;
        }
    }
    
    void ShowMeWhatPhaseItIs(int nextPhase)
    {
    }
    //LobbyPhase Events

    public void LobbyPhaseStartGame()
    {
        Votingsystem.Instance.StartGame();
    }

    public void LobbyPhaseStartClient(string networkName, string clientName)
    {
        if (string.IsNullOrEmpty(networkName) || string.IsNullOrEmpty(clientName))
        {      
        }
        else
        {
            Votingsystem.Instance.StartClient(networkName, clientName);
        }
    }
    public void LobbyPhaseStartServer(string networkName, string serverName)
    {
        if (string.IsNullOrEmpty(networkName) || string.IsNullOrEmpty(serverName))
        {
        }
        else
        {
            Votingsystem.Instance.StartServer(networkName, serverName);
        }
    }

    //RolePhase Events
    public void RolePhaseSubmit(bool ready)
    {
         Votingsystem.Instance.SubmitForPhase(ready);
    }

    public void RolePhasePlaceCandle(Vector3 position)
    {
        if (Votingsystem.Instance.identity != null)
            Votingsystem.Instance.AddMessageToBuffer(Votingsystem.Instance.encoder.CandleHolderTransform(position, Votingsystem.Instance.identity.GetPlayerId()));      
       
    }


    //VotingPhase Events

    public void VotingPhaseButtonTrustChange(int targetPlayerId, bool trusting)
    {        
        if (trusting)
        {
            Votingsystem.Instance.identity.trustGiven[targetPlayerId] = 1;
            Votingsystem.Instance.GetParticipantById(targetPlayerId).trust[Votingsystem.Instance.identity.GetPlayerId()] = 1;
            Votingsystem.Instance.AddMessageToBuffer(Votingsystem.Instance.encoder.SendTrust(targetPlayerId, Votingsystem.Instance.identity.GetPlayerId(), 1));
        }
        else
        {
            Votingsystem.Instance.identity.trustGiven[targetPlayerId] = 0;
            Votingsystem.Instance.GetParticipantById(targetPlayerId).trust[Votingsystem.Instance.identity.GetPlayerId()] = 0;
            Votingsystem.Instance.AddMessageToBuffer(Votingsystem.Instance.encoder.SendTrust(targetPlayerId, Votingsystem.Instance.identity.GetPlayerId(), 0));
        }
    }

    private void UpdateCandlesInAr(int targetPlayer,int senderPlayer, int trustValue) { UpdateCandlesInAr(); }
    private void UpdateCandlesInAr()
    {
        //Player0
        if (Votingsystem.Instance.sessionParticipants.Count >= 1)
        {
            NetworkParticipant pOne = Votingsystem.Instance.GetParticipantById(0);
            int pOneTrust = 0;
            foreach (var item in pOne.trust)
                pOneTrust = pOneTrust + item;
            DeleventSystem.LitPlayersCandle(0, pOneTrust);

            //Player1
            if (Votingsystem.Instance.sessionParticipants.Count >= 2)
            {
                pOne = Votingsystem.Instance.GetParticipantById(1);
                pOneTrust = 0;
                foreach (var item in pOne.trust)
                    pOneTrust = pOneTrust + item;
                DeleventSystem.LitPlayersCandle(1, pOneTrust);

                //Player2
                if (Votingsystem.Instance.sessionParticipants.Count >= 3)
                {
                    pOne = Votingsystem.Instance.GetParticipantById(2);
                    pOneTrust = 0;
                    foreach (var item in pOne.trust)
                        pOneTrust = pOneTrust + item;
                    DeleventSystem.LitPlayersCandle(2, pOneTrust);


                    //Player3
                    if (Votingsystem.Instance.sessionParticipants.Count >= 4)
                    {
                        pOne = Votingsystem.Instance.GetParticipantById(3);
                        pOneTrust = 0;
                        foreach (var item in pOne.trust)
                            pOneTrust = pOneTrust + item;
                        DeleventSystem.LitPlayersCandle(3, pOneTrust);

                        //Player4
                        if (Votingsystem.Instance.sessionParticipants.Count >= 5)
                        {
                            pOne = Votingsystem.Instance.GetParticipantById(4);
                            pOneTrust = 0;
                            foreach (var item in pOne.trust)
                                pOneTrust = pOneTrust + item;
                            DeleventSystem.LitPlayersCandle(4, pOneTrust);

                            //Player5
                            if (Votingsystem.Instance.sessionParticipants.Count >= 6)
                            {
                                pOne = Votingsystem.Instance.GetParticipantById(5);
                                pOneTrust = 0;
                                foreach (var item in pOne.trust)
                                    pOneTrust = pOneTrust + item;
                                DeleventSystem.LitPlayersCandle(5, pOneTrust);


                                //Player3
                                if (Votingsystem.Instance.sessionParticipants.Count >= 7)
                                {
                                    pOne = Votingsystem.Instance.GetParticipantById(6);
                                    pOneTrust = 0;
                                    foreach (var item in pOne.trust)
                                        pOneTrust = pOneTrust + item;
                                    DeleventSystem.LitPlayersCandle(6, pOneTrust);

                                    //Player4
                                    if (Votingsystem.Instance.sessionParticipants.Count >= 8)
                                    {
                                        pOne = Votingsystem.Instance.GetParticipantById(7);
                                        pOneTrust = 0;
                                        foreach (var item in pOne.trust)
                                            pOneTrust = pOneTrust + item;
                                        DeleventSystem.LitPlayersCandle(7, pOneTrust);

                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }


    private bool crRunning = false;
    public void VotingPhaseSubmit(bool submitting)
    {
        if (!submitting)
        {
            if(Votingsystem.Instance.identity.GetfinishedPhase())
                Votingsystem.Instance.SubmitForPhase(false);
            //StopCoroutine("DoOrientationCheck");
            if (crRunning)
            {
                StopCoroutine("DoOrientationCheck");
                crRunning = false;
            }
        }
        else if(!Votingsystem.Instance.identity.GetfinishedPhase())
        {
            if (crRunning)
            {
                //StopCoroutine("DoOrientationCheck");
                //crRunning = false;
            }
            else
            {                
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
        while (restingCounter < restingSeconds)
        {
            //ProximityCheck();
            if (MobileDeviceOrientations.GetLyingDownGlobal())
                restingCounter += xthOfASecond;
            else
                restingCounter = 0;
            
            yield return new WaitForSeconds(xthOfASecond);
        }
        crRunning = false;
        DeleventSystem.OnPhoneResting();
        Votingsystem.Instance.SubmitForPhase(true);        
    }

    //VesselPhase

    public void VesselPhaseAction(int exorcismOrChecking, int targetPlayer)
    {
        //exorcism = 1
        //chechinkg = 2
        if (Votingsystem.Instance.exorcismUsed == true)
        {
            //vessel sees that exorcism was already used
            Votingsystem.Instance.AddMessageToBuffer(Votingsystem.Instance.encoder.VesselAction(exorcismOrChecking, -1));
            //vSystem.AddMessageToBuffer(vSystem.encoder.VesselAction(exorcismOrChecking, -1));
            //submitVesselPhase.SetActive(true);
        }
        else
        {
            //might get relevant for clients aswell as soon as the client does not get his own message back again
            if (Votingsystem.Instance.isServer)
            {
                Votingsystem.Instance.EvaluatVesselAction(exorcismOrChecking, targetPlayer);
            }

            Votingsystem.Instance.AddMessageToBuffer(Votingsystem.Instance.encoder.VesselAction(exorcismOrChecking, targetPlayer));
        }
    }
    public void VesselPhaseSubmit(bool ready)
    {
        Votingsystem.Instance.SubmitForPhase(ready);
    }

    // Start is called before the first frame update
    void Start()
    {
        DeleventSystem.ChangePlayerTrust += UpdateCandlesInAr;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
    
