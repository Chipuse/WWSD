using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonEvents : MonoBehaviour
{
    public Votingsystem vSystem;
    
    public Text NetworkName;
    public Text ClientName;
    public Text Lobby;
    public GameObject ButtonStartServer;
    public GameObject ButtonStartClient;
    public GameObject ButtonStopNetwork;
    public GameObject ButtonP1;
    public GameObject ButtonP2;

    bool AutofillEnabled = false;
    public void P1ButtonPressed()
    {

    }

    private void Autofill()
    {
        NetworkName.text = "ass";
        ClientName.text = "u" + Random.Range(0, 100);
    }

    public void StartServerPressed()
    {
        if (AutofillEnabled)
        {
            vSystem.StartServer("server", "user" + Random.Range(0, 100).ToString());
        }
        else if (/*string.IsNullOrEmpty(NetworkName.text) ||*/ string.IsNullOrEmpty(ClientName.text))
        {
            Lobby.text = "Enter a network name & enter a client Name";
        }
        else
        {
            //DeleventSystem.StartingServer(true);
            vSystem.StartServer("server"/*NetworkName.text*/, ClientName.text);

        }
    }

    public void StartClientPressed()
    {

        if (AutofillEnabled)
        {
            vSystem.StartClient("server", "user" + Random.Range(0, 100).ToString());
        }
        else if(/*string.IsNullOrEmpty(NetworkName.text) ||*/ string.IsNullOrEmpty(ClientName.text))
        {
            Lobby.text = "Enter a network name & enter a client Name";
        }
        else
        {
            vSystem.StartClient("server"/*NetworkName.text*/, ClientName.text);
        }
    }

    private void Start()
    {
        //vSystem = GetComponent<Votingsystem>();
        DeleventSystem.RefreshLobby += DisplayLobby;
        DeleventSystem.DisplayMessageEvent += DisplayText;
        //Autofill();
    }
    private void Update()
    {
        if (vSystem.sessionParticipants != null && vSystem.gameState == 0)
            DisplayLobby(vSystem.sessionParticipants);
       
    }

    void DisplayLobby(List<NetworkParticipant> newPlayerList)
    {
        string newLobbyText = "Lobby:";
        foreach (var item in newPlayerList)
        {
            newLobbyText += "\r\n" + item.GetName();
        }
        Lobby.text = newLobbyText;
    }

    void DisplayText(string textTotDisplay)
    {
        Lobby.text = textTotDisplay;
    }

    public void OnButton(Button button)
    {
        switch (button.name)
        {
            case "ButtonStartServer":
                break;
        }
    }
}
