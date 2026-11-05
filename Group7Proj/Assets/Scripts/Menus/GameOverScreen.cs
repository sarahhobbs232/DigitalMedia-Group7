using TMPro;
using UnityEngine;

public class GameOverScreen : MonoBehaviour
{
    [SerializeField] private TMP_Text scoreText;

    void Start()
    {
        Time.timeScale = 1f;
        if (scoreText != null)
            scoreText.text = "Final Score: " + GameStatManager.finalScore;
    }

    public void Retry()
    {
        GameStatManager.StartNewRun();
    }

    public void ToMainMenu()
    {
        GameStatManager.ReturnToMainMenu();
    }

    public void QuitGame()
    {
        GameStatManager.QuitGame();
    }
}
