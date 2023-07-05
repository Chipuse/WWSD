using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UI_AR_NF : MonoBehaviour
{
    
    public void ChangeActiveStatus(GameObject gO){
        if (gO.activeSelf){
            gO.SetActive(false);
        }else{
            gO.SetActive(true);
        }
    }

}
