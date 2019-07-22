using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum ShipType
{
    Scout = 0,
    Cruiser,
    Carrier
}

public class MapView : MonoBehaviour
{
    public GameSceneController controller;
    public Tilemap cursorLayer;
    public Tilemap fleetLayer;
	public Tile[] cursorTiles;
    public Tile cursorTile;
    public int size = 8;

	private string mode = "disabled";

    private Grid grid;
    private Vector3Int minCoordinate;
    private Vector3Int maxCoordinate;

    // Start is called before the first frame update
    void Start()
    {
        grid = GetComponent<Grid>();
		SetPlacementMode();
    }

    // Update is called once per frame
    void Update()
    {
		if (mode == "disabled") return;

        Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int coordinate = grid.WorldToCell(pos);

        coordinate.Clamp(minCoordinate, maxCoordinate);

        cursorLayer.ClearAllTiles();
        cursorLayer.SetTile(coordinate, cursorTile);
    }

    void OnMouseDown()
    {
        Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int coordinate = grid.WorldToCell(pos);

        coordinate.Clamp(minCoordinate, maxCoordinate);

        if (mode == "place")
        {
            controller.PlaceShip(coordinate);
        }
    }

    public void SetDisabled()
	{
		mode = "disabled";
		cursorLayer.ClearAllTiles();
	}

	public void SetPlacementMode()
	{
		mode = "place";
		minCoordinate = new Vector3Int(0, 0, 0);
		maxCoordinate = new Vector3Int(size - 1, size - 1, 0);
	}

    public void SetShipCursor(ShipType shipType, bool horizontal)
    {
        Debug.Log(shipType + ", " + horizontal);

        int index = (int)shipType * 2 + (horizontal ? 0 : 1);
        cursorTile = cursorTiles[index];
    }

    public void SetShip(ShipType shipType, Vector3Int coordinate)
    {
        int index = (int)shipType * 2;
        Tile tile = cursorTiles[index];

        fleetLayer.SetTile(coordinate, tile);
    }
}
