using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Experimental.PlayerLoop;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

public class SlideBehavior : MonoBehaviour, IPointerDownHandler
{
    public int optScreenSize = 1920;
    public int slideThreshold = 20;
    private float screenScaleFac;


    public SpellManager spellMan;
    public bool spell;

    public GameObject otherSpell;

    public bool slide;
    public bool animate;
    public bool slidable;

    public int animationSpeed;

    private Vector3 lastMousePos;

    public Vector3 upPos;
    public Vector3 downPos;
    public float moveDistance;

    public bool startUp;
    public bool up;
    public int moveState;
    public GameObject objectToMove;
    public float distanceUp;
    public float distanceDown;


    private bool slideInAnimation;
    private Vector3 currentDestination;
    public bool goAlwaysDown;
    public Vector3 outPos;


    // Start is called before the first frame update
    void Start()
    {
        moveDistance = upPos.y - downPos.y;
        spell = spellMan != null;

        screenScaleFac = optScreenSize * 1.0f / Screen.height;

        if (!spell)
        {


            SetPos(startUp);
        }


    }

    // Update is called once per frame
    void Update()
    {
        


            if (slide)
            {
                
                
                MoveObject();
                

            }
            else if (animate)
            {
                AnimatedSlideTo();
            }
        
    }


    public void isTouched()
    {
//        if (Input.GetMouseButtonDown(0))
//        {
//            print("gets mouse klick");
//
//            RaycastHit2D hit;
//            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
//             
//            if (Physics2D.Raycast(ray.origin, out hit)) {
//                if (hit.transform == this.transform )
//                {Debug.Log( "My object is clicked by mouse");}
//                else
//                {
//                    print("nope not hit");
//                }
//            }
//        }
    }


    public void MoveObject()
    {
        slideInAnimation = false;
        animate = false;
        Vector3 diff = Input.mousePosition - lastMousePos;
        lastMousePos = Input.mousePosition;


      

            //checks if the input move has gone the other way and sets which direction the object should move


            
            if (diff.y > 0)
            {
                distanceDown = 0;
                distanceUp += diff.y * screenScaleFac;
                if (distanceUp > slideThreshold)
                {
                    moveState = 1;
                }
            }
            else if (diff.y < 0)
            {
                distanceUp = 0;
                distanceDown += diff.y * screenScaleFac;
                if (distanceDown < -slideThreshold)
                {
                    moveState = -1;
                }


            }
            


            //updates the position and makes it not move over the limit

            objectToMove.transform.localPosition += new Vector3(0, diff.y * screenScaleFac);


            if (diff.y > 0 && objectToMove.transform.localPosition.y >= upPos.y)
            {
               // SetPos(true);
               objectToMove.transform.localPosition = upPos;

            }
            else if (diff.y < 0 && objectToMove.transform.localPosition.y <= downPos.y)
            {
                //SetPos(false);
                objectToMove.transform.localPosition = downPos;
            }



            //moves the other object with the sliding one
            if (spell)
            {
                float moveDis = (transform.localPosition.y - downPos.y) / (upPos.y - downPos.y);
                otherSpell.transform.localPosition = upPos + outPos * moveDis;
            }
            
            
            
            

        

        //checks if the button is still down

        if (Input.GetMouseButtonUp(0))
        {
            slide = false;
            bool goUp;
            if (goAlwaysDown)
            {
                goUp = false;
            }
            else if (moveState > 0)
            {
                goUp = true;
            }

            else if (moveState < 0)
            {
                goUp = false;
            }
            else
            {
                goUp = !up;
            }

            Vector3 nextDestination = goUp ? upPos : downPos;
            StartAnimation(nextDestination);
            if (spell)
            {
                Vector3 otherDestination = goUp ? outPos : upPos;
                otherSpell.GetComponent<SlideBehavior>().StartAnimation(otherDestination);
            }
        }
    }


    public void SetPos(bool setUp)
    {
        animate = false;
        if (setUp)
        {
            objectToMove.transform.localPosition = upPos;
            up = true;
            if (spell&&slidable)
            {
                spellMan.SwitchSpell();
            }
        }
        else
        {
            objectToMove.transform.localPosition = downPos;
            up = false;
        }
    }


    /*
    private void Animate()
    {
        bool goUp;
        if (moveState > 0)
        {
            goUp = true;
        }

        else if (moveState < 0)
        {
            goUp = false;
        }
        else
        {
            goUp = !up;
        }

        float curDis = goUp
            ? upPos.y - objectToMove.transform.localPosition.y
            : downPos.y - objectToMove.transform.localPosition.y;
        if (curDis < 3 && goUp || curDis > -3 && !goUp)
        {
            SetPos(goUp);
        }

        objectToMove.transform.localPosition += new Vector3(0, animationSpeed * (curDis / moveDistance));
    }
    
    */


    private void AnimatedSlideTo()
    {
        float curYDis = currentDestination.y - objectToMove.transform.localPosition.y;
        float curXDis = currentDestination.x - objectToMove.transform.localPosition.x;

        if (curXDis < 3 && curXDis > -3 && curYDis < 3 && curYDis > -3)
        {
            objectToMove.transform.localPosition = currentDestination;
            animate = false;
            if (currentDestination == upPos)
            {
                SetPos(true);
            }
            else if (currentDestination == downPos)
            {
                SetPos(false);
            }
        }
        else
        {
            objectToMove.transform.localPosition += new Vector3(animationSpeed * (curXDis / moveDistance),
                animationSpeed * (curYDis / moveDistance));
        }
    }


    public void StartAnimation(Vector3 destinationPos)
    {
        currentDestination = destinationPos;
        animate = true;
    }

/*

private void SetInAnimation()
{
    print("curveset happens");
    animationIn.legacy = false;
    
    inCurve=new AnimationCurve();
    inCurve.AddKey(0f, objectToMove.transform.localPosition.y);
    inCurve.AddKey(0.4f, downPos.y);
    animationIn.SetCurve("",typeof(RectTransform),"localPosition.y",inCurve);
}
*/


    public void StartSlide()
    {
        slide = true;
        lastMousePos = Input.mousePosition;
        moveState = 0;
    }


    public void SlideInAnimation()
    {
        objectToMove.transform.localPosition = new Vector3(0, downPos.y - 500);
        StartAnimation(downPos);
        slideInAnimation = true;
    }


    public void OnPointerDown(PointerEventData eventData)
    {
        if (slidable)
        {
                    StartSlide();

        }
    }
    
    
}