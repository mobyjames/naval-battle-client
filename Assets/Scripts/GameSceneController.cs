using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;

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

    void Start()
    {
        BeginShipPlacement();
        //StartTurn();
        cellCount = mapSize * mapSize;
        placement = new int[cellCount];

        for (var i = 0; i < cellCount; i++)
        {
            placement[i] = 0; // nothing
        }
    }

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
        message.text = "Waiting for opponent to finish placing ships";
        mapView.SetDisabled();
    }

    public void WaitForOpponentTurn()
    {
        mapView.SetDisabled();
    }

    public void StartTurn()
    {
        mapView.SetAttackMode();
    }

    public void TakeTurn(Vector3Int coordinate)
    {

    }

    public void ShowResult()
    {
        mapView.SetDisabled();
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
            WaitForOpponentToPlace();
        }
    }

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
}
