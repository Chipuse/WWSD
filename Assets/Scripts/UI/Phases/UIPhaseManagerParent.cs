using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UIPhaseManagerParent : MonoBehaviour
{
    public GameObject[] objectsToActivate;

    public GameObject[] objectsToDisable;
    public UIPhaseManagerParent nextPhaseObj;


    public int phase;
    // Start is called before the first frame update
    void Start()
    { 
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    
    public void ActivateObjects()
    {
        foreach (GameObject obj in objectsToActivate)
        {
            obj.SetActive(true);
        }
    }

    public void DisableObjects()
    {
        foreach (GameObject obj in objectsToDisable)
        {
            obj.SetActive(false);
        }
    }

    public virtual void StartPhase()
    {
        
        
    }
    
    public virtual void EndPhase()
    {
        nextPhaseObj.gameObject.SetActive(true);
        nextPhaseObj.StartPhase();
        gameObject.SetActive(false);


    }
}
