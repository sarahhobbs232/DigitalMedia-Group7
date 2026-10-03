using UnityEngine;

public class BHPlayerHitDetection : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnTriggerEnter2D(Collider2D other){
        if(other.gameObject.tag == "Bullet"){
            GameStatManager.takeDamage(1);
            Destroy(other.gameObject);
        } else if(other.gameObject.tag == "Coin"){
            //Progress Success Condition
        }
    }
}
