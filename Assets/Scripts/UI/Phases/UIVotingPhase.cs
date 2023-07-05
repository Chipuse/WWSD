using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIVotingPhase : UIPhaseManagerParent
{
    private bool firstTime;
    public SlideBehavior paper;
    public Animator anim;
    private bool submitted;
    public FadeController pleaseLayDown, youSubmitted, submitImage;

    // Start is called before the first frame update
    void Awake()
    {
        firstTime = true;
    }

    // Update is called once per frame
    void Update()
    {
    }

    public override void StartPhase()
    {
        base.StartPhase();
        if (firstTime)
        {
            DeleventSystem.OnPhoneResting += LayedDown;
            DeleventSystem.ChangeGameState += ReallyEnd;
            GetComponent<OrderTrustButtons>().SpawnButtons();
            firstTime = false;
            
        }


        GemGlowManager.Instance.TurnGlow(true);
        submitted = false;

        DeleventSystem.HideOwnCandle(true);
        paper.SetPos(true);
        GetComponent<OrderTrustButtons>().SetAllFlames(true);
        FadeController.FogFade.Fade(false);
    }

    private void ReallyEnd(int nextPhase)
    {
        if (nextPhase == 3)
        {
            EndPhase();
        }
    }

    public override void EndPhase()
    {
        pleaseLayDown.Fade(false);
        submitImage.Fade(false);
        youSubmitted.Fade(false);
        GemGlowManager.Instance.TurnGlow(false);

        FadeController.FogFade.Fade(true);
        base.EndPhase();
    }


    public void ThisPlayerReady()
    {
        submitted = !submitted;
        GameLoopTriggers.Instance.VotingPhaseSubmit(submitted);
        FadeController.FogFade.Fade(submitted);

        if (submitted)
        {
            anim.SetTrigger("hidePaper");
            pleaseLayDown.Fade(true);
        }
        else
        {
            pleaseLayDown.Fade(false);
            anim.SetTrigger("showPaper");
            submitImage.Fade(false);
            youSubmitted.Fade(false);
        }
    }

    public void LayedDown()
    {
        pleaseLayDown.Fade(false);
        youSubmitted.Fade(true);
        submitImage.GetComponent<Image>().sprite = PlayerInfoManager.Instance.icons[PlayerInfoManager.Instance.ownID];
        submitImage.GetComponent<Image>().color = PlayerInfoManager.Instance.iconColors[PlayerInfoManager.Instance.ownID];
        submitImage.Fade(true);
    }
}