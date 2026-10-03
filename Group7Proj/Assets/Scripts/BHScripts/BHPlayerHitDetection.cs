using UnityEngine;

public class BHPlayerHitDetection : MonoBehaviour
{
    public BulletHellManager manager;
    private bool finished;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnTriggerEnter2D(Collider2D other){
        if(other.gameObject.CompareTag("Bullet") && !finished){
            finished = true;
            Destroy(other.gameObject);
            if (manager == null)
                manager = FindFirstObjectByType<BulletHellManager>();

            if (manager != null)
                manager.Finish(false);
        } else if(other.gameObject.tag == "Coin"){
            //Progress Success Condition
        }
    }
}
