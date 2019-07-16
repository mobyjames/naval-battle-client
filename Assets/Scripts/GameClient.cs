using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Colyseus;
using Colyseus.Schema;
using System;

public class GameClient : GenericSingleton<GameClient>
{
    public EventHandler OnConnect;
    public EventHandler OnClose;
    public EventHandler OnJoin;
    public EventHandler<State> OnInitialState;
    public EventHandler<object> OnMessage;

    private Client client;
    private Room<State> room;


    void OnDestroy()
    {
        if (client != null) client.Close();
    }

    private void OnApplicationQuit()
    {
        Destroy(this);
    }

    public void Connect()
    {
        string uri = "ws://localhost:2567";
        client = new Client(uri);
        client.OnOpen += OnOpenHandler;
        client.OnClose += OnCloseHandler;

        StartCoroutine(ConnectAndListen());
    }

    public void Join()
    {
        room = client.Join<State>("game");
        room.OnReadyToConnect += (sender, e) => StartCoroutine(room.Connect());
        room.OnMessage += OnMessageHandler;
        room.OnJoin += OnJoinHandler;
        room.OnStateChange += OnRoomStateChangeHandler;
    }

    IEnumerator ConnectAndListen()
    {
        yield return StartCoroutine(client.Connect());

        while (true)
        {
            client.Recv();
            yield return 0;
        }
    }

    // handlers

    void OnOpenHandler(object sender, EventArgs e)
    {
        Debug.Log("Connected to server. Client id: " + client.Id);

        OnConnect?.Invoke(this, e);
    }

    void OnCloseHandler(object sender, EventArgs e)
    {
        Debug.Log("Disconnected from server.");

        OnClose?.Invoke(this, e);
    }

    void OnJoinHandler(object sender, EventArgs e)
    {
        room.State.OnChange += OnStateChangeHandler;

        OnJoin?.Invoke(this, e);
    }

    void OnRoomStateChangeHandler(object sender, StateChangeEventArgs<State> e)
    {
        if (e.IsFirstState)
        {
            Debug.Log("Received initial state");
            OnInitialState?.Invoke(this, e.State);
        }
    }

    void OnStateChangeHandler(object sender, OnChangeEventArgs e)
    {
        foreach (var change in e.Changes)
        {
            Debug.Log(change.Field + " changed");
        }
    }

    void OnMessageHandler(object sender, MessageEventArgs e)
    {
        var message = e.Message;

        OnMessage?.Invoke(this, message);
    }
}