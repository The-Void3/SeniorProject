using UnityEngine;
using TMPro;
using Mirror;

public class GameOverUI : MonoBehaviour
{
    public TMP_Text resultText;
    public TMP_Text countdownText;

    public GameObject rematchButton;
    public GameObject leaveButton;
    public TMP_Text statusText;

    private uint localPlayerNetId;
    private bool hasLocalPlayerId = false;

    void Start()
    {
        HideResult();
        HideCountdown();
    }

    void Update()
    {
        if (!hasLocalPlayerId && NetworkClient.localPlayer != null)
        {
            localPlayerNetId = NetworkClient.localPlayer.netId;
            hasLocalPlayerId = true;
        }
    }

    public void ShowResult(uint winnerNetId)
    {
        resultText.gameObject.SetActive(true);

        if (hasLocalPlayerId && localPlayerNetId == winnerNetId)
            resultText.text = "YOU WIN";
        else
            resultText.text = "YOU LOSE";

        rematchButton.SetActive(true);
        leaveButton.SetActive(true);

        if (statusText != null)
        {
            statusText.gameObject.SetActive(true);
            statusText.text = "";
        }
    }

    public void HideResult()
    {
        resultText.text = "";
        resultText.gameObject.SetActive(false);

        rematchButton.SetActive(false);
        leaveButton.SetActive(false);

        if (statusText != null)
        {
            statusText.text = "";
            statusText.gameObject.SetActive(false);
        }

        localPlayerNetId = 0;
        hasLocalPlayerId = false;
    }

    public void ShowCountdown(string text)
    {
        countdownText.gameObject.SetActive(true);
        countdownText.text = text;
    }

    public void HideCountdown()
    {
        countdownText.text = "";
        countdownText.gameObject.SetActive(false);
    }

    public void OnRematchClicked()
    {
        if (statusText != null)
        {
            statusText.gameObject.SetActive(true);
            statusText.text = "Waiting for opponent...";
        }

        if (NetworkClient.localPlayer != null)
        {
            movement m = NetworkClient.localPlayer.GetComponent<movement>();

            if (m != null)
                m.CmdRequestRematch();
        }
    }

    public void OnLeaveClicked()
    {
        if (NetworkServer.active && NetworkClient.isConnected)
            NetworkManager.singleton.StopHost();
        else if (NetworkClient.isConnected)
            NetworkManager.singleton.StopClient();
    }
}