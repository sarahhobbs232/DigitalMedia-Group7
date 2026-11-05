using UnityEngine;

public class MainMenu : MonoBehaviour
{
    void Start()
    {
        Time.timeScale = 1f;
        GameStatManager.ResetRun();
    }

    public void StartGame()
    {
        GameStatManager.StartNewRun();
    }

    public void QuitGame()
    {
        GameStatManager.QuitGame();
    }
}
