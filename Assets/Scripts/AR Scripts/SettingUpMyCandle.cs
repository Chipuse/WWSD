using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingUpMyCandle : MonoBehaviour
{
    [SerializeField]GameObject candlePreviewPrefab;
    [SerializeField]GameObject candleObjectPrefab;
    [SerializeField]GameObject mySymbolGO;
    public Camera cam;

    [HideInInspector]public GameObject candleInScene;
    GameObject previewInScene;

    Vector3 posCandleRelToCenter;

    ImageRecognition_GreenLight ImageRecognition;

    bool previewIns = false;
    bool candleIns = false;
    bool fixedPos;
    bool mySymbolInst = false;
    bool CandleToPreviewDone;

    bool settingCandleUpState;

    public static SettingUpMyCandle Instance;

    


    public Vector3 PosCandleRelToCenter { get => posCandleRelToCenter; set => posCandleRelToCenter = value; }

    void Awake(){

        if(Instance==null){

            Instance=this;
        }
        else{
            Destroy(this);
        }
        //cam = FindObjectOfType<Camera>();
        ImageRecognition = GameObject.FindObjectOfType<ImageRecognition_GreenLight>();

        DeleventSystem.OnCandlePlacePreview += CandleToPreview;
        DeleventSystem.OnCandlePlacementSubmit += SendDataFromCandle;
        DeleventSystem.OnCandlePlacementCanceled +=  CancelCandleHolderPlacement;
        DeleventSystem.OnQRCodeScanned += ChangeBooleanState;
        DeleventSystem.ChangeGameState += SetMySymbolGameObject;
        // candleInScene = GameObject.Find("MyCandlePreview");


        
    }

    void Update(){
        
        // DebugText.LogMessage(CandleToPreviewDone.ToString() + settingCandleUpState.ToString());
        if ( settingCandleUpState == true){//Fixed Pos
            if (CandleToPreviewDone == false){
                
                Ray ray = cam.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));
                RaycastHit hitObject;
                if(Physics.Raycast(ray, out hitObject))
                {   //DebugText.LogMessage("Sending Raycast");
                    if(hitObject.transform.tag == "Surface")
                    {
                        if (previewIns == false){
                            InstantiatePreview(hitObject.point);

                        }else{
                            previewInScene.transform.position = hitObject.point;

                        }
                        
                        
                    }
                }
            }
     
        }

    }

    public void CandleToPreview(){
        CandleToPreviewDone = true;

        Vector3 pos = previewInScene.transform.position;
        Quaternion rot = previewInScene.transform.rotation;

        Vector3 posCenter = ImageRecognition.CenterGameObject.transform.position;
        posCenter.y = 0;

        Vector3 dir = posCenter - pos;
        dir.y = 0;

        // float angle = Vector3.Angle(dir, candleInScene.transform.forward);

       
        // DebugText.EnableDebuggerStatic(true);
        Destroy(previewInScene.gameObject);
        previewIns = false;

        if (candleIns == false){
            
            candleInScene = Instantiate(candleObjectPrefab, pos, Quaternion.LookRotation(dir), ImageRecognition_GreenLight.Instance.CenterGameObject.transform);
            candleIns = true;
            DebugText.LogMessage("After Instantiate");
            
            DebugText.LogMessage("BeforeAssigning Id");
            candleInScene.GetComponent<FireManager>().SetCandleId(-1);
            
        }else{
            candleInScene.transform.position = pos;
            candleInScene.transform.rotation = Quaternion.LookRotation(dir);
        }  

    }

    void InstantiatePreview(Vector3 tr){

        previewInScene = Instantiate(candlePreviewPrefab, tr, Quaternion.identity);

        previewIns = true;

    }

    public void SendDataFromCandle(){
        //Set the transform of the candle relative to the object in the center
        fixedPos = true;

        posCandleRelToCenter = ImageRecognition.CenterGameObject.transform.InverseTransformPoint(candleInScene.transform.position);
        // posCandleRelToCenter.y = ImageRecognition.CenterGameObject.transform.position.y; //NEW TO FIX FLIPPING CANDLES ?¿
        // DebugText.LogMessage("Position relative to center: "  + posCandleRelToCenter.ToString());

        // DebugText.LogMessage("BEFORE");
        ImageRecognition.SetReferenceToMyCandle(candleInScene, posCandleRelToCenter);
        // DebugText.LogMessage("AFTER");

        settingCandleUpState = false;


        //HERE PROBLEM WITH SEEING OTHER PLAYERS CANDLES

        //DeleventSystem.CandlePlacedLocalPlayer(posCandleRelToCenter);
        DeleventSystem.SetLocalCandlePosition(posCandleRelToCenter);

        // DebugText.LogMessage("Sent data from candle");

    }

    public void CancelCandleHolderPlacement(){
        CandleToPreviewDone = false;
        settingCandleUpState = true;

    }



    public Vector3 GetTheCPositionRelativeToCenter(){
        return posCandleRelToCenter;
    }

    void ChangeBooleanState(){
        settingCandleUpState = true;
    }

    void SetMySymbolGameObject(int newPhase){
        if (newPhase == 1){

            if (mySymbolInst == false){
                Vector3 posCenter = ImageRecognition.CenterGameObject.transform.position;
                posCenter.y = 0;

                Vector3 dir = posCenter - candleInScene.transform.position;
                dir.y = 0;

                GameObject symb = Instantiate(mySymbolGO, candleInScene.transform.position, Quaternion.LookRotation(dir), ImageRecognition_GreenLight.Instance.CenterGameObject.transform);

                Transform textureGO = symb.transform.Find("Texture");

                Material ogMat = textureGO.GetComponent<Renderer>().material;

                Material instMat = new Material(ogMat);

                instMat.mainTexture = PlayerInfoManager.Instance.icons[Votingsystem.Instance.identity.GetPlayerId()].texture;
                
                textureGO.GetComponent<Renderer>().material = instMat;

                mySymbolInst = true;
            }
                
        }

        
    }
    
}
