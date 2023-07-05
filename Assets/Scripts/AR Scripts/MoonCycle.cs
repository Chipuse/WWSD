using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MoonCycle : MonoBehaviour
{
    
    GameObject Moon;

    [SerializeField]Transform[] SpotsTr;

    Camera c;

    [SerializeField]Text tmp1;
    [SerializeField]Text tmp2;
    int position = 0;

    bool stopLooking;


    void Awake(){
        Moon = transform.Find("Moon").gameObject;
        // SpotsTr = Spots.transform.GetComponentsInChildren<Transform>();
        c = Camera.main;
        
        // foreach (var item in SpotsTr)
        // {
        //     print(item.position);
        // }

        DeleventSystem.ChangeGameState += MoveToNextPosition;
        DeleventSystem.SetLocalCandlePosition += MyCandlePos;
    }

    void MoveToNextPosition(int newPhase)
    {



        if (newPhase == 3)
        {
            position++;
            Moon.transform.position = SpotsTr[position].position;

        }

        if (position == 3)
        {
            if (Votingsystem.Instance.exorcismTargetsId[0] != -1)
            { // NEWW
              // tmp1.text = Votingsystem.Instance.GetParticipantById(Votingsystem.Instance.exorcismTargetsId[0]).GetName(); 
            }
        }
        else if (position == 6)
        {
            if (Votingsystem.Instance.exorcismTargetsId[1] != -1)
            { // NEWW
              // tmp2.text = Votingsystem.Instance.GetParticipantById(Votingsystem.Instance.exorcismTargetsId[1]).GetName();
            }
        }

    }

    void Update(){
        if (stopLooking == true){
            Vector3 p = c.transform.position; 
            p.y = this.transform.position.y;

            transform.LookAt(p);
        }        if (tmp1.text == "" && position == 3){

            if (Votingsystem.Instance.exorcismTargetsId[0] != -1){ // NEWW
                tmp1.text = Votingsystem.Instance.GetParticipantById(Votingsystem.Instance.exorcismTargetsId[0]).GetName(); 
            }
        }
        if (tmp2.text == "" && position == 6){
            if (Votingsystem.Instance.exorcismTargetsId[1] != -1){ // NEWW
                tmp2.text = Votingsystem.Instance.GetParticipantById(Votingsystem.Instance.exorcismTargetsId[1]).GetName();
            }
        }

    }

    void MyCandlePos(Vector3 k){
        Vector3 pp = SettingUpMyCandle.Instance.candleInScene.transform.position;
        pp.y = this.transform.position.y;

         transform.LookAt(pp);
    }
}
