using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum GameMode
{
    Placement,
    Battle,
    Result
}

public class GameSceneController : MonoBehaviour
{
    public MapView mapView;
    public int mapSize = 8;

    private bool _placeShipHorizontally;
    public bool placeShipHorizontally
    {
        get { return _placeShipHorizontally; }
        set { _placeShipHorizontally = value; UpdateCursor(); }
    }

    private GameMode mode = GameMode.Placement;
    private int shipsPlaced = 0;

    // Start is called before the first frame update
    void Start()
    {
        BeginShipPlacement();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void BeginShipPlacement()
    {
        mode = GameMode.Placement;
        shipsPlaced = 0;
        mapView.SetPlacementMode();
        UpdateCursor();
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

        shipWidth = placeShipHorizontally ? size : 1;
        shipHeight = placeShipHorizontally ? 1 : size;

        if (coordinate.x < 0 || coordinate.x + (shipWidth - 1) >= mapSize || coordinate.y - (shipHeight - 1) < 0)
        {
            Debug.Log("out of bounds: " + coordinate);
            return;
        }

        shipsPlaced++;

        UpdateCursor();
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
            default:
                mapView.SetDisabled();
                break;
        }
    }
}
