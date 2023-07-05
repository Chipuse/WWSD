using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner_GLcs : MonoBehaviour
{
    public GameObject objectToSpawn;
    //private PlacementIndicator placementIndicator;

    private GameObject[] obj;

    [SerializeField]
    private Camera arCamera;

    [SerializeField]
    private GameObject FirePrefab;

    [SerializeField]
    private GameObject RunesPanel;

    [SerializeField]
    private GameObject ShowRunes;

    [SerializeField]
    private Color activeColor = Color.red;

    [SerializeField]
    private Color inactiveColor = Color.gray;

    private GameObject SelectGO;

    void Start(){
        //placementIndicator = FindObjectOfType<PlacementIndicator>();
    }

    void Update(){
        // if (Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began){
        //     GameObject obj = Instantiate(objectToSpawn, placementIndicator.transform.position, placementIndicator.transform.rotation);
        // }

        if(Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            
            if(touch.phase == TouchPhase.Began)
            {
                Ray ray = arCamera.ScreenPointToRay(touch.position);
                RaycastHit hitObject;
                if(Physics.Raycast(ray, out hitObject))
                {
                    if(hitObject.transform.parent.tag == "Player")
                    {
                        SelectCandle(hitObject.transform.parent.gameObject);
                    }
                }
            }
        }

    }

    // public void SpawnObject(){
    //     GameObject GO = Instantiate(objectToSpawn, placementIndicator.transform.position, placementIndicator.transform.rotation);
    //     MeshRenderer meshRenderer = GO.transform.Find("Selection").GetComponent<MeshRenderer>();
    //     meshRenderer.material.color = inactiveColor;
    //     SelectCandle(GO);
    // }

    public void LightCandle(){

        GameObject Fire = SelectGO.transform.Find("FireAnim").gameObject;

        if (Fire.activeSelf == false){
            Fire.SetActive(true);
        }else{
            Fire.SetActive(false);
        }

        // Transform posInst = SelectGO.transform.Find("FirePos").transform;

        // Instantiate(FirePrefab, posInst.position, posInst.rotation, SelectGO.transform);

        // // if (SelectGO.transform.Find("Fire").gameObject != null){
        // //     Destroy(SelectGO.transform.Find("Fire").gameObject);
        // // }else{
        // //     Instantiate(FirePrefab, posInst.position, posInst.rotation, SelectGO.transform);
        // // }
       
    }

    public void ActiveRunes(){

        if (RunesPanel.activeSelf == false){
            RunesPanel.SetActive(true);
            ShowRunes.SetActive(false);
            
        }else{
            RunesPanel.SetActive(false);
            ShowRunes.SetActive(true);
        }
       
    }

    void SelectCandle(GameObject selected){

        SelectGO = selected;

        obj = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject current in obj)
        {   
            MeshRenderer meshRenderer = current.transform.Find("Selection").GetComponent<MeshRenderer>();
            if(selected != current) 
            {   
                meshRenderer.material.color = inactiveColor;
            }
            else 
            {
                meshRenderer.material.color = activeColor;  
            }
            
        }
    }

    
}
