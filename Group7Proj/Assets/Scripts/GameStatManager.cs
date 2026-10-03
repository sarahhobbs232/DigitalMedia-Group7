using UnityEngine;
using UnityEngine.SceneManagement;

public static class GameStatManager
{
    private static string[] SceneNames = {"BulletHell", "CircleClicker", "FPSShooter"};
    public static int health = 3;
    public static int score = 0;
    

    public static void LoadScene(int index = 0){
        SceneManager.LoadScene(SceneNames[index]);
    }

    public static void addScore(int increase){
        score += increase;
    }

    public static void takeDamage(int decrease = 1){
        health -= decrease; 
        if(health <= 0){
            restartGame();
        }
    }

    public static void restartGame(){
        health = 3;
        score = 0;
        LoadScene();
    }
}
