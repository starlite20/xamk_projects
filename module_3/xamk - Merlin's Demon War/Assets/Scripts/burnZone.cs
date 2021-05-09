using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class burnZone : MonoBehaviour, IDropHandler
{
    public AudioSource burnAudio = null;
    public void OnDrop(PointerEventData eventData)
    {
        //Card Dropped into Burn Zone

        //pointerDrag is a ptr referring to the gameObject that is being dragged
        GameObject objDragged = eventData.pointerDrag;
        //we retrieve the card from the dragged
        card cardDragged = objDragged.GetComponent<card>();

        if(cardDragged != null)
        {
            //card found
            sfxBurn();
            gameController.instance.playersHand.removeUsedCard(cardDragged);
            gameController.instance.nextPlayersTurn();
            gameController.instance.isPlayable = false;
        }
    }

    internal void sfxBurn()
    {
        burnAudio.Play();
    }
}
