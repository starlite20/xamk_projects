using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class gameController : MonoBehaviour
{
    //singleton class
    //created only once

    const float WIDTH = 4f; 
    const float HEIGHT = 7f;

    public static gameController instance = null;

    public float speedOfSnake = 1;

    public bodyPart bodyPrefab = null;
    public GameObject rockPrefab = null;
    public GameObject eggPrefab = null;
    public GameObject goldenEggPrefab = null;
    public GameObject spikePrefab = null;

    public Sprite tailSprite = null;
    public Sprite bodySprite = null;

    public snakeHead snakeHeadObj = null;

    public bool isAlive = true;
    public bool waitingToPlay = true;

    List<egg> eggsList = new List<egg>();
    List<spike> spikesList = new List<spike>();

    int level = 0;
    int noOfEggsForNextLevel = 0;

    public int score = 0;
    public int hiScore = 0;

    public Text scoreTxt = null;
    public Text hiScoreTxt = null;
    public Text tapToPlayTxt = null;
    public Text gameOverTxt = null;


    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        isAlive = false;

        createWalls();
    }

    // Update is called once per frame
    void Update()
    {   
        if(waitingToPlay)
        {
            foreach(Touch touch in Input.touches)
            {
                if(touch.phase == TouchPhase.Ended)
                    startGamePlay();
            }

            if(Input.GetMouseButtonUp(0))
                startGamePlay();
        }
    }


    void startGamePlay()
    {
        score = 0;
        level = 0;

        scoreTxt.text = "Score = " + score;
        hiScoreTxt.text = "HiScore = " + hiScore;

        tapToPlayTxt.gameObject.SetActive(false);
        gameOverTxt.gameObject.SetActive(false);


        waitingToPlay = false;
        isAlive = true;

        killOldEggs();
        killOldSpike();

        levelUp();
    }

    void levelUp()
    {
        level++;

        noOfEggsForNextLevel = 4 + (level * 2);

        speedOfSnake = 1f + (level / 4f);
        if (speedOfSnake > 6)
            speedOfSnake = 6;

        snakeHeadObj.resetSnake();
        killOldSpike();

        createSpike();
        createEgg();      
    }

    public void gameOver()
    {
        isAlive = false;
        waitingToPlay = true;

        gameOverTxt.gameObject.SetActive(true);
        tapToPlayTxt.gameObject.SetActive(true);
    }

    void createWalls()
    {
        Vector3 startPt = new Vector3(-WIDTH, -HEIGHT, 0);
        Vector3 finishPt = new Vector3(-WIDTH, +HEIGHT, 0);
        createWall(startPt, finishPt);

        startPt = new Vector3(+WIDTH, -HEIGHT, 0);
        finishPt = new Vector3(+WIDTH, +HEIGHT, 0);
        createWall(startPt, finishPt);

        startPt = new Vector3(-WIDTH, -HEIGHT, 0);
        finishPt = new Vector3(+WIDTH, -HEIGHT, 0);
        createWall(startPt, finishPt);

        startPt = new Vector3(-WIDTH, +HEIGHT, 0);
        finishPt = new Vector3(+WIDTH, +HEIGHT, 0);
        createWall(startPt, finishPt);
    }

    void createWall(Vector3 start, Vector3 finish)
    {
        float distance = Vector3.Distance(start, finish);
        int numberOfRocks = (int)(distance * 3);
        Vector3 delta = (finish - start) / numberOfRocks;

        Vector3 position = start;
        for(int rock = 0; rock<=numberOfRocks; rock++)
        {
            float rotationValue = Random.Range(0, 360f);
            float scaleValue = Random.Range(1.5f, 2f);

            createRock(position, scaleValue, rotationValue);
            position = position + delta;
        }
        
    }

    void createRock(Vector3 position, float scale, float rotation)
    {
        GameObject rock = Instantiate(rockPrefab, position, Quaternion.Euler(0, 0, rotation));
        rock.transform.localScale = new Vector3(scale, scale, 1);
    }


    //the eggs
    void createEgg(bool goldenEgg = false)
    {
        Vector3 positionOfEgg;
        bool isThePositionSame = false;

        do
        {
            positionOfEgg.x = -WIDTH + Random.Range(1f, (WIDTH * 2) - 2f);
            positionOfEgg.y = -HEIGHT + Random.Range(1f, (HEIGHT * 2) - 2f);
            positionOfEgg.z = 0;

            foreach (spike spikeInList in spikesList)
            {
                if ( ((positionOfEgg.x >= spikeInList.transform.position.x - 0.5) && (positionOfEgg.x <= spikeInList.transform.position.x + 0.5))
                    && 
                    ((positionOfEgg.y >= spikeInList.transform.position.y - 0.5) && (positionOfEgg.y <= spikeInList.transform.position.x + 0.5))  )
                {
                    isThePositionSame = true;
                    Debug.Log("pos same");
                    break;
                }
                else
                {
                    isThePositionSame = false;
                }
            }
        } while (isThePositionSame);

        Debug.Log("done");

        egg referenceEgg = null;

        if(goldenEgg)
            referenceEgg = Instantiate(goldenEggPrefab, positionOfEgg, Quaternion.identity).GetComponent<egg>();
        else
            referenceEgg = Instantiate(eggPrefab, positionOfEgg, Quaternion.identity).GetComponent<egg>();

        eggsList.Add(referenceEgg);
    }

    public void eggEaten(egg eggToVanish)
    {
        score++;
        noOfEggsForNextLevel--;

        if (noOfEggsForNextLevel == 0)
        {
            score += 10;
            Debug.Log("golden eaten");
            levelUp();
        }

        else if (noOfEggsForNextLevel == 1)
        {
            createEgg(true);
        }

        else
        {
            createEgg(false);
        }


        if (score > hiScore)
        {
            hiScore = score;
            hiScoreTxt.text = "HiScore = " + hiScore;
        }

        scoreTxt.text = "Score = " + score;
        Debug.Log("eggs count " + noOfEggsForNextLevel);

        eggsList.Remove(eggToVanish);
        Destroy(eggToVanish.gameObject);
    }

    void killOldEggs()
    {
        foreach(egg eggInList in eggsList)
        {
            Destroy(eggInList.gameObject);
        }
        eggsList.Clear();
    }


    void createSpike()
    {
        Vector3 positionOfSpike;

        for(int i = 0; i < level; i++)
        {
            positionOfSpike.x = -WIDTH + Random.Range(1f, (WIDTH * 2) - 2f);
            positionOfSpike.y = -HEIGHT + Random.Range(1f, (HEIGHT * 2) - 2f);
            positionOfSpike.z = 0;

            spike referenceSpike = null;

            referenceSpike = Instantiate(spikePrefab, positionOfSpike, Quaternion.identity).GetComponent<spike>();

            spikesList.Add(referenceSpike);
        }
    }

    void killOldSpike()
    {
        foreach (spike spikeInList in spikesList)
        {
            Destroy(spikeInList.gameObject);
        }
        spikesList.Clear();
    }
}