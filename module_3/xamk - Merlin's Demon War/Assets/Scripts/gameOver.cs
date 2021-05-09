using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class gameOver : MonoBehaviour
{
    public Text scoreText = null;
    public Text killText = null;

    private void Awake()
    {
        scoreText.text = "Score : " + gameController.instance.playerScore.ToString();
        killText.text = "Demons Killed : " + gameController.instance.playerKills.ToString();
    }

    public void mainMenu()
    {
        SceneManager.LoadScene(0);
    }
}
