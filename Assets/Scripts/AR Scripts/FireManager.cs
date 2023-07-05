using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FireManager : MonoBehaviour
{    public GameObject[] objectsToBeHidden;
    [SerializeField]GameObject[] allFires;
    GameObject[] fires;

    [SerializeField]GameObject uneven;
    [SerializeField]GameObject twoP;
    [SerializeField]GameObject fourP;
    [SerializeField]GameObject sixP;


    // [SerializeField]Material[] discMaterials;
    [SerializeField]GameObject[] Discs;


    int candleId = 0;
    int amountTrustLocal = 0;
    
    ParticleSystem[] pS;

    [SerializeField]Gradient colorGradient;

    bool showingTr;

    bool candleHidden;

    float timer;

    int otherPlayersCount;

    [SerializeField]Text amountText;
    [SerializeField]Canvas canvasTopOfCandle;


    Material ogMat;
    Material instMat;

    int numberOfP;


    public int CandleId { get => candleId; set => candleId = value; }
    public void SetCandleId(int newId)
    {
        candleId = newId;
        DebugText.LogMessage("After Assigning Id" + candleId.ToString());

        if (candleId != -1){
            DebugText.LogMessage("Setting Gradient");
            SetGradient();
            DebugText.LogMessage("Setting Color");
            FireColor(colorGradient);
            DebugText.LogMessage("Setting Materials");
            SetDiscMaterials();
            DebugText.LogMessage("Setting Candles to be displayed");
            SetCandlesToBeDisplayed();

        }
        
        
    }

    private void SetGradient()
    {
        // colorGradient = Votingsystem.Instance.GetColorGradientByColorCode(candleId); HERE PICKS THE COLOR OF THE GRADIENTS DETERMINED IN VOTING SYSTME
        Color color1 = PlayerInfoManager.Instance.iconColors[candleId];
        Color color2 = PlayerInfoManager.Instance.iconColors2[candleId];

        GradientColorKey[] colorKey = new GradientColorKey[3];
        colorKey[0].color = color1;
        colorKey[0].time = 0.0f;
        colorKey[1].color = color2;
        colorKey[1].time = 0.3f;
        colorKey[2].color = Color.black;
        colorKey[2].time = 1.0f;

        GradientAlphaKey[] alphaKey = new GradientAlphaKey[3];
        alphaKey[0].alpha = 0.0f;
        alphaKey[0].time = 0.0f;
        alphaKey[1].alpha = 1.0f;
        alphaKey[1].time = 0.115f;
        alphaKey[2].alpha = 1.0f;
        alphaKey[2].time = 1.0f;

        colorGradient.SetKeys(colorKey, alphaKey);
    }

     private void SetDiscMaterials()
    {
        ogMat = Discs[0].GetComponent<Renderer>().material;
        instMat = new Material(ogMat);
        instMat.mainTexture = PlayerInfoManager.Instance.icons[candleId].texture;


        foreach (var d in Discs)
        {   
            d.GetComponent<Renderer>().material = instMat;
            
        }
        // Renderer[] mRenderer = transform.Find("Textures").GetComponentsInChildren<Renderer>();
        // instMat.SetTexture("_MainTex",PlayerInfoManager.Instance.icons[candleId].texture);
    }

    void Start(){
        DeleventSystem.LitPlayersCandle += FireUpdate;
        // DeleventSystem.LitPlayersCandle(0, 3);

        pS = GetComponentsInChildren<ParticleSystem>();

        // canvasTopOfCandle = transform.GetComponentInChildren<Canvas>();
        // amountText = transform.Find("Preview").transform.Find("Trust").GetComponentInChildren<TextMeshProUGUI>();

        //FOR COACHING
        
        //FireColor(colorGradient);
        DeleventSystem.SubmitForPhase += OnSubmitEvent;
        DeleventSystem.ChangeGameState += OnGameStateChangeEvent;
        DeleventSystem.HideOwnCandle += OnEventHideOwnCandle;

        ogMat = GetComponent<Renderer>().material;


    }

    void Update(){
        amountText.text = amountTrustLocal.ToString();
        if (showingTr){
            timer -= Time.deltaTime;
            if (timer <= 0 && candleHidden == false){
                HideTrust();
            }
        }
    }

    void FireUpdate(int id, int amount){
        if (id == candleId){
            amountTrustLocal = amount;
        }
        if (id != -1){ //NEW
            if (id == candleId && candleHidden == false){
                int i = 0;
                // DebugText.LogMessage("The amount of trust: " + amount);
                // DebugText.LogMessage("The size of fire array: " + fires.Length.ToString());
                //New to be adapted for different players
                foreach (GameObject fire in fires)
                {
                    if (i >=  amount){
                        fire.transform.Find("FireAnim").gameObject.SetActive(false);
                        fire.transform.Find("CandleLit").gameObject.SetActive(false);
                        fire.transform.Find("CandleNoLIt").gameObject.SetActive(true); 
                    
                    }else{
                        fire.transform.Find("FireAnim").gameObject.SetActive(true);
                        fire.transform.Find("CandleLit").gameObject.SetActive(true);
                        fire.transform.Find("CandleNoLIt").gameObject.SetActive(false);
                    }
                    i++;
                }
            }
        }
        
    }

    void FireColor(Gradient g){
        pS = GetComponentsInChildren<ParticleSystem>();
        foreach (ParticleSystem p in pS)
        {
            var x = p.colorOverLifetime;

            x.color = g;
        }
    }

    public void ShowTrust(){
        canvasTopOfCandle.gameObject.SetActive(true);
        timer = 3;
        showingTr = true;
    }

    void HideTrust(){
        canvasTopOfCandle.gameObject.SetActive(false);
        showingTr = false;
    }

    void OnDestroy(){
        DeleventSystem.LitPlayersCandle -= FireUpdate;
    }

    public void HideCandle(){
        //Every child in candle
        DebugText.LogMessage("This is hide candle");
        foreach (Transform child in transform){
            child.gameObject.SetActive(false);
        }

        //NEW

        candleHidden = true;
    }

    public void OnEventHideOwnCandle(bool b){
        if ( candleId == -1 ){
            if (b == true){
                // DebugText.LogMessage("HIDE YOUR OWN CANDLE");
                HideCandle();
            }else{
                ShowCandle();
            }
        }
    }

    public void ShowCandle(){
        
        // DebugText.LogMessage("This is show candle");
        foreach (Transform child in transform)
        {
            if (child.name == "Branchs"){
                DebugText.LogMessage("From ShowCandle" + numberOfP + " , from candle: " + candleId);
                child.gameObject.SetActive(true);
                ShowBranchDependingOnCandles(numberOfP);
                // DebugText.LogMessage("After Branchs");

            }else if(child.name == "uneven"){
                // DebugText.LogMessage("finding uneven before if");
                if (otherPlayersCount % 2 == 0){
                    uneven.gameObject.SetActive(false);
                    
                }else{
                    uneven.gameObject.SetActive(true);
                }
            }else if(child.name == "Fires"){
                // DebugText.LogMessage("showing all the fires");
                child.gameObject.SetActive(true);
                // foreach (var item in fires)
                // {
                //     item.gameObject.SetActive(true);
                // }
            }else{
                // DebugText.LogMessage("showing all else");
                child.gameObject.SetActive(true);
            }
        }
        candleHidden = false; 
        // DebugText.LogMessage("before calling fire update");
        FireUpdate(candleId, amountTrustLocal);
    }

    public void OnSubmitEvent(int senderPlayerId, int gameState, bool ready)
    {
        if(senderPlayerId == candleId)
        {
            if(gameState == 2)
            {
                if (ready)
                {
                    HideCandle();
                }
                else
                {
                    ShowCandle();
                }
            }
        }
    }
    public void OnGameStateChangeEvent(int gameState)
    {
        if(gameState == 2)
        {
            ShowCandle();
        }
        else if(gameState == 3)
        {
            // DebugText.LogMessage("HIDE CANDLE THROUGHT GAME CHANGE EVENT");
            HideCandle();
        }
    }

    void SetCandlesToBeDisplayed()
    { // NEWWWW
        // DebugText.LogMessage("This is set candles to be displayed");
        
        foreach (var item in allFires)
        {
            item.SetActive(false);
        }
        // DebugText.LogMessage("1st loop");
        otherPlayersCount = Votingsystem.Instance.sessionParticipants.Count - 1;
        numberOfP = otherPlayersCount;

        fires = new GameObject[otherPlayersCount];
        // DebugText.LogMessage("OtherPlayersCount = " + otherPlayersCount);
        // DebugText.LogMessage("Before if");
        if (otherPlayersCount % 2 == 0)
        {
            uneven.gameObject.SetActive(false);

            int k = 3 - (otherPlayersCount / 2);
            int j = 0;
            
            for (int i = k; i < 7 - k; i++)
            {
                if (i != 3)
                {
                    // DebugText.LogMessage("Assigning the values of the array in 2nd loop");
                    fires[j] = allFires[i];
                    j++;
                }
            }
            // DebugText.LogMessage("2nd loop");

        }
        else
        {
            uneven.gameObject.SetActive(true);

            if (otherPlayersCount == 1){
                // DebugText.LogMessage("Assigning fires only once coz there is only one other player");
                fires[0] = allFires[3];
            }else{
                int k = 3 - ((otherPlayersCount - 1) / 2);
                int j = 0;
                // DebugText.LogMessage("Before for 2nd loop");
                for (int i = k; i < 7 - k; i++)
                {
                    fires[j] = allFires[i];
                    j++;
                }
            }

            

            // DebugText.LogMessage("3rd loop");
        }

        foreach (var item in fires)
        {
            item.SetActive(true);
        }

        //  DebugText.LogMessage("4th loop");


        DebugText.LogMessage("From Set candles to be displayed: " + otherPlayersCount + " , from candle: " + candleId);
        ShowBranchDependingOnCandles(otherPlayersCount);

    }

    private void ShowBranchDependingOnCandles(int o)
    { 
        if (o <= 1)
        {
            DebugText.LogMessage("1 player or less: players: " + o + " , from candle: " + candleId);
            twoP.gameObject.SetActive(false);
            fourP.gameObject.SetActive(false);
            sixP.gameObject.SetActive(false);
        }
        else if (o <= 3)
        {
                      //  DebugText.LogMessage("3 players or 2: players: " + o + " , from candle: " + candleId);

            twoP.gameObject.SetActive(true);
            
          //  DebugText.LogMessage("Two state: " + twoP.activeSelf.ToString());
            fourP.gameObject.SetActive(false);
           // DebugText.LogMessage("Four state: " + fourP.activeSelf.ToString());
            sixP.gameObject.SetActive(false);
           // DebugText.LogMessage("Six state: " + sixP.activeSelf.ToString());
           // DebugText.LogMessage("end of else if");
        }
        else if (o <= 5)
        {
           // DebugText.LogMessage("4 players or 5: players: " + o + " , from candle: " + candleId);
            twoP.gameObject.SetActive(false);
            fourP.gameObject.SetActive(true);           
            sixP.gameObject.SetActive(false);
        }
        else
        {
            //DebugText.LogMessage("6 players or 7: players: " + o + " , from candle: " + candleId);
            twoP.gameObject.SetActive(false);
            fourP.gameObject.SetActive(false);
            sixP.gameObject.SetActive(true);            
        }
    }
}
