using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TrustButton : MonoBehaviour
{
    public PlayerInfo myInfo;

    public GameObject flame;


    public bool trust;

    private Material mat;
    public Material ogMat;
    public SpellManager sMan;
    public bool vision;
    public Image icon;
    public GameObject burn;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void SetFlameColor()
    {
        mat=new Material(ogMat);
        flame.GetComponent<Image>().material = mat;
        flame.GetComponent<Image>().material.SetColor("_Color1", PlayerInfoManager.Instance.iconColors[myInfo.UserIcon]);
        flame.GetComponent<Image>().material.SetColor("_Color2", PlayerInfoManager.Instance.iconColors2[myInfo.UserIcon]);
    }

    private void SetIcon()
    {
        icon.sprite = PlayerInfoManager.Instance.icons[myInfo.UserIcon];
        icon.color = PlayerInfoManager.Instance.inkBrown;
    }

    public void SetSize(float aSize)
    {
        GetComponent<RectTransform>().sizeDelta = new Vector2(aSize, aSize);
        icon.GetComponent<RectTransform>().sizeDelta = new Vector2(aSize, aSize);
        burn.GetComponent<RectTransform>().sizeDelta = new Vector2(aSize, aSize);
    }

    public void SetInfos(PlayerInfo pInfo)
    {
        myInfo = pInfo;
        SetIcon();
        SetFlameColor();
    }


    
    public void ToggleTrust()
    {


        if (sMan != null)
        {

            if (sMan.selectedPlayer != myInfo.PlayerID || sMan.visionOut!=vision)
            {
                sMan.ChangeSelect(myInfo.PlayerID, vision);
            }
            trust = true;
            flame.SetActive(trust);

        }
        else
        {       
            trust = !trust;
            flame.SetActive(trust);
            GameLoopTriggers.Instance.VotingPhaseButtonTrustChange(myInfo.PlayerID, trust);
        }




    }


    public void Deselect()
    {
        trust = false;
        flame.SetActive(false);
        
    }


}