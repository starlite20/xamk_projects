using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class deck
{
    public List<cardData> cardsInDeck = new List<cardData>();

    public void createDeck()
    {
        //1. create list of card data for the pack
        List<cardData> cardDataInOrder = new List<cardData>();
        foreach(cardData currentCardData in gameController.instance.cardsAll)
        {
            for( int i=0; i<currentCardData.numberInDeck; i++)
            {
                cardDataInOrder.Add(currentCardData);
            }
        }

        //2. randomize the order
        while(cardDataInOrder.Count > 0)
        {
            int randomIndex = UnityEngine.Random.Range(0, cardDataInOrder.Count);
            cardsInDeck.Add(cardDataInOrder[randomIndex]);
            cardDataInOrder.RemoveAt(randomIndex);
        }
    }

    private cardData randomCard()
    {
        cardData result = null;

        if(cardsInDeck.Count == 0)
        {
            //we ran out of cards in deck
            createDeck();
        }

        result = cardsInDeck[0];
        cardsInDeck.RemoveAt(0);

        return result;
    }

    private card createNewCard(Vector3 position, string animName)
    {
        GameObject newCard = GameObject.Instantiate(gameController.instance.cardPrefab,
                                                    gameController.instance.canvas.gameObject.transform);
        newCard.transform.position = position;
        card generatedCard = newCard.GetComponent<card>();

        if(generatedCard)
        {
            generatedCard.cardDataObj = randomCard();
            generatedCard.initialise();

            Animator animator = newCard.GetComponentInChildren<Animator>();
            if(animator)
            {
                animator.CrossFade(animName,0); //name, duration
            }
            else
            {
                Debug.LogError("No animator found!");
            }

            return generatedCard;
        }
        else
        {
            Debug.LogError("No card component found!");
            return null;
        }
    }

    //function to deal the hand
    public void dealCards(hand selectedHand)
    {
        for(int h=0; h<3; h++)
        {
            if(selectedHand.isPlayer)
            {
                gameController.instance.userPlayer.sfxDeal();
            }
            else
            {
                gameController.instance.enemyPlayer.sfxDeal();
            }

            if (selectedHand.cardsInHand[h] == null)
            {
                selectedHand.cardsInHand[h] = createNewCard(selectedHand.positions[h].position, selectedHand.animationNames[h]);
                return;
            }
        }
    }
}
