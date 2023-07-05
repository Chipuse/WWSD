using Shatalmic;

public class NetworkParticipant
{
    private string name;
    private int playerId;
    private Networking.NetworkDevice device;
    private bool connected;
    private bool server;

    private bool finishedPhase;
    private bool vessel;
    private int vesselPoints;
    private int colorCode;

    //private Vector3 candlePosition;

    //1 equals witch, 2 equals satanist, 3 equals possesed witch
    //0 means no role is assigned yet
    private int role;

    public int[] trust;
    public int[] trustGiven;

    //maybe more constructors
    public NetworkParticipant(string _name, Networking.NetworkDevice _device = null, bool _server = false)
    {
        name = _name;
        playerId = 0;
        device = _device;
        
        connected = true;
        server = _server;
        role = 1;
        vesselPoints = 0;
        colorCode = 0;
    }

    public NetworkParticipant(int _playerId, string _name)
    {
        name = _name;
        playerId = _playerId;
        device = null;

        connected = true;
        server = false;
        role = 1;

        vesselPoints = 0;
        colorCode = 0;
    }

    //resets player for voting phase
    public void ResetPlayerTrust()
    {
        for (int i = 0; i < trust.Length; i++)
        {
            if (i == playerId)
                trust[i] = 0;
            else
                trust[i] = 1;
        }
        for (int i = 0; i < trustGiven.Length; i++)
        {
            if (i == playerId)
                trustGiven[i] = 0;
            else
                trustGiven[i] = 1;
        }
    }
    public void StartGame(int playerCount)
    {
        //finishedPhase = false;
        trust = new int[playerCount];
        trustGiven = new int[playerCount];
        for (int i = 0; i < playerCount; i++)
        {
            trust[i] = 0;
            trustGiven[i] = 0;
        }
    }

    public void GiveTrust(int _playerId, int newTrust)
    {
        if (playerId != _playerId)
            trustGiven[_playerId] = newTrust;
    }

    public void ReceiveTrust(int _playerId, int newTrust)
    {
        if(playerId != _playerId)
        {
            trust[_playerId] = newTrust;
        }
    }


    public string GetName(){return name;}

    public void SetPlayerId(int _playerId){playerId = _playerId;}
    public int GetPlayerId() { return playerId; }

    public void SetNewtworkDevice(Networking.NetworkDevice _device){device = _device;}
    public Networking.NetworkDevice GetNetworkDevice() {return device; }

    

    public void SetServer(bool _server) { server = _server; }
    public bool GetServer() { return server; }

    public void SetVessel(bool _vessel) { vessel = _vessel; }
    public bool GetVessel() { return vessel; }

    public void SetConnected(bool _connected) { connected = _connected; }
    public bool GetConnected() { return connected; }

    public void SetfinishedPhase(bool _finishedPhase) { finishedPhase = _finishedPhase; }
    public bool GetfinishedPhase() { return finishedPhase; }

    public void SetRole(int _role) { role = _role; }
    public int GetRole() { return role; }

    public void SetVesselPoints(int _vesselPoints) { vesselPoints = _vesselPoints; }
    public int GetVesselPoints() { return vesselPoints; }

    public void SetColorCode(int _colorCode) { colorCode = _colorCode; }
    public int GetColorCode() { return colorCode; }



}
