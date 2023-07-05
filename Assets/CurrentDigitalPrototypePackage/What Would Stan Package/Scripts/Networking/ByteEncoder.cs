using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ByteEncoder
{

    private List<NetworkParticipant> sessionParticipants;

    //the first to bytes are representing a int16
    private static int signatureSendTrust = 1;
    private static int signatureSubmitVotes = 2;
    private static int signatureGameLoopProgress = 3;
    private static int signatureSimpleString = 4;
    private static int signatureSynchronizePlayerList = 5;
    private static int signatureTooLongArrayStart = 6;
    private static int signatureTooLongArrayMid = 7;
    private static int signatureTooLongArrayEnd = 8;
    private static int signatureIdZuweiser = 9;
    private static int signatureTestPing = 10;
    private static int signatureSetVessel = 11;
    private static int signatureVesselAction = 12;
    private static int signatureRoleAssignment = 13;
    private static int signatureGameLoopData = 14;
    private static int signatureCandleHolderTransform = 15;
    private static int signatureSynchronizePlayerData = 16;



    private Votingsystem vSystem;

    private byte[] tooLongMessage;

    public ByteEncoder()
    {
        vSystem = null;
        sessionParticipants = null;
    }
    public ByteEncoder(Votingsystem _vSystem)
    {
        vSystem = _vSystem;
        sessionParticipants = vSystem.sessionParticipants;
    }

    public void StartDecode(byte[] message)
    {
        int signature = BitConverter.ToInt16(message, 0);
        
        switch (signature)
        {
            case 1:
                SendTrustDecode(message);
                break;
            case 2:
                SubmitVotesDecode(message);
                break;
            case 3:
                GameLoopProgressDecode(message);
                break;
            case 4:
                SimpleStringDecode(message);
                break;
            case 5:
                SynchronizePlayerListDecode(message);
                break;
            case 6:
                MakeSmallBigAgain(message, signatureTooLongArrayStart);
                break;
            case 7:
                MakeSmallBigAgain(message, signatureTooLongArrayMid);
                break;
            case 8:
                MakeSmallBigAgain(message, signatureTooLongArrayEnd);
                break;
            case 9:
                IdZuweiserDecode(message);
                break;
            case 10:
                TestPingDecode(message);
                break;
            case 11:
                SetVesselDecode(message);
                break;
            case 12:
                VesselActionDecode(message);
                break;
            case 13:
                RoleAssignmentDecode(message);
                break;
            case 14:
                GameLoopDataDecode(message);
                break;
            case 15:
                CandleHolderTransformDecode(message);
                break;
            case 16:
                SynchronizePlayerDataDecode(message);
                break;
            default:
                Console.WriteLine("Default case");
                break;
        }

    }

    public byte[] BareBoneFunction()
    {
       
        var bytes = new byte[16];
        return bytes;
    }

    public void BareBoneFunctionDecode(byte[] message)
    {
        
    }
    public byte[] TestPing()
    {
        var bytes = new byte[2];
        Array.Copy(BitConverter.GetBytes(signatureTestPing), 0, bytes, 0, 2);
        return bytes;
    }

    public void TestPingDecode(byte[] message)
    {
        DebugText.LogMessage("received test Ping");
    }
    public Queue<byte[]> MessageDivider(byte[] message)
    {
        Queue<byte[]> result = new Queue<byte[]>();
        byte[] buffer;
        for (int i = 0; i < message.Length ; i+= 16)
        {
            
            buffer = new byte[18];
            if(i == 0)
                Array.Copy(BitConverter.GetBytes(signatureTooLongArrayStart), 0, buffer, 0, 2);
            else if(i + 16 >= message.Length)
            {
                if(message.Length % 16 != 0)
                {
                    buffer = new byte[message.Length % 16 + 2];
                }
                Array.Copy(BitConverter.GetBytes(signatureTooLongArrayEnd), 0, buffer, 0, 2);
            }                
            else 
                Array.Copy(BitConverter.GetBytes(signatureTooLongArrayMid), 0, buffer, 0, 2);

            Array.Copy(message, i, buffer, 2, buffer.Length-2);

            //might be a problem because of the pointer of the array
            result.Enqueue(buffer);
            
        }
        
        return result;
    }

    public void MakeSmallBigAgain(byte[] message, int signature)
    {
        if(signature == signatureTooLongArrayStart)
        {
            tooLongMessage = new byte[message.Length-2];
            Array.Copy(message, 2, tooLongMessage, 0, tooLongMessage.Length);            
        }
        else if (signature == signatureTooLongArrayMid)
        {
            byte[] buffer = new byte[tooLongMessage.Length + message.Length-2];
            tooLongMessage.CopyTo(buffer, 0);
            Array.Copy(message, 2, buffer, tooLongMessage.Length, message.Length - 2);
            tooLongMessage = buffer;
        }
        else if (signature == signatureTooLongArrayEnd)
        {
            byte[] buffer = new byte[tooLongMessage.Length + message.Length-2];
            tooLongMessage.CopyTo(buffer, 0);
            Array.Copy(message, 2, buffer, tooLongMessage.Length, message.Length - 2);
            tooLongMessage = buffer;

            //message is on full length again. Send it to the decoder again
            StartDecode(tooLongMessage);
        }
    }

    public byte[] SendTrust(int playerIdReceiver, int playerIdSender, int trustValue)
    {
        var bytes = new byte[14];
        Array.Copy(BitConverter.GetBytes(signatureSendTrust), 0, bytes, 0, 2);
        Array.Copy(BitConverter.GetBytes(playerIdReceiver), 0, bytes, 2, 4);
        Array.Copy(BitConverter.GetBytes(playerIdSender), 0, bytes, 6, 4);
        Array.Copy(BitConverter.GetBytes(trustValue), 0, bytes, 10, 4);
        DeleventSystem.ChangePlayerTrust(playerIdReceiver, playerIdSender, trustValue);
        return bytes;        
    }

    public void SendTrustDecode(byte[] message)
    {
        int playerIdReceiver = BitConverter.ToInt32(message, 2);
        int playerIdSender = BitConverter.ToInt32(message, 6);
        int trustValue = BitConverter.ToInt32(message, 10);
        vSystem.GetParticipantById(playerIdReceiver).ReceiveTrust(playerIdSender, trustValue);
        vSystem.GetParticipantById(playerIdSender).GiveTrust(playerIdReceiver, trustValue);
        //vSystem.uiManager.UpdateVotingButtons();
        DeleventSystem.ChangePlayerTrust(playerIdReceiver, playerIdSender, trustValue);
    }

    public byte[] SubmitVotes(int playerIdSender, bool voteFinished, int phase)
    {
        var bytes = new byte[11];
        Array.Copy(BitConverter.GetBytes(signatureSubmitVotes), 0, bytes, 0, 2);
        Array.Copy(BitConverter.GetBytes(playerIdSender), 0, bytes, 2, 4);
        Array.Copy(BitConverter.GetBytes(phase), 0, bytes, 6, 4);
        Array.Copy(BitConverter.GetBytes(voteFinished), 0, bytes, 10, 1);
        return bytes;
    }

    public void SubmitVotesDecode(byte[] message)
    {
        int playerIdSender = BitConverter.ToInt32(message, 2);
        int phase = BitConverter.ToInt32(message, 6);
        bool voteFinished = BitConverter.ToBoolean(message, 10);
        if(phase == vSystem.gameState)
        {
            
            vSystem.GetParticipantById(playerIdSender).SetfinishedPhase(voteFinished);
            DeleventSystem.SubmitForPhase(playerIdSender, phase, voteFinished);
            if (vSystem.isServer && vSystem.gameState == 0)
            {
                DeleventSystem.OnClientReadyLobby(vSystem.sessionParticipants);
            }
        }      
    }

    public byte[] GameLoopProgress(int nextPhase)
    {
        var bytes = new byte[6];
        Array.Copy(BitConverter.GetBytes(signatureGameLoopProgress), 0, bytes, 0, 2);
        Array.Copy(BitConverter.GetBytes(nextPhase), 0, bytes, 2, 4);
        return bytes;
    }

    public void GameLoopProgressDecode(byte[] message)
    {
        vSystem.gameState = BitConverter.ToInt32(message, 2);
        DebugText.LogMessage("Triggering new GameState: " + BitConverter.ToInt32(message, 2).ToString());
        DeleventSystem.ChangeGameState(BitConverter.ToInt32(message, 2));
        DebugText.LogMessage("No errors in triggering: " + BitConverter.ToInt32(message, 2).ToString());
        
    }


    public byte[] SimpleString(string inputString)
    {
        Char[] chars = inputString.ToCharArray();
        int textLength = chars.Length;
        var bytes = new byte[(textLength * 2) + 6];
        Array.Copy(BitConverter.GetBytes(signatureSimpleString), 0, bytes, 0, 2);
        Array.Copy(BitConverter.GetBytes(textLength), 0, bytes, 2, 4);
        for (int i = 0; i < textLength; i++)
        {
            Array.Copy(BitConverter.GetBytes(chars[i]), 0, bytes, 6 + i*2, 2);
        }       
        return bytes;
    }

    public string SimpleStringDecode(byte[] message)
    {
        int textLength = BitConverter.ToInt32(message, 2);
        string result = "";
        for (int i = 0; i < textLength; i++)
        {
            result += BitConverter.ToChar(message, 6 + i*2);
        }
        return result;
    }

    public byte[] SynchronizePlayerList(List<NetworkParticipant> playerList)
    {
        int lengthOfByteArray = 6;
        foreach (var item in playerList)
        {
            //for each Player in the playerlist the length of the byte array gets increased by 4 (for the playerID), by 4 for the length of the name and for each character of the chosen name by 2 (each char gets converted into a byte array of length 2)
            lengthOfByteArray += 8 + item.GetName().Length * 2;
        }

        int numberPlayers = playerList.Count;
        var bytes = new byte[lengthOfByteArray];
        Array.Copy(BitConverter.GetBytes(signatureSynchronizePlayerList), 0, bytes, 0, 2);
        Array.Copy(BitConverter.GetBytes(numberPlayers), 0, bytes, 2, 4);
        
        int currentPosition = 6;
        foreach (var item in playerList)
        {
            string currentName = item.GetName();
            int currentId = item.GetPlayerId();
            Array.Copy(BitConverter.GetBytes(currentId), 0, bytes, currentPosition, 4);
            currentPosition += 4;

            Char[] chars = currentName.ToCharArray();
            int textLength = chars.Length;
            Array.Copy(BitConverter.GetBytes(textLength), 0, bytes, currentPosition, 4);
            currentPosition += 4;
            for (int i = 0; i < textLength; i++)
            {
                Array.Copy(BitConverter.GetBytes(chars[i]), 0, bytes, currentPosition, 2);
                currentPosition += 2;
            }
        }
        return bytes;
    }

    public void SynchronizePlayerListDecode(byte[] message)
    {
        int numberPlayers = BitConverter.ToInt32(message, 2);
        List<NetworkParticipant> newPlayerList = new List<NetworkParticipant>();
        int currentPosition = 6;
        for (int i = 0; i < numberPlayers; i++)
        {
            int currentId = BitConverter.ToInt32(message, currentPosition);
            currentPosition += 4;
            int nameLength = BitConverter.ToInt32(message, currentPosition);
            currentPosition += 4;
            
            string currentName = "";
            
            for (int j = 0; j < nameLength; j++)
            {
                currentName += BitConverter.ToChar(message, currentPosition);
                currentPosition += 2;
            }
            newPlayerList.Add(new NetworkParticipant(currentId, currentName));
        }
        foreach (var item in newPlayerList)
        {
            item.StartGame(newPlayerList.Count);
        }
        if(!vSystem.isServer)
            vSystem.sessionParticipants = newPlayerList;
        
        DeleventSystem.RefreshLobby(newPlayerList);
    }

    public byte[] IdZuweiser(int playerId)
    {
        var bytes = new byte[6];
        Array.Copy(BitConverter.GetBytes(signatureIdZuweiser), 0, bytes, 0, 2);
        Array.Copy(BitConverter.GetBytes(signatureIdZuweiser), 0, bytes, 2, 4);
        return bytes;
    }
    public void IdZuweiserDecode(byte[] message)
    {
        //
    }


    public byte[] SetVessel(int targetPlayerId, bool vesselStatus)
    { 
        var bytes = new byte[7];
        Array.Copy(BitConverter.GetBytes(signatureSetVessel), 0, bytes, 0, 2);
        Array.Copy(BitConverter.GetBytes(targetPlayerId), 0, bytes, 2, 4);
        Array.Copy(BitConverter.GetBytes(vesselStatus), 0, bytes, 6, 1);
        return bytes;
    }

    public void SetVesselDecode(byte[] message)
    {
        int targetPlayerId = BitConverter.ToInt32(message, 2);
        bool vesselStatus = BitConverter.ToBoolean(message, 6);
        vSystem.GetParticipantById(targetPlayerId).SetVessel(vesselStatus);
        if(vesselStatus)
            vSystem.GetParticipantById(targetPlayerId).SetVesselPoints(vSystem.GetParticipantById(targetPlayerId).GetVesselPoints()+1);
    }

    public byte[] VesselAction(int chosenActionType, int playerIdChosenTarget)
    {

        var bytes = new byte[10];
        Array.Copy(BitConverter.GetBytes(signatureVesselAction), 0, bytes, 0, 2);
        Array.Copy(BitConverter.GetBytes(chosenActionType), 0, bytes, 2, 4);
        Array.Copy(BitConverter.GetBytes(playerIdChosenTarget), 0, bytes, 6, 4);
        return bytes;
    }

    public void VesselActionDecode(byte[] message)
    {
        int chosenActionType = BitConverter.ToInt32(message, 2);
        int chosentargetPlayerIdActionType = BitConverter.ToInt32(message, 6);
        vSystem.EvaluatVesselAction(chosenActionType, chosentargetPlayerIdActionType);
    }

    public byte[] RoleAssignment(int role, int targetPlayer)
    {

        var bytes = new byte[10];
        Array.Copy(BitConverter.GetBytes(signatureRoleAssignment), 0, bytes, 0, 2);
        Array.Copy(BitConverter.GetBytes(role), 0, bytes, 2, 4);
        Array.Copy(BitConverter.GetBytes(targetPlayer), 0, bytes, 6, 4);
        return bytes;
    }

    public void RoleAssignmentDecode(byte[] message)
    {
        int role = BitConverter.ToInt32(message, 2);
        int targetPlayer = BitConverter.ToInt32(message, 6);

        vSystem.GetParticipantById(targetPlayer).SetRole(role);
    }

    public byte[] GameLoopData()
    {
        int _roundCounter = vSystem.roundCounter;
        bool _exorcismUsed = vSystem.exorcismUsed;
        var bytes = new byte[7];
        Array.Copy(BitConverter.GetBytes(signatureGameLoopData), 0, bytes, 0, 2);
        Array.Copy(BitConverter.GetBytes(_roundCounter), 0, bytes, 2, 4);
        Array.Copy(BitConverter.GetBytes(_exorcismUsed), 0, bytes, 6, 1);
        return bytes;        
    }

    public void GameLoopDataDecode(byte[] message)
    {
        vSystem.roundCounter = BitConverter.ToInt32(message, 2);
        vSystem.exorcismUsed = BitConverter.ToBoolean(message, 6);
        if (vSystem.roundCounter % vSystem.mooncycleLength == 0 && vSystem.roundCounter != 0)
        {
            //vSystem.exorcismUsed = false;
            if(!vSystem.isServer)
                vSystem.moonCounter++;
            vSystem.exorcisedEndOfMoonList = vSystem.exorcisedDebugList;
        }
    }

    public byte[] CandleHolderTransform(Vector3 position, int playerId)
    {
        var bytes = new byte[16];

        Array.Copy(BitConverter.GetBytes(signatureCandleHolderTransform), 0, bytes, 0, 2);
        /*//first rotaTION
        Array.Copy(BitConverter.GetBytes(rotation.x), 0, bytes, 4, 4);
        Array.Copy(BitConverter.GetBytes(rotation.y), 0, bytes, 8, 4);
        Array.Copy(BitConverter.GetBytes(rotation.z), 0, bytes, 12, 4);
        Array.Copy(BitConverter.GetBytes(rotation.w), 0, bytes, 16, 4);
        */
        //then position
        Array.Copy(BitConverter.GetBytes(position.x), 0, bytes, 2, 4);
        Array.Copy(BitConverter.GetBytes(position.y), 0, bytes, 6, 4);
        Array.Copy(BitConverter.GetBytes(position.z), 0, bytes, 10, 4);
        //and the playerId as an identifier
        Array.Copy(BitConverter.GetBytes(playerId), 0, bytes, 14, 2);



        return bytes;
    }

    public void CandleHolderTransformDecode(byte[] message)
    {
        //Quaternion candleholderRotation;
        //candleholderRotation = new Quaternion(BitConverter.ToSingle(message, 4), BitConverter.ToSingle(message, 8), BitConverter.ToSingle(message, 12), BitConverter.ToSingle(message, 12));
        Vector3 candleholderPosition;
        candleholderPosition = new Vector3(BitConverter.ToSingle(message, 2), BitConverter.ToSingle(message, 6), BitConverter.ToSingle(message, 10));
        int targetPlayerId = BitConverter.ToInt16(message, 14);
        if (targetPlayerId != vSystem.identity.GetPlayerId())
        {
            DeleventSystem.CandleReceived(candleholderPosition, targetPlayerId);
        }
    }

    public byte[] SynchronizePlayerData(int playerId)
    {
        var bytes = new byte[20];
        NetworkParticipant p = vSystem.GetParticipantById(playerId);
        bool finishedPhase = p.GetfinishedPhase();
        bool vessel = p.GetVessel();
        int vesselPoints = p.GetVesselPoints();
        int colorCode = p.GetColorCode();
        int role = p.GetRole();
        Array.Copy(BitConverter.GetBytes(signatureSynchronizePlayerData), 0, bytes, 0, 2);
        Array.Copy(BitConverter.GetBytes(playerId), 0, bytes, 2, 4);
        Array.Copy(BitConverter.GetBytes(finishedPhase), 0, bytes, 6, 1);
        Array.Copy(BitConverter.GetBytes(vessel), 0, bytes, 7, 1);
        Array.Copy(BitConverter.GetBytes(vesselPoints), 0, bytes, 9, 4);
        Array.Copy(BitConverter.GetBytes(colorCode), 0, bytes, 12, 4);
        Array.Copy(BitConverter.GetBytes(role), 0, bytes, 16, 4);

        return bytes;
    }

    public void SynchronizePlayerDataDecode(byte[] message)
    {
        int playerId = BitConverter.ToInt32(message, 2);
        bool finishedPhase = BitConverter.ToBoolean(message, 6);
        bool vessel = BitConverter.ToBoolean(message, 7);
        int vesselPoints = BitConverter.ToInt32(message, 8);
        int colorCode = BitConverter.ToInt32(message, 12);
        int role = BitConverter.ToInt32(message, 16);

        //Do stuff in vsystem
    }

}
