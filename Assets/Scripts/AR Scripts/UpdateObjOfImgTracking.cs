using System.Collections.Generic;

using UnityEngine;

using UnityEngine.EventSystems;

using UnityEngine.UI;

using UnityEngine.XR.ARFoundation;

using UnityEngine.XR.ARSubsystems;

//changed smth

[RequireComponent(typeof(ARRaycastManager))]

public class UpdateObjOfImgTracking : MonoBehaviour

{



    [SerializeField]

    private Button changeButton;

    [SerializeField]

    private GameObject FirePrefab;

    private GameObject FireGO;

    private GameObject selectionGO;

    private GameObject boundingArea;

    [SerializeField]

    private Material boundingAreaMaterial;

    private bool selectionStatus;



    private GameObject placedObject;



    private Vector2 touchPosition = default;



    private ARRaycastManager arRaycastManager;



    private ARSessionOrigin aRSessionOrigin;



    private bool onTouchHold = false;



    private static List<ARRaycastHit> hits = new List<ARRaycastHit>();



    //private PlacementObject lastSelectedObject;





    void Awake() 

    {

        arRaycastManager = GetComponent<ARRaycastManager>();

        aRSessionOrigin = GetComponent<ARSessionOrigin>();

        changeButton.onClick.AddListener(ChangeInstance);

    }



    void Start(){

        changeButton.enabled = false;

    }



    private void ChangeInstance()

    {

        GameObject FirePos = selectionGO.transform.Find("FirePos").gameObject;

        if (FireGO != null){

            if (FireGO.activeSelf){

                FireGO.SetActive(false);

            }else{

                FireGO.SetActive(true);

            }

        }else{

            FireGO = Instantiate(FirePrefab, FirePos.transform.position, FirePos.transform.rotation, selectionGO.transform);

        }

        

    }



    void Update()

    {

        if(Input.touchCount > 0)

        {

            Touch touch = Input.GetTouch(0);



            touchPosition = touch.position;



            if(touch.phase == TouchPhase.Began)

            {

                Ray ray = Camera.main.ScreenPointToRay(touch.position);

                RaycastHit hitObject;

                if(Physics.Raycast(ray, out hitObject))

                {

                    selectionGO = hitObject.transform.gameObject;



                    if (selectionGO != null){

                        selectionStatus = false;

                    }else{

                        selectionStatus = true;

                    }

                }



                if(arRaycastManager.Raycast(touchPosition, hits, TrackableType.Planes))

                {

                    

                }

            }  

        }



        AlterSelection();

    }



    void AlterSelection(){

        if (selectionStatus){

            DrawBoundingArea(true);

            changeButton.enabled = true;

        }else{

            changeButton.enabled = false;

            DrawBoundingArea(false);

        }



    }



    void DrawBoundingArea(bool draw)

    {

        if(boundingArea == null)

        {

            boundingArea = GameObject.CreatePrimitive(PrimitiveType.Sphere);

            boundingArea.name = "BoundingArea";

            boundingArea.transform.parent = selectionGO.transform.parent;

            boundingArea.GetComponent<MeshRenderer>().material = boundingAreaMaterial;

        }



        boundingArea.transform.localScale = new Vector3(1.0f * 1.5f, 1.0f * 1.5f, 1.0f * 1.5f);

        boundingArea.transform.localPosition = Vector3.zero;

        

        boundingArea.SetActive(draw);

    }



}