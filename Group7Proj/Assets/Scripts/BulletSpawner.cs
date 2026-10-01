using UnityEngine;

public class BulletSpawner : MonoBehaviour
{
    enum SpawnerType {Aim, Spin, Straight}
    
    [SerializeField] private SpawnerType spawnerType;
    public GameObject bullet;
    public float bulletTime;
    public float speed = 5f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
