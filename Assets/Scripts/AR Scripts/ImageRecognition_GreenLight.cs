using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.ARFoundation;
using UnityEngine.UI;


public class ImageRecognition_GreenLight : MonoBehaviour
{

    [SerializeField] GameObject spawnedCube;

    // [SerializeField] GameObject canvasSpawnCenter; 

    // [SerializeField] GameObject canvasCandleSetUp; 


    GameObject centerGameObject;

    GameObject MyCandleGO;
    bool prefabSpawned;
    [SerializeField]float distanceThreshold = 0.05f;
    float actualDistance;


    Transform sceneTRCenterStart;
    Transform sceneTRCenterUpdated;

    Vector3 sceneCenterStart;
    Vector3 sceneCenterUpdated;

    Quaternion sceneRotCenterStart;
    Quaternion sceneRotCenterUpdated; 

    private ARTrackedImageManager _arTrackedImageManager;

    Vector3 localCandleOffset;

    Vector3 position;
    Quaternion rotation;

    Vector3 directionMyCandleCenter;
    Vector3 directionOtherCandleCenter;

    float[] anglesNotOrdered;
    float[] anglesOrdered;
    public int[] idsOrdered;

    public Vector3 SceneCenterStart { get => sceneCenterStart; set => sceneCenterStart = value; }
    public Vector3 SceneCenterUpdated { get => sceneCenterUpdated; set => sceneCenterUpdated = value; }
    public GameObject CenterGameObject { get => centerGameObject; set => centerGameObject = value; }
    public Quaternion SceneRotCenterUpdated { get => sceneRotCenterUpdated; set => sceneRotCenterUpdated = value; }
    public Quaternion SceneRotCenterStart { get => sceneRotCenterStart; set => sceneRotCenterStart = value; }

    


    public static ImageRecognition_GreenLight Instance;

    void Awake(){ 
        if (Instance == null)
        {
            Instance = this;
        }


        _arTrackedImageManager = FindObjectOfType<ARTrackedImageManager>();
        DeleventSystem.ChangeGameState += StartSceneSetUp;
        prefabSpawned=false;
        
    }

    void OnEnable(){
        _arTrackedImageManager.trackedImagesChanged += OnImageChanged;
    }

    void OnDisable(){
        _arTrackedImageManager.trackedImagesChanged -= OnImageChanged;
    }

    public void OnImageChanged(ARTrackedImagesChangedEventArgs args){
       
        foreach (var trackedImage in args.added)
        {
            sceneTRCenterStart = trackedImage.transform;
            trackedImage.destroyOnRemoval = false;

            GameObject.FindObjectOfType<DebugText>().EnableDebugger(true);
            SpawnUpdateSceneCenter();            
        }

        foreach (var trackedImage in args.updated)
        {
            sceneTRCenterUpdated = trackedImage.transform;
            if (Votingsystem.Instance.gameState == 0){
                ScannedTheQR();
            }
        }

        foreach (var trackedImage in args.removed)
        {
            DebugText.LogMessage("Image Removed");
        }
    }

    // Sets the scene center where the QR is/was
    public void SpawnUpdateSceneCenter(){
        
        // DebugText.LogMessage("GO SPAWN");
        
        if (prefabSpawned == false){

            position = sceneTRCenterStart.position;
            //NEW to fix center flip
            rotation = Quaternion.identity;
            rotation.eulerAngles = new Vector3( rotation.eulerAngles.x, sceneTRCenterStart.rotation.eulerAngles.y, rotation.eulerAngles.z);
            centerGameObject = Instantiate(spawnedCube, position, rotation);
            prefabSpawned = true;
        }else{

            position = sceneTRCenterUpdated.position;
            //New to fix center flip
            rotation.eulerAngles = new Vector3( rotation.eulerAngles.x, sceneTRCenterStart.rotation.eulerAngles.y, rotation.eulerAngles.z);

            //Udating center position
            centerGameObject.transform.position = position;
            centerGameObject.transform.rotation = rotation;

            //MAKES THE SPAWNING OF CANDLES RELATIVE TO THE CENTER AND UPDATES THEM WITH IT.
            //Local candle
            // if (localCandleOffset != Vector3.zero){
                
            //     var localWorldOffset = rotation * localCandleOffset;
            //     var localMovePosition = position + localWorldOffset;

            //     // DebugText.LogMessage("MyCandlePosition is "  + MyCandleGO.transform.position.ToString());

            //     MyCandleGO.transform.position = localMovePosition;
            // }
            
            // //Players candles
            // OtherPlayersCandles sOther = this.GetComponent<OtherPlayersCandles>();
            // foreach (var item in sOther.playersCandles)
            // {   
            //     var worldOffset = rotation * item.offset;
            //     var spawnPosition = position + worldOffset;
            //     //CHANGED THIS SO THE CANDLES WONT FLOAT
            //     spawnPosition.y = item.offset.y;

            //     item.playerCandle.transform.position = spawnPosition;

            //     DebugText.LogMessage("Updated Position of other candles from: "  + item.offset + "to: " + spawnPosition);
            // }

        }
         
    }

    public void SetReferenceToMyCandle(GameObject go, Vector3 pos){
        MyCandleGO = go;
        localCandleOffset = pos;
        //DebugText.LogMessage("Set up reference of: "  + MyCandleGO.name.ToString() + ", Posicion: " + pos.ToString());
    }

    void StartSceneSetUp(int gameState){
        if (gameState == 1){
            // canvasSpawnCenter.SetActive(true);            
        }else{
            //PROVISIONAL
            // MyCandleGO.gameObject.SetActive(false);
        }
    }

    void Update(){
        //SO it updates automatically if the distance is longer than the threshold #NEW
        if (prefabSpawned == true){
            actualDistance = Vector3.Distance(centerGameObject.transform.position, sceneTRCenterUpdated.transform.position);
            if (actualDistance > distanceThreshold){
                SpawnUpdateSceneCenter();
            }
        }
        //NEW IMPLEMENTATION SO THE BOARD DOES NOT FLIP
        

        
    }

    void ScannedTheQR(){
        DeleventSystem.OnQRCodeScanned();
    }

    public int[] GetOrderOfCandleHolders()
    {       
        directionMyCandleCenter = CenterGameObject.transform.position - MyCandleGO.transform.position;
        directionMyCandleCenter.y = 0;

        OtherPlayersCandles sOther = this.GetComponent<OtherPlayersCandles>();
        int i = 0;
        anglesNotOrdered = new float[(sOther.playersCandles.Count + 1)]; 
        bool foundmyId = false;

        foreach (var item in sOther.playersCandles)
        {
            if (i == PlayerInfoManager.Instance.ownID){
                anglesNotOrdered[i] = 360;
                foundmyId = true;
                i++;
            }
            
            directionOtherCandleCenter = CenterGameObject.transform.position - item.playerCandle.transform.position;
            directionOtherCandleCenter.y = 0;

            anglesNotOrdered[i] = SignedAngleBetween(directionMyCandleCenter, directionOtherCandleCenter, Vector3.up);
            
            
            i++;
        }
        if (foundmyId == false){
            anglesNotOrdered[i] = 360;
        }


        anglesOrdered = (float[])anglesNotOrdered.Clone();
        System.Array.Sort(anglesOrdered);

        int k = 0;
        idsOrdered = new int[anglesNotOrdered.Length]; 

        foreach (float or in anglesOrdered)
        {
            for (int j = 0; j < anglesNotOrdered.Length; j++)
            {
                if (or == anglesNotOrdered[j]){
                    idsOrdered[k] = j;
                    DebugText.LogMessage("Ids ordered " + idsOrdered[k].ToString());
                    k++;
                }
            }
        }
        
        return idsOrdered;
        
    }

    float SignedAngleBetween(Vector3 a, Vector3 b, Vector3 n){
        
        float angle = Vector3.Angle(a,b);
        float sign = Mathf.Sign(Vector3.Dot(n,Vector3.Cross(a,b)));

        float signed_angle = angle * sign;

        float angle360 = signed_angle;
        if (signed_angle < 0){
            angle360 = 360 + signed_angle;
        }

        return angle360;
    }
}


