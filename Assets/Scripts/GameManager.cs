using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class GameManager : NetworkBehaviour
{
    private HashSet<uint> rematchVotes = new HashSet<uint>();

    public static GameManager Instance;

    public enum MatchState
    {
        Waiting,
        Countdown,
        Playing,
        GameOver
    }

    [SyncVar] public MatchState matchState = MatchState.Waiting;
    [SyncVar] public uint winnerNetId = 0;

    private GameOverUI gameOverUI;

    void Awake()
    {
        Instance = this;
        gameOverUI = FindFirstObjectByType<GameOverUI>();

        if (gameOverUI != null)
            gameOverUI.HideResult();
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        ResetMatchState();
    }

    [Server]
    public void ResetMatchState()
    {
        matchState = MatchState.Waiting;
        winnerNetId = 0;
    }

    [Server]
    public void ServerStartCountdown()
    {
        if (matchState != MatchState.Waiting && matchState != MatchState.GameOver)
            return;

        StartCoroutine(CountdownRoutine());
    }

    [Server]
    private IEnumerator CountdownRoutine()
    {
        matchState = MatchState.Countdown;

        RpcShowCountdown("3");
        yield return new WaitForSeconds(1f);

        RpcShowCountdown("2");
        yield return new WaitForSeconds(1f);

        RpcShowCountdown("1");
        yield return new WaitForSeconds(1f);

        RpcShowCountdown("SHOOT!");
        yield return new WaitForSeconds(0.75f);

        matchState = MatchState.Playing;
        RpcHideCountdown();
    }

    [Server]
    public void ServerDeclareWinner(uint winner)
    {
        if (matchState == MatchState.GameOver) return;

        matchState = MatchState.GameOver;
        winnerNetId = winner;

        Debug.Log($"Game Over. Winner netId = {winnerNetId}");

        RpcGameOver(winnerNetId);
    }

    [Server]
    public void ServerRequestRematch(uint playerNetId)
    {
        if (matchState != MatchState.GameOver) return;

        rematchVotes.Add(playerNetId);

        Debug.Log($"Player {playerNetId} requested rematch. Votes: {rematchVotes.Count}");

        if (rematchVotes.Count >= 2)
        {
            ServerRestartMatch();
        }
    }

    [Server]
    private void ServerRestartMatch()
    {
        rematchVotes.Clear();

        // Destroy old bullets
        List<GameObject> bulletsToDestroy = new List<GameObject>();

        foreach (var kv in NetworkServer.spawned)
        {
            if (kv.Value.GetComponent<Bullet>() != null)
                bulletsToDestroy.Add(kv.Value.gameObject);
        }

        foreach (GameObject bullet in bulletsToDestroy)
            NetworkServer.Destroy(bullet);

        // Reset players
        foreach (var kv in NetworkServer.spawned)
        {
            PlayerHealth health = kv.Value.GetComponent<PlayerHealth>();
            movement move = kv.Value.GetComponent<movement>();

            if (health != null && move != null)
            {
                health.ServerResetPlayer();

                Rigidbody2D rb = kv.Value.GetComponent<Rigidbody2D>();
                if (rb != null)
                    rb.linearVelocity = Vector2.zero;

                if (move.side == movement.Side.Bottom)
                    kv.Value.transform.position = NewNetworkManager.singleton.spawnBottom.position;
                else
                    kv.Value.transform.position = NewNetworkManager.singleton.spawnTop.position;
            }
        }

        ResetMatchState();
        RpcResetUI();

        ServerStartCountdown();
    }

    [ClientRpc]
    private void RpcResetUI()
    {
        if (gameOverUI == null)
            gameOverUI = FindFirstObjectByType<GameOverUI>();

        if (gameOverUI != null)
            gameOverUI.HideResult();
    }

    [ClientRpc]
    void RpcGameOver(uint winner)
    {
        Debug.Log($"[CLIENT] Game Over. Winner netId = {winner}");

        if (gameOverUI == null)
            gameOverUI = FindFirstObjectByType<GameOverUI>();

        if (gameOverUI != null)
            gameOverUI.ShowResult(winner);
    }

    [ClientRpc]
    void RpcShowCountdown(string text)
    {
        if (gameOverUI == null)
            gameOverUI = FindFirstObjectByType<GameOverUI>();

        if (gameOverUI != null)
            gameOverUI.ShowCountdown(text);
    }

    [ClientRpc]
    void RpcHideCountdown()
    {
        if (gameOverUI == null)
            gameOverUI = FindFirstObjectByType<GameOverUI>();

        if (gameOverUI != null)
            gameOverUI.HideCountdown();
    }
}