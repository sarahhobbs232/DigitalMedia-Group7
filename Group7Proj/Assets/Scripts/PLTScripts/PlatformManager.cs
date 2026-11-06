using UnityEngine;
using TMPro;

public class PlatformManager : MonoBehaviour
{
    public TMP_Text timerText;
    public float timeLimit = 15f;
    public int nextSceneIndex = 0;
    public Transform player;

    SceneResultTransition sceneTransition;
    bool gameOver = false;
    float timeLeft;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
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
        UpdateUI();
    }

    // Update is called once per frame
    void Update()
    {
        if (gameOver) return;
        
        timeLeft -= Time.deltaTime;
        if (timeLeft <= 0f)
        {
            timeLeft = 0.0f;
            Finish(false);
        }
        UpdateUI();
    }

    public void Finish(bool won)
    {
        gameOver = true;
        GameStatManager.addScore(100 * (int)(timeLimit - timeLeft));
        if (!won)
            GameStatManager.takeDamage(1, false);

        if (sceneTransition != null)
            sceneTransition.Play(player, won, nextSceneIndex);
        else
            GameStatManager.AdvanceToScene(nextSceneIndex);
    }

    void UpdateUI()
    {
        timerText.text = "Time: " + timeLeft.ToString("F1");
    }
}
