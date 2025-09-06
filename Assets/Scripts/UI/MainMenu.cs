using GlobalControllers;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void ContinueGame()
    {
        SceneController.Instance.StartGame();
    }

    public void NewGame()
    {
        SceneController.Instance.StartGame(true);
    }

    public void QuitGame()
    {
        SceneController.Instance.QuitGame();
    }
}
