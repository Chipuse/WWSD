using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellManager : MonoBehaviour
{

    public GameObject exorciseObj;
    public GameObject visionObj;

    public GameObject initButtons;

    public Vector3 initVisionPos;
    public Vector3 initExorcisePos;

    public bool visionOut;


    public OrderTrustButtons visionButtons;
    public GameObject visionChoose;
    public DisplayPlayerInfo visionWho;
    public GameObject visionSubmit;



    public OrderTrustButtons exoButtons;
    public GameObject exoChoose;
    public DisplayPlayerInfo exoWho;
    public GameObject exoSubmit;


    public int selectedPlayer;
    public UIOraclePhase phaseMan;

    public GameObject youAreMessage;
    private bool firstTime;

    private void Awake()
    {
        firstTime = true;
    }

    public void StartPhase()
    {
        if (firstTime)
        {

            firstTime = false;
        }
        visionButtons.SpawnButtons();
        exoButtons.SpawnButtons();
        
        
        
        visionObj.transform.localPosition = initVisionPos;
        exorciseObj.transform.localPosition = initExorcisePos;
        exorciseObj.GetComponent<SlideBehavior>().slidable = false;
        visionObj.GetComponent<SlideBehavior>().slidable = false;
        exorciseObj.GetComponent<Canvas>().sortingOrder = 5;
        visionObj.GetComponent<Canvas>().sortingOrder = 6;
        exorciseObj.SetActive(true);
        visionObj.SetActive(true);

        ToggleChooseMessage(true,false);
        ToggleChooseMessage(false,false);
        selectedPlayer = -1;

        
    }

    public void SwitchSpell()
    {
        //selectedPlayer = -1;
        if (visionOut)
        {

            visionObj.GetComponent<SlideBehavior>().slidable = true;
            visionObj.GetComponent<Canvas>().sortingOrder = 6;
            exorciseObj.GetComponent<SlideBehavior>().slidable = false;
            visionObj.GetComponent<SlideBehavior>().SlideInAnimation();
            visionButtons.SetAllFlames(false);

            ToggleChooseMessage(true,true);


        }
        else
        {
            
            
            visionObj.GetComponent<SlideBehavior>().slidable = false;
            visionObj.GetComponent<Canvas>().sortingOrder = 4;
            exorciseObj.GetComponent<SlideBehavior>().slidable=true;
            exorciseObj.GetComponent<SlideBehavior>().SlideInAnimation();
            exoButtons.SetAllFlames(false);

            ToggleChooseMessage(false,true);

        }

        visionOut = !visionOut;
    }



    public void setFirstSpell(bool exorciseOut)
    {


        initButtons.SetActive(false);
        visionOut = exorciseOut;
        youAreMessage.SetActive(false);


        if (exorciseOut)
        {
            exorciseObj.GetComponent<SlideBehavior>().slidable=true;

            exorciseObj.GetComponent<SlideBehavior>().StartAnimation(exorciseObj.GetComponent<SlideBehavior>().upPos);
            visionObj.GetComponent<SlideBehavior>().StartAnimation(visionObj.GetComponent<SlideBehavior>().downPos+new Vector3(0,-500));

            ToggleChooseMessage(false,true);


        }
        else
        {
            visionObj.GetComponent<SlideBehavior>().slidable = true;

            exorciseObj.GetComponent<SlideBehavior>().StartAnimation(exorciseObj.GetComponent<SlideBehavior>().downPos+new Vector3(0,-500));

            visionObj.GetComponent<SlideBehavior>().StartAnimation(visionObj.GetComponent<SlideBehavior>().upPos);
            ToggleChooseMessage(true,true);


        }
        visionButtons.SetAllFlames(false);
        exoButtons.SetAllFlames(false);
        
        //SwitchSpell();
        

    }



    public void ToggleChooseMessage(bool vision,bool on)
    {

        if (vision)
        {
            visionChoose.SetActive(on);
            visionWho.gameObject.SetActive(!on);
            visionSubmit.SetActive(!on);

        }
        else
        {
            exoChoose.SetActive(on);
            exoWho.gameObject.SetActive(!on);
            exoSubmit.SetActive(!on);
        }
    }
    
    
    
    public void ChangeSelect(int pID, bool vision)
    {


        if (vision != visionOut)
        {
            ToggleChooseMessage(vision, false);

        }

        
        OrderTrustButtons oTB = vision ? visionButtons : exoButtons;
        
        if (selectedPlayer >= 0)
        {
            oTB.GetButtonByID(selectedPlayer).Deselect();
        }
       
            ToggleChooseMessage(vision, false);
        
        selectedPlayer = pID;

        exoWho.PrintPlayerName(pID);
        visionWho.PrintPlayerName(pID);
    }


    public void SubmitSpell(bool vision)
    {
        
        
        
        if (vision)
        {
            visionButtons.GetButtonByID(selectedPlayer).Deselect();
            
            phaseMan.DisplayIsRole(selectedPlayer, PlayerInfoManager.Instance.playerInfos[PlayerInfoManager.Instance.ownID].PlayerRole==3);
            visionObj.GetComponent<SlideBehavior>().animate=false;
            
            exorciseObj.GetComponent<SlideBehavior>().slidable = false;
            exorciseObj.GetComponent<SlideBehavior>().StartAnimation(exorciseObj.GetComponent<SlideBehavior>().downPos+new Vector3(0,-500));
            DissolveManager.Instance.SetUpDissolve(visionObj);

        }
        else
        {
            exoButtons.GetButtonByID(selectedPlayer).Deselect();
            phaseMan.DisplayExorcise(selectedPlayer);

            exorciseObj.GetComponent<SlideBehavior>().animate=false;

            visionObj.GetComponent<SlideBehavior>().slidable = false;
            visionObj.GetComponent<SlideBehavior>().StartAnimation(visionObj.GetComponent<SlideBehavior>().downPos+new Vector3(0,-500));
            DissolveManager.Instance.SetUpDissolve(exorciseObj);

        }
        
        
        GemGlowManager.Instance.TurnGlow(true);
        //TODO exrocismor checking integer??
        GameLoopTriggers.Instance.VesselPhaseAction(0,selectedPlayer);

    }
}
