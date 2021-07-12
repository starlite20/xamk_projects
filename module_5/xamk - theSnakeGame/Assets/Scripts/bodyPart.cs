using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bodyPart : MonoBehaviour
{
    Vector2 deltaPosition;

    public bodyPart following = null;

    private SpriteRenderer spriteRenderer = null;

    const int PARTSREMEMBERED = 10;
    public Vector3[] previousPositions = new Vector3[PARTSREMEMBERED];

    public int setIndex = 0;
    public int getIndex = -(PARTSREMEMBERED - 1);

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    virtual public void Update()
    {
        if (!gameController.instance.isAlive)
            return;


        Vector3 followPosition;

        if(following != null)
        {
            if (following.getIndex > -1)
                followPosition = following.previousPositions[following.getIndex];
            else
                followPosition = following.transform.position;
        }
        else
        {
            followPosition = gameObject.transform.position;
        }


        previousPositions[setIndex].x = gameObject.transform.position.x;
        previousPositions[setIndex].y = gameObject.transform.position.y;
        previousPositions[setIndex].z = gameObject.transform.position.z;

        setIndex++;
        if (setIndex >= PARTSREMEMBERED)
            setIndex = 0;

        getIndex++;
        if (getIndex >= PARTSREMEMBERED)
            getIndex = 0;

        if(following != null)
        {
            Vector3 newPosition;

            if(following.getIndex > -1)
            {
                newPosition = followPosition;
            }
            else
            {
                newPosition = following.transform.position;
            }

            newPosition.z = newPosition.z + 0.01f;

            setMovement(newPosition - gameObject.transform.position);

            updateDirection();
            updatePosition();
        }
    }

    public void setMovement( Vector2 movement )
    {
        deltaPosition = movement;
    }

    public void updatePosition()
    {
        gameObject.transform.position += (Vector3)deltaPosition;
    }

    public void updateDirection()
    {
        if(deltaPosition.y > 0) //up
        {
            gameObject.transform.localEulerAngles = new Vector3(0, 0, 0);
        }
        else if (deltaPosition.y < 0) //down
        {
            gameObject.transform.localEulerAngles = new Vector3(0, 0, 180);
        }
        else if (deltaPosition.x < 0) //left
        {
            gameObject.transform.localEulerAngles = new Vector3(0, 0, 90);
        }
        else if (deltaPosition.x > 0) //right
        {
            gameObject.transform.localEulerAngles = new Vector3(0, 0, -90);
        }
    }

    public void turnToTail()
    {
        spriteRenderer.sprite = gameController.instance.tailSprite;
    }
    public void turnToBody()
    {
        spriteRenderer.sprite = gameController.instance.bodySprite;
    }

    public void resetMemory()
    {
        setIndex = 0;
        getIndex = -(PARTSREMEMBERED - 1);
    }
}
