using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
    using UnityEditor;
#endif

public class mainMenu : MonoBehaviour
{
    public void newGame()
    {
        SceneManager.LoadScene(1);
    }

    public void quitGame()
    {
        //We use preprocessor directives to check whether it is the build or not.
        #if UNITY_EDITOR
        //For Unity Editor build
            EditorApplication.isPlaying = false;

        #else
        //For a finished build
            Application.Quit();

        #endif
    }
}
