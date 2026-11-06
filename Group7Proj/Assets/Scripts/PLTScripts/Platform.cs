using UnityEngine;

public class Platform : MonoBehaviour
{
    public Transform player;
    public float speed = 1.0f;
    public float minHeight;
    public float maxHeight;
    public float y;
    bool movingUp = true;
    BoxCollider2D collider;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        collider = GetComponent<BoxCollider2D>();
        y = transform.position.y;

    }

    // Update is called once per frame
    void Update()
    {
        // Check if platform is enabled
        if (player.position.y >= transform.position.y + 0.5f) collider.enabled = true;
        else collider.enabled = false;
    }

    void FixedUpdate()
    {
        // Move platform
        //if (transform.position.y >= maxHeight || transform.position.y <= minHeight) movingUp = !movingUp;
        if (transform.position.y >= maxHeight) movingUp = false;
        if (transform.position.y <= minHeight) movingUp = true;

        if (movingUp) transform.position += Vector3.up * speed * Time.deltaTime;
        else transform.position += Vector3.up * -1 * speed * Time.deltaTime;
    }
}
