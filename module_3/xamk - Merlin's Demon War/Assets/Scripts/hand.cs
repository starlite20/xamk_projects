using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class hand
{
    public card[]       cardsInHand     = new card[3];
    public Transform[]  positions       = new Transform[3];
    public string[]     animationNames  = new string[3];
    public bool         isPlayer;

    public void removeUsedCard(card cardToBurn)
    {
        for(int i=0; i<3; i++)
        {
            if(cardsInHand[i] == cardToBurn)
            {
                GameObject.Destroy(cardsInHand[i].gameObject);
                cardsInHand[i] = null;

                if(isPlayer)
                {
                    gameController.instance.playerDeck.dealCards(this);
                }
                else
                {
                    gameController.instance.enemyDeck.dealCards(this);
                }
                break;
            }
        }
    }

    internal void clearHand()
    {
        for(int c=0; c<3; c++)
        {
            GameObject.Destroy(cardsInHand[c].gameObject);
            cardsInHand[c] = null;
        }
    }
}
