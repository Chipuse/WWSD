using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OtherPlayersCandles : MonoBehaviour
{
    [SerializeField]GameObject OtherPlayersCandlesPrefab;
    
    // List<Vector3> candlePos;

    public struct PlayerCandleData{
        public GameObject playerCandle;
        public Vector3 offset;
    }

    public List<PlayerCandleData> playersCandles;
    // public List<GameObject> PlayersCandles { get => playersCandles; set => playersCandles = value; }

    void Awake(){
        DeleventSystem.CandleReceived += CandlePlaceFromPlayer; 
        playersCandles = new List<PlayerCandleData>();
    }
   public void CandlePlaceFromPlayer(Vector3 newPos, int player){

        Transform center = this.GetComponent<ImageRecognition_GreenLight>().CenterGameObject.transform;

        var localOffset = newPos;
        var worldOffset = center.rotation * localOffset;
        var spawnPosition = center.position + worldOffset;

        Vector3 dir = center.position - spawnPosition;

        PlayerCandleData newStruct;
        newStruct.offset = newPos;
        DebugText.LogMessage("Created Candle: " + player.ToString());

        Quaternion rot = Quaternion.LookRotation(dir); // NEW TO FIX FLIP CANDLES
        rot.eulerAngles = new Vector3( center.eulerAngles.x, rot.eulerAngles.y , center.eulerAngles.z);

        newStruct.playerCandle = Instantiate(OtherPlayersCandlesPrefab, spawnPosition, Quaternion.LookRotation(dir), this.GetComponent<ImageRecognition_GreenLight>().CenterGameObject.transform); // MAKE IT CHILD OF CENTER GAME OBJECT
        
        newStruct.playerCandle.GetComponent<FireManager>().SetCandleId(player);

        
        playersCandles.Add(newStruct);
    }
}
