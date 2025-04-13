using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public GameObject[] menuObjectsToDisable; // Obiekty: Title, Play, Exit, tlo, Credits...
    public GameObject animationStartObject;   // np. AnimacjaStart
    public Animator animationStartAnimator;   // Animator na AnimacjaStart
    public string animationEndTrigger = "End"; // Trigger do zakończenia (jeśli masz)
    public string nextSceneName = "GameScene";

    public void OnPlayClicked()
    {
        // Wyłącz inne elementy menu
        foreach (var obj in menuObjectsToDisable)
        {
            if (obj != null) obj.SetActive(false);
        }

        // Włącz animację startową
        animationStartObject.SetActive(true);

        // Zacznij coroutine czekania na koniec animacji
        StartCoroutine(WaitForAnimationAndLoadScene());
    }

    private IEnumerator WaitForAnimationAndLoadScene()
    {
        // Czekamy do końca animacji (np. jeśli ma 3 sekundy)
        // Można też sprawdzić animację przez AnimatorStateInfo
        float animationLength = animationStartAnimator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(animationLength);

        // Można też odpalić trigger i czekać aż się zakończy przez Animation Events lub flagę

        SceneManager.LoadScene(nextSceneName);
    }
}