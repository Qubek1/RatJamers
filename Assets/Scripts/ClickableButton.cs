using UnityEngine;
using UnityEngine.SceneManagement;

public class ClickableButton : MonoBehaviour
{
    public enum ButtonType { Start, Options, Exit }
    public ButtonType buttonType;

    private void OnMouseDown()
    {
        switch (buttonType)
        {
            case ButtonType.Start:
                SceneManager.LoadScene("SZYMON_MERGE_FINAL"); // zmień na własną scenę
                break;
            case ButtonType.Options:
                Debug.Log("Opcje jeszcze niezaimplementowane");
                break;
            case ButtonType.Exit:
                Application.Quit();
                Debug.Log("Wychodzę z gry...");
                break;
        }
    }
}