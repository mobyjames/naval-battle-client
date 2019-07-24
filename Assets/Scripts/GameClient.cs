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
    public EventHandler<string> OnGamePhaseChange;

    private Client client;
    private Room<State> room;
    private bool initialStateReceived = false;

    public string ClientId
    {
        get { return client?.Id; }
    }

    public string SessionId
    {
        get { return room?.SessionId; }
    }

    public bool Connected
    {
        get { return ClientId != null; }
    }

    public State State
    {
        get { return room?.State; }
    }

    public bool Joined
    {
        get { return room != null && room.Connection.IsOpen; }
    }

    void OnDestroy()
    {
        if (client != null) client.Close();
    }

    private void OnApplicationQuit()
    {
        if (room != null) room.Leave();
        if (client != null) client.Close();
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

    public void Leave()
    {
        if (room != null) room.Leave();
        room = null;
    }

    public void SendPlacement(int[] placement)
    {
        room.Send(new { command = "place", placement });
    }

    public void SendTurn(int targetIndex)
    {
        room.Send(new { command = "turn", targetIndex });
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
        if (e.IsFirstState && !initialStateReceived)
        {
            Debug.Log("Received initial state " + e.State.phase);
            initialStateReceived = true;
            OnInitialState?.Invoke(this, e.State);
        }
    }

    void OnStateChangeHandler(object sender, OnChangeEventArgs e)
    {
        if (!initialStateReceived) return;

        foreach (var change in e.Changes)
        {
            //Debug.Log(change.Field + " changed to " + change.Value);

            if (change.Field == "phase")
            {
                OnGamePhaseChange?.Invoke(this, (string)change.Value);
            }
        }
    }

    void OnMessageHandler(object sender, MessageEventArgs e)
    {
        var message = e.Message;

        OnMessage?.Invoke(this, message);
    }
}