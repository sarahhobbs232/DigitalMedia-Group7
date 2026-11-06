using UnityEngine;

public class Arrow : MonoBehaviour
{
    public float speed = 2.0f;
    Vector3 start;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        start = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.x <= -8.5) transform.position = start;
    }

    void FixedUpdate()
    {
        transform.position += new Vector3(-1 * speed * Time.deltaTime, 0, 0);
    }
}
