using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public TMP_Text timerText;
    public TMP_Text scoreText;
    public TMP_Text resultText;
    public Circle circle;
    public int nextSceneIndex = 0;
    public float timeLimit = 10f;
    public int target = 10;

    float timeLeft;
    int clicks;
    bool gameOver;
    SceneResultTransition sceneTransition;

    void Start()
    {
        Camera mainCamera = Camera.main;
        if (mainCamera != null)
        {
            sceneTransition = mainCamera.GetComponent<SceneResultTransition>();
            if (sceneTransition == null)
                sceneTransition = mainCamera.gameObject.AddComponent<SceneResultTransition>();
        }

        timeLeft = timeLimit;
        resultText.text = "";
        circle.MoveToRandomSpot();
        UpdateUI();
    }

    void Update()
    {
        if (gameOver) return;

        timeLeft -= Time.deltaTime;
        if (timeLeft <= 0f)
        {
            timeLeft = 0f;
            EndGame(false);
        }
        UpdateUI();
    }

    public void RegisterClick()
    {
        if (gameOver) return;

        clicks++;
        if (clicks >= target) EndGame(true);
        else circle.MoveToRandomSpot();
        UpdateUI();
    }

    void EndGame(bool won)
    {
        gameOver = true;
        resultText.text = "";
        GameStatManager.addScore(100 * clicks);
        if (!won)
            GameStatManager.takeDamage(1, false);

        if (sceneTransition != null)
            sceneTransition.Play(circle.transform, won, nextSceneIndex);
        else
            GameStatManager.LoadScene(nextSceneIndex);
    }

    void UpdateUI()
    {
        timerText.text = "Time: " + timeLeft.ToString("F1");
        scoreText.text = clicks + " / " + target;
    }
}