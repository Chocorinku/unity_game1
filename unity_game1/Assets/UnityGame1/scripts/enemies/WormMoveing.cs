using UnityEngine;

public class WormMoveing : MonoBehaviour
{
    public float jumpForce = 30.0f;
    public float jumpInterval = 2.5f;

    private float timeSinceLastJump = 0f;
    private Rigidbody2D rbody;

    void Start()
    {
        rbody = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        timeSinceLastJump += Time.fixedDeltaTime;

        if (timeSinceLastJump >= jumpInterval)
        {
            rbody.AddForce(transform.up * jumpForce * 10);
            timeSinceLastJump = 0f;
        }
    }

}
