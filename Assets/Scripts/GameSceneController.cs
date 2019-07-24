﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;

enum GameMode
{
    Placement,
    Battle,
    Result
}

public class GameSceneController : MonoBehaviour
{
    public MapView mapView;
    public Text message;
    public Button rotateShipButton;
    public Tile debugTile;
    public int mapSize = 8;

    private bool _placeShipHorizontally;
    public bool placeShipHorizontally
    {
        get { return _placeShipHorizontally; }
        set { _placeShipHorizontally = value; UpdateCursor(); }
    }

    private GameMode mode = GameMode.Placement;
    private int shipsPlaced = 0;
    private int cellCount = 0;
    private int[] placement;

    private GameClient client;
    private State state;
    private int myPlayerNumber;

    // lifecycle

    void Start()
    {
        client = GameClient.Instance;

        if (!client.Connected)
        {
            SceneManager.LoadScene("ConnectingScene");
            return;
        }

        cellCount = mapSize * mapSize;
        placement = new int[cellCount];

        for (var i = 0; i < cellCount; i++)
        {
            placement[i] = 0; // empty
        }

        client.OnInitialState += InitialStateHandler;
        client.OnGamePhaseChange += GamePhaseChangeHandler;
        client.OnClose += CloseHandler;

        // hmm... Rx?
        if (client.State != null)
        {
            InitialStateHandler(this, client.State);
        }
    }

    private void OnDestroy()
    {
        if (state != null)
        {
            state.OnChange -= StateChangeHandler;
            state.player1Shots.OnChange -= ShotsChangedPlayer1;
            state.player2Shots.OnChange -= ShotsChangedPlayer2;
        }

        client.OnInitialState -= InitialStateHandler;
        client.OnGamePhaseChange -= GamePhaseChangeHandler;
        client.OnClose -= CloseHandler;
    }

    // networking

    private void InitialStateHandler(object sender, State initialState)
    {
        state = initialState;

        myPlayerNumber = state.player1 == client.SessionId ? 1 : 2;

        Debug.Log("I am player " + myPlayerNumber);

        state.OnChange += StateChangeHandler;
        state.player1Shots.OnChange += ShotsChangedPlayer1;
        state.player2Shots.OnChange += ShotsChangedPlayer2;

        GamePhaseChangeHandler(this, state.phase);
    }

    private void StateChangeHandler(object sender, Colyseus.Schema.OnChangeEventArgs args)
    {
        foreach (var change in args.Changes)
        {
            switch (change.Field)
            {
                case "playerTurn":
                    CheckTurn();
                    break;
            }
        }
    }

    private void ShotsChangedPlayer1(object sender, Colyseus.Schema.KeyValueEventArgs<short, int> changes)
    {
    }

    private void ShotsChangedPlayer2(object sender, Colyseus.Schema.KeyValueEventArgs<short, int> changes)
    {
    }

    private void CloseHandler(object sender, object args)
    {
        message.text = "Server Disconnected";
    }

    // state changes & player input

    public void BeginShipPlacement()
    {
        message.text = "Place your ships";
        mode = GameMode.Placement;
        shipsPlaced = 0;
        mapView.SetPlacementMode();
        UpdateCursor();
    }

    public void WaitForOpponentToPlace()
    {
        message.text = "Waiting for opponent to place ships...";
        rotateShipButton.enabled = false;
        mapView.SetDisabled();
    }

    public void WaitForOpponent()
    {
        mapView.SetDisabled();
        message.text = "Waiting for opponent...";
    }

    public void StartTurn()
    {
        mapView.SetAttackMode();
        message.text = "Your Turn!";
    }

    public void TakeTurn(Vector3Int coordinate)
    {
        // TODO: check if we've already targeting this cell - or let player be stupid? I guess you could in be stupid in the boardgame too so let's leave it
        // TODO: send to server, wait for response

        //mapView.SetMarker(coordinate, Marker.Miss, true);
        //mapView.SetMarker(coordinate, Marker.Hit, false);

        int targetIndex = coordinate.y * mapSize + coordinate.x;
        client.SendTurn(targetIndex);
    }

    public void ShowResult()
    {
        mapView.SetDisabled();
        message.text = "The winner is player " + state.winningPlayer;
    }

    public void RotateShip()
    {
        placeShipHorizontally = !placeShipHorizontally;
    }

    public void PlaceShip(Vector3Int coordinate)
    {
        if (mode != GameMode.Placement) return;

        int size = 0;
        int shipWidth = 0;
        int shipHeight = 0;
        ShipType shipType = ShipType.Scout;

        if (shipsPlaced == 0)
        {
            size = 2;
            shipType = ShipType.Scout;
        }
        else if (shipsPlaced == 1)
        {
            size = 3;
            shipType = ShipType.Cruiser;
        }
        else if (shipsPlaced == 2)
        {
            size = 5;
            shipType = ShipType.Carrier;
        }
        else
        {
            return;
        }

        shipWidth = placeShipHorizontally ? size : 1;
        shipHeight = placeShipHorizontally ? 1 : size;

        if (coordinate.x < 0 || coordinate.x + (shipWidth - 1) >= mapSize || coordinate.y - (shipHeight - 1) < 0)
        {
            Debug.Log("out of bounds: " + coordinate);
            return;
        }

        for (var i = 0; i < size; i++)
        {
            if (placeShipHorizontally)
            {
                if (!SetPlacementCell(coordinate + new Vector3Int(i, 0, 0), shipType, true)) return;
            }
            else
            {
                if (!SetPlacementCell(coordinate + new Vector3Int(0, -i, 0), shipType, true)) return;
            }
        }

        for (var i = 0; i < size; i++)
        {
            if (placeShipHorizontally)
            {
                SetPlacementCell(coordinate + new Vector3Int(i, 0, 0), shipType);
            }
            else
            {
                SetPlacementCell(coordinate + new Vector3Int(0, -i, 0), shipType);
            }
        }

        mapView.SetShip(shipType, coordinate, placeShipHorizontally);
        shipsPlaced++;
        UpdateCursor();

        if (shipsPlaced == 3)
        {
            client.SendPlacement(placement);
            WaitForOpponentToPlace();
        }
    }

    public void Leave()
    {
        client.Leave();
        SceneManager.LoadScene("ConnectingScene");
    }

    // private

    private bool SetPlacementCell(Vector3Int coordinate, ShipType shipType, bool testOnly = false)
    {
        int cellIndex = coordinate.y * mapSize + coordinate.x;

        if (cellIndex < 0 || cellIndex >= cellCount) return false;

        if (placement[cellIndex] > 0)
        {
            Debug.Log("overlap: " + coordinate + ", index: " + cellIndex);
            return false;
        }

        if (testOnly) return true;

        placement[cellIndex] = (int)shipType;

        if (true) // TODO: debug mode?
        {
            mapView.SetDebugTile(coordinate, debugTile);
        }

        return true;
    }

    private void UpdateCursor()
    {
        switch (shipsPlaced)
        {
            case 0:
                mapView.SetShipCursor(ShipType.Scout, placeShipHorizontally);
                break;
            case 1:
                mapView.SetShipCursor(ShipType.Cruiser, placeShipHorizontally);
                break;
            case 2:
                mapView.SetShipCursor(ShipType.Carrier, placeShipHorizontally);
                break;
        }
    }

    private void GamePhaseChangeHandler(object sender, string phase)
    {
        Debug.Log("phase change: " + phase);

        switch (phase)
        {
            case "waiting":
                Leave();
                break;
            case "place":
                BeginShipPlacement();
                break;
            case "battle":
                CheckTurn();
                break;
            case "result":
                ShowResult();
                break;
        }
    }

    private void CheckTurn()
    {
        if (state.playerTurn == myPlayerNumber)
        {
            StartTurn();
        }
        else
        {
            WaitForOpponent();
        }
    }
}
