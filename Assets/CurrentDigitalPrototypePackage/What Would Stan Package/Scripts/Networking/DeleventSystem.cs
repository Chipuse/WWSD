using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleventSystem : MonoBehaviour
{
    public static DeleventSystem Instance;
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        
    }

    public delegate void ButtonChangePlayerTrust(int targetPlayerId, int senderPlayerId, int trustValue);
    public static ButtonChangePlayerTrust ChangePlayerTrust = delegate { }; //now in Usage!!!!!!!!!! 

    public delegate void ButtonSubmitTrust(int senderPlayerId, int gameState, bool ready);
    //Will be triggered everytime the FinishedPhaseValue  gets synchronized over the network. 
    //gameState is always the phase from which the call originates
    //this event will not be triggered if it is lingering from a phase which is NOT the same as the current phase on this device
    public static ButtonSubmitTrust SubmitForPhase = delegate { };

    public delegate void StartingServerEvent(bool failed);
    public static StartingServerEvent StartingServer = delegate { };

    public delegate void StartingClientEvent(bool failed);
    public static StartingClientEvent StartingClient = delegate { };

    public delegate void OnNewPlayerlist(List<NetworkParticipant> playerList);
    public static OnNewPlayerlist RefreshLobby = delegate { };
    public static OnNewPlayerlist OnClientReadyLobby = delegate { };

    public delegate void OnUpdatePlayerData();
    public static OnUpdatePlayerData UpdatePlayerData = delegate { }; //not in usage

    public delegate void OnChangeGameState(int newGameState);
    //newGameState defs:
    //0 = lobbyphase
    //1 = rolephase
    //2 = votingphase
    //3 = vesselphase
    //4 = endphase
    //5 = witchesWonphase
    //6 = satanistsWonphase
    public static OnChangeGameState ChangeGameState = delegate { };

    public delegate void OnMessageToDisplay(string newMessage);
    public static OnMessageToDisplay DisplayMessageEvent = delegate { };

    public delegate void OnCandleHolderPlaced(Vector3 position);
    public static OnCandleHolderPlaced CandlePlacedLocalPlayer = delegate { };

    public delegate void OnCandleHolderTransformReceived(Vector3 position, int PlayerId);
    public static OnCandleHolderTransformReceived CandleReceived = delegate { };

    public delegate void OnUpdatePlayerTrust(int playertId, int trustAmount);
    public static OnUpdatePlayerTrust LitPlayersCandle = delegate { };

    public delegate void ARTrigger();
    public static ARTrigger OnQRCodeScanned = delegate { };
    
    
    public static ARTrigger OnCandlePlacePreview = delegate {  };
    public static ARTrigger OnCandlePlacementCanceled = delegate {  };
    public static ARTrigger OnCandlePlacementSubmit = delegate {  };

    public static ARTrigger OnPhoneResting = delegate { };

    public delegate void ARTriggerBool(bool b);
    public static ARTriggerBool HideOwnCandle = delegate {  };

    public delegate void ARTriggerVector3(Vector3 v);

    public static ARTriggerVector3 SetLocalCandlePosition = delegate {  };
    

    //Example of creating an event in this class:
    //public delegate void EnemyHitDelegate(GameObject enemy);
    //public static EnemyHitDelegate EnemyHit = delegate { };

    //To trigger an event (in any other class):
    //DeleventSystem.EnemyHit(gameObject);

    //To subscribe to event (in any other class):
    //DeleventSystem.EnemyHit += OnHitExplosion;
    //+
    //void OnHitExplosion(GameObject enemy)
    //{
    //    EnemyHandler eh = enemy.GetComponent<EnemyHandler>();
    //    if (eh != null)
    //        eh.LittleExplosion();
    //}

    
}
