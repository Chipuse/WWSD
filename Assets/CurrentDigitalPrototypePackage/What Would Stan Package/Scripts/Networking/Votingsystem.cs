using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Shatalmic;
using System;

public class Votingsystem : MonoBehaviour
{
    public static Votingsystem Instance;
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        encoder = new ByteEncoder(this);
        messageQueue = new NetworkMessageQueue();
        messageBuffer = new Queue<byte[]>();
        smartMessageQueue = new SmartMessageQueue();
        gameState = 0;
    }


    public List<NetworkParticipant> sessionParticipants = null;
    public UiManager uiManager;

    public int roundCounter = 0;
    public int mooncycleLength = 3;
    public int moonCounter = 0;
    public bool exorcismUsed = false;
    public bool witchesWon = false;
    public bool satanistsWon = false;
    public int maxNumberRounds = 9;

    public bool candleSet = false;
    private void OnLocalCandleSet(Vector3 v3) { candleSet = true; }
    private bool crRunning = false;
    IEnumerator DoLocalCandleCheck()
    {
        while (!candleSet)
        { 
            yield return new WaitForSeconds(0.2f);
        }
        crRunning = false;
        //could also already exchange candleholderpositions
        if(!isServer)
            SubmitForPhase(true);
    }
    private void StartCoroutineLobbyPhase(List<NetworkParticipant> playerList)
    {
        if (!crRunning)
            StartCoroutine("DoLocalCandleCheck");
    }

    public string exorcisedEndOfMoonList = "";
    public string exorcisedDebugList = "";
    public int[] exorcismTargetsId = {-1,-1,-1};

    private int sessionSize = 0;

     string peripheralName;
    
    public NetworkParticipant identity;

    public ByteEncoder encoder;
    private NetworkMessageQueue messageQueue;
    private SmartMessageQueue smartMessageQueue;

    //alternative to messageQueue:
    private Queue<Byte[]> messageBuffer;
    private List<Byte[]> smartMessageBuffer;
    
    
    public Color[] playerColorList;
    public Color GetColorByColorCode(int colorCode)
    {
        if (playerColorList.Length >= colorCode - 1)
        {
            return playerColorList[colorCode];
        }
        return new Color(0, 0, 0, 1);
    }
    public Gradient[] playerColorListGradient;
    public Gradient GetColorGradientByColorCode(int colorCode)
    {
        if (playerColorListGradient.Length >= colorCode - 1)
        {
            return playerColorListGradient[colorCode];
        }
        return new Gradient();
    }
    

    private bool NetworkDeviceLookUp(List<NetworkParticipant> _sessionParticipants, Networking.NetworkDevice target)
    {
        foreach (NetworkParticipant item in _sessionParticipants)
        {
            if (item.GetNetworkDevice() == target)
            {
                return true;
            }
        }
        return false;
    }
    public NetworkParticipant GetParticipantById(int _playerId)
    {
        foreach (NetworkParticipant item in sessionParticipants)
        {
            if (item.GetPlayerId() == _playerId)
            {
                return item;
            }
        }
        return null;
    }

    public int GetCurrentOracleId()
    {
        if(sessionParticipants != null)
        {
            foreach (var item in sessionParticipants)
            {
                if(item.GetVessel() == true)
                {
                    return item.GetPlayerId();
                }
            }
        }
        //if theres no list of networkparticipants or no oracle was elected yet in the game it will return -1
        //in voting phase the oracle of the last oracle phase will still be returned
        return -1;
    }

    //gives Player a new ID and increases the player count of the session by one
    private void RegisterNewPlayer(NetworkParticipant newPlayer)
    {
        newPlayer.SetPlayerId(sessionSize);
        sessionSize++;
    }


    //At the start of the game
    private void ChangeGameState(int newState)
    {
        if (isServer)
        {
            deviceToSkip = null;
            AddMessageToBuffer(encoder.GameLoopProgress(newState));
        }
        DebugText.LogMessage("Triggering new GameState: " + newState.ToString());
        DeleventSystem.ChangeGameState(newState);
        DebugText.LogMessage("No errors in triggering: " + newState.ToString());
    }
    private void ResetPlayerTrust(int newState)
    {
        DebugText.LogMessage("ResetPlayerTrust Votingsystem");
        if (newState == votingPhase)
        {
            foreach (var item in sessionParticipants)
            {
                item.ResetPlayerTrust();
            }
            DeleventSystem.ChangePlayerTrust(identity.GetPlayerId(), identity.GetPlayerId(), 0);
        }
        DebugText.LogMessage("ResetPlayerTrust Votingsystem");
    }

    private void ChangeLocalGameState(int newState)
    {
        DebugText.LogMessage("ChangeLocalGameState Votingsystem");
        gameState = newState;
        DebugText.LogMessage("ChangeLocalGameState Votingsystem");
    }

    public void TestPing()
    {
        AddMessageToBuffer(encoder.TestPing());
    }
    //when all Players have made their votes
    private void EndVotingPhase()
    {

    }

    private void TestVoteFinished()
    {
        sessionParticipants = new List<NetworkParticipant>();
        sessionParticipants.Add(new NetworkParticipant(0, "phil"));
        sessionParticipants.Add(new NetworkParticipant(1, "lukas"));
        byte[] testMessage = encoder.SubmitVotes(1, false, gameState);
        encoder.StartDecode(testMessage);
        Debug.Log(sessionParticipants[1].GetfinishedPhase());
        
    }

    private void TestMessageDivider()
    {
        //encoder.SynchronizePlayerList(sessionParticipants);
        Queue<byte[]> testBuffer = new Queue<byte[]>();
        List<NetworkParticipant> testParticipants = new List<NetworkParticipant>();

        testParticipants.Add(new NetworkParticipant(0, "steve"));
        testParticipants.Add(new NetworkParticipant(1, "bär"));
        testParticipants.Add(new NetworkParticipant(2, "Philip"));
        testParticipants.Add(new NetworkParticipant(3, "Markus"));
        testParticipants.Add(new NetworkParticipant(4, "Johannes"));
        

        byte[] testMessage = encoder.SynchronizePlayerList(testParticipants);

        if (testMessage.Length > 16)
        {
            Queue<byte[]> parts = encoder.MessageDivider(testMessage);
            while (parts.Count != 0)
            {
                testBuffer.Enqueue(parts.Dequeue());
            }
        }
        else
            testBuffer.Enqueue(testMessage);

        while(testBuffer.Count != 0)
        {
            encoder.StartDecode(testBuffer.Dequeue());            
        }

        foreach (var item in sessionParticipants)
        {
            Debug.Log(item.GetName());
        }
        Debug.Log("finished");
    }

    public void AddMessageToBuffer(byte[] bytes)
    {
        if (bytes.Length > 16)
        {
            Queue<byte[]> parts = encoder.MessageDivider(bytes);
            while(parts.Count != 0)
            {
                //messageBuffer.Enqueue(parts.Dequeue());
                AddMessageToSmartMessageQueue(parts.Dequeue());
            }
        }
        else
        {
            //messageBuffer.Enqueue(bytes);
            AddMessageToSmartMessageQueue(bytes);
        }
           
    }
    void AddMessageToSmartMessageQueue(string sender, byte[] bytes)
    {
        smartMessageQueue.AddMessage(sender, bytes);
    }
    void AddMessageToSmartMessageQueue(byte[] bytes)
    {
        smartMessageQueue.AddMessage("undef",bytes);
    }

    private void MessageEmitter(byte[] bytes, Networking.NetworkDevice device = null)
    {
        if (isServer)
        {
            //bytesToWrite = bytes;
        }
        if (!sendingCubeRotation)
        {
            //i do not understand completely, but it may be fine
            if (isServer)
            {
                if (device == null)
                {
                    if (connectedDeviceList != null)
                    {
                        if ( //connectedDeviceList.Count == 1)
                            false)
                        {
                            if (deviceToSkip == null)
                            {
                                networking.WriteDevice(connectedDeviceList[0], bytes, () =>
                                {
                                    sendingCubeRotation = false;
                                });
                            }
                            else
                            {
                                deviceToSkip = null;
                                sendingCubeRotation = false;
                            }
                        }
                        else
                        {
                            deviceToWriteIndex = 0;
                            writeDeviceBytes = true;
                            bytesToWrite = bytes;
                        }
                    }
                    else if (deviceToSkip != null)
                        deviceToSkip = null;
                }
                else
                {
                    networking.WriteDevice(device, bytes, () =>
                    {
                        sendingCubeRotation = false;
                    });
                }
            }
            else
            {
                
                networking.SendFromClient(bytes);
                sendingCubeRotation = false;
            }
        }

    }

    private void MessageReceiver(byte[] bytes)
    {
        
        
        
        if (isServer)
        {
            //DebugText.LogNewMessage("Message From Client");
            //messageQueue.Add(() => MessageEmitter(bytes));
            AddMessageToBuffer(bytes);
        }
        encoder.StartDecode(bytes);
       
    }


    //FunctionsTriggered by buttons
    //LobbyPhase:

    public void StartGame()
    {
        if (isServer)
            DistributeRoles();
        //ChangeGameState(rolePhase);
        identity.SetfinishedPhase(true);
    }

    //ClientFunctions:
    void SettingIdentityClient(List<NetworkParticipant> newPlayerList)
    {
        
        if (!isServer && peripheralName !=null)
        {
            foreach (var item in newPlayerList)
            {
                if (peripheralName.Replace("@net@:", "") == item.GetName())
                {
                    identity = item;
                }
            }
            if(identity == null)
            {
            }
        }
    }

    void ResetSubmitStatus(int Phase)
    {
        //SubmitForPhase(false);
        foreach (var item in sessionParticipants)
        {
            AddMessageToBuffer(encoder.SubmitVotes(item.GetPlayerId(), false, gameState));
        }
    }

    public void SubmitForPhase(bool newSubmit)
    {
        AddMessageToBuffer(encoder.SubmitVotes(identity.GetPlayerId(), newSubmit, gameState));
        identity.SetfinishedPhase(newSubmit);
    }
    //Server Functions:
    private void SetRoleServerAndNetwork(int targetPlayerIDm, int newRole)
    {
        GetParticipantById(targetPlayerIDm).SetRole(newRole);
        AddMessageToBuffer(encoder.RoleAssignment(newRole, targetPlayerIDm));
    }

    public void DistributeRoles()
    {
        switch (sessionParticipants.Count)
        {
            case 0:
                //0 satanist
                
            case 1:
                //0 satanist

                break;
            case 2:
                //1 satanist
                int twoPlayersSatanistOne = UnityEngine.Random.Range(0, sessionParticipants.Count - 0);
                SetRoleServerAndNetwork(twoPlayersSatanistOne, 2);
                int twoPlayerPossess = UnityEngine.Random.Range(0, sessionParticipants.Count - 0);
                while(twoPlayerPossess == twoPlayersSatanistOne)
                    twoPlayerPossess = UnityEngine.Random.Range(0, sessionParticipants.Count - 0);
                SetRoleServerAndNetwork(twoPlayerPossess, 3);

                break;
            case 3:
                //1 satanist
                int threePlayersSatanistOne = UnityEngine.Random.Range(0, sessionParticipants.Count - 0);
                SetRoleServerAndNetwork(threePlayersSatanistOne, 2);

                int threePlayerPossess = UnityEngine.Random.Range(0, sessionParticipants.Count - 0);
                while (threePlayerPossess == threePlayersSatanistOne)
                    threePlayerPossess = UnityEngine.Random.Range(0, sessionParticipants.Count - 0);
                SetRoleServerAndNetwork(threePlayerPossess, 3);

                break;
            case 4:
                //1 satanist
                int fourPlayersSatanistOne = UnityEngine.Random.Range(0, sessionParticipants.Count - 0);
                SetRoleServerAndNetwork(fourPlayersSatanistOne, 2);

                int fourPlayerPossess = UnityEngine.Random.Range(0, sessionParticipants.Count - 0);
                while (fourPlayerPossess == fourPlayersSatanistOne)
                    fourPlayerPossess = UnityEngine.Random.Range(0, sessionParticipants.Count - 0);
                SetRoleServerAndNetwork(fourPlayerPossess, 3);

                break;
            case 5:
                //2 satanist
                int fivePlayersSatanistOne = UnityEngine.Random.Range(0, sessionParticipants.Count - 0);
                int fivePlayersSatanistTwo = UnityEngine.Random.Range(0, sessionParticipants.Count - 0);
                while(fivePlayersSatanistOne == fivePlayersSatanistTwo)
                    fivePlayersSatanistTwo = UnityEngine.Random.Range(0, sessionParticipants.Count - 0);

                SetRoleServerAndNetwork(fivePlayersSatanistOne, 2);
                SetRoleServerAndNetwork(fivePlayersSatanistTwo, 2);

                int fivePlayerPossess = UnityEngine.Random.Range(0, sessionParticipants.Count - 0);
                while (fivePlayerPossess == fivePlayersSatanistOne || fivePlayerPossess == fivePlayersSatanistTwo)
                    fivePlayerPossess = UnityEngine.Random.Range(0, sessionParticipants.Count - 0);
                SetRoleServerAndNetwork(fivePlayerPossess, 3);

                break;
            case 6:
                //2 satanist
                int sixPlayersSatanistOne = UnityEngine.Random.Range(0, sessionParticipants.Count - 0);
                int sixPlayersSatanistTwo = UnityEngine.Random.Range(0, sessionParticipants.Count - 0);
                while(sixPlayersSatanistOne == sixPlayersSatanistTwo)
                    sixPlayersSatanistTwo = UnityEngine.Random.Range(0, sessionParticipants.Count - 0);

                SetRoleServerAndNetwork(sixPlayersSatanistOne, 2);
                SetRoleServerAndNetwork(sixPlayersSatanistTwo, 2);

                int sixPlayerPossess = UnityEngine.Random.Range(0, sessionParticipants.Count - 0);
                while (sixPlayerPossess == sixPlayersSatanistOne || sixPlayerPossess == sixPlayersSatanistTwo)
                    sixPlayerPossess = UnityEngine.Random.Range(0, sessionParticipants.Count - 0);
                SetRoleServerAndNetwork(sixPlayerPossess, 3);


                break;
            case 7:
                //3 satanist
                int sevenPlayersSatanistOne = UnityEngine.Random.Range(0, sessionParticipants.Count - 0);
                int sevenPlayersSatanistTwo = UnityEngine.Random.Range(0, sessionParticipants.Count - 0);
                int sevenPlayersSatanistThree = UnityEngine.Random.Range(0, sessionParticipants.Count - 0);
                while(sevenPlayersSatanistOne == sevenPlayersSatanistTwo)
                    sevenPlayersSatanistTwo = UnityEngine.Random.Range(0, sessionParticipants.Count - 0);
                while(sevenPlayersSatanistThree == sevenPlayersSatanistOne || sevenPlayersSatanistThree == sevenPlayersSatanistTwo)
                    sevenPlayersSatanistThree = UnityEngine.Random.Range(0, sessionParticipants.Count - 0);

                SetRoleServerAndNetwork(sevenPlayersSatanistOne, 2);
                SetRoleServerAndNetwork(sevenPlayersSatanistTwo, 2);
                SetRoleServerAndNetwork(sevenPlayersSatanistThree, 2);

                int sevenPlayerPossess = UnityEngine.Random.Range(0, sessionParticipants.Count - 0);
                while (sevenPlayerPossess == sevenPlayersSatanistOne || sevenPlayerPossess == sevenPlayersSatanistTwo || sevenPlayerPossess == sevenPlayersSatanistThree)
                    sevenPlayerPossess = UnityEngine.Random.Range(0, sessionParticipants.Count - 0);
                SetRoleServerAndNetwork(sevenPlayerPossess, 3);

                break;
            case 8:
                //3 satanist
                int eightPlayersSatanistOne = UnityEngine.Random.Range(0, sessionParticipants.Count - 0);
                int eightPlayersSatanistTwo = UnityEngine.Random.Range(0, sessionParticipants.Count - 0);
                int eightPlayersSatanistThree = UnityEngine.Random.Range(0, sessionParticipants.Count - 0);
                while(eightPlayersSatanistOne == eightPlayersSatanistTwo)
                    eightPlayersSatanistTwo = UnityEngine.Random.Range(0, sessionParticipants.Count - 0);
                while(eightPlayersSatanistThree == eightPlayersSatanistOne || eightPlayersSatanistThree == eightPlayersSatanistTwo)
                    eightPlayersSatanistThree = UnityEngine.Random.Range(0, sessionParticipants.Count - 0);

                SetRoleServerAndNetwork(eightPlayersSatanistOne, 2);
                SetRoleServerAndNetwork(eightPlayersSatanistTwo, 2);
                SetRoleServerAndNetwork(eightPlayersSatanistThree, 2);

                int eightPlayerPossess = UnityEngine.Random.Range(0, sessionParticipants.Count - 0);
                while (eightPlayerPossess == eightPlayersSatanistOne || eightPlayerPossess == eightPlayersSatanistTwo || eightPlayerPossess == eightPlayersSatanistThree)
                    eightPlayerPossess = UnityEngine.Random.Range(0, sessionParticipants.Count - 0);
                SetRoleServerAndNetwork(eightPlayerPossess, 3);

                break;

        }
        
    }

    public void EvaluatVesselAction(int chosenActionType, int playerIdChosenTarget)
    {
        if(chosenActionType == 1)
        {
            exorcisedDebugList += GetParticipantById(playerIdChosenTarget).GetName() + "\r\n"/* + " exorcised in " + (roundCounter+1).ToString()*/;
            exorcismTargetsId[moonCounter] = GetParticipantById(playerIdChosenTarget).GetPlayerId();
            exorcismUsed = true;
        }
        if(chosenActionType == 1 && GetParticipantById(playerIdChosenTarget).GetRole() == 3)
        {
            //Witches have won!
            if(isServer)
                ChangeGameState(witchesWonPhase);
        }
    }




    //Plugin Example Code from here on


    private Networking networking = null;
    private List<Networking.NetworkDevice> connectedDeviceList = null;



    public bool isServer = false;
    private bool writeDeviceBytes = false;
    private byte[] bytesToWrite = null;
    private int deviceToWriteIndex = 0;
    private bool sendingCubeRotation = false;
    private Quaternion newCubeRotation = Quaternion.identity;
    private Networking.NetworkDevice deviceToSkip = null;

    public GameObject Cube;
    public Text TextStatus;

    //Signals in which segment of the GameLoop the party currently is:
    //0 -> hosting and connecting phase
    //1 -> voting phase
    //2 ->
    public int gameState;

    public int lobbyPhase = 0;
    public int rolePhase = 1;
    public int votingPhase = 2;
    public int vesselPhase = 3;
    public int endPhase = 4;
    public int witchesWonPhase = 5;
    public int satanistsWonPhase = 6;

    // because the networking library is asynchronous due to the nature of
    // writing to the bluetooth stack, we have to write each client device
    // and then wait for that write to be completed before we write the next
    // device.
    // notice how this update loop watches for the writeDeviceBytes flag to be
    // true, calls the bluetooth write and then when the bluetooth
    // callback occurs goes on to the next device or signals that it is done


    private void Update()
    {
        
        DebugText.LogNewMessage("UPdate");
        /*
        if (sessionParticipants != null)
        {
            foreach (var item in sessionParticipants)
            {
                DebugText.LogMessage(item.GetRole().ToString());
                DebugText.LogMessage(item.GetVessel().ToString());
                DebugText.LogMessage(item.GetVesselPoints().ToString());
                DebugText.LogMessage(item.GetPlayerId().ToString());
                DebugText.LogMessage(item.GetName());
            }
        }*/

        // if (!isServer)
        {
            if (false)
            if (identity != null && identity.trust.Length >= 2)
                DebugText.LogNewMessage(
                    //"Identity values: \r\n" +
                    //identity.GetName() + " playerName \r\n" +
                    //identity.trustGiven[0].ToString() + "-" + identity.trustGiven[1].ToString() + "-" + identity.trustGiven[2].ToString() + " trustGiven\r\n" +
                    //identity.trust[0].ToString() + "-" + identity.trust[1].ToString() + "-" + identity.trust[2].ToString() + " trustReceived\r\n" +
                    //identity.GetfinishedPhase().ToString() + " finished Phase \r\n" +
                    identity.GetVessel().ToString() + " Vessel\r\n" +
                    identity.GetRole().ToString() + " Role\r\n" +
                    identity.GetPlayerId().ToString() + " playerID \r\n End"
                    + exorcisedDebugList
                    );
        }
        if (isServer)
        {
            switch (gameState)
            {
                case 0:
                    bool allreadyLobby = true;
                    foreach (var item in sessionParticipants)
                    {
                        if (!item.GetfinishedPhase())
                        {
                            allreadyLobby = false;
                            //break;
                        }

                    }
                    if (allreadyLobby)
                    {

                        foreach (var item in sessionParticipants)
                        {
                            item.SetfinishedPhase(false);
                            AddMessageToBuffer(encoder.SubmitVotes(item.GetPlayerId(), false, gameState));
                        }
                        ChangeGameState(rolePhase);
                    }
                    break;
                case 1:
                    
                    bool allready = true;
                    foreach (var item in sessionParticipants)
                    {
                        if (!item.GetfinishedPhase())
                        {
                            allready = false;
                            //break;
                        }

                    }
                    if (allready)
                    {
                        
                        foreach (var item in sessionParticipants)
                        {
                            item.SetfinishedPhase(false);
                            AddMessageToBuffer(encoder.SubmitVotes(item.GetPlayerId(), false, gameState));
                        }
                        ChangeGameState(votingPhase);
                    }
                    break;
                case 2:
                    bool allreadyVoting = true;
                    foreach (var item in sessionParticipants)
                    {
                        if (!item.GetfinishedPhase())
                        {
                            allreadyVoting = false;
                           
                        }

                    }
                    if (allreadyVoting)
                    {
                        int highestTrust = 0;
                        List<NetworkParticipant> highestTrustsList = new List<NetworkParticipant>();
                        foreach (var item in sessionParticipants)
                        {
                            int trustSum = 0;
                            foreach (var trust in item.trust)
                            {
                                trustSum += trust;
                            }
                            /*
                            if (item.GetVessel())
                            {

                            }
                            */
                            if(trustSum == highestTrust)
                            {
                                highestTrustsList.Add(item);
                            }
                            else if (trustSum > highestTrust)
                            {
                                highestTrustsList = new List<NetworkParticipant>();
                                highestTrustsList.Add(item);
                                highestTrust = trustSum;
                            }
                        }
                        int playerIdOfNewVessel = -1;
                        if (highestTrustsList.Count > 1)
                        {
                            //bei gleichstand logik hier:
                            List<NetworkParticipant> badBoyList = new List<NetworkParticipant>();
                            foreach (var item in highestTrustsList)
                            {
                                if(item.GetRole() == 2)
                                {
                                    //Satanist priority is currently depricated. Keeping functionality if later it will be reenabled
                                    //badBoyList.Add(item);
                                }
                            }
                            //if two or more satanists have the highest trust
                            if (badBoyList.Count > 1)
                            {

                                List<NetworkParticipant> lowestVesselPoints = new List<NetworkParticipant>();
                                int currentLowestVesselPoints = 100;
                                foreach (var item in badBoyList)
                                {
                                    if (item.GetVesselPoints() == currentLowestVesselPoints)
                                        lowestVesselPoints.Add(item);
                                    else if (item.GetVesselPoints() < currentLowestVesselPoints)
                                    {
                                        lowestVesselPoints = new List<NetworkParticipant>();
                                        currentLowestVesselPoints = item.GetVesselPoints();
                                        lowestVesselPoints.Add(item);
                                    }
                                }
                                if (lowestVesselPoints.Count > 1)
                                {
                                    playerIdOfNewVessel = lowestVesselPoints[UnityEngine.Random.Range(0, lowestVesselPoints.Count - 0)].GetPlayerId();
                                }
                                else if (lowestVesselPoints.Count == 1)
                                {
                                    playerIdOfNewVessel = lowestVesselPoints[0].GetPlayerId();
                                }
                            }
                            else if (badBoyList.Count == 1)
                            {
                                playerIdOfNewVessel = badBoyList[0].GetPlayerId();
                            }
                            else
                            {
                                //no satanists have the highest trust
                                List<NetworkParticipant> lowestVesselPoints = new List<NetworkParticipant>();
                                int currentLowestVesselPoints = 100;
                                foreach (var item in highestTrustsList)
                                {
                                    if (item.GetVesselPoints() == currentLowestVesselPoints)
                                        lowestVesselPoints.Add(item);
                                    else if (item.GetVesselPoints() < currentLowestVesselPoints)
                                    {
                                        lowestVesselPoints = new List<NetworkParticipant>();
                                        currentLowestVesselPoints = item.GetVesselPoints();
                                        lowestVesselPoints.Add(item);
                                    }
                                }
                                if (lowestVesselPoints.Count > 1)
                                {
                                    playerIdOfNewVessel = lowestVesselPoints[UnityEngine.Random.Range(0, lowestVesselPoints.Count - 0)].GetPlayerId();
                                }
                                else if (lowestVesselPoints.Count == 1)
                                {
                                    playerIdOfNewVessel = lowestVesselPoints[0].GetPlayerId();
                                }
                            }
                           
                        }
                        else if (highestTrustsList.Count == 1)
                        {
                            playerIdOfNewVessel = highestTrustsList[0].GetPlayerId();
                        }
                        if(playerIdOfNewVessel != -1)
                        {
                            //AddMessageToBuffer(encoder.SetVessel(playerIdOfNewVessel, true));
                        }
                        

                        foreach (var item in sessionParticipants)
                        {
                            if(item.GetPlayerId() != playerIdOfNewVessel)
                            {
                                //All but new vessel get set to true by default
                                item.SetfinishedPhase(true);
                                AddMessageToBuffer(encoder.SubmitVotes(item.GetPlayerId(), true, gameState));
                            }
                            else
                            {
                                item.SetfinishedPhase(false);
                                AddMessageToBuffer(encoder.SubmitVotes(item.GetPlayerId(), false, gameState));
                            }
                            
                            if (item.GetPlayerId() == playerIdOfNewVessel)
                            {
                                AddMessageToBuffer(encoder.SetVessel(item.GetPlayerId(), true));
                                item.SetVessel(true);
                                item.SetVesselPoints(item.GetVesselPoints() + 1);
                            }
                            else
                            {
                                AddMessageToBuffer(encoder.SetVessel(item.GetPlayerId(), false));
                                item.SetVessel(false);
                            }
                                
                        }
                        if(GetParticipantById(playerIdOfNewVessel).GetRole() == 2 && GetParticipantById(playerIdOfNewVessel).GetVesselPoints() >= 3)
                            ChangeGameState(satanistsWonPhase);
                        else
                            ChangeGameState(vesselPhase);
                    }
                    break;

                case 3:
                    bool allreadyVessel = true;
                    
                    foreach (var item in sessionParticipants)
                    {
                        if (!item.GetfinishedPhase())
                        {                            
                            allreadyVessel = false;
                        }

                    }
                    if (allreadyVessel)
                    {

                        foreach (var item in sessionParticipants)
                        {
                            item.SetfinishedPhase(false);
                            AddMessageToBuffer(encoder.SubmitVotes(item.GetPlayerId(), false, gameState));
                        }
                        ChangeGameState(endPhase);
                    }
                    break;
                case 4:
                    if (witchesWon)
                    {
                        ChangeGameState(witchesWonPhase);
                    }
                    else
                    {
                        roundCounter++;
                        if(roundCounter >= maxNumberRounds)
                        {
                            ChangeGameState(satanistsWonPhase);
                        }
                        else
                        {
                            if (roundCounter % mooncycleLength == 0 && roundCounter != 0)
                            {
                                exorcismUsed = false;
                                moonCounter++;
                                exorcisedEndOfMoonList = exorcisedDebugList;
                            }
                                
                            AddMessageToBuffer(encoder.GameLoopData());
                            ChangeGameState(votingPhase);
                        }
                    }
                    break;
                case 5:

                    break;
                default:
                    Console.WriteLine("Default case");
                    break;
            }
            
        }


        



        if (smartMessageQueue.smartMessageList.Count != 0 && bytesToWrite == null && sendingCubeRotation == false)
        {
            //MessageEmitter(messageBuffer.Dequeue());
            MessageEmitter(smartMessageQueue.DequeueMessage());            
        }


        if (writeDeviceBytes)
        {
            writeDeviceBytes = false;

            if (bytesToWrite != null && connectedDeviceList != null && deviceToWriteIndex >= 0 && deviceToWriteIndex < connectedDeviceList.Count)
            {
                Action afterWrite = () =>
                {
                    deviceToWriteIndex++;
                    writeDeviceBytes = (deviceToWriteIndex < connectedDeviceList.Count);
                    if (!writeDeviceBytes)
                    {
                        bytesToWrite = null;
                        /*
                        if (!messageQueue.IsEmpty)
                        {
                            //in message Queue there are a number of executions of MessageEmitter(byte[] message) buffered
                            messageQueue.Pop();
                        }
                        */
                        
                        
                        sendingCubeRotation = false;
                    }
                };

                var deviceToWrite = connectedDeviceList[deviceToWriteIndex];
                if (deviceToWrite != deviceToSkip)
                {
                    networking.WriteDevice(deviceToWrite, bytesToWrite, afterWrite);
                }
                else
                {
                    afterWrite();
                }
            }
        }
        /*
        else if (!messageQueue.IsEmpty)
        {
            Action action = messageQueue.Pop();
            action.Invoke();
            //messageQueue.Pop()();
        }
        
        else if (messageBuffer.Count != 0)
        {
            MessageEmitter(messageBuffer.Dequeue());
        }
        
        if (newCubeRotation != Quaternion.identity)
        {
            if (Cube != null)
                Cube.transform.rotation = newCubeRotation;
            newCubeRotation = Quaternion.identity;
        }
        */
    }

    protected void SendCubeRotation(Quaternion rotation, Networking.NetworkDevice device = null)
    {
        if (!sendingCubeRotation)
        {
            sendingCubeRotation = true;

            var bytes = new byte[16];
            Array.Copy(BitConverter.GetBytes(rotation.x), 0, bytes, 0, 4);
            Array.Copy(BitConverter.GetBytes(rotation.y), 0, bytes, 4, 4);
            Array.Copy(BitConverter.GetBytes(rotation.z), 0, bytes, 8, 4);
            Array.Copy(BitConverter.GetBytes(rotation.w), 0, bytes, 12, 4);

            if (isServer)
            {
                if (device == null)
                {
                    if (connectedDeviceList != null)
                    {
                        if (connectedDeviceList.Count == 1)
                        {
                            if (deviceToSkip == null)
                            {
                                networking.WriteDevice(connectedDeviceList[0], bytes, () =>
                                {
                                    sendingCubeRotation = false;
                                });
                            }
                            else
                            {
                                deviceToSkip = null;
                                sendingCubeRotation = false;
                            }
                        }
                        else
                        {
                            deviceToWriteIndex = 0;
                            writeDeviceBytes = true;
                            bytesToWrite = bytes;
                        }
                    }
                    else if (deviceToSkip != null)
                        deviceToSkip = null;
                }
                else
                {
                    networking.WriteDevice(device, bytes, () =>
                    {
                        sendingCubeRotation = false;
                    });
                }
            }
            else
            {
                networking.SendFromClient(bytes);
                sendingCubeRotation = false;
            }
        }
    }

    protected void ParseCubePosition(byte[] bytes)
    {
        newCubeRotation = new Quaternion(BitConverter.ToSingle(bytes, 0), BitConverter.ToSingle(bytes, 4), BitConverter.ToSingle(bytes, 8), BitConverter.ToSingle(bytes, 12));

        if (isServer)
            SendCubeRotation(newCubeRotation);
    }


    //depricated:
    public Text NetworkName;
    public Text ClientName;
    public GameObject ButtonStartServer;
    public GameObject ButtonStartClient;
    public GameObject ButtonStopNetwork;
    public GameObject ButtonP1;
    public GameObject ButtonP2;

    public void OnButton(Button button)
    {
        switch (button.name)
        {
            case "ButtonStartServer":
                if (string.IsNullOrEmpty(NetworkName.text) || string.IsNullOrEmpty(ClientName.text))
                {
                    TextStatus.text = "Enter a network name & enter a client Name";
                }
                else
                {
                    TextStatus.text = "";
                    sessionSize = 0;
                    identity = new NetworkParticipant(ClientName.text, null, true);
                    RegisterNewPlayer(identity);
                    if (sessionParticipants == null)
                        sessionParticipants = new List<NetworkParticipant>();
                    sessionParticipants.Add(identity);



                    //from example
                    deviceToSkip = null;
                    deviceToWriteIndex = 0;
                    sendingCubeRotation = false;
                    isServer = true;
                    ButtonStartServer.SetActive(false);
                    ButtonStartClient.SetActive(false);
                    ButtonStopNetwork.SetActive(true);

                    if (Cube != null)
                        Cube.SetActive(false);

                    networking.StartServer(NetworkName.text, (connectedDevice) =>
                    {
                        //this event will be triggered when a new device (connectedDevice) joins the server
                        if (connectedDeviceList == null)
                            connectedDeviceList = new List<Networking.NetworkDevice>();

                        if (sessionParticipants == null)
                            sessionParticipants = new List<NetworkParticipant>();

                        if (!connectedDeviceList.Contains(connectedDevice))
                        {
                            connectedDeviceList.Add(connectedDevice);

                            if (Cube != null)
                            {
                                Cube.SetActive(true);
                                SendCubeRotation(Cube.transform.rotation, connectedDevice);
                            }
                        }
                        if (!NetworkDeviceLookUp(sessionParticipants, connectedDevice))
                        {
                            NetworkParticipant newPlayer = new NetworkParticipant(connectedDevice.Name, connectedDevice);

                            RegisterNewPlayer(newPlayer);

                            sessionParticipants.Add(newPlayer);
                        }
                    }, (disconnectedDevice) =>
                    {
                        if (connectedDeviceList != null && connectedDeviceList.Contains(disconnectedDevice))
                            connectedDeviceList.Remove(disconnectedDevice);
                    }, (dataDevice, characteristic, bytes) =>
                    {
                        deviceToSkip = dataDevice;

                        ParseCubePosition(bytes);
                    });
                }
                break;

            case "ButtonStartClient":
                if (string.IsNullOrEmpty(NetworkName.text) || string.IsNullOrEmpty(ClientName.text))
                {
                    TextStatus.text = "Enter both a network and client name";
                }
                else
                {
                    TextStatus.text = "";

                    isServer = false;
                    ButtonStartServer.SetActive(false);
                    ButtonStartClient.SetActive(false);
                    ButtonStopNetwork.SetActive(true);

                    if (Cube != null)
                        Cube.SetActive(false);

                    networking.StartClient(NetworkName.text, ClientName.text, () =>
                    {
                        networking.StatusMessage = "Started advertising";
                    }, (clientName, characteristic, bytes) =>
                    {
                        if (Cube != null)
                            Cube.SetActive(true);
                        ParseCubePosition(bytes);
                    });
                }
                break;

            case "ButtonStopNetwork":
                if (isServer)
                {
                    if (connectedDeviceList != null)
                        connectedDeviceList = null;
                    networking.StopServer(() =>
                    {
                        ButtonStartServer.SetActive(true);
                        ButtonStartClient.SetActive(true);
                        ButtonStopNetwork.SetActive(false);

                        if (Cube != null)
                            Cube.SetActive(false);
                    });
                }
                else
                {
                    networking.StopClient(() =>
                    {
                        ButtonStartServer.SetActive(true);
                        ButtonStartClient.SetActive(true);
                        ButtonStopNetwork.SetActive(false);

                        if (Cube != null)
                            Cube.SetActive(false);
                    });
                }
                break;

            case "ButtonP1":
                Debug.Log(1);
                if (GetParticipantById(0) != null)
                {
                    int newTrust = 0;
                    if (GetParticipantById(0).trust[identity.GetPlayerId()] == 0)
                    {
                        newTrust = 1;
                    }
                    DeleventSystem.ChangePlayerTrust(0, identity.GetPlayerId(), newTrust);
                }

                break;

            case "ButtonP2":
                if (GetParticipantById(1) != null)
                {
                    int newTrust = 0;
                    if (GetParticipantById(0).trust[identity.GetPlayerId()] == 0)
                    {
                        newTrust = 1;
                    }
                    DeleventSystem.ChangePlayerTrust(1, identity.GetPlayerId(), newTrust);
                }
                break;
        }
    }

    // Use this for initialization
    void Start()
    {
        //creating a ByteEncoder to digest byte array messages
        DeleventSystem.ChangeGameState += ChangeLocalGameState;
        DeleventSystem.RefreshLobby += SettingIdentityClient;
        DeleventSystem.ChangeGameState += ResetPlayerTrust;
        DeleventSystem.SetLocalCandlePosition += OnLocalCandleSet;
        DeleventSystem.RefreshLobby += StartCoroutineLobbyPhase;
        if (networking == null)
        {
            networking = GetComponent<Networking>();
            networking.Initialize((error) =>
            {
                BluetoothLEHardwareInterface.Log("Error: " + error);
            }, (message) =>
            {
                if (TextStatus != null)
                    TextStatus.text = message;

                BluetoothLEHardwareInterface.Log("Message: " + message);
            });
        }
        //TestVoteFinished();
        //TestMessageDivider();
    }

    public void StartServer(String serverName, String playerName)
    {
        if (string.IsNullOrEmpty(serverName) || string.IsNullOrEmpty(playerName))
        {
            DeleventSystem.StartingServer(false);
        }
        else
        {
            DeleventSystem.StartingServer(true);

            isServer = true;
            deviceToSkip = null;
            deviceToWriteIndex = 0;

            sessionSize = 0;
            identity = new NetworkParticipant(playerName, null, true);
            RegisterNewPlayer(identity);
            if (sessionParticipants == null)
                sessionParticipants = new List<NetworkParticipant>();
            sessionParticipants.Add(identity);
            DebugText.LogMessage(identity.GetName() + " started Server");
            networking.StartServer(serverName, (connectedDevice) =>
            {
                DebugText.LogMessage(connectedDevice.Address + " joined Server");
                if (connectedDeviceList == null)
                    connectedDeviceList = new List<Networking.NetworkDevice>();
               // RegisterNewPlayer(new NetworkParticipant(connectedDevice.Name, connectedDevice, false));

                if (!NetworkDeviceLookUp(sessionParticipants, connectedDevice))
                {
                    
                    NetworkParticipant newPlayer = new NetworkParticipant(connectedDevice.Name.Replace("@net@:",""), connectedDevice);

                    RegisterNewPlayer(newPlayer);

                    sessionParticipants.Add(newPlayer);
                }
                //sessionParticipants.Add(newPlayer);
                if (!connectedDeviceList.Contains(connectedDevice))
                {
                    connectedDeviceList.Add(connectedDevice);
                    // share NetworkParticipants with everyone
                    // Share
                    //encoder.SynchronizePlayerList(sessionParticipants);

                    messageQueue.Add(() => MessageEmitter(encoder.SynchronizePlayerList(sessionParticipants)));
                    
                    AddMessageToBuffer(encoder.SynchronizePlayerList(sessionParticipants));
                    //trigger the same event on server as on client
                    encoder.SynchronizePlayerListDecode(encoder.SynchronizePlayerList(sessionParticipants));
                    foreach (var item in sessionParticipants)
                    {
                        item.StartGame(sessionParticipants.Count);
                    }
                }

            }, (disconnectedDevice) =>
            {
                DebugText.LogMessage("Device disconnected!!!");
                DebugText.LogMessage(disconnectedDevice.Name);
                if (connectedDeviceList != null && connectedDeviceList.Contains(disconnectedDevice))
                    connectedDeviceList.Remove(disconnectedDevice);
            }, (dataDevice, characteristic, bytes) =>
            {
                //deviceToSkip = dataDevice;
                //since the messagebuffer does not feature a way to check whether a message in the queue comes from a client we cant use the skipping device feature
                deviceToSkip = null;
                //when client sends data to server
                //the following code will be executed with the sending client as dataDevice and bytes as the send message
                MessageReceiver(bytes);
                
            });

        }
    }

    public void StartClient(string serverName, string playerName)
    {
        if (string.IsNullOrEmpty(serverName) || string.IsNullOrEmpty(playerName))
        {
            DeleventSystem.StartingClient(false);
        }
        else
        {
            DeleventSystem.StartingClient(true);

            isServer = false;
            networking.StatusMessage = "Right Before Advertising";
            peripheralName = serverName + ":" + playerName;
            networking.StartClient(serverName, playerName, () =>
            {
                networking.StatusMessage = "Started advertising";
                //StartCoroutine("DisconnectClient");
            }, (clientName, characteristic, bytes) =>
            {
                networking.StatusMessage = "Now im here";
                MessageReceiver(bytes);
            });
        }
    }

    IEnumerator DisconnectClient()
    {
        DebugText.LogMessage("StartedtCoroutine");
        yield return new WaitForSeconds(10);
        if (!isServer)
        {
            DebugText.LogMessage("Waited Long enough");
            networking.StopClient(()=> DebugText.LogMessage("Disconnecting"));
        }
    }
}

        