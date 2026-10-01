using UnityEngine;

public class BulletSpawner : MonoBehaviour
{
    enum SpawnerType {Aim, Spin, Straight}
    
    [SerializeField] private SpawnerType spawnerType;
    [SerializeField] private float firingRate;
    [SerializeField] private float rotationSpeed;

    public GameObject bullet;
    public float bulletTime;
    public float speed = 5f;
    public float bulletCount;

    private float timer = 0f;
    private GameObject spawnedBullet;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if(timer >= firingRate){
            performAimBehavior();
            timer = 0;
        }
        
    }

    private void performAimBehavior(){
        if(spawnerType == SpawnerType.Spin){
            transform.eulerAngles = new Vector3(0f, 0f, transform.eulerAngles.z+rotationSpeed);
            StandardFire();
        } else if (spawnerType == SpawnerType.Aim){
            
        }
    }

    private void StandardFire(){
        if(bullet){
            spawnedBullet = Instantiate(bullet, transform.position, Quaternion.identity);
            spawnedBullet.GetComponent<Bullet>().speed = speed;
            spawnedBullet.GetComponent<Bullet>().bulletTime = bulletTime;
            spawnedBullet.transform.rotation = transform.rotation;
        }
    }

    private void FireTowardsSelf(){
        
    }
}
