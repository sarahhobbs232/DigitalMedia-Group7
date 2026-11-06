using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerPlatformMove : MonoBehaviour
{
    public float moveSpeed = 5.0f;
    public float jumpHeight = 2.0f;
    public Rigidbody2D rb;
    public InputActionReference move;

    Vector2 movement;
    bool grounded = true;

    public PlatformManager manager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        movement = move.action.ReadValue<Vector2>();
        //jumping = move.action.ReadValue<KeyControl>();

    }

    void FixedUpdate()
    {
        rb.linearVelocityX = movement.x * 100 * moveSpeed * Time.deltaTime;
    }

    public void OnJump()
    {
        if (grounded)
        {
            grounded = false;
            rb.linearVelocityY = jumpHeight * Time.deltaTime;
        }
    }


    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Hazard") manager.Finish(false);
        if (other.tag == "Victory") manager.Finish(true);
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        grounded = true;
    }
}
