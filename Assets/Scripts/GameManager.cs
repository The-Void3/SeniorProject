using UnityEngine;
using Mirror;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance;

    [SyncVar] public bool gameOver = false;
    [SyncVar] public uint winnerNetId = 0;

    void Awake()
    {
        Instance = this;
    }

    [Server]
    public void ServerDeclareWinner(uint winner)
    {
        if (gameOver) return;

        gameOver = true;
        winnerNetId = winner;

        Debug.Log($"Game Over. Winner netId = {winnerNetId}");

        RpcGameOver(winnerNetId);
    }

    [ClientRpc]
    void RpcGameOver(uint winner)
    {
        // For now just log; we’ll wire UI next
        Debug.Log($"[CLIENT] Game Over. Winner netId = {winner}");

        // Optional: freeze local player input by disabling movement scripts, etc.
        // We'll do this cleanly once UI exists.
    }
}