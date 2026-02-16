using UnityEngine;

public class example : MonoBehaviour
{

    float speedX;
    float speedY;
    public float speed;
    Rigidbody2D square;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        square = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        // want player to move freely around
        speedX = Input.GetAxisRaw("Horizontal") * speed; //speed for player
        speedY = Input.GetAxisRaw("Vertical") * speed;

        square.linearVelocity = new Vector2(speedX, speedY);
    }
}
