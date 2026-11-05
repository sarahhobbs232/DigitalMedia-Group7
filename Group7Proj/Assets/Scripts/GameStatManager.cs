using UnityEngine;
using UnityEngine.SceneManagement;

public static class GameStatManager
{
    public const string MainMenuScene = "MainMenu";
    public const string GameOverScene = "GameOver";
    public const int StartingHealth = 3;

    private static string[] SceneNames = {"BulletHell", "CircleClicker", "FPSShooter"};
    public static int health = StartingHealth;
    public static int score = 0;

    // Score of the run that just ended, shown on the Game Over screen.
    public static int finalScore { get; private set; }

    // Resets static state when entering Play Mode with domain reload turned off.
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetStatics()
    {
        health = StartingHealth;
        score = 0;
        finalScore = 0;
    }

    public static bool IsMinigameScene(string sceneName)
    {
        return System.Array.IndexOf(SceneNames, sceneName) >= 0;
    }

    public static void LoadScene(int index = 0){
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneNames[index]);
    }

    // Use this after a minigame ends: goes to the next minigame, or to Game Over if out of health.
    public static void AdvanceToScene(int nextIndex){
        if (health <= 0)
            GameOver();
        else
            LoadScene(nextIndex);
    }

    public static void addScore(int increase){
        score += increase;
    }

    public static void takeDamage(int decrease = 1, bool endRunWhenEmpty = true){
        health -= decrease;
        if(health <= 0 && endRunWhenEmpty){
            GameOver();
        }
    }

    public static void ResetRun(){
        health = StartingHealth;
        score = 0;
    }

    public static void StartNewRun(){
        ResetRun();
        LoadScene();
    }

    // Kept so older calls still work.
    public static void restartGame(){
        StartNewRun();
    }

    public static void GameOver(){
        finalScore = score;
        Time.timeScale = 1f;
        SceneManager.LoadScene(GameOverScene);
    }

    public static void ReturnToMainMenu(){
        ResetRun();
        Time.timeScale = 1f;
        SceneManager.LoadScene(MainMenuScene);
    }

    public static void QuitGame(){
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
