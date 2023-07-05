using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinManager : MonoBehaviour
{

    public GameObject satanistsWin, witchesWin, votingPhase, oraclePhase;
    
    // Start is called before the first frame update
    void Start()
    {
        satanistsWin.SetActive(false);
        witchesWin.SetActive(false);
        DeleventSystem.ChangeGameState += SomeoneWon;

    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void SomeoneWon(int phase)
    {

        if (phase == 5)
        {
            FadeController.FogFade.Fade(true);

            witchesWin.SetActive(true);
            votingPhase.SetActive(false);
            oraclePhase.SetActive(false);
        }
        else if (phase == 6)
        {
            FadeController.FogFade.Fade(true);

            satanistsWin.SetActive(true);
            votingPhase.SetActive(false);
            oraclePhase.SetActive(false);
        }
        
    }
}
