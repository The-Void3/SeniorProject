using UnityEngine;
using Mirror;

public class PlayerHealth : NetworkBehaviour
{
    public int health = 1;
    void TakeHealth(){
        health--;
        if (health <= 0){
            NetworkServer.Destroy(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("bullet")){
            TakeHealth();
        }
    }
}
