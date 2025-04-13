using UnityEngine;
using UnityEngine.SceneManagement;

using UnityEngine;

public class ClickableButton : MonoBehaviour
{
    public enum ButtonType { Start, Options, Exit }
    public ButtonType buttonType;

    public MainMenuController menuController; // przypisz w Inspectorze

    private void OnMouseDown()
    {
        switch (buttonType)
        {
            case ButtonType.Start:
                if (menuController != null)
                    menuController.OnPlayClicked();
                else
                    Debug.LogWarning("Brak przypisanego MainMenuController!");
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
