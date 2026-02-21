using UnityEngine;
using Mirror;

public class movement : NetworkBehaviour
{
    public float speed = 5f;
    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public override void OnStartLocalPlayer()
    {
        Debug.Log("OnStartLocalPlayer fired!");
    }

    void FixedUpdate()
    {
        if (!isLocalPlayer) return;

        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");

        Debug.Log($"input: {x},{y}");

        rb.linearVelocity = new Vector2(x, y) * speed;
    }
}
