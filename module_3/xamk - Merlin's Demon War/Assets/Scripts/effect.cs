using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class effect : MonoBehaviour
{
    public player targetPlayer = null;
    public card sourceCard = null;
    public Image effectImage = null;

    public AudioSource iceAudio = null;
    public AudioSource fireAudio = null;
    public AudioSource destructAudio = null;

    public void endTrigger()
    {
        bool bounce = false;

        //check if opponent has mirror first, else attack is usccessful
        if (targetPlayer.hasMirror())
        {
            bounce = true;

            targetPlayer.setMirror(false);
            targetPlayer.sfxSmash();

            if(targetPlayer.isPlayer)
            {
                gameController.instance.castAttackEffect(sourceCard, gameController.instance.enemyPlayer);
            }
            else
            {
                gameController.instance.castAttackEffect(sourceCard, gameController.instance.userPlayer);
            }
        }
        else
        {
            int damage = sourceCard.cardDataObj.damage;

            //check if target is enemy or not
            if (!targetPlayer.isPlayer)
            {
                //it is the enemy

                //fire ball
                if (sourceCard.cardDataObj.damageTypeObj == cardData.damageType.Fire && targetPlayer.isFire)
                {
                    damage = damage / 2;
                }

                //ice ball
                if (sourceCard.cardDataObj.damageTypeObj == cardData.damageType.Ice && !targetPlayer.isFire)
                {
                    damage = damage / 2;
                }

                //destroy ball
                if (sourceCard.cardDataObj.damageTypeObj == cardData.damageType.Destroy)
                {
                    damage = 9;
                }
            }

            targetPlayer.health -= damage;
            targetPlayer.playHitAnim();

            if(!bounce)
                gameController.instance.nextPlayersTurn();

            gameController.instance.updateHealthForPlayers();


            //check for death
            if(targetPlayer.health <= 0)
            {
                targetPlayer.health = 0;
                
                if(targetPlayer.isPlayer)
                {
                    gameController.instance.sfxUserDie();
                }
                else
                {
                    gameController.instance.sfxEnemDie();
                }
            }

        }   
     
        //stop animation
        Destroy(gameObject);
        gameController.instance.isPlayable = true;
    }


    internal void sfxIce()
    {
        iceAudio.Play();
    }
    internal void sfxFire()
    {
        fireAudio.Play();
    }
    internal void sfxDestruct()
    {
        destructAudio.Play();
    }
}
