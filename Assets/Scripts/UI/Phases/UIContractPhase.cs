using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;

public class UIContractPhase : UIPhaseManagerParent
{

    public Animator anim;
    public Animator lobbyAnim;
    public InputField nameField;
    public HoldButton submitButton;
    public GameObject contract;
    public Animator bloodAnim;





    public void DissolveContract()
    {
        DissolveManager.Instance.SetUpDissolve(contract);
    }
    
    public void StartBlood(){
    
        bloodAnim.gameObject.SetActive(true);
    bloodAnim.SetTrigger("goBlood");
    
    }

    public void CheckIfFieldEmpty()
    {

        if (nameField.text == "")
        {
            HideSubmitButton();
        }
        else
        {
            DisplaySubmitButton();
        }
        
    }

    public void StopButton()
    {
        anim.SetFloat("buttonSpeed", 0);

    }
    private void HideSubmitButton()
    {
        anim.SetTrigger("showButton");
        anim.SetFloat("buttonSpeed", -1);
        submitButton.enabled = false;
    }


    private void DisplaySubmitButton()
    {
        anim.SetTrigger("showButton");
        anim.SetFloat("buttonSpeed", 1);
        submitButton.enabled = true;
        submitButton.gameObject.SetActive(true);

    }

    void Start()
    {
       // submitButton.gameObject.SetActive(false);
            
    }


}
