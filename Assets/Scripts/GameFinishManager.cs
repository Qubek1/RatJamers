using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameFinishManager : MonoBehaviour
{
    public string sceneToLoadName;
    public float sceneLoadDelay;

    public GameObject redWinScreen;
    public GameObject greenWinScreen;

    public MainProgressBarSnap playerRedProgressBar;
    public MainProgressBarSnap playerGreenProgressBar;

    private bool over = false;

    private void Awake()
    {
        playerRedProgressBar.OnProgressFull += onRedWin;
        playerGreenProgressBar.OnProgressFull += onGreenWin;
    }

    private void onRedWin()
    {
        if (over)
        {
            return;
        }
        over = true;
        redWinScreen.SetActive(true);
        StartCoroutine(loadSceneAfterDelay());
    }

    private void onGreenWin()
    {
        if (over)
        {
            return;
        }
        over = true;
        greenWinScreen.SetActive(true);
        StartCoroutine(loadSceneAfterDelay());
    }

    private IEnumerator loadSceneAfterDelay()
    {
        yield return new WaitForSeconds(sceneLoadDelay);
        SceneManager.LoadScene(sceneToLoadName);
    }
}
