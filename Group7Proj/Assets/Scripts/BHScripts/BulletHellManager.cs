using UnityEngine;

public class BulletHellManager : MonoBehaviour
{
    public float timeLimit = 5f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timeLimit -= Time.deltaTime;
        if(timeLimit <= 0){
            GameStatManager.addScore(100);
            GameStatManager.LoadScene(1);
        }
    }
}
