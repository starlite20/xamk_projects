using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class gameController : MonoBehaviour
{
    //Creating a singleton
    static public gameController instance = null;

    public deck playerDeck = new deck();
    public deck enemyDeck = new deck();

    public player userPlayer = null;
    public player enemyPlayer = null;

    public List<cardData> cardsAll = new List<cardData>();

    public Sprite[] healthNmbrs = new Sprite[10];
    public Sprite[] damageNmbrs = new Sprite[10];

    public hand playersHand;
    public hand enemysHand;

    public GameObject cardPrefab = null;
    public Canvas canvas = null;

    public bool isPlayable = false;

    //effect prefabs
    public GameObject effectLtoR = null;
    public GameObject effectRtoL = null;

    public GameObject nearingDeath = null;

    //sprites for cards
    public Sprite fireBallImg = null;
    public Sprite iceBallImg = null;
    public Sprite multiFireBallImg = null;
    public Sprite multiIceBallImg = null;
    public Sprite fireIceBallImg = null;
    public Sprite destructBallImg = null;

    public Text scoreText = null;
    public int playerScore = 0;
    public int playerKills = 0;

    public Text turnText = null;
    public bool playersTurn = true;

    public Image enemySkipTurnImg = null;
    
    public Sprite fireDemon = null;
    public Sprite iceDemon = null;

    public AudioSource playerDieAudio = null;
    public AudioSource enemyDieAudio = null;

    //death animation objects
    public GameObject deadGuyAnimation = null;
    public Image deadGuyAnimationBG = null;
    public Image deadGuyAnimationChar = null;

    public Sprite iceManDead = null;
    public Sprite fireManDead = null;
    public Sprite heroManDead = null;
    public Sprite iceManDeadBG = null;
    public Sprite fireManDeadBG = null;
    public Sprite heroManDeadBG = null;

    private void Awake()
    {
        instance = this;

        //Random enemy
        setUpEnemy();

        //Generate Deck
        playerDeck.createDeck();
        enemyDeck.createDeck();

        //Deal Cards
        StartCoroutine(dealHands());

        isPlayable = true;
        deadGuyAnimation.SetActive(false);
    }

    public void quitGame()
    {
        SceneManager.LoadScene(0);
    }


    internal IEnumerator dealHands()
    {
        
        yield return new WaitForSeconds(0.5f);

        //t < 3 cards
        for (int t=0; t<3; t++)
        {   
            isPlayable = false;
            playerDeck.dealCards(playersHand);
            enemyDeck.dealCards(enemysHand);
            yield return new WaitForSeconds(1.5f);
        }
        isPlayable = true;
    }

    internal bool useCard(card cardToUse, player usingOnPlayer, hand fromHand)
    {
        //check if card valid
        if (!cardValid(cardToUse, usingOnPlayer, fromHand))
            return false;

        isPlayable = false;
        castCard(cardToUse, usingOnPlayer, fromHand);

        userPlayer.glowImg.gameObject.SetActive(false);
        enemyPlayer.glowImg.gameObject.SetActive(false);

        //remove card
        fromHand.removeUsedCard(cardToUse);

        return false;
    }

    internal bool cardValid(card cardToCheck, player usingOnPlayer, hand fromHand)
    {
        bool valid = false;

        if(cardToCheck == null)
            return false;

        if(fromHand.isPlayer)
        {
            //Players hand
            if(cardToCheck.cardDataObj.cost <= userPlayer.mana)
            {
                //Player must use Defense Cards on themselves
                if (usingOnPlayer.isPlayer && cardToCheck.cardDataObj.isDefenseCard)
                    valid = true;
                if (!usingOnPlayer.isPlayer && !cardToCheck.cardDataObj.isDefenseCard)
                    valid = true;
            }
        }
        else
        {
            //Enemies Hard
            if (cardToCheck.cardDataObj.cost <= enemyPlayer.mana)
            {
                //Player must use Defense Cards on themselves
                if (!usingOnPlayer.isPlayer && cardToCheck.cardDataObj.isDefenseCard)
                    valid = true;
                if (usingOnPlayer.isPlayer && !cardToCheck.cardDataObj.isDefenseCard)
                    valid = true;
            }
        }

        return valid;
    }


    internal void castCard(card cardToCast, player usingOnPlayer, hand fromHand)
    {
        if(cardToCast.cardDataObj.isMirrorCard) //mirror effect
        {
            usingOnPlayer.sfxMirror();
            usingOnPlayer.setMirror(true);
            nextPlayersTurn();
            //isPlayable = true;
        }

        else
        {
            if(cardToCast.cardDataObj.isDefenseCard) //health card
            {
                usingOnPlayer.sfxHeal();

                usingOnPlayer.health += cardToCast.cardDataObj.damage;
                if(usingOnPlayer.health > usingOnPlayer.maxHealth)
                {
                    usingOnPlayer.health = usingOnPlayer.maxHealth;
                }

                updateHealthForPlayers();
                StartCoroutine(castHealEffect(usingOnPlayer));
            }

            else //attack cards
            {
                castAttackEffect(cardToCast, usingOnPlayer);
            }

            //Score update
            if (fromHand.isPlayer)
            {
                playerScore += cardToCast.cardDataObj.damage;
                updateScore();
            }

        }

        //update player mana
        if(fromHand.isPlayer)
        {
            gameController.instance.userPlayer.mana -= cardToCast.cardDataObj.cost;
            gameController.instance.userPlayer.updateManaBalls();
        }
        else
        {
            gameController.instance.enemyPlayer.mana -= cardToCast.cardDataObj.cost;
            gameController.instance.enemyPlayer.updateManaBalls();
        }
    }

    private IEnumerator castHealEffect(player usingOnPlayer)
    {
        yield return new WaitForSeconds(0.5f);
        nextPlayersTurn();
        //isPlayable = true;
    }

    internal void castAttackEffect(card cardBeingUsed, player usingOnPlayer)
    {
        GameObject effectGO = null;

        if(usingOnPlayer.isPlayer)
        {
            
            effectGO = Instantiate(effectRtoL, canvas.gameObject.transform);
        }
        else
        {
            
            effectGO = Instantiate(effectLtoR, canvas.gameObject.transform);
        }

        effect effectInProgress = effectGO.GetComponent<effect>();
        if(effectInProgress != null)
        {
            effectInProgress.targetPlayer = usingOnPlayer;
            effectInProgress.sourceCard = cardBeingUsed;
            switch(cardBeingUsed.cardDataObj.damageTypeObj)
            {
                case cardData.damageType.Fire   :
                        effectInProgress.sfxFire();
                        if(cardBeingUsed.cardDataObj.isMultipleAttack)
                            effectInProgress.effectImage.sprite = multiFireBallImg;
                        else
                            effectInProgress.effectImage.sprite = fireBallImg;
                        break;

                case cardData.damageType.Ice:
                    effectInProgress.sfxIce();
                    if (cardBeingUsed.cardDataObj.isMultipleAttack)
                            effectInProgress.effectImage.sprite = multiIceBallImg;
                        else
                            effectInProgress.effectImage.sprite = iceBallImg;

                        break;

                case cardData.damageType.Both   :
                        effectInProgress.sfxFire();
                        effectInProgress.sfxIce();
                        effectInProgress.effectImage.sprite = fireIceBallImg;
                        break;

                case cardData.damageType.Destroy:
                        effectInProgress.sfxDestruct();
                        effectInProgress.effectImage.sprite = destructBallImg;
                        break;
            }
        }
    }

    internal void updateHealthForPlayers()
    {
        userPlayer.updateHealth();
        enemyPlayer.updateHealth();

        if(userPlayer.health == 1)
        {
            nearingDeath.SetActive(true);
        }
        else
        {
            nearingDeath.SetActive(false);
        }

        if(userPlayer.health <= 0)
        {
            deadGuyAnimationCalled("Player");
            StartCoroutine(gameOver());
        }

        if(enemyPlayer.health <= 0)
        {
            playerKills += 1;
            playerScore += 100;
            updateScore();

            deadGuyAnimationCalled("Enemy");
            StartCoroutine(newEnemy());
        }
    }

    private IEnumerator gameOver()
    {
        yield return new WaitForSeconds(3);
        SceneManager.LoadScene(2);
    }

    private void deadGuyAnimationCalled(string deadPlayer)
    {
        GameObject deadGuyAnimCreated = GameObject.Instantiate(gameController.instance.deadGuyAnimation,
                                                    gameController.instance.canvas.gameObject.transform);
        //deadGuyAnimCreated.transform.position = new Vector3(0, 0, 0);

        Debug.Log("DEAD GUY!!");

        GameObject deadGuyAnimTemp = null;
        deadGuyAnimTemp = deadGuyAnimCreated.transform.Find("BG").gameObject;
        deadGuyAnimationBG = deadGuyAnimTemp.GetComponent<Image>();
        deadGuyAnimTemp = deadGuyAnimCreated.transform.Find("DeadGuy").gameObject;
        deadGuyAnimationChar = deadGuyAnimTemp.GetComponent<Image>();

        deadGuyAnimTemp = deadGuyAnimCreated.transform.Find("Text").gameObject;

        if (deadPlayer == "Player")
        {
            deadGuyAnimationBG.sprite = heroManDeadBG;
            deadGuyAnimationChar.sprite = heroManDead;
            deadGuyAnimTemp.GetComponent<Text>().text = "Merlin\nDied...";
            deadGuyAnimCreated.SetActive(true);
            deadGuyAnimCreated.GetComponent<Animator>().Play("deadHeroAnimated");
        }

        else if(deadPlayer == "Enemy")
        {
            deadGuyAnimTemp.GetComponent<Text>().text = "Demon\nEliminated";
            if (enemyPlayer.isFire)
            {
                deadGuyAnimationBG.sprite = fireManDeadBG;
                deadGuyAnimationChar.sprite = fireManDead;
                deadGuyAnimCreated.SetActive(true);
                deadGuyAnimCreated.GetComponent<Animator>().Play("deadEnemyAnimated");
            }
            else
            {
                deadGuyAnimationBG.sprite = iceManDeadBG;
                deadGuyAnimationChar.sprite = iceManDead;
                deadGuyAnimCreated.SetActive(true);
                deadGuyAnimCreated.GetComponent<Animator>().Play("deadEnemyAnimated");
            }
        }


        StartCoroutine(deathAnimPlayback(deadGuyAnimCreated));
    }


    private IEnumerator deathAnimPlayback(GameObject deleteThis)
    {
        yield return new WaitForSeconds(3);
        Destroy(deleteThis);
    }

    private IEnumerator newEnemy()
    {
        //hide old enemy
        enemyPlayer.gameObject.SetActive(false);

        //clear hand
        enemysHand.clearHand();

        yield return new WaitForSeconds(4);

        //set new enemy
        setUpEnemy();

        //show new enemy
        enemyPlayer.gameObject.SetActive(true);

        //deal new hand
        StartCoroutine(dealHands());
}

    private void setUpEnemy()
    {
        enemyPlayer.mana = 0;
        enemyPlayer.health = 5;
        enemyPlayer.isFire = true;
        enemyPlayer.playerImg.sprite = fireDemon;
        enemyPlayer.updateHealth();
        enemyPlayer.updateManaBalls();

        //Setting Fire or Ice demon by 50% probability
        if( (UnityEngine.Random.Range(0,2)) == 1 )
        {
            enemyPlayer.isFire = false;
            enemyPlayer.playerImg.sprite = iceDemon;
        }

        
    }

   
    //Turn controller
    internal void nextPlayersTurn()
    {
        isPlayable = false;
        playersTurn = !playersTurn;
        bool enemyIsDead = false;

        if(playersTurn)
        {
            isPlayable = true;
            if(userPlayer.mana < 5)
                userPlayer.mana++;
        }
        else
        {
            if (enemyPlayer.health > 0)
            {
                if (enemyPlayer.mana < 5)
                {
                    enemyPlayer.mana++;
                }
            }
            else
                enemyIsDead = true;
        }

        if (enemyIsDead)
        {
            playersTurn = !playersTurn;

            if (userPlayer.mana < 5)
                userPlayer.mana++;
        }
        else
        {
            //Set turn text
            setTurnText();
            if (!playersTurn)
            {
                isPlayable = false;
                monsterPlays();
                
            }
        }

        //mana display
        userPlayer.updateManaBalls();
        enemyPlayer.updateManaBalls();

    }

    public void skipTurn()
    {
        if(playersTurn && isPlayable)
        {
            nextPlayersTurn();
            isPlayable = false;
        }   
    }

    internal void setTurnText()
    {
        if(playersTurn)
        {
            turnText.text = "Merlin's Turn";
        }
        else
        {
            turnText.text = "Demon's Turn";
        }
    }

    //The Monster AI
    private void monsterPlays()
    {
        card cardChosen = aiChooseCard();
        StartCoroutine(monsterCastCard(cardChosen));
    }

    private IEnumerator monsterCastCard(card cardToPlay)
    {
        yield return new WaitForSeconds(0.5f);

        if (cardToPlay)   //card available
        {
            //turn over card
            turnCard(cardToPlay);
            yield return new WaitForSeconds(1.5f);

            //use card
            if (cardToPlay.cardDataObj.isDefenseCard)           //defense card
                useCard(cardToPlay, enemyPlayer, enemysHand);
            else                                                //attack card 
                useCard(cardToPlay, userPlayer, enemysHand);
            yield return new WaitForSeconds(1f);

            //deal card
            enemyDeck.dealCards(enemysHand);
            yield return new WaitForSeconds(1f);
        }
        else  //no card to choose
        {
            //show skip turn
            enemySkipTurnImg.gameObject.SetActive(true);
            
            yield return new WaitForSeconds(1.5f);

            //hide enemy skip turn
            enemySkipTurnImg.gameObject.SetActive(false);
        }
        
        isPlayable = true;
    }

    internal void turnCard(card cardToTurn)
    {
        Animator anim = cardToTurn.GetComponentInChildren<Animator>();

        if(anim)
        {
            anim.SetTrigger("flipEnemyCard");
        }
        else
        {
            Debug.LogError("Anim failed");
        }
    }

    private card aiChooseCard()
    {
        List<card> validCardList = new List<card>();

        for(int i = 0;i<3;i++)
        {
            if(cardValid(enemysHand.cardsInHand[i], enemyPlayer, enemysHand))
            {
                validCardList.Add(enemysHand.cardsInHand[i]);
            }
            else if(cardValid(enemysHand.cardsInHand[i], userPlayer, enemysHand))
            {
                validCardList.Add(enemysHand.cardsInHand[i]);
            }
        }

        if(validCardList.Count == 0) //No valid card available
        {
            nextPlayersTurn();
            return null;
        }

        int choice = UnityEngine.Random.Range(0, validCardList.Count);
        return validCardList[choice];
    }

    private void updateScore()
    {
        scoreText.text = "Demons Killed : " + playerKills.ToString();
        scoreText.text += "   Score : " + playerScore.ToString();
    }


    internal void sfxUserDie()
    {
        playerDieAudio.Play();
    }
    internal void sfxEnemDie()
    {
        enemyDieAudio.Play();
    }
}
