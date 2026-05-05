using UnityEngine;
using Mirror;

public class PlayerHealth : NetworkBehaviour
{
    [SyncVar] public int health = 1;
    [SyncVar(hook = nameof(OnDeadChanged))] public bool isDead = false;

    private SpriteRenderer spriteRenderer;
    private Collider2D playerCollider;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerCollider = GetComponent<Collider2D>();
    }

    [Server]
    public void TakeDamage(int amount)
    {
        if (isDead) return;

        health -= amount;

        if (health <= 0)
        {
            isDead = true;
            Debug.Log($"Player {netId} died");

            foreach (var kv in NetworkServer.spawned)
            {
                PlayerHealth other = kv.Value.GetComponent<PlayerHealth>();

                if (other != null && other.netId != netId)
                {
                    GameManager.Instance.ServerDeclareWinner(other.netId);
                    break;
                }
            }
        }
    }

    [Server]
    public void ServerResetPlayer()
    {
        health = 1;
        isDead = false;
    }

    private void OnDeadChanged(bool oldValue, bool newValue)
    {
        if (spriteRenderer != null)
            spriteRenderer.enabled = !newValue;

        if (playerCollider != null)
            playerCollider.enabled = !newValue;
    }
}