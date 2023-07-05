using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HoldButton : MonoBehaviour, IPointerDownHandler, IPointerExitHandler, IPointerUpHandler
{
    public float timer;
    public float holdTime;
    public bool timeFilled;

    public bool holdDown;

    public Animator anim;
    public InputField nameField;



    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (holdDown)
        {
            timer += Time.deltaTime;
            if (timer >= holdTime)
            {
                timeFilled = true;
            }
        }
        else if (timer > 0)
        {
            timer -= Time.deltaTime;
            if (timer < 0)
            {
                timer = 0;
            }
        }
    }


    private void StartHold()
    {
        holdDown = true;
        anim.SetTrigger("submit");
        anim.SetFloat("speedMult", 1);

    }

    private void StopHold()
    {
        holdDown = false;

        if (!timeFilled)
        {
            CancelHold();
        }
    }

    private void CancelHold()
    {
        anim.SetFloat("speedMult", -1);
    }

    public void CheckField()
    {
        if (nameField.text == "")
        {
            anim.SetTrigger("empty");
        }
        else
        {
            PlayerInfoManager.Instance.ownName = nameField.text;

            StartHold();
        }
    }

    
    

    public void OnPointerDown(PointerEventData eventData)
    {
        CheckField();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        StopHold();
    }


    public void OnPointerUp(PointerEventData eventData)
    {
        StopHold();
    }
}