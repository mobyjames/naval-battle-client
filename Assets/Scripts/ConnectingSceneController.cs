using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;

public class ConnectingSceneController : MonoBehaviour
{
    public Text message;

    // Start is called before the first frame update
    void Start()
    {
        message.text = "Connecting...";

        GameClient.Instance.OnConnect += OnConnect;
        GameClient.Instance.OnJoin += OnJoin;
        GameClient.Instance.Connect();
    }

    void OnConnect(object sender, EventArgs e)
    {
        message.text = "Finding a game...";

        GameClient.Instance.Join();
    }

    void OnJoin(object sender, EventArgs e)
    {
        message.text = "Joined! Finding another player...";
    }

    private void OnDestroy()
    {
        GameClient.Instance.OnConnect -= OnConnect;
        GameClient.Instance.OnJoin -= OnJoin;
    }
}
