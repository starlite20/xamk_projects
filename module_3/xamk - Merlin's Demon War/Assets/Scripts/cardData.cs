using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Card", menuName = "CardGame/Card")]
public class cardData : ScriptableObject
{
    public enum damageType
    {
        Fire,
        Ice,
        Both,
        Destroy
    }

    public string cardTitle;
    public string description;
    public int cost;
    public int damage;
    public damageType damageTypeObj;
    
    public Sprite cardImage;
    public Sprite frameImage;
    public int numberInDeck;

    public bool isDefenseCard = false;
    public bool isMirrorCard = false;
    public bool isMultipleAttack = false;
    public bool isDestruct = false;

}
