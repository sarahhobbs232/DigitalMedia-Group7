using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public TMP_Text timerText;
    public TMP_Text scoreText;
    public TMP_Text resultText;
    public Circle circle;
    public float timeLimit = 10f;
    public int target = 10;

    float timeLeft;
    int clicks;
    bool gameOver;

    void Start()
    {
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
        circle.gameObject.SetActive(false);
        resultText.text = won ? "You win!" : "Time's up!";
    }

    void UpdateUI()
    {
        timerText.text = "Time: " + timeLeft.ToString("F1");
        scoreText.text = clicks + " / " + target;
    }
}