using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class player : MonoBehaviour, IDropHandler
{
    public Image playerImg = null;
    public Image mirrorImg = null;
    public Image healthNumberImg = null;
    public Image glowImg = null;

    public int maxHealth = 5;
    public int health = 5;  //current health

    public bool isPlayer;
    public bool isFire;

    public int mana = 1;
    public GameObject[] manaBalls = new GameObject[5];

    private Animator animator = null;

    public AudioSource dealAudio = null;
    public AudioSource healAudio = null;
    public AudioSource mirrorAudio = null;
    public AudioSource smashAudio = null;
    

    // Start is called before the first frame update
    void Start()
    {
        //you can also use the public method and drag it
        animator = GetComponent<Animator>();
        updateHealth();
        updateManaBalls();
    }

    internal void playHitAnim()
    {
        if(animator!=null)
        {
            animator.SetTrigger("Hit");
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (gameController.instance.isPlayable != true)
            return;


        //Card dropped onto player or enemy
        GameObject draggedObj = eventData.pointerDrag;
        if( draggedObj != null )
        {
            card draggedCard = draggedObj.GetComponent<card>();
            if(draggedCard != null)
            {
                gameController.instance.useCard(draggedCard, this, gameController.instance.playersHand);
            }
        }
    }

    internal void updateHealth()
    {
        if( (health >= 0) && (health < gameController.instance.healthNmbrs.Length) )
        {
            healthNumberImg.sprite = gameController.instance.healthNmbrs[health];
        }
        else
        {
            Debug.LogError("Invalid health number " + health.ToString());
        }
    }

    internal void setMirror(bool on)
    {
        mirrorImg.gameObject.SetActive(on);
    }

    internal bool hasMirror()
    {
        return mirrorImg.gameObject.activeInHierarchy;            
    }

    //mana ball
    internal void updateManaBalls()
    {
        
        for (int m = 0; m < 5; m++)
        {
            if(mana > m)
            {
                manaBalls[m].SetActive(true);
            }
            else
            {
                manaBalls[m].SetActive(false);
            }
        }
    }

    //sfx
    internal void sfxDeal()
    {
        dealAudio.Play();
    }
    internal void sfxSmash()
    {
        smashAudio.Play();
    }
    internal void sfxHeal()
    {
        healAudio.Play();
    }
    internal void sfxMirror()
    {
        mirrorAudio.Play();
    }

}
