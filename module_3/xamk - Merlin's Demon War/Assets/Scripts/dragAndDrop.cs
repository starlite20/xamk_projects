using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class dragAndDrop : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Vector3 originalPosition;
    public void OnBeginDrag(PointerEventData eventData)
    {
        originalPosition = transform.position;

        /* By default, GUI Elements block raycasts
         * To resolve it, we add Canvas Group component to the Inspector panel.
         * It provides the option to enable Raycasts or block them.
         */

        GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (gameController.instance.isPlayable != true)
            return;
         
        transform.position += (Vector3)(eventData.delta);
        card currentCard = GetComponent<card>();
        bool overCard = false;

        //hovered is an array of objects that are being active at the moment
        foreach(GameObject hover in eventData.hovered)
        {
            player playerCard = hover.GetComponent<player>();
            if(playerCard != null)
            {
                if (gameController.instance.cardValid(currentCard, playerCard, gameController.instance.playersHand))
                {
                    playerCard.glowImg.gameObject.SetActive(true);
                    overCard = true;
                }
            }

            burnZone checkForBurnZone = hover.GetComponent<burnZone>();
            if(checkForBurnZone != null)
            {
                currentCard.burnImg.gameObject.SetActive(true);
            }
            else
            {
                currentCard.burnImg.gameObject.SetActive(false);
            }
        }

        if(!overCard)
        {
            gameController.instance.userPlayer.glowImg.gameObject.SetActive(false);
            gameController.instance.enemyPlayer.glowImg.gameObject.SetActive(false);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        transform.position = originalPosition;

        GetComponent<CanvasGroup>().blocksRaycasts = true;
    }
}
