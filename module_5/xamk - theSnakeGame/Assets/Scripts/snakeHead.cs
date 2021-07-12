using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class snakeHead : bodyPart
{
    Vector2 movement;

    private bodyPart tail = null;

    const float TIMETOADDBODYPART = 0.1f;
    float addTimer = TIMETOADDBODYPART;

    public int partsToAdd = 0;

    List<bodyPart> bodyPartList = new List<bodyPart>();

    public AudioSource[] gulpSounds = new AudioSource[3];
    public AudioSource dieSound = null;


    // Start is called before the first frame update
    void Start()
    {
        swipeControls.onSwipe += swipeDetection;

    }

    // Update is called once per frame
    override public void Update()
    {
        if (!gameController.instance.isAlive)
            return;

        base.Update();
        setMovement(movement * Time.deltaTime);
        updateDirection();
        updatePosition();

        if(partsToAdd > 0)
        {
            addTimer -= Time.deltaTime;
            if(addTimer <= 0)
            {
                addTimer = TIMETOADDBODYPART;
                addBodyPart();
                partsToAdd--;
            }
        }
    }

    void addBodyPart()
    {
        if(tail==null)
        {
            Vector3 newPosition = transform.position;
            newPosition.z = newPosition.z - 0.001f;

            bodyPart newPart = Instantiate(gameController.instance.bodyPrefab, newPosition, Quaternion.identity);
            newPart.following = this;
            tail = newPart;

            newPart.turnToTail();

            bodyPartList.Add(newPart);
        }
        else
        {
            Vector3 newPosition = tail.transform.position;
            newPosition.z = newPosition.z - 0.001f;

            bodyPart newPart = Instantiate(gameController.instance.bodyPrefab, newPosition, tail.transform.rotation);
            newPart.following = tail;
            newPart.turnToTail();

            tail.turnToBody();
            tail = newPart;

            bodyPartList.Add(newPart);
        }
    }

    void swipeDetection(swipeControls.swipeDirection directionOfSwipe)
    {
        switch(directionOfSwipe)
        {
            case swipeControls.swipeDirection.up:
                moveUp();
                break;

            case swipeControls.swipeDirection.down:
                moveDown();
                break;

            case swipeControls.swipeDirection.left:
                moveLeft();
                break;

            case swipeControls.swipeDirection.right:
                moveRight();
                break;


        }
    }


    void moveUp()
    {
        movement = Vector2.up * gameController.instance.speedOfSnake;

    }
    void moveDown()
    {
        movement = Vector2.down * gameController.instance.speedOfSnake;
    }
    void moveLeft()
    {
        movement = Vector2.left * gameController.instance.speedOfSnake;
    }
    void moveRight()
    {
        movement = Vector2.right * gameController.instance.speedOfSnake;
    }

    public void resetSnake()
    {
        foreach(bodyPart partInList in bodyPartList)
        {
            Destroy(partInList.gameObject);
        }
        bodyPartList.Clear();

        tail = null;
        moveUp();

        gameObject.transform.localEulerAngles = new Vector3(0, 0, 0);
        gameObject.transform.position = new Vector3(0, 0, 0);

        resetMemory();

        partsToAdd = 5;
        addTimer = TIMETOADDBODYPART;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        egg eggDetected = collision.GetComponent<egg>();

        if(eggDetected)
        {
            Debug.Log("Egg");
            eatEgg(eggDetected);
            int rand = Random.Range(0, 3);
            gulpSounds[rand].Play();
        }
        else
        {
            //obstacle hit
            gameController.instance.gameOver();
            dieSound.Play();
        }
        

    }

    void eatEgg( egg eggToBeEaten)
    {
        partsToAdd = 5;
        addTimer = 0;
        gameController.instance.eggEaten(eggToBeEaten);
    }
}
