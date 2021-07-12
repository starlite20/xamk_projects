using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class swipeControls : MonoBehaviour
{
    Vector2 swipeStart;
    Vector2 swipeEnd;

    float swipeSensitivity = 10;

    //events
    //system.action is a premade delegate
    // delegae references to function
    // only the parent class can invoke this
    // -- a list of fn to call when we detect a swipe
    public static event System.Action<swipeDirection> onSwipe = delegate { };

    public enum swipeDirection{
        up,
        down,
        left,
        right
    };

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        foreach(Touch touch in Input.touches)
        {
            if(touch.phase == TouchPhase.Began)
            {
                swipeStart = touch.position;
            }
            else if(touch.phase == TouchPhase.Ended)
            {
                swipeEnd = touch.position;
                processSwipe();
            }
        }

        //mouse touch simulation for testing purpose
        if(Input.GetMouseButtonDown(0))
        {
            swipeStart = Input.mousePosition;
        }
        else if(Input.GetMouseButtonUp(0))
        {
            swipeEnd = Input.mousePosition;
            processSwipe();
        }
    }

    void processSwipe()
    {
        //calculates distance between the points
        float distance = Vector2.Distance(swipeStart, swipeEnd);

        //check if distance is greater than minimum distance to swipe
        if(distance > swipeSensitivity)
        {
            if (isVerticalSwipe())
            {
                if(swipeEnd.y > swipeStart.y)
                {
                    onSwipe(swipeDirection.up);
                    
                }
                else
                {
                    onSwipe(swipeDirection.down);
                }
            }

            else
            {
                if(swipeEnd.x > swipeStart.x)
                {
                    onSwipe(swipeDirection.right);
                }
                else
                {
                    onSwipe(swipeDirection.left);
                }
            }
        }
    }

    bool isVerticalSwipe()
    {
        float verticalVariation = Mathf.Abs(swipeEnd.y - swipeStart.y);
        float horizontalVariation = Mathf.Abs(swipeEnd.x - swipeStart.x);

        if ( verticalVariation > horizontalVariation )
        {
            //vertical
            return true;
        }
        //horizontal
        return false;
    }
}
