using UnityEngine;

public class BulletHellManager : MonoBehaviour
{
    public float timeLimit = 5f;
    public int nextSceneIndex = 1;
    private SceneResultTransition sceneTransition;
    private bool finished;

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
    }

    // Update is called once per frame
    void Update()
    {
        if (finished)
            return;

        timeLimit -= Time.deltaTime;
        if(timeLimit <= 0){
            Finish(true);
        }
    }

    public void Finish(bool won)
    {
        if (finished)
            return;

        finished = true;
        GameStatManager.addScore(won ? 100 : 0);
        if (!won)
            GameStatManager.takeDamage(1, false);

        BHPlayerHitDetection hitDetection = FindFirstObjectByType<BHPlayerHitDetection>();
        Transform player = hitDetection != null ? hitDetection.transform : null;
        if (player == null)
            player = transform;

        if (sceneTransition != null)
            sceneTransition.Play(player, won, nextSceneIndex);
        else
            GameStatManager.LoadScene(nextSceneIndex);
    }
}
