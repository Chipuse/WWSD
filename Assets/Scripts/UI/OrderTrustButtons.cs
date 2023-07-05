using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrderTrustButtons : MonoBehaviour
{
    public GameObject[] buttons;

    private float maxSize;
    private float iconSize;
    public GameObject spawnField;
    public float spacing;
    private int iconCount;
    public GameObject trustButtonPrefab;
    public SpellManager sMan;
    public bool startOn;
    public bool vision;
    public static PlayerInfo[] reorderedList;
    public bool exo;


    // Start is called before the first frame update
    public void SpawnButtons()
    {
        
        if (reorderedList == null)
        {

            reorderedList = ReorderList(PlayerInfoManager.Instance.playerInfos);
        }



        maxSize = spawnField.GetComponent<RectTransform>().rect.height;
        iconCount = reorderedList.Length - 1;


        if (exo)
        {
            foreach (int theID in PlayerInfoManager.Instance.exoList)
            {
                if (theID != PlayerInfoManager.Instance.ownID)
                {
                    iconCount--;
                }
            }
        }

        DebugText.LogMessage("size delta it uses: "+ spawnField.GetComponent<RectTransform>().rect.width);

        if (iconCount > 0)
        {
            iconSize = spawnField.GetComponent<RectTransform>().rect.width / iconCount * 0.9f;
        }

        iconSize = iconSize > maxSize ? maxSize : iconSize;
        spacing = 0;
        if (iconCount > 1)
            spacing = (spawnField.GetComponent<RectTransform>().rect.width - iconSize * iconCount) / (iconCount - 1);


        DestroyOldButtons();
        buttons = new GameObject[iconCount];


//       DeleventSystem.ChangeGameState += SetPos;
        SetPos();
    }


    private void DestroyOldButtons()
    {
        if (buttons.Length > 0)
        {
            foreach (GameObject o in buttons)
            {
                Destroy(o);
            }
        }
    }

//    public void SetPos(int rolePhase)
//    {
//        if (rolePhase != 1)
//        {
//            return;
//        }
//        int pID = PlayerInfoManager.Instance.ownID;
//
//
//        int transNum = 0;
//        for (int i = 0; i < buttons.Length; i++)
//        {
//            PlayerInfo pInfo = PlayerInfoManager.Instance.playerInfos[i];
//            if (pInfo.PlayerID != pID)
//            {
//                float sizeX = buttonPos[transNum].gameObject.GetComponent<RectTransform>().rect.width;
//                float sizeY = buttonPos[transNum].gameObject.GetComponent<RectTransform>().rect.height;
//                buttons[pInfo.PlayerID].GetComponent<RectTransform>().sizeDelta= new Vector2(sizeX,sizeY);
//                buttons[pInfo.PlayerID].transform.position = buttonPos[transNum].position;
//
//                PlayerInfoManager.Instance.AssignButton(buttons[pInfo.PlayerID], pInfo.PlayerID);
//                transNum++;
//
//            }
//
//
//        }
//        
//    }


    public void SetPos()
    {

        int pID = PlayerInfoManager.Instance.ownID;


        int transNum = 0;
        for (int i = 0; i <= buttons.Length; i++)
        {
            PlayerInfo pInfo = reorderedList[i];
            if (pInfo.PlayerID != pID&&(!exo||PlayerInfoManager.Instance.exoList.Contains(pInfo.PlayerID)==false))
            {
                buttons[transNum] = Instantiate(trustButtonPrefab, spawnField.transform);
                buttons[transNum].GetComponent<TrustButton>().SetInfos(pInfo);
                buttons[transNum].GetComponent<TrustButton>().SetSize(iconSize);
                buttons[transNum].transform.localPosition = new Vector3(iconSize * transNum + spacing * transNum, 0, 0);
                buttons[transNum].GetComponent<TrustButton>().sMan = sMan;
                buttons[transNum].GetComponent<TrustButton>().vision = vision;


                /*
                buttons[pInfo.PlayerID].GetComponent<RectTransform>().sizeDelta= new Vector2(iconSize,iconSize);
                buttons[pInfo.PlayerID].transform.localPosition = new Vector3(iconSize*transNum+spacing*transNum,0,0);

                PlayerInfoManager.Instance.AssignButton(buttons[pInfo.PlayerID], pInfo.PlayerID);
                
                
                */
                transNum++;
            }
        }

        SetAllFlames(startOn);

    }


    public void SetAllFlames(bool on)
    {
        foreach (GameObject b in buttons)
        {
            b.GetComponent<TrustButton>().flame.SetActive(on);
            b.GetComponent<TrustButton>().trust = on;
        }
    }


    public TrustButton GetButtonByID(int pID)
    {
        foreach (GameObject b in buttons)
        {
            if (b.GetComponent<TrustButton>().myInfo.PlayerID == pID)
            {
                return b.GetComponent<TrustButton>();
            }
        }

        return null;
    }


    private PlayerInfo[] ReorderList(PlayerInfo[] aList)
    {

        int[] order = ImageRecognition_GreenLight.Instance.GetOrderOfCandleHolders();

           
            
        PlayerInfo[] rList = new PlayerInfo[aList.Length];


        for (int k = 0; k < order.Length; k++)
        {
            for (int i = 0; i < rList.Length; i++)
            {
                if (order[k] == aList[i].PlayerID)
                {
                    rList[k] = aList[i];
                }
            }
        }

        return rList;
    }
}