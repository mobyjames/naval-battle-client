using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;

public class ConnectingSceneController : MonoBehaviour
{
    public Text message;

    private GameClient client;

    void Start()
    {
        message.text = "Connecting...";

        client = GameClient.Instance;

        client.OnConnect += OnConnect;
        client.OnJoin += OnJoin;

        if (!client.Connected)
        {
            client.Connect();
        }
        else
        {
            OnConnect(this, null);
        }
    }

    void OnConnect(object sender, EventArgs e)
    {
        message.text = "Finding a game...";

        if (!client.Joined)
        {
            client.Join();
        }
        else
        {
            OnJoin(this, null);
        }
    }

    void OnJoin(object sender, EventArgs e)
    {
        message.text = "Joined! Finding another player...";

        client.OnGamePhaseChange += GamePhaseChangeHandler;
    }

    private void GamePhaseChangeHandler(object sender, string phase)
    {
        if (phase == "place")
        {
            SceneManager.LoadScene("GameScene");
        }
    }

    private void OnDestroy()
    {
        if (client != null)
        {
            client.OnConnect -= OnConnect;
            client.OnJoin -= OnJoin;
            client.OnGamePhaseChange -= GamePhaseChangeHandler;
        }
    }
}
