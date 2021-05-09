using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class card : MonoBehaviour
{
    public cardData cardDataObj = null;

    public Text titleTxt = null;
    public Text descriptionTxt = null;
    public Image damageImg = null;
    public Image costNumberImg = null;
    public Image cardImg = null;
    public Image frameImg = null;
    public Image burnImg = null;

    public void initialise()
    {
        if(cardDataObj == null)
        {
            Debug.LogError("Card has no card Data...");
        }

        titleTxt.text = cardDataObj.cardTitle;
        descriptionTxt.text = cardDataObj.description;
        cardImg.sprite = cardDataObj.cardImage;
        frameImg.sprite = cardDataObj.frameImage;

        costNumberImg.sprite = gameController.instance.healthNmbrs[cardDataObj.cost];
        damageImg.sprite = gameController.instance.damageNmbrs[cardDataObj.damage];
    }
}
